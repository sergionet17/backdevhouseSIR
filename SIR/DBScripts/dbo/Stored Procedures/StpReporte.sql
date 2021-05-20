-- =============================================
-- Author:		Jonathan Edgardo Atencia Barranco
-- Create date: 02-03-2021
-- Description:	CRUD de reportes, accion 1 = Crear, 2 = Actualizar
-- =============================================
CREATE PROCEDURE [dbo].[StpReporte]
	@accion int = 0, 
	@IdReporte	int= 0, 
	@Descripcion varchar(50) ='',
	@Norma varchar(500) = '',
	@Activo	bit = 1,
	@ActivoEmpresa	bit = 1,
	@idEmpresa int = 0,
	@idServicio int = 0
AS
BEGIN

	DECLARE @IdEmpresaServicio INT;

	IF @accion = 1
	BEGIN 

		IF EXISTS(SELECT [IdEmpresaServicio] 
					FROM [dbo].[EmpresaServicio] 
					WHERE [IdEmpresa] = @idEmpresa AND [IdServicio] = @idServicio AND [Activo] = 1)
		BEGIN
			BEGIN TRAN
			BEGIN TRY
			--Obtener un Id valido de relacion de empresa servicio.
			SELECT @IdEmpresaServicio = [IdEmpresaServicio] 
					FROM [dbo].[EmpresaServicio] 
					WHERE [IdEmpresa] = @idEmpresa AND [IdServicio] = @idServicio AND [Activo] = 1;
		
			--Insertar informacion de reporte y obtener Id generado
			INSERT INTO [dbo].[Reporte] ([Descripcion], [Activo], [Norma])
			VALUES (@Descripcion, @Activo, @Norma);

			SELECT @IdReporte = SCOPE_IDENTITY();

			--Insertar la relacion y finalizar el procreso
			INSERT INTO [dbo].[ReporteEmpresaServicio] ([IdReporte], [IdEmpresaServicio], [Activo])
			VALUES (@IdReporte, @IdEmpresaServicio, 1);

			COMMIT TRAN
			SELECT '' Error, @IdReporte IdReporte;
			END TRY
			BEGIN CATCH
				ROLLBACK TRAN
				SELECT ERROR_MESSAGE() Error, 0 IdReporte;
			END CATCH
		END
	END
	ELSE IF @accion = 2
	BEGIN
		BEGIN TRAN
		BEGIN TRY

		--Obtener un Id valido de relacion de empresa servicio.
		SELECT @IdEmpresaServicio = [IdEmpresaServicio] 
			FROM [dbo].[EmpresaServicio] 
			WHERE [IdEmpresa] = @idEmpresa AND [IdServicio] = @idServicio AND [Activo] = 1;
		--Actualizar informacion de reporte y obtener Id generado
		UPDATE [dbo].[Reporte] SET [Descripcion] = @Descripcion, [Activo] = @Activo, [Norma] = @Norma
		WHERE [IdReporte] = @IdReporte;

		--Actualizar la relacion y finalizar el procreso
		UPDATE [dbo].[ReporteEmpresaServicio] SET [IdEmpresaServicio] = @IdEmpresaServicio, [Activo] = @ActivoEmpresa
		WHERE [IdReporte] = @IdReporte;
		
		--Se crea la temporal para acutalizar campos
		CREATE TABLE ##CampoTemporal ([Id] [int] NOT NULL, 
			[IdEmpresa] [int] NOT NULL, 
			[IdServicio] [int] NOT NULL, 
			[IdReporte] [int] NOT NULL, 
			[Nombre] [varchar](500) NULL, 
			[Tipo] [int] NOT NULL, 
			[largo] [varchar](10) NULL, 
			[Ordinal] [int] NULL, 
			[esVersionable] [bit] NOT NULL);

		COMMIT TRAN
		SELECT '' Error, @IdReporte IdReporte;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			SELECT ERROR_MESSAGE() Error, 0 IdReporte;
		END CATCH
	END
	ELSE IF @accion = 3
	BEGIN
		BEGIN TRAN
		BEGIN TRY

		MERGE [dbo].[Campo] AS TARGET
		USING ##CampoTemporal AS SOURCE 
		ON (TARGET.Id = SOURCE.Id) 
		--When records are matched, update the records if there is any change
		WHEN MATCHED
		THEN UPDATE SET TARGET.[IdEmpresa] = SOURCE.[IdEmpresa],
			TARGET.[IdServicio] = SOURCE.[IdServicio],
			TARGET.[IdReporte] = SOURCE.[IdReporte],
			TARGET.[Nombre] = SOURCE.[Nombre],
			TARGET.[Tipo] = SOURCE.[Tipo],
			TARGET.[largo] = SOURCE.[largo],
			TARGET.[Ordinal] = SOURCE.[Ordinal],
			TARGET.[esVersionable] = SOURCE.[esVersionable]
		WHEN NOT MATCHED BY TARGET 
		THEN INSERT ([IdEmpresa],[IdServicio],[IdReporte],[Nombre],[Tipo],[largo],[Ordinal],[esVersionable]) 
			VALUES (SOURCE.[IdEmpresa],SOURCE.[IdServicio],SOURCE.[IdReporte],SOURCE.[Nombre],SOURCE.[Tipo],SOURCE.[largo],SOURCE.[Ordinal],SOURCE.[esVersionable])
		WHEN NOT MATCHED BY SOURCE AND TARGET.IdEmpresa = @idEmpresa AND TARGET.IdServicio = @idServicio AND TARGET.IdReporte = @IdReporte 
		THEN DELETE ;

		DROP TABLE ##CampoTemporal;

		COMMIT TRAN
		SELECT '' Error, @IdReporte IdReporte;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			SELECT ERROR_MESSAGE() Error, 0 IdReporte;
		END CATCH
	END
	ELSE IF @accion = 4
	BEGIN
		--Obtener Reportes
		SELECT r.[IdReporte] Id, r.[Descripcion] Nombre, Norma Descripcion, emp.[IdEmpresa], [IdServicio], emp.[RazonSocial] [NombreEmpresa]
		FROM [SIR2].[dbo].[Reporte] r
		JOIN [SIR2].[dbo].[ReporteEmpresaServicio] res ON R.[IdReporte] = res.[IdReporte]
		JOIN [SIR2].[dbo].[EmpresaServicio] es ON res.[IdEmpresaServicio] = es.[IdEmpresaServicio]
		JOIN [SIR2].[dbo].[Empresa] emp ON emp.[IdEmpresa] = es.[IdEmpresa]

		--Obtener Campos reportes
		SELECT [Id], [IdEmpresa], [IdServicio], [IdReporte], [Nombre], [Tipo], [largo], [Ordinal], [esVersionable]
		FROM [SIR2].[dbo].[Campo]
	END

END