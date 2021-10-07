using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManager.Exceptions
{
    public class InvalidCardFormatException : Exception
    {
        public InvalidCardFormatException() { }

        public InvalidCardFormatException(string message) : base(message) { }

        public InvalidCardFormatException(string message, Exception inner) : base(message, inner) { }
    }
}
