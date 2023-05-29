using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.Json;
using TransversalLibrary.Standard;

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
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>El query de la tabla a crear</returns>
        public static Response<string> GetCreateTableQuery(this object currentObject, string tableName = "", string identityKeyName = "Record", string primaryKeyName = "Id")
        {
            //Establece el nombre de la tabla
            Response<string> tableNameResponse = GetTableName(currentObject, tableName);
            if (tableNameResponse?.Errors?.Any() == true) return tableNameResponse;
            tableName = tableNameResponse?.Data;
            //Establece las propiedades y sus valores
            Response<List<(string, string)>> propertiesAndValuesResponse = GetAllPropertiesAndValues(currentObject, identityKeyName, primaryKeyName);
            if (propertiesAndValuesResponse?.Errors?.Any() == true) return new Response<string>() { Errors = propertiesAndValuesResponse?.Errors };
            List<(string, string)> propertiesAndValues = propertiesAndValuesResponse?.Data;
            //Establece las definiciones
            List<string> definitions = CreateDefinitionTable(propertiesAndValues, primaryKeyName);
            string properties = string.Join($",{Environment.NewLine}", definitions);
            //Establece el Query
            string query = $"CREATE TABLE {tableName}{Environment.NewLine}({Environment.NewLine}{properties}{Environment.NewLine}) SELECT 1";
            return new Response<string>() { Data = query };
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
        /// Obtiene el query del Select de la tabla especificada por su condición especificada (Id por defecto)
        /// </summary>
        /// <param name="tableName">El nombre de la tabla</param>
        /// <param name="conditionValue">El valor de la condición</param>
        /// <param name="conditionProperty">El nombre de la condición (Id por defecto)(Opcional pero obligatorio)</param>
        /// <returns>El query del Select de la tabla especificada por su condición especificada (Id por defecto)</returns>
        public static Response<string> GetSimpleSelectByIdQuery(string tableName, string selectValues = "", string conditionProperty = "", string conditionValue = "")
        {
            List<string> errors = new List<string>();
            //Valida el nombre de la tabla
            if (string.IsNullOrWhiteSpace(tableName))
                errors?.Add("El nombre de la tabla es obligatorio");
            //Valida el nombre de la condición
            if (!string.IsNullOrWhiteSpace(conditionValue) && string.IsNullOrWhiteSpace(conditionProperty))
                errors?.Add("El nombre de la condición es obligatorio");
            //Valida el valor de la condición
            if (!string.IsNullOrWhiteSpace(conditionProperty) && string.IsNullOrWhiteSpace(conditionValue))
                errors?.Add("El valor de la condición es obligatorio");
            //Si hay errores entonces
            if (errors?.Any() == true)
                return Response<string>.ReturnBadRequest(errors?.ToArray());
            //Genera la consulta
            if (string.IsNullOrWhiteSpace(selectValues)) selectValues = "*";
            string where = !string.IsNullOrWhiteSpace(conditionProperty) && !string.IsNullOrWhiteSpace(conditionValue) ? $"WHERE {conditionProperty} = '{conditionValue}'" : string.Empty;
            string query = $"SELECT {selectValues} FROM {tableName} {where}";
            return new Response<string>() { Data = query };
        }

        /// <summary>
        /// Obtiene el query del Insert del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Insertar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <param name="isPrimaryKeyDefaultValue">Si el valor por defecto de la Llave primaria es DEFAULT</param>
        /// <returns>El query del Insert del Objeto</returns>
        public static Response<string> GetInsertQuery(this object currentObject, string tableName = "", string identityKeyName = "Record", string primaryKeyName = "Id", bool isPrimaryKeyDefaultValue = false, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            if (currentObject == null) return new Response<string>() { Errors = new List<string>() { "El objeto es nulo" } };
            //Establece el nombre de la tabla
            Response<string> tableNameResponse = GetTableName(currentObject, tableName);
            if (tableNameResponse?.Errors?.Any() == true) return tableNameResponse;
            tableName = tableNameResponse?.Data;
            //Establece las propiedades y sus valores
            Response<List<(string, string)>> propertiesAndValuesResponse = GetAllPropertiesAndValues(currentObject, identityKeyName, primaryKeyName, isPrimaryKeyDefaultValue);
            if (propertiesAndValuesResponse?.Errors?.Any() == true) return new Response<string>() { Errors = propertiesAndValuesResponse?.Errors };
            List<(string, string)> propertiesAndValues = propertiesAndValuesResponse?.Data;
            string properties = string.Join(",", propertiesAndValues?.Select(x => x.Item1));
            string values = string.Join(",", propertiesAndValues?.Select(x => AddSimpleQuotesOrNot(x.Item2, dateFormat)));
            //Establece el Query
            string query = $"INSERT INTO {tableName} ({properties}) VALUES ({values}) SELECT {primaryKeyName} From {tableName} Where {identityKeyName} = (SELECT SCOPE_IDENTITY())";
            return new Response<string>() { Data = query };
        }

        /// <summary>
        /// Obtiene el query del Update del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Actualizar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <param name="isPrimaryKeyDefaultValue">Si el valor por defecto de la Llave primaria es DEFAULT</param>
        /// <returns>El query del Update del Objeto</returns>
        public static Response<string> GetUpdateQuery(this object currentObject, string tableName = "", string identityKeyName = "Record", string primaryKeyName = "Id", bool isPrimaryKeyDefaultValue = false)
        {
            if (currentObject == null) return new Response<string>() { Errors = new List<string>() { "El objeto es nulo" } };
            //Establece el nombre de la tabla
            Response<string> tableNameResponse = GetTableName(currentObject, tableName);
            if (tableNameResponse?.Errors?.Any() == true) return tableNameResponse;
            tableName = tableNameResponse?.Data;
            //Establece las propiedades y sus valores
            Response<List<(string, string)>> propertiesAndValuesResponse = GetAllPropertiesAndValues(currentObject, identityKeyName, primaryKeyName, isPrimaryKeyDefaultValue);
            if (propertiesAndValuesResponse?.Errors?.Any() == true) return new Response<string>() { Errors = propertiesAndValuesResponse?.Errors };
            List<(string, string)> propertiesAndValues = propertiesAndValuesResponse?.Data;
            string properties = string.Join(",", propertiesAndValues?.Select(x => $"{x.Item1} = {AddSimpleQuotesOrNot(x.Item2)}"));
            //Establece el valor de la Llave primaria
            Response<string> primarykeyValueResponse = GetPrimaryKeyValue(currentObject, primaryKeyName);
            if (primarykeyValueResponse?.Errors?.Any() == true) return primarykeyValueResponse;
            //Establece el Query
            string query = $"UPDATE FROM {tableName} SET {properties} WHERE {primaryKeyName} = '{primarykeyValueResponse?.Data}'";
            return new Response<string>() { Data = query };
        }

        /// <summary>
        /// Obtiene el query del Delete del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a Eliminar</param>
        /// <param name="tableName">El nombre de la tabla (Opcional)</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>El query del Delete del Objeto</returns>
        public static Response<string> GetDeleteQuery(this object currentObject, string tableName = "", string primaryKeyName = "Id")
        {
            if (currentObject == null) return new Response<string>() { Errors = new List<string>() { "El objeto es nulo" } };
            //Establece el nombre de la tabla
            Response<string> tableNameResponse = GetTableName(currentObject, tableName);
            if (tableNameResponse?.Errors?.Any() == true) return tableNameResponse;
            tableName = tableNameResponse?.Data;
            //Establece el valor de la Llave primaria
            Response<string> primarykeyValueResponse = GetPrimaryKeyValue(currentObject, primaryKeyName);
            if (primarykeyValueResponse?.Errors?.Any() == true) return primarykeyValueResponse;
            //Establece el Query
            string query = $"DELETE FROM {tableName} WHERE {primaryKeyName} = '{primarykeyValueResponse?.Data}'";
            return new Response<string>() { Data = query };
        }

        #endregion

        #region FUNCTIONS

        /// <summary>
        /// Obtiene el nombre de la tabla
        /// </summary>
        /// <param name="currentObject">El objeto a obtener su nombre de tabla</param>
        /// <param name="tableName">El nombre de tabla (Opcional)</param>
        /// <returns>El nombre de la tabla</returns>
        private static Response<string> GetTableName(object currentObject, string tableName)
        {
            Type type = currentObject?.GetType();
            try
            {
                //Si se especificó el nombre de la tabla entonces
                if (!string.IsNullOrWhiteSpace(tableName))
                    return new Response<string>() { Data = tableName };
                //Si el objeto es System.Type
                if (currentObject?.GetType()?.Name == "RuntimeType")
                {
                    Type currentType = currentObject as Type;
                    return new Response<string>() { Data = currentType?.Name };
                }
                //Si el objeto es de tipo JsonElement entonces
                if (type == typeof(JsonElement))
                {
                    //Si no se especificó un nombre de tabla entonces
                    if (string.IsNullOrWhiteSpace(tableName))
                        return new Response<string>() { Errors = new List<string>() { "El objeto de tipo JsonElement no puede especificar un nombre de tabla por sí mismo, Usted necesita especificar un nombre de tabla." } };
                    //Si se especificó un nombre de tabla entonces
                    return new Response<string>() { Data = tableName };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex?.Message);
            }
            //Si el objeto es una clase (Ejemplo: Persona) puede extraer el nombre por mediante su tipo o ser especificado
            return new Response<string>() { Data = type?.Name };
        }

        /// <summary>
        /// Obtiene el valor de la Llave primaria
        /// </summary>
        /// <param name="currentObject">El objeto a obtener el valor de su Llave primaria</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns></returns>
        private static Response<string> GetPrimaryKeyValue(object currentObject, string primaryKeyName = "Id")
        {
            string primaryKeyValue;
            //Si el objeto es de tipo JsonElement entonces
            if (currentObject?.GetType() == typeof(JsonElement))
            {
                JsonElement? JsonElement = (JsonElement)currentObject;
                primaryKeyValue = JsonElement?.GetProperty(primaryKeyName).GetString();
            }
            //Si el objeto es de tipo clase (Ejemplo: Persona)
            else
            {
                primaryKeyValue = currentObject?.GetType()?.GetProperty(primaryKeyName)?.GetValue(currentObject) as string;
            }
            //Retorna la respuesta
            Response<string> response = new Response<string>() { Data = primaryKeyValue };
            if (string.IsNullOrWhiteSpace(primaryKeyValue)) response?.Errors?.Add("El valor de la Llave primaria es nulo o vacío");
            return response;
        }

        /// <summary>
        /// Obtiene las propiedades y valores formateados del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a obtener sus propiedades y valores formateados</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <param name="isPrimaryKeyDefaultValue">Si el valor por defecto de la Llave primaria es DEFAULT</param>
        /// <returns>Las propiedades y los valores</returns>
        private static Response<List<(string, string)>> GetAllPropertiesAndValues(object currentObject, string identityKeyName = "Record", string primaryKeyName = "Id", bool isPrimaryKeyDefaultValue = false)
        {
            List<(string, string)> propertiesAndValues;
            //Si el objeto es de tipo JsonElement entonces
            if (currentObject?.GetType() == typeof(JsonElement))
            {
                propertiesAndValues = GetPropertiesAndValues((JsonElement)currentObject, identityKeyName, primaryKeyName, isPrimaryKeyDefaultValue);
            }
            //Si el objeto es de tipo clase (Ejemplo: Persona)
            else
            {
                propertiesAndValues = GetPropertiesAndValues(currentObject, identityKeyName, primaryKeyName, isPrimaryKeyDefaultValue);
            }
            //Retorna la respuesta
            Response<List<(string, string)>> response = new Response<List<(string, string)>>() { Data = propertiesAndValues };
            if (propertiesAndValues?.Any() == false) response?.Errors?.Add("El objeto no tiene propiedades");
            return response;
        }

        /// <summary>
        /// Obtiene las propiedades y valores formateados del Objeto
        /// </summary>
        /// <param name="currentObject">El objeto a obtener sus propiedades y valores formateados</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <param name="isPrimaryKeyDefaultValue">Si el valor por defecto de la Llave primaria es DEFAULT</param>
        /// <returns>Las propiedades y los valores</returns>
        private static List<(string, string)> GetPropertiesAndValues(object currentObject, string identityKeyName = "Record", string primaryKeyName = "Id", bool isPrimaryKeyDefaultValue = false)
        {
            if (currentObject == null) return null;
            List<(string, string)> propertiesAndValues = new List<(string, string)>();
            if (currentObject?.GetType()?.GetProperties()?.Any() == true)
                foreach (PropertyInfo propertyInfo in currentObject?.GetType()?.GetProperties())
                {
                    Type propertyType = propertyInfo?.PropertyType;
                    //Si es la llave de identidad entonces
                    if (propertyInfo?.Name == identityKeyName && isPrimaryKeyDefaultValue)
                    {
                        //En el momento no hay instrucciones
                    }
                    else
                    //Si es la llave primaria y el valor es el por defecto entonces
                    if (propertyInfo?.Name == primaryKeyName && isPrimaryKeyDefaultValue)
                    {
                        propertiesAndValues?.Add((propertyInfo?.Name, "DEFAULT"));
                    }
                    else
                    //Si es una cadena entonces (El String es de tipo lista, y además clase, por eso se pone acá de primero)
                    if (propertyType == typeof(string))
                    {
                        propertiesAndValues?.Add((propertyInfo?.Name, $"{propertyInfo?.GetValue(currentObject)}"));
                    }
                    else
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

        /// <summary>
        /// Obtiene las propiedades y valores formateados del JsonElement
        /// </summary>
        /// <param name="jsonElement">El JsonElement a obtener sus propiedades y valores formateados</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <param name="isPrimaryKeyDefaultValue">Si el valor por defecto de la Llave primaria es DEFAULT</param>
        /// <returns>Las propiedades y los valores</returns>
        private static List<(string, string)> GetPropertiesAndValues(JsonElement jsonElement, string identityKeyName = "Record", string primaryKeyName = "Id", bool isPrimaryKeyDefaultValue = false)
        {
            List<(string, string)> propertiesAndValues = new List<(string, string)>();
            try
            {
                foreach (JsonProperty jsonProperty in jsonElement.EnumerateObject())
                {
                    //Si es la llave de identidad entonces
                    if (jsonProperty.Name == identityKeyName && isPrimaryKeyDefaultValue)
                    {
                        //En el momento no hay instrucciones
                    }
                    //Si es la llave primaria y el valor es el por defecto entonces
                    if (jsonProperty.Name == primaryKeyName && isPrimaryKeyDefaultValue)
                    {
                        propertiesAndValues?.Add((jsonProperty.Name, "DEFAULT"));
                    }
                    else
                    //Si es una cadena entonces
                    if (jsonProperty.Value.ValueKind == JsonValueKind.String)
                    {
                        propertiesAndValues?.Add((jsonProperty.Name, $"{jsonProperty.Value.GetString()}"));
                    }
                    else
                    //Si es cualquier tipo de lista (Array, List, Collection) entonces
                    if (jsonProperty.Value.ValueKind == JsonValueKind.Array)
                    {
                        //En el momento no hay instrucciones
                    }
                    else
                    //Si es una clase entonces
                    if (jsonProperty.Value.ValueKind == JsonValueKind.Object)
                    {
                        //En el momento no hay instrucciones
                    }
                    else
                    //Si son números entonces
                    {
                        propertiesAndValues?.Add((jsonProperty.Name, $"{jsonProperty.Value.ToString()}")); //jsonProperty.Value.GetString() NO porque cuando es número se revienta
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex?.Message);
            }
            return propertiesAndValues;
        }

        /// <summary>
        /// Crea la definición de los campos de la tabla
        /// </summary>
        /// <param name="propertiesAndValues">Las propiedades y valores</param>
        /// <param name="primaryKeyName">La llave primaria (Por defecto: Id)</param>
        /// <returns>La definición de los campos de la tabla</returns>
        private static List<string> CreateDefinitionTable(List<(string, string)> propertiesAndValues, string primaryKeyName = "Id")
        {
            List<string> list = new List<string>();
            foreach ((string, string) item in propertiesAndValues)
            {
                string value = string.Empty;
                //Si la propiedad es una Llave primaria entonces
                if (item.Item1?.ToLower() == primaryKeyName?.ToLower())
                {
                    value = "VARCHAR (36) PRIMARY KEY NOT NULL DEFAULT CONVERT(VARCHAR(36), NEWID(), 0)";
                }
                //Si es una fecha entonces
                else
                if (DateTime.TryParse(item.Item2, out _))
                {
                    value = "DATETIME NOT NULL";
                }
                //Si es un número entero entonces
                else
                if (int.TryParse(item.Item2, out _))
                {
                    value = "INTEGER NOT NULL";
                }
                //Si es un número decimal entonces
                else
                if (decimal.TryParse(item.Item2, out _))
                {
                    value = "DECIMAL NOT NULL";
                }
                //Si es un Boolean entonces
                else
                if (bool.TryParse(item.Item2, out _))
                {
                    value = "BIT NOT NULL";
                }
                //Si es una cadena (String) entonces
                else
                {
                    value = $"VARCHAR({item.Item2?.Length}) NOT NULL";
                }
                list?.Add($"{item.Item1} {value}");
            }
            return list;
        }

        /// <summary>
        /// Agrega comillas simples o no
        /// </summary>
        /// <returns>El valor envuelto en un objeto</returns>
        private static object AddSimpleQuotesOrNot(string value, string dateFormat = "yyyy-MM-dd HH:mm:ss")
        {
            //Si es el valor por defecto entonces
            if (value == "DEFAULT")
            {
                return value;
            }
            //Si es un numero decimal entonces
            if (decimal.TryParse(value, out decimal decimalValue))
            {
                return decimalValue;
            }
            //Si es un número entero entonces
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }
            //Si es un boolean entonces
            if (bool.TryParse(value, out bool booleanValue))
            {
                return Convert.ToInt16(booleanValue);
            }
            //Si es una fecha entonces
            if (DateTime.TryParse(value, out DateTime dateTime))
            {
                return $"'{dateTime.ToString(dateFormat)}'";
            }
            //Si es una cadena (String) entonces
            return $"'{value}'";
        }

        #endregion
    }
}