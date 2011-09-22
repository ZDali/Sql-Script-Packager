using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SqlScriptPackager.Core;
using System.ComponentModel;

namespace SqlScriptPackager.WPF.DisplayWrappers
{
    public class ScriptWrapper : INotifyPropertyChanged
    {
        private readonly Script _script;

        #region Properties
        public string Location
        {
            get { return _script.ContentResource.Location; }
        }

        public DatabaseConnection Connection
        {
            get { return _script.Connection; }
            set { _script.Connection = value; RaisePropertyChanged("Connection"); }
        }

        public bool IsEnabled
        {
            get { return _script.IsEnabled; }
            set { _script.IsEnabled = value; }
        }

        public ScriptStatus Status
        {
            get { return _script.Status; }
        }

        public string StatusMessage
        {
            get { return _script.StatusMessage; }
        }

        public string ScriptType
        {
            get { return this.Script.ScriptType; }
        }

        public Script Script
        {
            get { return _script; }
        }
        #endregion

        public ScriptWrapper(Script script)
        {
            this._script = script;
            this._script.StatusMessageChanged += new Core.Script.ScriptChange(OnStatusInformationChanged);
            this._script.StatusChanged += new Core.Script.ScriptChange(OnStatusChanged);
        }

        private void OnStatusChanged(Script script)
        {
            RaisePropertyChanged("Status");
        }

        private void OnStatusInformationChanged(Script script)
        {
            RaisePropertyChanged("StatusMessage");
        }

        #region INotifyPropertyChanged Members

        private void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
