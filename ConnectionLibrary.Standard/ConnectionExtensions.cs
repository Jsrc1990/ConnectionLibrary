using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConnectionLibrary.Standard
{
    /// <summary>
    /// Define las extensiones de la Conexión
    /// </summary>
    public static class ConnectionExtensions
    {
        #region DDL

        /// <summary>
        /// Obtiene el query del Create de la tabla
        /// </summary>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <returns>El query de la tabla a crear</returns>
        public static string GetCreateTableQuery(this Type type, string tableName = "")
        {
            //En el momento no hay instrucciones
            tableName = string.IsNullOrWhiteSpace(tableName) ? type?.Name : tableName;
            _ = tableName;
            string query = string.Empty;
            return query;
        }

        /// <summary>
        /// Obtiene el query del Alter de la tabla
        /// </summary>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <returns>El query de la tabla a alterar</returns>
        public static string GetAlterTableQuery(this Type type, string tableName = "")
        {
            //En el momento no hay instrucciones
            tableName = string.IsNullOrWhiteSpace(tableName) ? type?.Name : tableName;
            _ = tableName;
            string query = string.Empty;
            return query;
        }

        /// <summary>
        /// Obtiene el query del Drop de la tabla
        /// </summary>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <returns>El query de la tabla a eliminar</returns>
        public static string GetDropTableQuery(this Type type, string tableName = "")
        {
            tableName = string.IsNullOrWhiteSpace(tableName) ? type?.Name : tableName;
            string query = $"DROP TABLE {tableName}";
            return query;
        }

        #endregion

        #region DML

        /// <summary>
        /// Obtiene el query del Select del Objeto
        /// </summary>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <returns>El query del tipo a consultar</returns>
        public static string GetSimpleSelectQuery(this Type type, string tableName = "")
        {
            tableName = string.IsNullOrWhiteSpace(tableName) ? type?.Name : tableName;
            string properties = string.Join(",", type?.GetProperties()?.Select(x => x?.Name));
            if (properties?.Any() == false) return string.Empty;
            string query = $"SELECT {properties} FROM {tableName}";
            return query;
        }

        /// <summary>
        /// Obtiene el query del Select del Objeto por su Id
        /// </summary>
        /// <param name="idValue">El tipo a consultar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>El query del tipo a consultar</returns>
        public static string GetSimpleSelectByIdQuery(this Type type, string idValue, string tableName = "", string primaryKeyName = "Id")
        {
            tableName = string.IsNullOrWhiteSpace(tableName) ? type?.Name : tableName;
            string properties = string.Join(",", type?.GetProperties()?.Select(x => x?.Name));
            if (properties?.Any() == false) return string.Empty;
            string query = $"SELECT {properties} FROM {tableName} WHERE {primaryKeyName} = '{idValue}'";
            return query;
        }
        
        /// <summary>
        /// Obtiene el query del Insert del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Insertar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <returns>El query del Insert del Objeto</returns>
        public static string GetInsertQuery(this object currentObject, string tableName = "")
        {
            typeof(object)?.GetSimpleSelectByIdQuery("");

            if (currentObject == null) return string.Empty;
            tableName = string.IsNullOrWhiteSpace(tableName) ? currentObject?.GetType()?.Name : tableName;
            List<(string, string)> propertiesAndValues = GetPropertiesAndValues(currentObject);
            if (propertiesAndValues?.Any() == false) return string.Empty;
            string properties = string.Join(",", propertiesAndValues?.Select(x => x.Item1));
            string values = string.Join(",", propertiesAndValues?.Select(x => x.Item2));
            string query = $"INSERT INTO {tableName} ({properties}) VALUES ({values})";
            return query;
        }

        /// <summary>
        /// Obtiene el query del Update del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Actualizar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>El query del Update del Objeto</returns>
        public static string GetUpdateQuery(this object currentObject, string tableName = "", string primaryKeyName = "Id")
        {
            if (currentObject == null) return string.Empty;
            tableName = string.IsNullOrWhiteSpace(tableName) ? currentObject?.GetType()?.Name : tableName;
            string idValue = currentObject?.GetType()?.GetProperty(primaryKeyName)?.GetValue(currentObject) as string;
            if (string.IsNullOrWhiteSpace(idValue)) return string.Empty;
            List<(string, string)> propertiesAndValues = GetPropertiesAndValues(currentObject);
            if (propertiesAndValues?.Any() == false) return string.Empty;
            string properties = string.Join(",", propertiesAndValues?.Select(x => $"{x.Item1} = {x.Item2}"));
            string query = $"UPDATE FROM {tableName} SET {properties} WHERE {primaryKeyName} = '{idValue}'";
            return query;
        }

        /// <summary>
        /// Obtiene el query del Delete del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Eliminar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>El query del Delete del Objeto</returns>
        public static string GetDeleteQuery(this object currentObject, string tableName = "", string primaryKeyName = "Id")
        {
            if (currentObject == null) return string.Empty;
            tableName = string.IsNullOrWhiteSpace(tableName) ? currentObject?.GetType()?.Name : tableName;
            string idValue = currentObject?.GetType()?.GetProperty(primaryKeyName)?.GetValue(currentObject) as string;
            if (string.IsNullOrWhiteSpace(idValue)) return string.Empty;
            string query = $"DELETE FROM {tableName} WHERE {primaryKeyName} = '{idValue}'";
            return query;
        }

        /// <summary>
        /// Obtiene las propiedades y valores formateados del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a obtener sus propiedades y valores formateados</param>
        /// <returns>Las propiedades y los valores</returns>
        private static List<(string, string)> GetPropertiesAndValues(object currentObject)
        {
            if (currentObject == null) return null;
            List<(string, string)> propertiesAndValues = new List<(string, string)>();
            foreach (PropertyInfo propertyInfo in currentObject?.GetType()?.GetRuntimeProperties() ?? new List<PropertyInfo>())
            {
                Type propertyType = propertyInfo?.PropertyType;
                //Si es una cadena entonces (El String es de tipo lista, y además clase, por eso se pone acá de primero)
                if (propertyType == typeof(string))
                {
                    propertiesAndValues?.Add((propertyInfo?.Name, $"'{propertyInfo?.GetValue(currentObject)}'"));
                }
                //Si es cualquier tipo de lista (Array, List, Collection) entonces
                if (typeof(IEnumerable).IsAssignableFrom(propertyType))
                {
                    //En el momento no hay instrucciones
                }
                else
                //Si es una clase entonces
                if (propertyType?.IsClass == true)
                {
                    //En el momento no hay instrucciones
                }
                //Si son números entonces
                else
                {
                    propertiesAndValues?.Add((propertyInfo?.Name, $"{propertyInfo?.GetValue(currentObject)}"));
                }
            }
            return propertiesAndValues;
        }

        #endregion
    }
}