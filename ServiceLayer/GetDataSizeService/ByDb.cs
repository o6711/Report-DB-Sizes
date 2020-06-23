using Npgsql;
using ServiceLayer.DataBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.GetDataSizeService
{
    public class ByDb : IGetData
    {
        private const string _SQLQuery = "SELECT pg_database_size(current_database()) as size;";

        public static List<string> DBConnectionsList;

        public IEnumerable<Data> GetData()
        {
            StringBuilder size = new StringBuilder();
            StringBuilder serverName = new StringBuilder();
            StringBuilder dbName = new StringBuilder();

            foreach (var connectionString in DBConnectionsList)
            {
                ParseConnectionString(connectionString, ref serverName, ref dbName);
                ReadDbSize(connectionString, ref size);

                yield return new Data()
                {
                    Allocation = serverName.ToString(),
                    SourceFileName = dbName.ToString(),
                    Size = size.ToString()
                };

                serverName.Clear();
                dbName.Clear();
                size.Clear();
            }
        }

        #region The class helpers
        private void ReadDbSize(string connectionString, ref StringBuilder size)
        {
            DataSet oDataSet = new DataSet();
            using NpgsqlConnection oConn = new NpgsqlConnection(connectionString);
            oConn.Open();

            using NpgsqlCommand oCmdGetData = new NpgsqlCommand(_SQLQuery, oConn);
            oCmdGetData.ExecuteNonQuery();

            NpgsqlDataAdapter executeAdapter = new NpgsqlDataAdapter(oCmdGetData);
            executeAdapter.Fill(oDataSet);

            oConn.Close();

            size.Append(ConvertBytesToGigaBytes(Convert.ToInt32(oDataSet.Tables[0].Rows[0].ItemArray[0])).ToString());
        }

        private double ConvertBytesToGigaBytes(double bytesFormat) => Math.Round(bytesFormat / 1024 / 1024 / 1024, 3);

        private void ParseConnectionString(string connectionString, ref StringBuilder serverName, ref StringBuilder dbName)
        {
            //connectionString example: "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=321;"
            
            foreach (var item in connectionString
                                 .Split(';', StringSplitOptions.RemoveEmptyEntries)
                                 .Select(item => item.Split('='))
                                 .Where(item => item[0].Equals("Server") || item[0].Equals("Database")))
            {
                switch (item[0])
                {
                    case "Server":
                        serverName.Append(item[1]);
                        break;
                    case "Database":
                        dbName.Append(item[1]);
                        break;
                    default:
                        throw new Exception("ParseConnectionString > connection string is wrong");
                }
            }
        }

        #endregion
    }
}
