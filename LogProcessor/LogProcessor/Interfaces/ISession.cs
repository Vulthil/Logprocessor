using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Automata.Interfaces;
using LogProcessor.Services;
using Services.Shared.Models;

namespace LogProcessor.Interfaces
{
    public interface ISession : IDisposable, ILogMessageEnricher
    {
        string SessionId { get; }

        public string SessionName { get; }

        DateTime LastModified { get; }

        (List<(string,int)> sent, List<(string,int)> received) State {get;}

        List<(string, IStatefulAutomaton<string, string>)> ParticipantAutomata {get;}

        public void HandleEndResult(LogMessage message, DateTime requestTime, out bool sessionFinished);

        void AddReceptionAsserter(LogMessage message);

        Task<bool> IsBalanced();

        public MoveResult<string> Advance(LogMessage message, DateTime requestTime);

        bool AllAwaiting();
    }
}