using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SqlScriptPackager.Core
{
    public abstract class Script
    {
        #region Fields
        private string _statusMessage;
        private ScriptStatus _status;
        #endregion

        #region Properties
        public abstract string ScriptName
        {
            get;
        }

        public abstract string ScriptType
        {
            get;
        }

        public abstract string ScriptContents
        {
            get;
        }

        public DatabaseConnection Connection
        {
            get;
            set;
        }

        public ScriptContentResource ContentResource
        {
            get;
            protected set;
        }

        public ScriptStatus Status
        {
            get { return _status; }
            protected set { _status = value; RaiseStatusChanged(); }
        }

        public virtual string StatusMessage
        {
            get { return _statusMessage; }
            set { _statusMessage = value; RaiseStatusMessageChanged(); }
        }

        public virtual bool IsEnabled
        {
            get { return this.Status != ScriptStatus.Disabled; }
            set { this.Status = value ? ScriptStatus.NotExecuted : ScriptStatus.Disabled; }
        }
        #endregion

        public delegate void ScriptChange(Script script);
        public event ScriptChange StatusMessageChanged;
        public event ScriptChange StatusChanged;
        

        protected Script(ScriptContentResource resource, DatabaseConnection connection)
        {
            InitializeScript(resource, connection);
        }

        protected Script()
        {

        }

        public virtual void InitializeScript(ScriptContentResource resource, DatabaseConnection connection)
        {
            this.ContentResource = resource;
            this.Connection = connection;
        }

        public virtual void ExecuteScript()
        {
            this.Status = ScriptStatus.Executing;

            try
            {
                this.ExecuteScriptInternal();

                if (this.Status == ScriptStatus.Executing)
                {
                    this.Status = ScriptStatus.Executed;
                    this.StatusMessage = this.ScriptType + " successfully executed.";
                }
            }
            catch (Exception ex) // generally a bad idea
            {
                this.Status = ScriptStatus.Failed;
                this.StatusMessage = ex.ToString();
            }
        }

        public virtual void AbortExecution()
        {
            this.Status = ScriptStatus.Aborted;
        }

        public virtual void ResetScript()
        {
            if(this.Status != ScriptStatus.Disabled)
                this.Status = ScriptStatus.NotExecuted;

            this.StatusMessage = string.Empty;
        }

        protected abstract void ExecuteScriptInternal();

        protected void RaiseStatusMessageChanged()
        {
            if (this.StatusMessageChanged != null)
                this.StatusMessageChanged(this);
        }

        protected void RaiseStatusChanged()
        {
            if (this.StatusChanged != null)
                this.StatusChanged(this);
        }
    }
}
