using System.Collections.Generic;

namespace RavenDesk.Core.Data
{
    public class DataObjectOperationResult
    {
        public DataObjectOperationResult()
        {
            Success = true;
            ErrorMessages = new Dictionary<string, string>();
        }

        public bool Success { get; set; }
        public string Message { get; set; }

        public Dictionary<string, string> ErrorMessages { get; set; }

    }
}
