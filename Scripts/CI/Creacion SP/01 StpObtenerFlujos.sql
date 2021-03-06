USE [SIR2]
GO

-- =============================================
-- Author:		Jonathan Edgardo Atencia Barranco
-- Create date: 24/02/2021
-- Description:	Se Obtienen todas los flujos y tareas axistentes que esten activos
-- EXEC dbo.StpObtenerFlujos
-- =============================================

CREATE procedure [dbo].[StpObtenerFlujos]
as
begin
	--Obtener Flujos
	SELECT [Id]
	,[Accion]
	,F.[IdEmpresa]
	,F.[IdElemento]
	, E.[RazonSocial] [NombreEmpresa]
	,[Elemento]
	,[Tipo]
	FROM [SIR2].[dbo].[Flujos] F
	LEFT JOIN [SIR2].[dbo].[Empresa] E ON F.[IdEmpresa] = E.[IdEmpresa]

  --Obtener Tareas
	SELECT [Id]
	,[Proceso]
	,[IdPadre]
	,[IdFlujo]
	,[Orden]
	,[IdConfiguracion]
	FROM [SIR2].[dbo].[Tareas]
	ORDER BY [Orden];

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
	FROM [SIR2].[dbo].[OrigenDeDatos];

	--Obtener Omologaciones
	SELECT O.[id]
	,[IdTarea]
	,[IdCampo]
	, C.[nombre] NombreCampo
	, C.[tipo] TipoCampo
	,[ValorOriginal]
	,[ValorNuevo]
	FROM [SIR2].[dbo].[Omologaciones] O
	JOIN [SIR2].[dbo].[Campo] C ON O.[IdCampo] = C.id
end