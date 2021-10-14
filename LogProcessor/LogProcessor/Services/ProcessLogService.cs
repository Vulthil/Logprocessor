using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LogProcessor.HostedServices.Channels;
using LogProcessor.Interfaces;
using LogProcessor.Models;
using Microsoft.AspNetCore.Http;
using Services.Shared.Models;

namespace LogProcessor.Services
{
    public class LogService : ILogService
    {
        private readonly ISessionManager _sessionManager;
        private readonly IChannelWriter<ErrorResult> _channelWriter;
        private readonly IConfiguredMemoryCache _cache;

        public LogService(ISessionManager sessionManager, 
            IChannelWriter<ErrorResult> channelWriter,
            IConfiguredMemoryCache cache)
        {
            _sessionManager = sessionManager;
            _channelWriter = channelWriter;
            _cache = cache;
        }

#if DEBUG
        public int GetSessionManagerCount() => _sessionManager.Count;
#endif

        private bool ArePoisoned(IEnumerable<LogMessage> messages)
        {
            // If cache contains session ID, the messages are poisoned
            return messages.FirstOrDefault()?.SessionId is { } g && _cache.TryGetValue(g, out _);
        }

        public Task LogMessages(IEnumerable<LogMessage> messages, DateTime requestTime)
        {
            if (ArePoisoned(messages))
            {
                //messages.ForEach(async m => await PersistError(ExceptionResult.PoisonedMessage(m)));
                return Task.WhenAll(messages.Select(m => PersistError(ExceptionResult.PoisonedMessage(m))));
            }

            var cTokenSource = new CancellationTokenSource();
            return Task.WhenAll(messages.Select(m => LogMessage(m, requestTime, cTokenSource)));
        }



        public async Task LogMessage(LogMessage message, DateTime requestTime, CancellationTokenSource cancellationTokenSource)
        {
            var messageResult = await _sessionManager.AdvanceSession(message, requestTime, cancellationTokenSource.Token);

            if (messageResult is ErrorResult e)
            {
                cancellationTokenSource.Cancel();
                await HandleError(e);
            }

            //switch (messageResult) { 
            //    case ErrorResult e:
            //        cancellationTokenSource.Cancel();
            //        return HandleError(e);
            //    case EndResult t:
            //        _sessionManager.HandleEndResult(t);
            //        break;
            //    case SkippedMessageResult sm:
            //        break;
            //    case SuccessResult s:
            //        break;
            //}

            //return Task.CompletedTask;
        }

        private async Task HandleError(ErrorResult error)
        {
            await _cache.CacheError(error);
            //_sessionErrorHandler.Handle(error);
            await PersistError(error);
        }

        private async Task PersistError(ErrorResult error)
        {
            await _channelWriter.WriteAsync(error);
        }
    }
}