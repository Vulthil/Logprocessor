using System;
using System.Collections.Generic;
using System.Linq;
using Automata.Automaton;
using LogProcessor.Interfaces;
using LogProcessor.Models.Infrastructure;
using MoreLinq;
using Services.Shared.Models;

namespace LogProcessor.Models.Dto
{
    public enum SessionStatus{
        Live,
    }
    public abstract class SessionDto{
        public string SessionId {get;set;}

        public SessionDto(String sessionId){
            SessionId = sessionId;
        }
    }
    public class LabelAndCountDto
    {
        public LabelAndCountDto(string label, int count)
        {
            Label = label;
            Count = count;
        }

        public string Label { get; set; }
        public int Count { get; set; }
    }
    public class SessionStateDto
    {
        public IEnumerable<LabelAndCountDto> Sent { get; set; }
        public IEnumerable<LabelAndCountDto> Received { get; set; }

    }
    public class ServiceTransitionDto
    {
        public ServiceTransitionDto(string serviceId, IEnumerable<string> labels)
        {
            ServiceId = serviceId;
            TransitionLabels = labels;
        }

        public string ServiceId { get; set; }
        public IEnumerable<string> TransitionLabels { get; set; }
    }


    public class TrackedSessionDto : SessionDto
    {
        public SessionStatus Status {get;}
        public SessionStateDto State {get;}
        public IEnumerable<ServiceTransitionDto> Transitions { get; }

        public TrackedSessionDto(ISession session) : base(session.SessionId){
            State = new SessionStateDto() { 
                Sent = session.State.sent.Select(s => new LabelAndCountDto(s.Item1, s.Item2)), 
                Received = session.State.received.Select(s => new LabelAndCountDto(s.Item1, s.Item2))
            };
            Transitions = session.ParticipantAutomata.Select(p => new ServiceTransitionDto(p.Item1, p.Item2.GetTransitionsFromCurrentState())).ToList();
        }

    }

    public class AwaitingSessionDto : TrackedSessionDto
    {
        public DateTime LastModified { get; set; }
        public AwaitingSessionDto(ISession session) : base(session)
        {
            LastModified = session.LastModified;
        }
    }

    public class FaultedSessionDto : SessionDto{
        public ViolatingMessage ViolatingMessage {get; set;}
        public FaultedSessionDto(ViolatingMessage violatingMessage) : base(violatingMessage.LogMessage.Message.SessionId){
            ViolatingMessage = violatingMessage;
            violatingMessage.PoisonedMessages.ForEach(m => m.ViolatingMessage = null);
        }
    }

}