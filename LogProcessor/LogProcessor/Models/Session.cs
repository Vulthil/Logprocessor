using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automata.Automaton;
using Automata.Interfaces;
using LogProcessor.Interfaces;
using LogProcessor.Model.Utilities;
using Services.Shared.Models;

namespace LogProcessor.Models
{
    public class Session : ISession
    {

        private readonly string _id;

        public string SessionId => _id;

        public string SessionName { get; set; }

        public DateTime LastModified { get; set; }





        private Dictionary<string, int> _sent = new();

        private Dictionary<string, int> _received = new();

        private CancellationTokenSource _cancellationTokenSource;

        private SemaphoreSlim _dictLock = new(1);


        private readonly IAutomatonFactory<string, string> _automatonFactory;

        private readonly ConcurrentDictionary<string, IStatefulAutomaton<string, string>> _serviceAutomata = new();

        public (List<(string, int)> sent, List<(string, int)> received) State => (_sent.Select(p => (p.Key, p.Value)).ToList(), _received.Select(p => (p.Key, p.Value)).ToList());
        public List<(string, IStatefulAutomaton<string, string>)> ParticipantAutomata => _serviceAutomata.Select(p => (p.Key, p.Value)).ToList();


        public Session(string id, IAutomatonFactory<string, string> automatonFactory)
        {
            _id = id;
            _automatonFactory = automatonFactory;
        }

        private IStatefulAutomaton<string, string> GetParticipantAutomaton(LogMessage message)
        {

            var serviceId = message.Originator;
            if (!_serviceAutomata.ContainsKey(serviceId))
            {
                AddSessionParticipant(serviceId, _automatonFactory);
            }

            return _serviceAutomata[serviceId];
        }

        private void AddSessionParticipant(string serviceId, IAutomatonFactory<string, string> automatonFactory)
        {
            if (!automatonFactory.TrackedServices.Contains(serviceId))
            {
                throw new ArgumentException($"The service with id <{serviceId}> has no defined session type.");
            }
            (_serviceAutomata[serviceId], SessionName) = automatonFactory.GetAutomaton(serviceId);
        }

        public void HandleEndResult(LogMessage message, DateTime requestTime, out bool sessionFinished)
        {
            LastModified = requestTime;
            var serviceId = message.Originator;
            _serviceAutomata.Remove(serviceId, out _);
            sessionFinished = !_serviceAutomata.Keys.Any();
        }

        public void AddReceptionAsserter(LogMessage message)
        {
            if (message.IsInbound)
            {
                TryAssertReceived(message);
            }
            else
            {
                TryAssertSent(message);
            }
        }

        private async void TryAssertReceived(LogMessage message)
        {
            var expectedId = $"{message.Originator}:{message.TargetApi}";
            await _dictLock.WaitAsync();
            BalanceDicts(expectedId, _sent, _received);
            _dictLock.Release();
        }

        private async void TryAssertSent(LogMessage message)
        {
            var expectedId = $"{message.GetOpposing()}:{message.TargetApi}";
            await _dictLock.WaitAsync();
            BalanceDicts(expectedId, _received, _sent);
            _dictLock.Release();
        }

        private void BalanceDicts(string expectedId, Dictionary<string, int> source, Dictionary<string, int> sink)
        {
            if (source.TryGetValue(expectedId, out var count) && count > 0)
            {
                source[expectedId] = --count;
            }
            else
            {
                sink.TryGetValue(expectedId, out count);
                sink[expectedId] = ++count;
            }
        }

        public async Task<bool> IsBalanced()
        {
            await _dictLock.WaitAsync();
            var hasAnyUnbalancedMessages = DictContainsNonZeroValue(_sent) || DictContainsNonZeroValue(_received);
            _dictLock.Release();
            return !hasAnyUnbalancedMessages;
        }

        public MoveResult<string> Advance(LogMessage message, DateTime requestTime)
        {
            LastModified = requestTime;
            return GetParticipantAutomaton(message).Move(message.ToTransitionLabel());
        }

        private bool DictContainsNonZeroValue(Dictionary<string, int> d)
        {
            return d.Any(x => x.Value != 0);
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }

        public void EnrichMessage(LogMessage logMessage)
        {
            if (logMessage.IsOutbound && logMessage.GetOpposing().IsNullOrWhiteSpace())
            {
                //TODO: Handle end states
                logMessage.Destination = _serviceAutomata[logMessage.Originator]
                    .GetTransitionsFromCurrentState()
                    .Select(x => x.Split(":"))
                    .Select(x => new { recipient = x[0], expectedApi = x[1] })
                    .SingleOrDefault(x => x.expectedApi.Equals(logMessage.TargetApi, StringComparison.InvariantCultureIgnoreCase))?
                    .recipient ?? "";
            }
        }

        public bool AllAwaiting()
        {
            return _serviceAutomata.All(a => a.Value.GetTransitionsFromCurrentState().All(t => t.EndsWith("inbound", StringComparison.InvariantCultureIgnoreCase)));
        }
    }
}