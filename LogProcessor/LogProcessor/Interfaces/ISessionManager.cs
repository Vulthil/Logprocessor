using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LogProcessor.Models;
using Services.Shared.Models;

namespace LogProcessor.Interfaces
{
    public interface ISessionManager
    {
        public Task<SessionResult> AdvanceSession(LogMessage message, DateTime requestTime, CancellationToken token);

        (IEnumerable<ISession> sessions, int total) TrackedSessions(int skip, int take);

        (IEnumerable<ISession> sessions, int total) AwaitingSessions(int skip, int take);


#if DEBUG
        public int Count { get; }
#endif

        bool EvictSession(string sessionId);

        void StopTrackingSessionsFor(string sessionName);
    }
}