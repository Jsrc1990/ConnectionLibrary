using System.Collections.Generic;
using TransversalLibrary.Standard;

namespace ConnectionLibrary.Standard.RelationalDatabase
{
    /// <summary>
    /// Define el comportamiento de las bases de datos relaciones
    /// </summary>
    public interface IRelationalDatabaseConnector
    {
        /// <summary>
        /// Ejecuta la consulta y obtiene el numero de filas afectadas
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El numero de filas afectadas</returns>
        Response<int> ExecuteNonQuery(string query, int retry = 2);

        /// <summary>
        /// Ejecuta la consulta y obtiene el valor
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El valor obtenido</returns>
        Response<string> ExecuteScalar(string query, int retry = 2);

        /// <summary>
        /// Ejecuta la consulta y obtiene el objeto genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico</returns>
        Response<T> ExecuteScalar<T>(string query, int retry = 2);

        /// <summary>
        /// Ejecuta la consulta y obtiene la lista de filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades en formato JSON</returns>
        Response<string> GetDataListFromQuery(string query, int retry = 2);

        /// <summary>
        /// Ejecuta la consulta y obtiene el conjunto de tablas y sus filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="query">La consulta</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas y sus filas convertidas a entidades en formato JSON</returns>
        Response<string> GetDataSetFromQuery(string query, int retry = 2);

        /// <summary>
        /// Ejecuta el procedimiento en la base de datos y obtiene el numero de filas afectadas
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El numero de filas afectadas</returns>
        Response<int> ExecuteStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2);

        /// <summary>
        /// Ejecuta el procedimiento en la base de datos y obtiene el escalar del tipo genérico
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El objeto genérico</returns>
        Response<T> ExecuteScalarFromStoredProcedure<T>(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2);

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas a entidades en formato JSON
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades en formato JSON</returns>
        Response<string> GetDataListFromStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2);

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas a entidades
        /// </summary>
        /// <typeparam name="T">El tipo genérico</typeparam>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>La lista de filas convertidas a entidades</returns>
        Response<T> GetDataListFromStoredProcedure<T>(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2);

        /// <summary>
        /// Ejecuta el procedimiento almacenado y obtiene el conjunto de tablas de la base de datos convertidas en JSON
        /// </summary>
        /// <param name="storedProcedureName">El procedimiento almacenado</param>
        /// <param name="parameters">Los parámetros</param>
        /// <param name="retry">El numero de intentos</param>
        /// <returns>El conjunto de tablas de la base de datos convertidas en JSON</returns>
        Response<string> GetDataSetFromStoredProcedure(string storedProcedureName, List<(string, object)> parameters = null, int retry = 2);
    }
}