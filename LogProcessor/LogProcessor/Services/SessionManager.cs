using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Automata.Interfaces;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using LogProcessor.Model.Utilities;
using Services.Shared.Models;
using MoreLinq.Extensions;

namespace LogProcessor.Services
{
    public class SessionManager : ISessionManager//, ISessionErrorHandler
    {
        private readonly IAutomatonFactory<string, string> _automatonFactory;
        private readonly ConcurrentDictionary<string, ISession> _sessions = new();
        private readonly IEnumerable<string> _trackedServices;

        public int Count { get; set; }

        public SessionManager(IAutomatonFactory<string, string> automatonFactory)
        {
            _automatonFactory = automatonFactory;
            _trackedServices = automatonFactory.TrackedServices;
        }

        public bool EvictSession(string sessionId)
        {
            return DisposeSession(sessionId);
        }

        public void StopTrackingSessionsFor(string sessionName)
        {
            _sessions.Values.Where(s => s.SessionName.Equals(sessionName, StringComparison.InvariantCultureIgnoreCase)).ForEach(s => EvictSession(s.SessionId));
        }
        
        public (IEnumerable<ISession> sessions, int total) TrackedSessions(int skip, int take)
        {
            return (_sessions.OrderBy(x => x.Key).Skip(skip).Take(take).Select(x => x.Value).ToList(), _sessions.Count);
        }

        public (IEnumerable<ISession> sessions, int total) AwaitingSessions(int skip, int take)
        {
            var awaiting = _sessions.Values.Where(s => s.AllAwaiting());
            return (awaiting.Skip(skip).Take(take).ToList(), awaiting.Count());
        }

        public async Task<SessionResult> AdvanceSession(LogMessage message, DateTime requestTime, CancellationToken token)
        {
            try
            {
                if (!_trackedServices.Contains(message.Originator))
                {
                    return new SkippedMessageResult(message);
                }

                if (!MessageHasTrackedSession(message) && !MessageCanCreateSession(message))
                {
                    // If the message doesn't belong to a tracked session, and can not start a session,
                    // we return an IllegalStartResult.
                    return new IllegalStartResult(message, $"Message {message.ToTransitionLabel()} can not start a local session.");
                }

                // Get the session instance which the message belongs to.
                var session = GetSession(message.SessionId, token);

                if (session is null)
                {
                    // Session will be null if the CancellationToken has been cancelled by the calling method.
                    // If so, the message is poisoned.
                    return ExceptionResult.PoisonedMessage(message);
                }

                // Enrich the message once we've ruled out some classes of errors,
                // to not have to waste resources enriching a message that can be ruled out before enrichment
                session.EnrichMessage(message);

                // Try to advance the session
                var moveResult = session.Advance(message, requestTime);

                if (moveResult.WasValid && _trackedServices.Contains(message.GetOpposing()))
                {
                    // If the received message was valid,
                    // and we are tracking the service on the other end of the message,
                    // we tell the session to keep track of whether the opposing service logged an equal and opposite message.
                    session.AddReceptionAsserter(message);
                }

                SessionResult result = moveResult switch
                {
                    // ErrorResult -> state machine rejected the move,
                    // create ExceptionResult with message from state machine lib.
                    ErrorResult<string> err => new ExceptionResult(message, err.ErrorMsg),

                    // EndStateResult -> state machine has reached its end state,
                    // call HandleEndResult to do decide what to do with the session as a whole.
                    // HandleEndResult can produce both SuccesResult and ErrorResult.
                    EndStateResult<string> _ => await HandleEndResult(message, requestTime),

                    // If none of the above applies,
                    // the move was valid and we create a SuccessResult
                    _ => new SuccessResult(message)
                };



                // Pass result to calling method, to let it decide what should happen for various results,
                // but check for cancellation first to ensure correctness.
                return token.IsCancellationRequested ? ExceptionResult.PoisonedMessage(message) : result;

            }
            catch (Exception e)
            {
                // Sumting wong
                return new ExceptionResult(message, e.Message);
            }

        }

        private async Task<SessionResult> HandleEndResult(LogMessage message, DateTime requestTime)
        {
            var session = _sessions[message.SessionId];

            session.HandleEndResult(message, requestTime, out var noParticipantsLeft);

            if (noParticipantsLeft && await session.IsBalanced())
            {
                // If the session is finished (no participants left)
                // and there's no messages unaccounted for,
                // we try to safely remove the session from tracking
                DisposeSession(session.SessionId);
                return new EndResult(message);
            }

            return new SuccessResult(message);
        }


        private bool MessageCanCreateSession(LogMessage message)
        {
            var expectedTransitionLabel = message.ToTransitionLabel();
            return _automatonFactory.GetStartStateTransitions(message.Originator).Any(t => t.Equals(expectedTransitionLabel));
        }

        private bool MessageHasTrackedSession(LogMessage message)
        {
            return _sessions.ContainsKey(message.SessionId);
        }

        private bool DisposeSession(string sessionId)
        {
            _sessions.Remove(sessionId, out var session);
            session?.Dispose();
            return session is not null;
        }

        private ISession GetSession(string sessionId, CancellationToken token)
        {

            if (!_sessions.TryGetValue(sessionId, out var session))
            {
                if (token.IsCancellationRequested)
                {
                    return null;
                }

                session = new Session(sessionId, _automatonFactory);
                _sessions[sessionId] = session;
            }

            return session;
        }


    }


}