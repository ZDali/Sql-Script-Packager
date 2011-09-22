using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace SqlScriptPackager.Core
{
    public class ScriptProviderCollection : ProviderCollection
    {
        /// <summary>
        /// Returns an instance of a ScriptProvider with the 
        /// specified provider name.
        /// </summary>
        new public ScriptProvider this[string name]
        {
            get { return (ScriptProvider)base[name]; }
        }
    }
}
