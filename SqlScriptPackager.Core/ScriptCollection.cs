using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace SqlScriptPackager.Core
{
    public class ScriptCollection : Collection<Script>
    {
        public ScriptCollection()
            : base()
        {

        }

        public ScriptCollection(IList<Script> scripts)
            : base(scripts)
        {

        }
    }
}
