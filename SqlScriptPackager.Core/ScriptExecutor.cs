using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SqlScriptPackager.Core
{
    public class ScriptExecutor
    {
        private readonly StringBuilder _logger;

        protected ParameterizedThreadStart ExecutionDelegate
        {
            get;
            set;
        }

        public string Log
        {
            get { return _logger.ToString(); }
        }

        public delegate void LogChanged();
        public event LogChanged LogUpdated;

        public bool IsExecuting
        {
            get;
            protected set;
        }

        protected bool ExecutionAborted
        {
            get;
            set;
        }

        public ScriptExecutor()
        {
            _logger = new StringBuilder();
        }

        public void ExecuteScripts(IEnumerable<Script> scripts)
        {
            IsExecuting = true;
            ExecutionDelegate = ExecuteScriptsInternal;
            ExecutionDelegate.BeginInvoke(scripts, ExecuteScriptsCallback, null);
        }

        public void AbortExecution()
        {
            this.ExecutionAborted = true;
        }

        protected void ExecuteScriptsCallback(IAsyncResult result)
        {
            IsExecuting = false;
            ExecutionDelegate.EndInvoke(result);
        }

        protected void ExecuteScriptsInternal(object scriptsObject)
        {
            IEnumerable<Script> scripts = scriptsObject as IEnumerable<Script>;
            _logger.Clear();

            foreach (Script script in scripts)
            {
                if (this.ExecutionAborted)
                {
                    script.AbortExecution();
                    break;
                }

                if (script.Status == ScriptStatus.Disabled)
                    continue;

                AppendLogMessage("Executing " + script.ContentResource.Location);
                script.StatusMessageChanged += new Script.ScriptChange(OnScriptStatusUpdated);
                script.ExecuteScript();
                script.StatusMessageChanged -= new Script.ScriptChange(OnScriptStatusUpdated);
            }
        }

        void script_StatusChanged(Script script)
        {
            throw new NotImplementedException();
        }

        private void AppendLogMessage(string message)
        {
            _logger.AppendLine(message);
            RaiseLogUpdated();
        }

        private void RaiseLogUpdated()
        {
            if (LogUpdated != null)
                LogUpdated();
        }

        private void OnScriptStatusUpdated(Script script)
        {
            AppendLogMessage(script.StatusMessage);
        }
    }
}
