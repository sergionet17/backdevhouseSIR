-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-03-03>
-- Description:	<Consulta los servicios asociados a una empresa>
-- EXEC StpConsultarServiciosxEmpresa 1
-- =============================================
CREATE PROCEDURE StpConsultarServiciosxEmpresa
	@prmIdEmpresa int
AS
BEGIN
	
	select 
		es.IdEmpresaServicio
		, e.IdEmpresa
		, s.IdServicio
		, s.Descripcion as DescripcionServicio
	from dbo.Empresa e with(nolock)
	join dbo.EmpresaServicio es with(nolock) on es.IdEmpresa = e.IdEmpresa
	join dbo.Servicio s with(nolock) on s.IdServicio = es.IdServicio
	where e.IdEmpresa = @prmIdEmpresa
		and e.Activo = 1
END