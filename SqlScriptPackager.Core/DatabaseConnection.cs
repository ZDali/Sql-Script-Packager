using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Xml.Serialization;

namespace SqlScriptPackager.Core
{
    public class DatabaseConnection
    {
        public string ConnectionName { get; protected set; }

        public string ConnectionString { get; protected set; }

        public DatabaseConnection(string connectionName, string connectionString)
        {
            this.ConnectionName = connectionName;
            this.ConnectionString = connectionString;
        }

        public SqlConnection CreateSqlConnection()
        {
            return new SqlConnection(this.ConnectionString);
        }
    }
}
