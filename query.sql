-- ============================================
-- CREAR BASE DE DATOS
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'PROMERICA')
BEGIN
    CREATE DATABASE PROMERICA;
END
GO

USE PROMERICA;
GO

-- ============================================
-- CREAR TABLA
-- ============================================
CREATE TABLE Puestos (
    Codigo INT IDENTITY(1,1) PRIMARY KEY,
    Puesto VARCHAR(50),
    Nombre VARCHAR(100),
    CodigoJefe INT NULL,
    CONSTRAINT FK_Puestos_Jefe FOREIGN KEY (CodigoJefe) REFERENCES Puestos(Codigo)
);

GO

-- ============================================
-- INSETAR DATOS
-- ============================================
DECLARE @Pedro INT, @Pablo INT, @Jose INT;

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Gerente', 'Pedro', NULL);
SET @Pedro = SCOPE_IDENTITY();

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Sub Gerente', 'Pablo', @Pedro);
SET @Pablo = SCOPE_IDENTITY();

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Supervisor', 'Juan', @Pablo);

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Sub Gerente', 'José', @Pedro);
SET @Jose = SCOPE_IDENTITY();

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Supervisor', 'Carlos', @Jose);

INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
VALUES ('Supervisor', 'Diego', @Jose);
GO

-- ============================================
-- STORED PROCEDURE
-- ============================================
CREATE OR ALTER PROCEDURE [dbo].[SP_M_Puestos]
(
    @Accion VARCHAR(10),
    @Codigo INT = NULL,
    @Puesto VARCHAR(50) = NULL,
    @Nombre VARCHAR(100) = NULL,
    @CodigoJefe INT = NULL
)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Accion = 'INSERT'
        BEGIN
            INSERT INTO Puestos (Puesto, Nombre, CodigoJefe)
            VALUES (@Puesto, @Nombre, @CodigoJefe);

            SELECT 1 AS Status, 'Registro insertado correctamente' AS Mensaje;
        END
        ELSE IF @Accion = 'UPDATE'
        BEGIN
            UPDATE Puestos
            SET 
                Puesto = @Puesto,
                Nombre = @Nombre,
                CodigoJefe = @CodigoJefe
            WHERE Codigo = @Codigo;

            IF @@ROWCOUNT = 0
                SELECT 0 AS Status, 'Registro a actualizar no encontrado' AS Mensaje;
            ELSE
                SELECT 1 AS Status, 'Registro actualizado correctamente' AS Mensaje;
        END
        ELSE IF @Accion = 'DELETE'
        BEGIN
            DELETE FROM Puestos
            WHERE Codigo = @Codigo;

            IF @@ROWCOUNT = 0
                SELECT 0 AS Status, 'Registro a eliminar no encontrado' AS Mensaje;
            ELSE
                SELECT 1 AS Status, 'Registro eliminado correctamente' AS Mensaje;
        END
        ELSE IF @Accion = 'SELECT'
        BEGIN
            SELECT 
                Codigo,
                Puesto,
                Nombre,
                CodigoJefe
            FROM Puestos
            ORDER BY CodigoJefe;

            SELECT 1 AS Status, 'Consulta realizada correctamente' AS Mensaje;
        END
        ELSE
        BEGIN
            SELECT 0 AS Status, 'Acción no válida' AS Mensaje;
        END

    END TRY
    BEGIN CATCH
        SELECT 
            0 AS Status,
            ERROR_MESSAGE() AS Mensaje;
    END CATCH
END;
GO
