using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Core
{
    public static class ScriptExecutor
    {
        private readonly static StringBuilder _logger;

        public static string Log
        {
            get { return _logger.ToString(); }
        }

        public delegate void LogChanged();
        public static event LogChanged LogUpdated;

        static ScriptExecutor()
        {
            _logger = new StringBuilder();
        }

        public static void ExecuteScripts(IEnumerable<Script> scripts)
        {
            _logger.Clear();

            foreach (Script script in scripts)
            {
                if (script.Status == ScriptStatus.Disabled)
                    continue;

                script.StatusInformationChanged += new Script.ScriptChange(OnScriptStatusUpdated);
                script.ExecuteScript();
                script.StatusInformationChanged -= new Script.ScriptChange(OnScriptStatusUpdated);
            }
            
        }        

        private static void RaiseLogUpdated()
        {
            if (LogUpdated != null)
                LogUpdated();
        }

        private static void OnScriptStatusUpdated(Script script)
        {
            _logger.Append(script.StatusMessage);
            RaiseLogUpdated();
        }
    }
}
