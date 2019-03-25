using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AgentHub.Entities.Utilities
{
    /// <summary>
    /// Static class providing data access functionalities using ADO.NET
    /// </summary>
    public static class SQLHelper
    {

        private static string ConnectionString { get; set; }

        private static string _connectionStringKey;
        /// <summary>
        /// Gets or sets the connection string key.
        /// </summary>
        /// <value>
        /// The connection string key.
        /// </value>
        public static string ConnectionStringKey
        {
            get
            {
                return _connectionStringKey;
            }
            set
            {
                _connectionStringKey = value;
                ConnectionString = ConfigurationManager.ConnectionStrings[_connectionStringKey].ConnectionString;
            }
        }

        static SQLHelper()
        {
            ConnectionStringKey = "AgentHubDataContext";
        }

        /// <summary>
        /// Creates a sql command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <returns>
        /// Sql command created.
        /// </returns>
        public static SqlCommand CreateCommand(string commandText, CommandType commandType)
        {
            var cmd = new SqlCommand
            {
                CommandText = commandText,
                CommandType = commandType,
                CommandTimeout = 60
            };

            return cmd;
        }

        /// <summary>
        /// Execute the SqlCommand and fill the data queried to a table
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// A data table.
        /// </returns>
        public static DataTable QueryDataTable(SqlCommand command, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ConnectionString;

            using (var connection = new SqlConnection { ConnectionString = connectionString })
            {
                using (var da = new SqlDataAdapter(command))
                {
                    connection.Open();
                    command.Connection = connection;
                    var dataSet = new DataSet();
                    da.Fill(dataSet);
                    connection.Close();

                    return dataSet.Tables[0];
                }
            }
        }

        /// <summary>
        /// Define the SqlCommand then Execute it and fill the data queried to a table
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>
        /// A data table.
        /// </returns>
        public static DataTable QueryDataTable(string sqlQuery, CommandType commandType, string connectionString = null)
        {
            return QueryDataTable(CreateCommand(sqlQuery, commandType), connectionString);
        }

        /// <summary>
        /// Executes non query with SqlCommand.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Execution status</returns>
        public static async Task<int> ExecuteNonQuery(SqlCommand command, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ConnectionString;

            using (var connection = new SqlConnection { ConnectionString = connectionString })
            {
                connection.Open();
                command.Connection = connection;
                var result = await command.ExecuteNonQueryAsync();
                connection.Close();

                return result;
            }
        }

        /// <summary>
        /// Executes non query with sql query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns>Execution status</returns>
        public static int ExecuteNonQuery(string queryString, string connectionString = null)
        {
            return ExecuteNonQuery(new SqlCommand(queryString), connectionString).Result;
        }

        /// <summary>
        /// Executes scalar with SqlCommand.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static object ExecuteScalar(SqlCommand command, string connectionString = null)
        {
            if (string.IsNullOrEmpty(connectionString))
                connectionString = ConnectionString;

            using (var connection = new SqlConnection { ConnectionString = connectionString })
            {
                connection.Open();
                command.Connection = connection;
                var result = command.ExecuteScalar();
                connection.Close();

                return result;
            }
        }

        /// <summary>
        /// Executes scalar with Sql query string.
        /// </summary>
        /// <param name="queryString">The query string.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public static object ExecuteScalar(string queryString, string connectionString = null)
        {
            return ExecuteScalar(new SqlCommand(queryString), connectionString);
        }

        /// <summary>
        /// Creates the in parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="value">The value.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <param name="size">The size.</param>
        /// <returns>
        /// Sql parameter.
        /// </returns>
        public static SqlParameter CreateInParameter(string parameterName, object value,
            SqlDbType sqlDbType = SqlDbType.VarChar, int size = 0)
        {
            if (size > 0)
            {
                return new SqlParameter
                {
                    ParameterName = parameterName,
                    SqlDbType = sqlDbType,
                    Direction = ParameterDirection.Input,
                    Value = value ?? DBNull.Value,
                    Size = size
                };
            }

            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Direction = ParameterDirection.Input,
                Value = value ?? DBNull.Value
            };
        }

        /// <summary>
        /// Creates the out parameter.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="sqlDbType">Type of the SQL database.</param>
        /// <returns>
        /// Sql parameter.
        /// </returns>
        public static SqlParameter CreateOutParameter(string parameterName, SqlDbType sqlDbType = SqlDbType.Int, int size = 0)
        {
            return new SqlParameter
            {
                ParameterName = parameterName,
                SqlDbType = sqlDbType,
                Direction = ParameterDirection.Output,
                Size = (size == 0 ? Int32.MaxValue : size)
            };
        }

        /// <summary>
        /// Creates data table from source data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IEnumerable<T> source)
        {
            var table = new DataTable();

            //// get properties of T
            const BindingFlags binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
            const PropertyReflectionOptions options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer;

            var properties = ReflectionExtensions.GetProperties<T>(binding, options).ToList();

            //// create table schema based on properties
            foreach (var property in properties)
            {
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            //// create table data from T instances
            var values = new object[properties.Count];

            foreach (var item in source)
            {
                for (var i = 0; i < properties.Count; i++)
                {
                    values[i] = properties[i].GetValue(item, null);
                }

                table.Rows.Add(values);
            }

            return table;
        }

        /// <summary>
        /// Convert a DataTable to a list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable">The data table.</param>
        /// <returns>
        /// List of the objects contained in the data table.
        /// </returns>
        public static List<T> ToList<T>(this DataTable dataTable)
        {
            return (from DataRow row in dataTable.Rows select GetItem<T>(row)).ToList<T>();
        }

        /// <summary>
        /// Get the first or default object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataTable">The data table.</param>
        /// <returns>
        /// Object found.
        /// </returns>
        public static Object FirstOrDefault<T>(this DataTable dataTable)
        {
            return (from DataRow row in dataTable.Rows select GetItem<T>(row)).FirstOrDefault();
        }

        /// <summary>
        /// Gets the item by DataRow.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRow">The data row.</param>
        /// <returns>
        /// Object found.
        /// </returns>
        public static T GetItem<T>(DataRow dataRow)
        {
            var temp = typeof(T);
            var obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dataRow.Table.Columns)
            {
                var columnName = column.ColumnName;
                foreach (var pro in temp.GetProperties().Where(pro => pro.Name == columnName && dataRow[columnName] != DBNull.Value))
                {
                    pro.SetValue(obj, dataRow[column.ColumnName], null);
                }
            }
            return obj;
        }

        /// <summary>
        /// Execute stored procedure with single table value parameter.
        /// </summary>
        /// <typeparam name="T">Type of object to store.</typeparam>
        /// <param name="data">Data to store</param>
        /// <param name="procedureName">Procedure name</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="userDefinedTableTypeName">Name of the user defined table type.</param>
        /// <returns>
        /// The executed sql status.
        /// </returns>
        public static int ExecuteTableValueProcedure<T>(IEnumerable<T> data, string procedureName, string parameterName, string userDefinedTableTypeName)
        {
            //// convert source data to DataTable
            var table = data.ToDataTable();

            //// create parameter
            var inParameter = new SqlParameter(parameterName, table) { SqlDbType = SqlDbType.Structured, TypeName = userDefinedTableTypeName };

            var command = CreateCommand(procedureName, CommandType.StoredProcedure);
            command.Parameters.Add(inParameter);

            //// execute sql
            var result = ExecuteScalar(command);

            if (result != null)
                return Convert.ToInt32(result);

            return -1;
        }
    }

}
