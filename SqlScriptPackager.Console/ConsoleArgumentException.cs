using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Console
{
    public class ConsoleArgumentException : Exception
    {
        public ConsoleArgumentException() : base()
        {

        }

        public ConsoleArgumentException(string message) : base(message)
        {

        }
    }
}
