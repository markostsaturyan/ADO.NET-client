using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;

namespace MyDAL
{
    public class DAL
    {
        private DbConnection connection;

        public DbConnection Connection
        {
            get
            {
                return this.connection;
            }
        }

        private DbCommand command;

        public ConnectionTypes ConnectionType;

        private string connectionString;

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                this.connectionString = value;
            }
        }

        public DAL()
        {
        }

        public IEnumerable<T> GetData<T>(string code, ICollection<KeyValuePair<string, object>> parameters)
        {
            this.command.Connection = this.connection;

            var fileText = File.ReadAllLines(@"C:\Users\marko\source\repos\MyDAL\MyDAL\CodeFile.txt");

            for(int i=0; i < fileText.Length; i++)
            {
                if (fileText[i].Contains(code))
                {
                    if (fileText[i + 1].Contains("StoredProcedure"))
                    {
                        this.command.CommandType = System.Data.CommandType.StoredProcedure;
                    }
                    else
                    {
                        this.command.CommandType = System.Data.CommandType.Text;
                    }

                    this.command.CommandText = fileText[i + 2].Split(':')[1];

                    var param = fileText[i + 3].Split(':');

                    CreateParametrs(param[1], parameters);
                    break;
                }
            }

            if (this.command.CommandText == null)
            {
                throw new Exception("Operation is not found");
            }

            this.command.Connection.Open();

            var dataReader = this.command.ExecuteReader();

            List<T> entityes = new List<T>();

            var propInfo = typeof(T).GetProperties();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    var constroctors = typeof(T).GetConstructors();

                    var entity = constroctors[0].Invoke(new Object[] { });

                    foreach(var prop in propInfo)
                    {
                        var value = dataReader[prop.Name];
                        typeof(T).GetProperty(prop.Name).SetValue(entity,value);
                    }
                    
                    entityes.Add((T)entity);
                }
            }

            this.command.Connection.Close();

            return entityes;
        }

        public void Connect()
        {
            switch (ConnectionType)
            {
                case ConnectionTypes.Sql:
                    {
                        this.connection = new SqlConnection(ConnectionString);
                        this.command = new SqlCommand();

                        break;
                    }
                case ConnectionTypes.OleDb:
                    {
                        this.connection = new OleDbConnection(ConnectionString);
                        this.command = new OleDbCommand();

                        break;
                    }
                case ConnectionTypes.Odbc:
                    {
                        this.connection = new OdbcConnection(ConnectionString);
                        this.command = new OdbcCommand();

                        break;
                    }
            }
        }

        private void CreateParametrs(string paramCompliance, ICollection<KeyValuePair<string, object>> parameters)
        {
            var paramsCmp = paramCompliance.Split(new char[] { '-', ',', }, StringSplitOptions.RemoveEmptyEntries);

            switch (ConnectionType)
            {
                case ConnectionTypes.Sql:
                    {
                        for(int i = 1; i < paramsCmp.Length; i+=2)
                        {
                            object par = null;

                            foreach(var a in parameters)
                            {
                                if (a.Key == paramsCmp[i])
                                {
                                    par = a.Value;
                                }
                            }
                            
                            this.command.Parameters.Add(new SqlParameter(paramsCmp[i-1],par));
                        }

                        break;
                    }
                case ConnectionTypes.OleDb:
                    {
                        for (int i = 1; i < paramsCmp.Length; i += 2)
                        {
                            object par = null;

                            foreach (var a in parameters)
                            {
                                if (a.Key == paramsCmp[i])
                                {
                                    par = a.Value;
                                }
                            }

                            this.command.Parameters.Add(new OleDbParameter(paramsCmp[i - 1], par));
                        }

                        break;
                    }
                case ConnectionTypes.Odbc:
                    {
                        for (int i = 1; i < paramsCmp.Length; i += 2)
                        {
                            object par = null;

                            foreach (var a in parameters)
                            {
                                if (a.Key == paramsCmp[i])
                                {
                                    par = a.Value;
                                }
                            }

                            this.command.Parameters.Add(new OdbcParameter(paramsCmp[i - 1], par));
                        }
                        break;
                    }
            }
        }
    }
}
