# ConnectionLibrary
Librería para gestionar las conexiones con bases de datos y microservicios externos

# RELATIONAL DATABASES

Gestiona las conexiones con bases de datos relacionales como SQL Server, MySQL, Oracle DB, entre otras.

```cs
string stringConnection = "Server=localhost;Database=MyDatabase;Trusted_Connection=True;";
RelationalDatabaseConnector relationalDatabaseConnector = new RelationalDatabaseConnector(
ConnectionLibrary.Enums.DatabaseEnum.SqlServer,
stringConnection);
```

## **ExecuteNonQuery**

Ejecuta la consulta y obtiene el numero de filas afectadas.

```cs
string query = "Insert Into Cliente Values (Default, 'Juan')";
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
    "Nombre": "Sebastian"
  },
  {
    "Consecutivo": 1,
    "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
    "Nombre": "Juan"
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

Result:

```
{{
  "Table": [
    {
      "Consecutivo": 2,
      "Id": "159DE6FD-45C6-4B68-BDE5-F8D26C6D2BB6",
      "Nombre": "Sebastian"
    },
    {
      "Consecutivo": 1,
      "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
      "Nombre": "Juan"
    }
  ],
  "Table1": [
    {
      "Consecutivo": 2,
      "Id": "79489DE8-EBD4-4D44-8195-558D53853A13",
      "Nombre": "Cerveza"
    },
    {
      "Consecutivo": 1,
      "Id": "914E4E23-3DD1-4AAF-9C6F-1F13F0E40823",
      "Nombre": "Cocacola"
    }
  ]
}}
```

## **ExecuteStoredProcedure**

Ejecuta el procedimiento en la base de datos y obtiene el numero de filas afectadas.

```
string storedProcedure = "SP_InsertClient";
Response<int> response = relationalDatabaseConnector?.ExecuteStoredProcedure(storedProcedure);
int result = response.Data;
```

Result:

```
1
```

## **GetDataListFromStoredProcedure**

Ejecuta el procedimiento almacenado y obtiene la lista de filas convertidas en entidades de formato Json.

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
    "Nombre": "Sebastian"
  },
  {
    "Consecutivo": 1,
    "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
    "Nombre": "Juan"
  }
]}
```

## **GetDataSetFromStoredProcedure**

Ejecuta el procedimiento almacenado y obtiene el conjunto de tablas y sus filas convertidas en entidades de formato Json.

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
      "Nombre": "Sebastian"
    },
    {
      "Consecutivo": 1,
      "Id": "C2091823-05C4-4486-8089-A163C5DE4F3F",
      "Nombre": "Juan"
    }
  ],
  "Table1": [
    {
      "Consecutivo": 2,
      "Id": "79489DE8-EBD4-4D44-8195-558D53853A13",
      "Nombre": "Cerveza"
    },
    {
      "Consecutivo": 1,
      "Id": "914E4E23-3DD1-4AAF-9C6F-1F13F0E40823",
      "Nombre": "Cocacola"
    }
  ]
}}
```
