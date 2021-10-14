using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using LogProcessor.Model.Json;
using Services.Shared.Models;

namespace SessionTypes.Grammar.Generator
{
    public class ServiceDefinitionGenerator
    {
        private readonly ServiceAutomata _serviceAutomata = new ()
        {
            ServicesDefinitions = new ()
        };
        private ServiceDefinition _serviceDefinition;
        private readonly Dictionary<string, string> _recursionDict = new();
        private readonly Dictionary<string, HashSet<string>> _states = new();
        public readonly HashSet<string> InternalParticipants = new();
        public readonly HashSet<string> ExternalParticipants = new();
        //private bool _inInternal = false;

        public void AddSendReceiveTransition(Direction direction, string participantName, string messageIdentifier, string state = null)
        {
            var newState = $"Post{messageIdentifier}"; 
            var tempState = $"Post{messageIdentifier}";
            var count = 1;
            while (_states[_serviceDefinition.ServiceId].Contains(newState))
            {
                newState = $"{tempState}_{count}";
                count++;
            }
            _states[_serviceDefinition.ServiceId].Add(newState);
            var serviceTransition = new ServiceTransition()
            {
                Direction = direction,
                Message = messageIdentifier,
                OpposingService = participantName,
                State = state ?? GetLatestState(),
                NewState = newState
            };
            _serviceDefinition.Transitions.Add(serviceTransition);
        }
        

        public void AddServiceDefinition(string participantName)
        {
            _serviceDefinition = new ServiceDefinition()
            {
                ServiceId = participantName,
                StartState = "Awaiting",
                Transitions = new List<ServiceTransition>()
            };
            _states.Add(_serviceDefinition.ServiceId, new HashSet<string>(){"Awaiting"});
            _serviceAutomata.ServicesDefinitions.Add(participantName, _serviceDefinition);
        }

        public void EndServiceDefinition()
        {
            _serviceDefinition = null;
        }

        public void MarkRecursiveStart(string recIdentifier)
        {
            _recursionDict.Add(recIdentifier, GetLatestState());
        }

        public void EndRecursion(string recIdentifier)
        {
            _recursionDict.Remove(recIdentifier);
        }

        public string GetLatestState()
        {
            return _serviceDefinition.Transitions.LastOrDefault()?.NewState ??
                   _serviceDefinition.StartState;
        }

        public bool AddRecursionState(string recIdentifier)
        {
            var transition = _serviceDefinition.Transitions.LastOrDefault();
            if (transition != null)
            {
                if (_recursionDict.TryGetValue(recIdentifier, out var state))
                {

                    transition.NewState = state;
                    return true;
                }
            }

            return false;
        }

        public void MarkEndState()
        {
            var transition = _serviceDefinition.Transitions.LastOrDefault();
            if (transition != null)
            {
                transition.NewState = "Completed";
                //_states[_serviceDefinition.ServiceId].("Completed");
            }
        }

        public bool MarkInternalParticipant(string participantName)
        {
            if (ExternalParticipants.Contains(participantName))
            {
                ExternalParticipants.Remove(participantName);
            }
            return InternalParticipants.Add(participantName);
        }

        public void MarkParticipant(string participantName)
        {
            //if (_inInternal)
            //{
                if (!InternalParticipants.Contains(participantName))
                {
                    ExternalParticipants.Add(participantName);
                }
            //}
        }

        public ServiceAutomata GetDefinition()
        {
            return _serviceAutomata;
        }
        
    }
}