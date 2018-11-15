using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Model
{
    public class ResponseModel
    {
        public bool respuesta { get; set; }
        public string redirect { get; set; }
        public string id { get; set; }
        public string error { get; set; }
        public string error2 { get; set; }
        public string extraValue1 { get; set; }
        public string extraValue2 { get; set; }
    }
}
