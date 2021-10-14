using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using Microsoft.Extensions.Logging;

namespace LogProcessor.Services
{
    public class ProtocolErrorHandler : IProtocolErrorHandler
    {
        private readonly ILogger<ProtocolErrorHandler> _logger;
        private readonly IProtocolErrorRepoitory _protocolErrorRepoitory;

        public ProtocolErrorHandler(ILogger<ProtocolErrorHandler> logger,
            IProtocolErrorRepoitory protocolErrorRepoitory)
        {
            _logger = logger;
            _protocolErrorRepoitory = protocolErrorRepoitory;
        }


        public async Task PersistError(ErrorResult error)
        {

            try
            {
                switch (error)
                {
                    case ExceptionResult er:
                        LogExceptionResult(er);
                        break;
                        //case IllegalStartResult ir:
                        //    LogIllegalStartResult(ir);
                        //    break;
                        //case TimeoutResult tr:
                        //    LogTimeoutResult(tr);
                        //    break;
                }

                await _protocolErrorRepoitory
                    .CommitProtocolError(error);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Something went wrong in background processing");
            }

        }

        public Task PersistErrors(List<ErrorResult> batch)
        {
            return _protocolErrorRepoitory.CommitProtocolErrors(batch);
        }

        private void LogTimeoutResult(TimeoutResult e)
        {
            //_logger.LogCritical($"An exception occurred while trying to advance session {e.Message.SessionId}: Expected receival of message was not logged from {e.Message.Destination}");
        }

        private void LogExceptionResult(ExceptionResult e)
        {
            //_logger.LogCritical(e.ErrorMessage, $"An exception occurred while trying to advance session {e.Message.SessionId}");
        }
        private void LogIllegalStartResult(IllegalStartResult e)
        {
            // _logger.LogCritical($" A session was attempted to be made for {e.Message.Originator}.{e.Message.TargetApi}, but it is not recognized as a valid session initiator.");
        }
    }
}