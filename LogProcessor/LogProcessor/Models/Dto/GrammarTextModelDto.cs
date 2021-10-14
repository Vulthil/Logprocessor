using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LogProcessor.Models.Dto
{
    public class GrammarTextModelDto
    {
        [Required]
        public string SessionTypeName { get; set; }
        public int? Id { get; set; }
        [Required]
        public string InputText { get; set; }

        public IEnumerable<string> ExternalParticipants { get; set; } = Array.Empty<string>();
    }
}