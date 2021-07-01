# ConnectionLibrary
Librería para gestionar las conexiones con bases de datos y servicios web.

## Introducción

Cuando trabajamos con proyectos de diferentes soluciones, por lo general creamos las capas de presentación, negocio, y datos. Independientemente de la arquitectura que usemos siempre debemos establecer la conexión con las bases de datos y servicios web ya sea en la capa de datos, o en una capa superior para separar las clases y funciones que realizan la conexión del resto de nuestro código. Esto se vuelve bastante repetitivo para cada proyecto de diferente solución, por lo que el objetivo de esta librería es evitar estar creando siempre las mismas clases de conexión, y copiando de un proyecto a otro. Solo con descargar el paquete en nuget, e implementándolo en tu capa de datos.

# Relational Databases

Gestiona las conexiones con bases de datos relacionales como Sql Server, MySql, Oracle db, entre otras.

```cs
string stringConnection = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;";
RelationalDatabaseConnector relationalDatabaseConnector = new RelationalDatabaseConnector(
ConnectionLibrary.Enums.DatabaseEnum.SqlServer,
stringConnection);
```

## **ExecuteNonQuery**

Ejecuta la consulta y obtiene el numero de filas afectadas.

```cs
string query = "Insert Into Client Values ('123', 'Juan', '01/01/1990')";
Response<int> response = relationalDatabaseConnector?.ExecuteNonQuery(query);
int result = response.Data;
```

Result:

```
1
```

## **GetDataListFromQuery**

Ejecuta la consulta y obtiene la lista de filas convertidas en entidades de formato Json.

```cs
string query = "Select * From Client";
Response<string> response = relationalDatabaseConnector?.GetDataListFromQuery(query);
string json = response?.Data;
```

Result:

```
{[
  {
    "Consecutivo": 2,
    "Id": "159DE6FD-45C6-4B68-BDE5-F8D26C6D2BB6",
    "Name": "Sebastian"
  },
  {
    "Consecutivo": 1,
    "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
    "Name": "Juan"
  }
]}
```

## **GetDataSetFromQuery**

Ejecuta la consulta y obtiene el conjunto de tablas y sus filas convertidas en entidades de formato Json.

```cs
string query = "Select * From Client Select * From Item";
Response<string> response = relationalDatabaseConnector?.GetDataSetFromQuery(query);
string json = response?.Data;
```

<details>
<summary>Otros motores de base de datos</summary>

  ### **MySql**

  Deben separarse las consultas por mediante punto y coma
  
  ```cs
  string query = "Select * From Client; Select * From Item;";
  ```
</details>

Result:

```
{{
  "Table": [
    {
      "Consecutivo": 2,
      "Id": "159DE6FD-45C6-4B68-BDE5-F8D26C6D2BB6",
      "Name": "Sebastian"
    },
    {
      "Consecutivo": 1,
      "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
      "Name": "Juan"
    }
  ],
  "Table1": [
    {
      "Consecutivo": 2,
      "Id": "79489DE8-EBD4-4D44-8195-558D53853A13",
      "Name": "Cerveza"
    },
    {
      "Consecutivo": 1,
      "Id": "914E4E23-3DD1-4AAF-9C6F-1F13F0E40823",
      "Name": "Cocacola"
    }
  ]
}}
```

## **ExecuteStoredProcedure**

Ejecuta el procedimiento en la base de datos y obtiene el numero de filas afectadas.

```cs
string storedProcedure = "SP_InsertClient";
List<(string, object)> parameters = new List<(string, object)>()
{
    new ("@Name", "Sebastian"),
    new ("@Birthdate", "01/01/2002")
};
Response<int> response = relationalDatabaseConnector?.ExecuteStoredProcedure(storedProcedure, parameters);
int result = response.Data;
```

<details>
<summary>Otros motores de base de datos</summary>

  ### **MySql**

 Los parametros no llevan simbolos arroba @
  
  ```cs
  new ("Name", "Sebastian")
  ```
</details>

Result:

```
1
```

## **GetDataListFromStoredProcedure**

Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas en entidades de formato Json.

<details>
<summary>Script</summary>

### **Sql Server**
  
```sql
CREATE PROCEDURE SP_GetClients
AS
SELECT * FROM CLIENTE
```
  
### **MySql**
  
```sql
CREATE PROCEDURE SP_GetClients()
SELECT * FROM Client;
```
</details>

```cs
string storedProcedure = "SP_GetClients";
Response<string> response = relationalDatabaseConnector?.GetDataListFromStoredProcedure(storedProcedure);
string json = response?.Data;
```

Result:

```
{[
  {
    "Consecutivo": 2,
    "Id": "159DE6FD-45C6-4B68-BDE5-F8D26C6D2BB6",
    "Name": "Sebastian"
  },
  {
    "Consecutivo": 1,
    "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
    "Name": "Juan"
  }
]}
```

## **GetDataSetFromStoredProcedure**

Ejecuta el procedimiento almacenado y obtiene el conjunto de tablas y sus filas convertidas en entidades de formato Json.

<details>
<summary>Script</summary>

### **Sql Server**
  
```sql
CREATE PROCEDURE SP_GetClientsAndItems
AS
BEGIN
SELECT * FROM Client
SELECT * FROM Item
END
```
  
### **MySql**
  
```sql
DELIMITER //
CREATE PROCEDURE SP_GetClientsAndItems()
BEGIN
SELECT * FROM Client;
SELECT * FROM Item;
END //
DELIMITER ;
```
</details>

```cs
string storedProcedure = "SP_GetClientsAndItems";
Response<string> response = relationalDatabaseConnector?.GetDataSetFromStoredProcedure(storedProcedure);
string json = response?.Data;
```

Result:

```
{{
  "Table": [
    {
      "Consecutivo": 2,
      "Id": "159DE6FD-45C6-4B68-BDE5-F8D26C6D2BB6",
      "Name": "Sebastian"
    },
    {
      "Consecutivo": 1,
      "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
      "Name": "Juan"
    }
  ],
  "Table1": [
    {
      "Consecutivo": 2,
      "Id": "79489DE8-EBD4-4D44-8195-558D53853A13",
      "Name": "Cerveza"
    },
    {
      "Consecutivo": 1,
      "Id": "914E4E23-3DD1-4AAF-9C6F-1F13F0E40823",
      "Name": "Cocacola"
    }
  ]
}}
```
