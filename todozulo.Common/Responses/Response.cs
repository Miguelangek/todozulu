using System;
using System.Collections.Generic;
using System.Text;

namespace todozulo.Common.Responses
{
     public class Response
    {
        public Boolean ItSuccess { get; set; }

        public string Message { get; set; }

        public object Result { get; set; }
    }
}
