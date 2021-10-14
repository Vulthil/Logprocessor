using System;
using Services.Shared.Models;

namespace LogProcessor.Model.Utilities
{
    public static class EnumExtensions
    {
        public static string ToString(this Direction d)
        {
            switch (d)
            {
                case Direction.Inbound:
                    return "receiving";
                case Direction.Outbound:
                    return "sending";
                default:
                    throw new ArgumentOutOfRangeException(nameof(d), d, null);
            }
        }

    }
    public enum TIMEOUT_TYPES
    {
        NO_RECEIVE_LOGS_TIMEOUT,
        MISMATCH_MESSAGES_TIMEOUT
    }
}