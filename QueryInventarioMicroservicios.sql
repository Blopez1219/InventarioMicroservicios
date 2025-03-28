CREATE DATABASE InventarioDB;
GO

USE InventarioDB;
GO

-- Tabla de Productos
CREATE TABLE Productos (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Descripcion NVARCHAR(255),
    Categoria NVARCHAR(50) NOT NULL,
    Imagen NVARCHAR(255),
    Precio DECIMAL(10,2) NOT NULL CHECK (Precio >= 0),
    Stock INT NOT NULL CHECK (Stock >= 0)
);
GO

-- Tabla de Transacciones
CREATE TABLE Transacciones (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL DEFAULT GETDATE(),
    TipoTransaccion NVARCHAR(10) NOT NULL CHECK (TipoTransaccion IN ('compra', 'venta')),
    ProductoID INT NOT NULL,
    Cantidad INT NOT NULL CHECK (Cantidad > 0),
    PrecioUnitario DECIMAL(10,2) NOT NULL CHECK (PrecioUnitario >= 0),
    PrecioTotal AS (Cantidad * PrecioUnitario) PERSISTED,
    Detalle NVARCHAR(255),
    FOREIGN KEY (ProductoID) REFERENCES Productos(ID) ON DELETE CASCADE
);
GO

-- Índices para optimización de consultas
CREATE INDEX IX_Productos_Nombre ON Productos(Nombre);
CREATE INDEX IX_Transacciones_Fecha ON Transacciones(Fecha);
CREATE INDEX IX_Transacciones_TipoTransaccion ON Transacciones(TipoTransaccion);
GO
