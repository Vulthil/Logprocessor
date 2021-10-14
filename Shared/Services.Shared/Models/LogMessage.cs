using System;

namespace Services.Shared.Models
{
    public class LogMessage
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public string TargetApi { get; set; }
        public string SessionId { get; set; }
        public Direction Direction { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;


        public override string ToString()
        {
            return System.Text.Json.JsonSerializer.Serialize(this);
        }

        public bool IsOutbound => Direction == Direction.Outbound;
        public bool IsInbound => Direction == Direction.Inbound;


        /// <summary>
        /// Gets the name of the service that logged the current <see cref="LogMessage"/>
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The serviceId of the service that logged <paramref name="message"/>.</returns>
        public string Originator =>
            Direction switch
            {
                Direction.Inbound => Destination,
                Direction.Outbound => Origin,
                _ => throw new ArgumentOutOfRangeException(nameof(Direction),
                    "Direction must either be Inbound or Outbound.")
            };
        
    }
}
