using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Provider;

namespace SqlScriptPackager.Core
{
    public abstract class ScriptProvider : ProviderBase
    {
        public abstract string ProviderName
        {
            get;
        }

        public abstract IEnumerable<Script> GetTasks(DatabaseConnection defaultDatabaseConnection);
    }
}
