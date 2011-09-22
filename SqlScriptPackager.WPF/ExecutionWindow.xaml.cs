using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using SqlScriptPackager.WPF.DisplayWrappers;
using SqlScriptPackager.Core;
using System.ComponentModel;
using Microsoft.Win32;

namespace SqlScriptPackager.WPF
{
    /// <summary>
    /// Interaction logic for ExecutionWindow.xaml
    /// </summary>
    public partial class ExecutionWindow : Window, INotifyPropertyChanged
    {
        public IEnumerable<ScriptWrapper> Scripts
        {
            get;
            protected set;
        }

        public string Log
        {
            get { return Executor.Log; }
        }

        protected SqlScriptPackager.Core.ScriptExecutor Executor
        {
            get;
            set;
        }

        public ExecutionWindow(IEnumerable<ScriptWrapper> scripts)
        {
            this.Executor = new SqlScriptPackager.Core.ScriptExecutor();
            this.Scripts = scripts;
            Executor.LogUpdated += new SqlScriptPackager.Core.ScriptExecutor.LogChanged(ScriptExecutor_LogUpdated);
            InitializeComponent();
            ExecuteScripts();

            logTextBox.TextChanged += new TextChangedEventHandler(logTextBox_TextChanged);
        }        

        private void ExecuteScripts()
        {
            var extractedScripts = (from ScriptWrapper wrapper in Scripts
                                    select wrapper.Script);

            Executor.ExecuteScripts(extractedScripts);
        }

        private void ScriptExecutor_LogUpdated(string latestMessage)
        {
            RaisePropertyChanged("Log");
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if(Executor.IsExecuting)
            {
                e.Cancel = true;
                Executor.AbortExecution();
            }

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)
        {
            Executor.LogUpdated -= ScriptExecutor_LogUpdated;
            base.OnClosed(e);
        }

        private void logTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            logTextBox.ScrollToEnd();
        }

        private void Close_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            this.Close();
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !Executor.IsExecuting;
        }

        private void Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "log";
            dialog.AddExtension = true;
            dialog.Filter = "Log file (*.log)|*.log";
            
            if (dialog.ShowDialog() != true)
                return;

            System.IO.File.WriteAllText(dialog.FileName, this.Log);
        }

        #region INotifyPropertyChanged Members
        protected void RaisePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
