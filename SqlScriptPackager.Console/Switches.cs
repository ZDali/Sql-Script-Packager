using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Console
{
    [Flags]
    enum Switches
    {
        None = 0,
        Verbose = 1,
        AbortOnFailure = 2,
        Quiet = 4
    }
}
