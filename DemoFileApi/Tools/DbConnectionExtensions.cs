using System.Data;
using System.Data.Common;
using System.Reflection;

namespace DemoFileApi.Tools
{
    public static class DbConnectionExtensions
    {
        public static object? ExecuteScalar(this DbConnection dbConnection, string query, bool isStoredProcedure, object? parameters)
        {
            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State is not ConnectionState.Open)
                    dbConnection.Open();

                object? result = dbCommand.ExecuteScalar();
                return result is DBNull ? null : result;
            }
        }

        public static int ExecuteNonQuery(this DbConnection dbConnection, string query, bool isStoredProcedure, object? parameters)
        {
            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State is not ConnectionState.Open)
                    dbConnection.Open();

                return dbCommand.ExecuteNonQuery();
            }
        }

        public static IEnumerable<TResult> ExecuteReader<TResult>(this DbConnection dbConnection, string query, bool isStoredProcedure, Func<IDataRecord, TResult> selector, object? parameters = null)
        {
            ArgumentNullException.ThrowIfNull(selector);

            using (DbCommand dbCommand = CreateCommand(dbConnection, query, isStoredProcedure, parameters))
            {
                if (dbConnection.State is not ConnectionState.Open)
                    dbConnection.Open();

                using(DbDataReader dbDataReader = dbCommand.ExecuteReader())
                {
                    while(dbDataReader.Read())
                    {
                        yield return selector(dbDataReader);
                    }
                }
            }
        }

        private static DbCommand CreateCommand(DbConnection dbConnection, string query, bool isStoredProcedure, object? parameters)
        {
            DbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = query;

            if (isStoredProcedure)
                dbCommand.CommandType = CommandType.StoredProcedure;

            if(parameters is not null)
            {
                Type type = parameters.GetType();

                foreach(PropertyInfo propertyInfo in type.GetProperties())
                {
                    DbParameter dbParameter = dbCommand.CreateParameter();
                    dbParameter.ParameterName = propertyInfo.Name;

                    if (propertyInfo.GetMethod is null)
                        throw new InvalidOperationException($"L'accesseur de la propriété {propertyInfo.Name} doit être public");

                    object? value = propertyInfo.GetMethod.Invoke(parameters, null);

                    if(value is DateOnly dateOnly)
                    {
                        value = dateOnly.ToDateTime(default);
                    }

                    dbParameter.Value = value ?? DBNull.Value;
                    dbCommand.Parameters.Add(dbParameter);
                }
            }

            return dbCommand;
        }
    }
}
