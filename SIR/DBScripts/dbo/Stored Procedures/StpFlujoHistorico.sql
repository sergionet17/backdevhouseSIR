-- =============================================
-- Author:		Jonathan Edgardo Atencia Barranco
-- Create date: 02-03-2021
-- Description:	CRUD de historico de reportes, accion 1 = Crear, 2 = Actualizar
-- =============================================
CREATE PROCEDURE [dbo].[StpFlujoHistorico]
	@accion int = 0, 
	@IdHistorico	int= 0, 
	@IdEmpresa	int= 0, 
	@IdServicio	int= 0, 
	@IdElemento	int= 0, 
	@TipoFlujo	int= 0,
	@IdTarea	int= 0, 
	@FechaCreacion datetime = null,
	@FechaFinalizacion datetime = null,
	@EsValido	bit = 1,
	@DescripcionError varchar(500) = '',
	@IdFlujo int = 0
AS
BEGIN

	IF @accion = 1
	BEGIN 
		BEGIN TRAN
		BEGIN TRY
			INSERT INTO [dbo].[FlujoHistorico] 
				( [IdEmpresa], [IdServicio], [IdElemento], [TipoFlujo], [IdTarea], [FechaCreacion], [FechaFinalizacion], [EsValido], [DescripcionError], [IdFlujo])
			VALUES
				(@IdEmpresa, @IdServicio, @IdElemento, @TipoFlujo, @IdTarea, @FechaCreacion, @FechaFinalizacion, @EsValido, @DescripcionError, @IdFlujo)

		COMMIT TRAN
		SELECT SCOPE_IDENTITY() IdHistorico;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			SELECT ERROR_MESSAGE() Error, 0 IdHistorico;
		END CATCH
	END
	ELSE IF @accion = 2
	BEGIN
		BEGIN TRAN
		BEGIN TRY
			UPDATE [dbo].[FlujoHistorico]
			   SET [IdEmpresa] = @IdEmpresa
				  ,[IdServicio] = @IdServicio
				  ,[IdElemento] = @IdElemento
				  ,[TipoFlujo] = @TipoFlujo
				  ,[IdTarea] = @IdTarea
				  ,[FechaFinalizacion] = @FechaFinalizacion
				  ,[EsValido] = @EsValido
				  ,[DescripcionError] = @DescripcionError
			 WHERE Id = @IdHistorico;
		COMMIT TRAN
		SELECT '' Error, @IdHistorico IdHistorico;
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			SELECT ERROR_MESSAGE() Error, 0 IdHistorico;
		END CATCH
	END
	ELSE IF @accion = 3
	BEGIN
		SELECT '' Error, COUNT(DISTINCT Id) IdHistorico FROM [dbo].[FlujoHistorico]
		WHERE [IdEmpresa] = @IdEmpresa 
			AND [IdServicio] = @IdServicio
			AND [IdElemento] = @IdElemento
			AND [TipoFlujo] = @TipoFlujo
			AND [IdFlujo] = @IdFlujo
			AND [IdTarea] = @IdTarea
			AND Convert(date, [FechaCreacion]) = Convert(date, @FechaCreacion);
		
	END


END