
-- =============================================
-- Author:		Jonathan Edgardo Atencia Barranco
-- Create date: 24/02/2021
-- Description:	Se Obtienen todas los flujos y tareas axistentes que esten activos
-- EXEC dbo.StpObtenerFlujos
-- =============================================

CREATE procedure [dbo].[StpObtenerFlujos]
as
begin
	--001. Obtener Flujos
	SELECT [Id] IdFlujo, [Accion], F.[IdEmpresa], E.[RazonSocial] [NombreEmpresa], F.[IdElemento], [Elemento], [Tipo], [IdServicio], [SubTipo]
	FROM [SIR2].[dbo].[Flujos] F
	LEFT JOIN [SIR2].[dbo].[Empresa] E ON F.[IdEmpresa] = E.[IdEmpresa]

  --Obtener Tareas
	SELECT [Id] IdTarea, [IdFlujo], [Proceso], [IdPadre], [Orden]
	FROM [SIR2].[dbo].[Tareas]
	ORDER BY [Orden];

  --Obtener Configuracion 
	SELECT [id], [nombre], [tipoOrigen], [Usuario], [Clave], [Servidor], [SID], [Puerto], [idTarea], [consulta], [tipoMando]
	FROM [SIR2].[dbo].[OrigenDeDatos];

	--Obtener Homologaciones
	SELECT O.[id], [IdTarea], [IdCampo], C.[nombre] NombreCampo, C.[tipo] TipoCampo, [ValorNo], [ValorSi], [TipoReemplazo]
	FROM [SIR2].[dbo].[Homologaciones] O
	JOIN [SIR2].[dbo].[Campo] C ON O.[IdCampo] = C.id;

	--Obtener Condiciones Homologacion
	SELECT Co.[Id], Co.[IdCampo], C.[nombre] Campo, C.[tipo] TipoCampo,[IdHomologacion],[Valor],[Condicion],[Conector],[Nivel]
	FROM [SIR2].[dbo].[Condiciones] Co
	JOIN [SIR2].[dbo].[Campo] C ON Co.[IdCampo] = C.id;

	--Obtener Reportes
	SELECT r.[IdReporte] Id, [Descripcion] Nombre, Norma Descripcion, [IdEmpresa], [IdServicio]
	FROM [SIR2].[dbo].[Reporte] r
	JOIN [SIR2].[dbo].[ReporteEmpresaServicio] res ON R.[IdReporte] = res.[IdReporte]
	JOIN [SIR2].[dbo].[EmpresaServicio] es ON res.[IdEmpresaServicio] = es.[IdEmpresaServicio]

	--Obtener Campos reportes
	SELECT [Id], [IdEmpresa], [IdServicio], [IdReporte], [Nombre], [Tipo], [largo], [Ordinal], [esVersionable]
	FROM [SIR2].[dbo].[Campo]
end