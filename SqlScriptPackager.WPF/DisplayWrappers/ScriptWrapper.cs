using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlScriptPackager.Core;

namespace SqlScriptPackager.WPF.DisplayWrappers
{
    public class ScriptWrapper
    {
        private readonly Script _script;

        public string Location
        {
            get { return _script.ContentResource.Location; }
        }

        public DatabaseConnection Connection
        {
            get { return _script.Connection; }
            set { _script.Connection = value; }
        }

        public bool IsEnabled
        {
            get { return _script.IsEnabled; }
            set { _script.IsEnabled = value; }
        }

        public ScriptWrapper(Script script)
        {
            this._script = script;
        }
    }
}
