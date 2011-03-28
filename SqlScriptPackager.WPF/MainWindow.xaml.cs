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
using System.Windows.Navigation;
using System.Windows.Shapes;
using SqlScriptPackager.Core;
using System.Collections.ObjectModel;
using SqlScriptPackager.WPF.DisplayWrappers;
using System.Collections;
using Microsoft.Win32;
using SqlScriptPackager.Core.Packaging;

namespace SqlScriptPackager.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string SCRIPT_PACKAGE_FILTER = "Script Package (*.sp)|*.sp";

        private static OpenFileDialog _sqlFileDialog = new OpenFileDialog();

        public DatabaseConnection DefaultDatabaseConnection
        {
            get;
            set;
        }

        public Collection<DatabaseConnection> DatabaseConnections
        {
            get;
            protected set;
        }

        public ObservableCollection<ScriptWrapper> Scripts
        {
            get;
            protected set;
        }

        public IList SelectedScripts
        {
            get { return scriptListView.SelectedItems; }
        }

        public MainWindow()
        {
            _sqlFileDialog.Filter = "Sql Scripts (*.sql)|*.sql";
            _sqlFileDialog.CheckFileExists = true;
            _sqlFileDialog.Multiselect = true;

            DatabaseConnections = new Collection<DatabaseConnection>();
            Scripts = new ObservableCollection<ScriptWrapper>();
            LoadConnectionStrings();
         
            InitializeComponent();
        }

        private void LoadConnectionStrings()
        {
            for (int i = 0; i < 10; i++)
                this.DatabaseConnections.Add(new DatabaseConnection("Connection " + i.ToString(), i.ToString()));
            
            this.DefaultDatabaseConnection = this.DatabaseConnections[0];
        }

        private void MoveScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            int direction = int.Parse(e.Parameter as string);
            var indiciesToMove = (from ScriptWrapper wrapper in SelectedScripts
                                    orderby Scripts.IndexOf(wrapper)
                                    select Scripts.IndexOf(wrapper));

            if (direction > 0)
                 indiciesToMove = indiciesToMove.Reverse();

            int ceilingIndex = -1;
            foreach(int curIndex in indiciesToMove)
            {
                if (curIndex + direction >= Scripts.Count || curIndex + direction < 0 || curIndex + direction == ceilingIndex)
                {
                    ceilingIndex = curIndex;
                    continue;
                }

                Scripts.Move(curIndex, curIndex + direction);
            }
        }

        private void MoveScript_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedScripts.Count > 0;
        }

        private void AddScript_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (_sqlFileDialog.ShowDialog() != true)
                return;
            
            foreach (string file in _sqlFileDialog.FileNames)
                Scripts.Add(new ScriptWrapper(new SqlScript(new DiskScriptResource(file), DefaultDatabaseConnection)));
        }

        private void DeleteScripts_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectedScripts.Count > 0;
        }

        private void DeleteScripts_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            while(SelectedScripts.Count > 0)
                Scripts.Remove(SelectedScripts[0] as ScriptWrapper);
        }

        private void SaveScriptPackage_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Scripts.Count > 0;
        }

        private void SaveScriptPackage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog packageSave = new SaveFileDialog();
            packageSave.CheckFileExists = false;
            packageSave.Filter = SCRIPT_PACKAGE_FILTER;
            packageSave.AddExtension = true;
            packageSave.DefaultExt = "sp";

            if (packageSave.ShowDialog() != true)
                return;

            ScriptPackage package = new ScriptPackage();
            var scripts = (from ScriptWrapper wrapper in Scripts
                           select wrapper.Script);

            package.Scripts.Add(scripts);
            package.Save(packageSave.FileName);
        }

        private void LoadScriptPackage_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog packageLoad = new OpenFileDialog();
            packageLoad.Filter = SCRIPT_PACKAGE_FILTER;
            packageLoad.CheckFileExists = true;

            if (packageLoad.ShowDialog() != true)
                return;

            ScriptPackage package = new ScriptPackage();
            package.Load(packageLoad.FileName);

            this.Scripts.Clear();
            foreach (Script script in package.Scripts)
            {
                DatabaseConnection matchingConnection = (from connection in DatabaseConnections
                                                         where connection.ConnectionString == script.Connection.ConnectionString
                                                         select connection).FirstOrDefault();

                if (matchingConnection == null)
                {
                    matchingConnection = new DatabaseConnection("[PACKAGE] " + script.Connection.ConnectionName, script.Connection.ConnectionString);
                    this.DatabaseConnections.Add(matchingConnection);
                }

                script.Connection = matchingConnection;
                this.Scripts.Add(new ScriptWrapper(script));
            }
        }

        private void ViewConnectionInfo_Executed(object sender, ExecutedRoutedEventArgs e)
        {

        }

    }

    public static class Commands
    {
        public static RoutedUICommand MoveScripts = new RoutedUICommand();
        public static RoutedUICommand AddScript = new RoutedUICommand();
        public static RoutedUICommand AddCustomScript = new RoutedUICommand();
        public static RoutedUICommand DeleteScripts = new RoutedUICommand();
        public static RoutedUICommand SaveScriptPackage = new RoutedUICommand();
        public static RoutedUICommand LoadScriptPackage = new RoutedUICommand();
        public static RoutedUICommand ViewConnectionInfo = new RoutedUICommand();
    }    
}
