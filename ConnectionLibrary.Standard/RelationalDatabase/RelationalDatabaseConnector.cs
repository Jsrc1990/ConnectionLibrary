using ConnectionLibrary.Enums;
using System.Collections.Generic;
using TransversalLibrary.Standard;

namespace ConnectionLibrary.RelationalDatabase
{
    /// <summary>
    /// Define el conector para cualquier base de datos relacional
    /// </summary>
    /// <remarks>
    /// Puede ejecutar cualquier base de datos relacional especificada
    /// </remarks>
    public class RelationalDatabaseConnector : IRelationalDatabaseConnector
    {
        /// <summary>
        /// Define la interfaz para cualquier base de datos
        /// </summary>
        private readonly IRelationalDatabaseConnector IConnection;

        /// <summary>
        /// Inicializa las propiedades de esta clase de conexión
        /// </summary>
        /// <param name="databaseEnum">El motor de base de datos</param>
        /// <param name="connectionString">La cadena de conexión de la base de datos</param>
        public RelationalDatabaseConnector(DatabaseEnum databaseEnum, string connectionString)
        {
            switch (databaseEnum)
            {
                case DatabaseEnum.None:
                    break;
                case DatabaseEnum.SqlServer:
                    IConnection = new SqlServerConnector(connectionString);
                    break;
                case DatabaseEnum.MySql:
                    IConnection = new MySqlConnector(connectionString);
                    break;
                case DatabaseEnum.Oracle:
                    IConnection = new OracleConnector(connectionString);
                    break;
            }
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el numero de filas afectadas
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El numero de filas afectadas</returns>
        public Response<int> ExecuteNonQuery(string query, int retry = 2)
        {
            return IConnection?.ExecuteNonQuery(query, retry);
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el valor
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El valor obtenido</returns>
        public Response<string> ExecuteScalar(string query, int retry = 2)
        {
            return IConnection?.ExecuteScalar(query, retry);
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el objeto genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico</returns>
        public Response<T> ExecuteScalar<T>(string query, int retry = 2)
        {
            return IConnection?.ExecuteScalar<T>(query, retry);
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene la lista de filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades en formato JSON</returns>
        public Response<string> GetDataListFromQuery(string query, int retry = 2)
        {
            return IConnection?.GetDataListFromQuery(query, retry);
        }

        /// <summary>
        /// Ejecuta la consulta y obtiene el conjunto de tablas y sus filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas y sus filas convertidas a entidades en formato JSON</returns>
        public Response<string> GetDataSetFromQuery(string query, int retry = 2)
        {
            return IConnection?.GetDataSetFromQuery(query, retry);
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
            return IConnection?.ExecuteStoredProcedure(storedProcedureName, parameters, retry);
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
            return IConnection?.ExecuteScalarFromStoredProcedure<T>(storedProcedureName, parameters, retry);
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
            return IConnection?.GetDataListFromStoredProcedure(storedProcedureName, parameters, retry);
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas a entidades
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades</returns>
        public Response<T> GetDataListFromStoredProcedure<T>(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            return IConnection?.GetDataListFromStoredProcedure<T>(storedProcedureName, parameters, retry);
        }

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene el conjunto de tablas de la base de datos convertidas en JSON
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas de la base de datos convertidas en JSON</returns>
        public Response<string> GetDataSetFromStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2)
        {
            return IConnection?.GetDataSetFromStoredProcedure(storedProcedureName, parameters, retry);
        }
    }
}