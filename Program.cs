using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MyDAL
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnectionStringBuilder cnStringbuilder = new SqlConnectionStringBuilder();

            cnStringbuilder.InitialCatalog = @"AdventureWorks2";

            cnStringbuilder.DataSource = @"DESKTOP-7S9MC1D\SQLEXPRESS";

            cnStringbuilder.IntegratedSecurity = true;

            DAL dAL = new DAL();

            dAL.ConnectionType = ConnectionTypes.Sql;
            dAL.ConnectionString = cnStringbuilder.ConnectionString;
            dAL.Connect();

            List<KeyValuePair<string, object>> kvList = new List<KeyValuePair<string, object>>();

            kvList.Add(new KeyValuePair<string, object>("FirstName", "Syed"));

            var PersonList = dAL.GetData<Person>("GetPersonData", kvList);

            foreach(var x in PersonList)
            {
                Console.WriteLine(x.FirstName + " : " + x.LastName);
            }

            Console.Read();

        }
    }
}
