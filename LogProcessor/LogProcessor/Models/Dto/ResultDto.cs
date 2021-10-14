using LogProcessor.Model.Utilities;

namespace LogProcessor.Models.Dto
{
    public class ResultDto
    {
        public bool Success => Error.IsNullOrWhiteSpace();
        public string Error { get; set; }
        public ResultDto(string error)
        {
            Error = error;
        }

        public ResultDto()
        {

        }
    }
}