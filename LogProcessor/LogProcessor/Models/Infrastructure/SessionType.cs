using System.ComponentModel.DataAnnotations.Schema;

namespace LogProcessor.Models.Infrastructure
{
    public class SessionType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Text { get; set; }
        public bool ShouldLoad { get; set; }

        [Column(TypeName = "jsonb")]
        public string[] InternalParticipants { get; set; }

        [Column(TypeName = "jsonb")]
        public string[] ExternalParticipants { get; set; }
    }
}