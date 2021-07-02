using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TransversalLibrary;

namespace ConnectionLibrary.RelationalDatabase
{
    /// <summary>
    /// Gestiona la conexión con Oracle Database
    /// </summary>
    internal class OracleConnector : IRelationalDatabaseConnector
    {
        /// <summary>
        /// Define la cadena de conexión
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Inicializa las propiedades de esta clase
        /// </summary>
        public OracleConnector(string connectionString)
        {
            ConnectionString = connectionString;
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el numero de filas afectadas
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El numero de filas afectadas</returns>
        public Response<int> ExecuteNonQuery(string query, int retry = 2)
        {
            Response<int> response = new Response<int>() { Data = 0 };
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(query, sqlConnection);
                    sqlConnection.Open();
                    response.Data = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return ExecuteNonQuery(query, retry);
                //Establece que hubo un error en la conexión
                response.Data = -1;
                response.Exception = ex;
            }
            return response;
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el valor
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El valor obtenido</returns>
        public Response<string> ExecuteScalar(string query, int retry = 2)
        {
            Response<string> response = new Response<string>() { Data = string.Empty };
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(query, sqlConnection);
                    sqlConnection.Open();
                    response.Data = Convert.ToString(sqlCommand.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return ExecuteScalar(query, retry);
                //Establece que hubo un error en la conexión
                response.Data = string.Empty;
                response.Exception = ex;
            }
            return response;
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el objeto genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico obtenido</returns>
        public Response<T> ExecuteScalar<T>(string query, int retry = 2)
        {
            Response<T> response = new Response<T>() { Data = default(T) };
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(query, sqlConnection);
                    sqlConnection.Open();
                    response.Data = (T)sqlCommand.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return ExecuteScalar<T>(query, retry);
                //Establece que hubo un error en la conexión
                response.Data = default(T);
                response.Exception = ex;
            }
            return response;
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene la lista de filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades en formato JSON</returns>
        public Response<string> GetDataListFromQuery(string query, int retry = 2)
        {
            Response<string> response = new Response<string>();
            DataTable table = new DataTable();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(query, connection))
                    {
                        adapter?.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return GetDataListFromQuery(query, retry);
                //Establece que hubo un error en la conexión
                table = null;
                response.Exception = ex;
            }
            //Establece el resultado
            response.Data = table != null ? JsonConvert.SerializeObject(table) : string.Empty;
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el conjunto de tablas y sus filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas y sus filas convertidas a entidades en formato JSON</returns>
        public Response<string> GetDataSetFromQuery(string query, int retry = 2)
        {
            Response<string> response = new Response<string>();
            DataSet dataSet = new DataSet();
            try
            {
                using (OracleConnection connection = new OracleConnection(ConnectionString))
                {
                    using (OracleDataAdapter adapter = new OracleDataAdapter(query, connection))
                    {
                        adapter?.Fill(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return GetDataSetFromQuery(query, retry);
                //Establece que hubo un error en la conexión
                dataSet = null;
                response.Exception = ex;
            }
            //Establece el resultado
            response.Data = dataSet != null ? JsonConvert.SerializeObject(dataSet) : string.Empty;
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta el procedimiento en la base de datos y obtiene el numero de filas afectadas
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El numero de filas afectadas</returns>
        public Response<int> ExecuteStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            Response<int> response = new Response<int>() { Data = 0 };
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(storedProcedureName, sqlConnection);
                    if (parameters?.Any() == true)
                        foreach ((string, object) currentTuple in parameters)
                        {
                            OracleParameter currentParameter = new OracleParameter(currentTuple.Item1, currentTuple.Item2);
                            sqlCommand.Parameters.Add(currentParameter);
                        }
                    //Connection.Open();
                    response.Data = sqlCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return ExecuteStoredProcedure(storedProcedureName, parameters, retry);
                //Establece que hubo un error en la conexión
                response.Data = -1;
                response.Exception = ex;
            }
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta el procedimiento en la base de datos y obtiene el escalar del tipo genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico</returns>
        public Response<T> ExecuteScalarFromStoredProcedure<T>(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            Response<T> response = new Response<T>() { Data = default(T) };
            try
            {
                //Define la conexión
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    //Define el comando
                    using (OracleCommand sqlCommand = new OracleCommand(storedProcedureName, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        //Agrega los parámetros
                        if (parameters?.Any() == true)
                            foreach ((string, object) currentTuple in parameters)
                            {
                                OracleParameter currentParameter = new OracleParameter(currentTuple.Item1, currentTuple.Item2);
                                sqlCommand.Parameters.Add(currentParameter);
                            }
                        //Abre la conexión y ejecuta la consulta
                        sqlConnection?.Open();
                        response.Data = (T)sqlCommand.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return ExecuteScalarFromStoredProcedure<T>(storedProcedureName, parameters, retry);
                //Establece que hubo un error en la conexión
                response.Data = default(T);
                response.Exception = ex;
            }
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades en formato JSON</returns>
        public Response<string> GetDataListFromStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            Response<string> response = new Response<string>();
            DataTable table = new DataTable();
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(storedProcedureName, sqlConnection);
                    if (parameters?.Any() == true)
                        foreach ((string, object) currentTuple in parameters)
                        {
                            OracleParameter currentParameter = new OracleParameter(currentTuple.Item1, currentTuple.Item2);
                            sqlCommand.Parameters.Add(currentParameter);
                        }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand))
                    {
                        adapter?.Fill(table);
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return GetDataListFromStoredProcedure(storedProcedureName, parameters, retry);
                //Establece que hubo un error en la conexión
                table = null;
                response.Exception = ex;
            }
            //Establece el resultado
            response.Data = table != null ? JsonConvert.SerializeObject(table) : string.Empty;
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta el procedimiento en la base de datos y obtiene el escalar del tipo genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico</returns>
        public Response<T> GetDataListFromStoredProcedure<T>(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            Response<T> response = new Response<T>();
            try
            {
                //Define la conexión
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    //Define el comando
                    using (OracleCommand sqlCommand = new OracleCommand(storedProcedureName, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        //Agrega los parámetros
                        if (parameters?.Any() == true)
                            foreach ((string, object) currentTuple in parameters)
                            {
                                OracleParameter currentParameter = new OracleParameter(currentTuple.Item1, currentTuple.Item2);
                                sqlCommand.Parameters.Add(currentParameter);
                            }
                        //Abre la conexión y ejecuta la consulta
                        using (OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand))
                        {
                            DataTable table = new DataTable();
                            adapter?.Fill(table);
                            if (table != null)
                            {
                                string json = JsonConvert.SerializeObject(table);
                                response.Data = JsonConvert.DeserializeObject<T>(json);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return GetDataListFromStoredProcedure<T>(storedProcedureName, parameters, retry);
                response.Exception = ex;
            }
            //Retorna el resultado
            return response;
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene el conjunto de tablas de la base de datos convertidas en JSON
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas de la base de datos convertidas en JSON</returns>
        public Response<string> GetDataSetFromStoredProcedure(string storedProcedureName, List<(string, object)> parameters, int retry = 2)
        {
            Response<string> response = new Response<string>();
            DataSet dataSet = new DataSet();
            try
            {
                using (OracleConnection sqlConnection = new OracleConnection(ConnectionString))
                {
                    OracleCommand sqlCommand = new OracleCommand(storedProcedureName, sqlConnection);
                    if (parameters?.Any() == true)
                        foreach ((string, object) currentTuple in parameters)
                        {
                            OracleParameter currentParameter = new OracleParameter(currentTuple.Item1, currentTuple.Item2);
                            sqlCommand.Parameters.Add(currentParameter);
                        }
                    using (OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand))
                    {
                        adapter?.Fill(dataSet);
                    }
                }
            }
            catch (Exception ex)
            {
                //Realiza otro intento
                if (retry-- > 0)
                    return GetDataListFromStoredProcedure(storedProcedureName, parameters, retry);
                //Establece que hubo un error en la conexión
                dataSet = null;
                response.Exception = ex;
            }
            //Establece el resultado
            response.Data = dataSet != null ? JsonConvert.SerializeObject(dataSet) : string.Empty;
            //Retorna el resultado
            return response;
        }
    }
}