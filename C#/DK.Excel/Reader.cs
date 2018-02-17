using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace DK.Excel
{
    internal class Reader
    {
        public Reader(String fileFullPath)
        {
            var factory = DbProviderFactories.GetFactory("System.Data.OleDb");

            using (var excelConnection = factory.CreateConnection())
            {
                var command = connect(excelConnection, fileFullPath);

                using (var dr = command.ExecuteReader())
                {
                    read(dr);
                }
            }
        }

        private void read(IDataReader dr)
        {
            data = new List<IDictionary<String, String>>();
            var getNext = true;

            while (dr.Read() && getNext)
            {
                var row = new Dictionary<String, String>();
                var filled = false;


                for (var i = 0; i < dr.FieldCount; i++)
                {
                    var value = dr[i].ToString();

                    if (String.IsNullOrEmpty(value))
                        continue;

                    filled = true;
                    row.Add(dr.GetName(i), value);
                }


                if (filled)
                    data.Add(row);
                else
                    getNext = false;
            }
        }


        private IList<IDictionary<String, String>> data;




        public String this[Int32 rowIndex, String columnName] =>
	        data.Count > rowIndex && data[rowIndex].Keys.Contains(columnName)
				? data[rowIndex][columnName]
				: null;

	    public Boolean IsFilled(Int32 rowIndex, String columnName)
        {
            return this[rowIndex, columnName] != null;
        }

        public Int32 RowCount => data.Count;


	    private static DbCommand connect(DbConnection excelConnection, string fileFullPath)
        {
            if (excelConnection == null)
                throw new Exception("File not found");

            var connectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileFullPath + ";Extended Properties='Excel 8.0;HDR=Yes;'";
            excelConnection.ConnectionString = connectionString;
            excelConnection.Open();

            var command = excelConnection.CreateCommand();
            var dataTable = excelConnection.GetSchema("Tables");
            var sheetName = dataTable.Rows[0]["TABLE_NAME"].ToString();

            command.CommandText = "SELECT * FROM [" + sheetName + "]";
            return command;
        }

    }
}
