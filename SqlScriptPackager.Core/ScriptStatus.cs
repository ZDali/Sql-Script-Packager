using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Core
{
    public enum ScriptStatus
    {        
        NotExecuted,
        Disabled,
        Executing,        
        Aborted,
        Failed,        
        Executed
    }
}
