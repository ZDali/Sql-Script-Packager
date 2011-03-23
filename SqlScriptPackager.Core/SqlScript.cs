using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.SqlServer.Management.Common;
using System.Data.SqlClient;

namespace SqlScriptPackager.Core
{
    public class SqlScript : Script
    {
        public override string ScriptName
        {
            get { return ContentResource.Location; }
        }

        public override string ScriptType
        {
            get { return "Sql Script"; }
        }

        public override string ScriptContents
        {
            get { return this.ContentResource.GetContent(); }
        }

        public SqlScript(ScriptContentResource resource, DatabaseConnection connection) : base(resource, connection)
        {

        }

        public SqlScript()
        {

        }

        protected override void ExecuteScriptInternal()
        {
            using (SqlConnection connection = this.Connection.CreateSqlConnection())
            {
                connection.InfoMessage += new SqlInfoMessageEventHandler(OnInfoMessageUpdated);
                
                Server server = new Server(new ServerConnection());
                server.ConnectionContext.ExecuteNonQuery(this.ScriptContents);

                connection.InfoMessage -= OnInfoMessageUpdated;
            }
        }

        private void OnInfoMessageUpdated(object sender, SqlInfoMessageEventArgs e)
        {
            this.StatusMessage = e.Message;
        }
    }
}
