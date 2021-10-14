using System;
using LogProcessor.Model.Json;
using Services.Shared.Models;

namespace LogProcessor.Model.Utilities
{
    public static class LogExtensions
    {
        public static string ToTransitionLabel(this ServiceTransition transition) =>
            string.Join(':', transition.OpposingService, transition.Message, transition.Direction);

        public static string ToTransitionLabel(this LogMessage message) =>
            string.Join(':', message.GetOpposing(), message.TargetApi, message.Direction);


        /// <summary>
        /// Gets the name of the service which the service that made the <see cref="LogMessage"/> is communicating with.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>A string with the serviceId for service which made the request that led to <paramref name="message"/> being logged.</returns>
        public static string GetOpposing(this LogMessage message)
        {
            return message.Direction switch
            {
                Direction.Inbound => message.Origin,
                Direction.Outbound => message.Destination,
                _ => throw new ArgumentOutOfRangeException(nameof(message.Direction),
                    "Direction must either be Inbound or Outbound.")
            };
        }
    }
}