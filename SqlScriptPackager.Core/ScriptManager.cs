using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SqlScriptPackager.Core
{
    public static class ScriptManager
    {
        private readonly static StringBuilder _logger;

        public static DatabaseConnection[] DatabaseConnections
        {
            get;
            private set;
        }

        public static ScriptCollection Scripts
        {
            get;
            private set;
        }

        public static string Log
        {
            get { return _logger.ToString(); }
        }

        public delegate void LogChanged();
        public static event LogChanged LogUpdated;

        static ScriptManager()
        {
            _logger = new StringBuilder();
            DatabaseConnections = LoadDatabaseConnections();
            Scripts = new ScriptCollection(CreateScripts());
        }

        public static void ExecuteScripts()
        {
            _logger.Clear();

            foreach (Script script in Scripts)
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

        public static Script[] CreateScripts()
        {
            SqlScript scriptA = new SqlScript(new DiskScriptResource(@"C:\Scripts\ScriptA.txt"), DatabaseConnections[0]);
            SqlScript scriptB = new SqlScript(new DiskScriptResource(@"C:\Scripts\ScriptB.txt"), DatabaseConnections[1]);
            return new Script[] { scriptA, scriptB };
        }

        public static DatabaseConnection[] LoadDatabaseConnections()
        {
            List<DatabaseConnection> connections = new List<DatabaseConnection>();

            for (int i = 0; i < 10; i++)
                connections.Add(new DatabaseConnection("connectionName" + i.ToString(), "connectionString" + i.ToString()));

            return connections.ToArray();
        }

        private static void OnScriptStatusUpdated(Script script)
        {
            _logger.Append(script.StatusMessage);
            RaiseLogUpdated();
        }
    }
}
