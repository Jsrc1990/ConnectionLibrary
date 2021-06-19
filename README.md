# ConnectionLibrary
Librería para gestionar las conexiones con bases de datos y microservicios externos

# RELATIONAL DATABASES

Gestiona las conexiones con bases de datos relacionales como SQL Server, MySQL, Oracle DB, entre otras.

```cs
RelationalDatabaseConnector relationalDatabaseConnector = new RelationalDatabaseConnector(
ConnectionLibrary.DatabaseEnum.SqlServer,
"Server=localhost;Database=MyDatabase;Trusted_Connection=True;");
```

## **GetDataList**

Ejecuta la consulta y obtiene la lista de filas convertidas en entidades de formato JSON.

```cs
Response<string> response = relationalDatabaseConnector?.GetDataList("Select * From Cliente");
string json = response?.Data;
```

Resultado:

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

## **GetDataSet**

Ejecuta la consulta y obtiene el conjunto de tablas y sus filas convertidas en entidades de formato JSON.

```cs
Response<string> response = relationalDatabaseConnector?.GetDataSet("Select * From Cliente Select * From Producto");
string json = response?.Data;
```

Resultado:

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

