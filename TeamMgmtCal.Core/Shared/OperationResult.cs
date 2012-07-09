using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMgmtCal.Core.Shared
{
    public class OperationResult
    {
        public bool Success { get; set; }
        public string RequestType { get; set; }
        public string RequestStartDate { get; set; }
        public string RequestEndDate { get; set; }
        public string Message { get; set; }
    }
}
