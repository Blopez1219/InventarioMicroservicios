# InventarioMicroservicios

## Requisitos
- .Net Core 9.0.100
- Microsoft SQL Server 2022

## Ejecución del backend
1. Instalar .NET Core SDK.
2. Clonar el repositorio.
3. Restaurar dependencias con dotnet restore.
4. Configurar la base de datos. Ejecutar el archivo sql QueryInventarioMicroservicios.sql.
5. En el archivo appsettings.json de ProductoService y TransaccionesServices modificar la cadena de conexion por la que corresponda a su base de datos
6. Ejecutar la aplicación con dotnet run.

## Listado de endpoints
API Microservicio de productos
1.	Consulta de todos los productos
- URL: http://localhost:5230/api/productos
- Método: GET
2.	Consulta de un producto por ID 
- URL: http://localhost:5230/api/productos/{productoID}
- Método: GET

3.	Registro de productos
- URL: http://localhost:5230/api/productos
- Método: POST
- Request:
{
  "Nombre": "Laptop",
  "Descripcion": "DELL 55667",
  "Categoria": "Electrónica",
  "Precio": 1200.00,
  "Stock": 5
}
4.	Edición de un producto
- URL: http://localhost:5230/api/productos/{productoID}
- http://localhost:5230/api/productos/4
- Método: PUT
- Request: 
{
  "Id": 4,
  "Nombre": "Laptop",
  "Descripcion": "DELL 55667 Windows Home",
  "Categoria": "Electrónica",
  "Precio": 1300.00,
  "Stock": 5
} 

5.	Eliminación de un producto
- URL: http://localhost:5230/api/productos/4
- Método: DELETE

6.	Búsqueda de productos con filtros dinámicos 
- URL: http://localhost:5230/api/productos/buscar?precioMin=500&precioMax=1000
- Método: GET
- URL: http://localhost:5230/api/productos/buscar?nombre=Laptop
- Método: GET

API Microservicios de Transacciones
1.	Consulta de todas las transacciones
-  URL: http://localhost:5260/api/transacciones
- Método: GET
2.	Consulta de transacciones por id 
- URL: http://localhost:5260/api/transacciones/{trasnsaccionID}
- http://localhost:5260/api/transacciones/2
- Método: GET 
3.	Generar una transacción 
- URL: http://localhost:5260/api/transacciones
- Método: POST 
- Request: 
- {
  "productoId": 1,
  "cantidad": 10,
  "TipoTransaccion": "compra",
  "precioUnitario": 500.00
}

4.	Consulta de transacciones con filtros dinámicos
- URL: http://localhost:5260/api/transacciones/buscar?tipo=compra
- Método:  GET
5.	Consulta de historial de transacciones
- URL: http://localhost:5230/api/transacciones/historial/{ProductID}?tipo=compra
- http://localhost:5230/api/transacciones/historial/1?tipo=compra



