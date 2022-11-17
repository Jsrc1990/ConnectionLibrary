using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ConnectionLibrary
{
    /// <summary>
    /// Define las extensiones de la Conexión
    /// </summary>
    public static class ConnectionExtensions
    {
        /// <summary>
        /// Obtiene el query del Insert del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Insertar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        public static string GetInsertQuery(this object currentObject, string tableName = "")
        {
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
        /// <returns></returns>
        public static string GetUpdateQuery(this object currentObject, string tableName = "", string primaryKeyName = "Id")
        {
            if (currentObject == null) return string.Empty;
            tableName = string.IsNullOrWhiteSpace(tableName) ? currentObject?.GetType()?.Name : tableName;
            string idValue = currentObject?.GetType()?.GetProperty(primaryKeyName)?.GetValue(currentObject) as string;
            if (string.IsNullOrWhiteSpace(idValue)) return string.Empty;
            List<(string, string)> propertiesAndValues = GetPropertiesAndValues(currentObject);
            if (propertiesAndValues?.Any() == false) return string.Empty;
            string properties = string.Join(", ", propertiesAndValues?.Select(x => $"{x.Item1} = {x.Item2}"));
            string query = $"UPDATE FROM {tableName} SET {properties} WHERE {primaryKeyName} = '{idValue}'";
            return query;
        }

        /// <summary>
        /// Obtiene el query del Delete del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Eliminar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
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
        /// Obtiene las propiedades y sus valores del Objeto
        /// </summary>
        /// <param name="currentObject"></param>
        /// <returns></returns>
        private static List<(string, string)> GetPropertiesAndValues(object currentObject)
        {
            if (currentObject == null) return null;
            List<(string, string)> propertiesAndValues = new List<(string, string)>();
            foreach (PropertyInfo propertyInfo in currentObject?.GetType()?.GetRuntimeProperties() ?? new List<PropertyInfo>())
            {
                Type type = propertyInfo?.PropertyType;
                //Si es una cadena entonces (El String es de tipo lista, y además clase, por eso se pone acá de primero)
                if (type == typeof(string))
                {
                    propertiesAndValues?.Add(new(propertyInfo?.Name, $"'{propertyInfo?.GetValue(currentObject)}'"));
                }
                //Si es cualquier tipo de lista (Array, List, Collection) entonces
                if (typeof(IEnumerable).IsAssignableFrom(type))
                {
                    //En el momento no hay instrucciones
                }
                else
                //Si es una clase entonces
                if (type?.IsClass == true)
                {
                    //En el momento no hay instrucciones
                }
                //Si son números entonces
                else
                {
                    propertiesAndValues?.Add(new(propertyInfo?.Name, $"{propertyInfo?.GetValue(currentObject)}"));
                }
            }
            return propertiesAndValues;
        }
    }
}