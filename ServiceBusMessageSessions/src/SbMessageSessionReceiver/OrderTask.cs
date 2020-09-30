using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SbMessageSessionsConsoleApp
{
    public class OrderTask
    {
        public string Id { get; set; }
        public string Index { get; set; }
        public string OrderId { get; set; }
        public string TaskId { get; set; }
        public string JobState { get; set; }
    }
}
