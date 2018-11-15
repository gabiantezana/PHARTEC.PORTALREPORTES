using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class EmailModel
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public int solID { get; set; }
        public string userName { get; set; }
        public string extraValue { get; set; }
    }
}
