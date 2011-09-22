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

        protected delegate void ExecuteScriptsAction(IEnumerable<Script> script, bool abortOnError);
        protected ExecuteScriptsAction ExecutionDelegate
        {
            get;
            set;
        }

        public string Log
        {
            get { return _logger.ToString(); }
        }

        public delegate void LogChanged(string latestMessage);
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
            ExecuteScripts(scripts, false);
        }

        public void ExecuteScripts(IEnumerable<Script> scripts, bool abortOnError)
        {
            IsExecuting = true;
            ExecutionDelegate = ExecuteScriptsInternal;
            ExecutionDelegate.BeginInvoke(scripts, abortOnError, ExecuteScriptsCallback, null);
        }

        public void AbortExecution()
        {
            AppendLogMessage("Execution aborted.");
            this.ExecutionAborted = true;
        }

        protected void ExecuteScriptsCallback(IAsyncResult result)
        {
            IsExecuting = false;
            ExecutionDelegate.EndInvoke(result);
        }

        protected void ExecuteScriptsInternal(IEnumerable<Script> scripts, bool abortOnError)
        {
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
                AppendLogMessage("Using connection '" + script.Connection.ConnectionName + "'");
                script.StatusMessageChanged += new Script.ScriptChange(OnScriptStatusUpdated);
                script.ExecuteScript();
                script.StatusMessageChanged -= new Script.ScriptChange(OnScriptStatusUpdated);

                if (script.Status == ScriptStatus.Executed)
                    AppendLogMessage(script.ContentResource.Location + " executed.");
                else if (abortOnError)
                    this.AbortExecution();
            }
        }

        private void AppendLogMessage(string message)
        {
            string messageDateTime = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + " - ";
            string indent = messageDateTime.PadLeft(messageDateTime.Length);

            string logMessage = string.Concat(messageDateTime, message.Replace(Environment.NewLine, Environment.NewLine + indent));
            _logger.AppendLine(logMessage);
            RaiseLogUpdated(logMessage);
        }

        private void RaiseLogUpdated(string latestMessage)
        {
            if (LogUpdated != null)
                LogUpdated(latestMessage);
        }

        private void OnScriptStatusUpdated(Script script)
        {
            AppendLogMessage(script.StatusMessage);
        }
    }
}
