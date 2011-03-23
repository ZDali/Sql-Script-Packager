using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Core
{
    public abstract class ScriptContentResource
    {
        public abstract string Location
        {
            get;
            protected set;
        }
        
        public abstract string GetContent();
    }
}
