--=============================================
-- Author:		Jonathan Edgardo Atencia Baranco
-- Create date: 24/02/2021
-- Description:	Obtenr configuraciones
-- accion 1 : origen de datos
-- =============================================
CREATE PROCEDURE [dbo].[StpObtenerConfiguracion]
	@accion int = 0, 
	@IdConfiguracion int= 0 
AS
BEGIN
	DECLARE @ids table(id INT);

	if @accion = 1
	begin
		--Obtener Configuracion 
		SELECT [id]
		,[id_configuracion]
		,[nombre]
		,[tipoOrigen]
		,[Usuario]
		,[Clave]
		,[Servidor]
		,[SID]
		,[Puerto]
		,[consulta]
		,[tipoMando]
		FROM [SIR2].[dbo].[OrigenDeDatos]
		WHERE [id] = @IdConfiguracion;
		--Obtener Campos
		SELECT C.[id]
		,[nombre]
		,[tipo]
		,[ordinal]
		FROM [SIR2].[dbo].[Campo] C
		JOIN [SIR2].[dbo].[PlantillaOrigenCampo] Poc On C.id = Poc.id_campo
		WHERE Poc.id_origen = @IdConfiguracion;
	end
	else if @accion = 2
	begin
		select 2
	end
	else if @accion = 3
	begin
		select 3
	end
	else if @accion = 4
	begin
		select 4
	end
	else if @accion = 5
	begin
		select 5
	end
	else if @accion = 6
	begin
		select 6
	end
END
