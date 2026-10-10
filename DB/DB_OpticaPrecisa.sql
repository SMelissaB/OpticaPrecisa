--use master
--DROP DATABASE DB_OpticaPrecisa

-- 1. Crear la base de datos
CREATE DATABASE DB_OpticaPrecisa;
GO

USE DB_OpticaPrecisa;
GO

-- 2. Crear la tabla Cliente
CREATE TABLE Cliente (
    IdCliente INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(100) NOT NULL,
    Dni VARCHAR(8) NOT NULL,
    Telefono VARCHAR(15),
    Correo VARCHAR(100),
    Direccion VARCHAR(150),
    FechaRegistro DATETIME DEFAULT GETDATE()
);
GO

-- 3. Crear la tabla Categoria 
CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre VARCHAR(50) NOT NULL,
    Descripcion VARCHAR(150)
);
GO

-- 4. Crear la tabla Usuario
CREATE TABLE Usuario (
    IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
    NombreUsuario VARCHAR(50) NOT NULL,
    Contrasena VARCHAR(100) NOT NULL,
	Correo VARCHAR(100) NOT NULL,
    Rol VARCHAR(30) CHECK (Rol IN ('Administrador', 'Vendedor')) NOT NULL,
    Estado BIT DEFAULT 1 -- 1: Activo, 0: Inactivo
);
GO

-- 5. Crear la tabla Vendedor
CREATE TABLE Vendedor (
    IdVendedor INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT UNIQUE FOREIGN KEY REFERENCES Usuario(IdUsuario),
    Nombre VARCHAR(100) NOT NULL,
    Dni VARCHAR(8) NOT NULL,
    Telefono VARCHAR(15),
    Correo VARCHAR(100),
    FechaIngreso DATE DEFAULT GETDATE(),
    Estado BIT DEFAULT 1
);
GO

-- 6. Crear la tabla Producto
CREATE TABLE Producto (
    IdProducto INT IDENTITY(1,1) PRIMARY KEY,
    IdCategoria INT FOREIGN KEY REFERENCES Categoria(IdCategoria),
    Nombre VARCHAR(100) NOT NULL,
    Descripcion VARCHAR(200),
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL,
    Estado BIT DEFAULT 1
);
GO

-- 7. Crear la tabla Venta
CREATE TABLE Venta (
    IdVenta INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME DEFAULT GETDATE(),
    IdCliente INT FOREIGN KEY REFERENCES Cliente(IdCliente),
    IdVendedor INT FOREIGN KEY REFERENCES Vendedor(IdVendedor),
    Total DECIMAL(10,2) NOT NULL
);
GO

-- 8. Crear la tabla Detalle_Venta
CREATE TABLE Detalle_Venta (
    IdDetalle INT IDENTITY(1,1) PRIMARY KEY,
    IdVenta INT FOREIGN KEY REFERENCES Venta(IdVenta),
    IdProducto INT FOREIGN KEY REFERENCES Producto(IdProducto),
    Cantidad INT NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL
);
GO

-- ==========================================
-- 9. CARGA DE DATOS DE PRUEBA
-- ==========================================

INSERT INTO Cliente (Nombre, Dni, Telefono, Correo, Direccion) VALUES 
('Juan Perez', '87654321', '987654321', 'juan@gmail.com', 'Av. Larco 123, Miraflores'),
('Maria Lopez', '12345678', '912345678', 'maria@gmail.com', 'Jr. de la Union 456, Lima');

INSERT INTO Categoria (Nombre, Descripcion) VALUES 
('Monturas', 'Monturas para lentes de medida y sol'),
('Lunas', 'Lunas oftálmicas con tratamientos'),
('Accesorios', 'Estuches, líquidos limpiadores y paños');

INSERT INTO Usuario (NombreUsuario, Contrasena, Correo, Rol) VALUES 
('admin', 'admin123', 'admin@sistema.com', 'Administrador'),
('atorres', 'venta2026', 'atorres@sistema.com', 'Vendedor');

INSERT INTO Vendedor (IdUsuario, Nombre, Dni, Telefono, Correo) VALUES 
(2, 'Ana Torres', '78912345', '955443322', 'atorres@opticaprecisa.com');

INSERT INTO Producto (IdCategoria, Nombre, Descripcion, Precio, Stock) VALUES 
(1, 'Montura RayBan', 'Montura de pasta color negro', 250.00, 10),
(2, 'Lunas Antirreflekt', 'Lunas con tratamiento UV', 120.00, 25);

INSERT INTO Venta (IdCliente, IdVendedor, Total) VALUES 
(1, 1, 370.00);

INSERT INTO Detalle_Venta (IdVenta, IdProducto, Cantidad, Subtotal) VALUES 
(1, 1, 1, 250.00),
(1, 2, 1, 120.00);
GO