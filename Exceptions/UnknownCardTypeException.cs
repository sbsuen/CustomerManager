using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManager.Exceptions
{
    public class UnknownCardTypeException : Exception
    {
        public UnknownCardTypeException() { }
        
        public UnknownCardTypeException(string message) : base(message) { }

        public UnknownCardTypeException(string message, Exception inner) : base(message, inner) { }

    }
}
