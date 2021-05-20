-- =============================================
-- Author:		Carlos Torres
-- Create date: 05-03-2021
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[StpAuditoria] @accion INT = 0,
@Id INT = 0,
@TipoAccionAuditoria INT = 0,
@ModeloDatos VARCHAR(500) = '',
@FechaAccion DATETIME = NULL,
@CampoClaveID BIGINT = 0,
@ValorAntes NVARCHAR(MAX) = '',
@ValorDespues NVARCHAR(MAX) = '',
@Cambios NVARCHAR(MAX) = '',
@Usuario varchar(30) = '',
@Ip VARCHAR(20) = '',
@FechaInicio DATETIME = NULL,
@FechaFin DATETIME = NULL
AS
BEGIN
	IF @accion = 1
	BEGIN
		SELECT *
		FROM RastroAuditoria;
	END
	ELSE IF @accion = 2		
	BEGIN
		SELECT *
		FROM RastroAuditoria
		WHERE CampoClaveID = @CampoClaveID AND ModeloDatos = @ModeloDatos
	END
	ELSE IF @accion = 3
	BEGIN
		INSERT INTO RastroAuditoria(TipoAccionAuditoria,ModeloDatos,FechaAccion,CampoClaveID,ValorAntes,ValorDespues,Cambios,Usuario,[Ip])
		VALUES (@TipoAccionAuditoria,@ModeloDatos,@FechaAccion,@CampoClaveID,@ValorAntes,@ValorDespues,@Cambios,@Usuario,@Ip);		
		SELECT SCOPE_IDENTITY();
	END
	ELSE IF @accion = 4
	BEGIN
		SELECT *
		FROM RastroAuditoria
		WHERE Usuario = @Usuario
		AND FechaAccion BETWEEN @FechaInicio AND @FechaFin
	END
END