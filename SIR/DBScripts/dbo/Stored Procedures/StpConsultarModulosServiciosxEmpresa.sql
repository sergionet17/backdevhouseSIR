-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2020-02-22>
-- Description:	<Retorna todos los modulos y servicios por empresa dado el IdEmpresa>
-- =============================================
--exec [StpConsultarModulosServiciosxEmpresa] 1, 2
CREATE procedure [dbo].[StpConsultarModulosServiciosxEmpresa]
(
	@prmIdEmpresa int,
	@prmIdServicio int = NULL
)
as
begin
	select 
		mes.IdModuloEmpresaServicio
		, s.IdServicio
		, s.Descripcion as DescripcionServicio
		, m.IdModulo
		, m.Descripcion as DescripcionModulo
	from dbo.Empresa e with(nolock)
	join dbo.EmpresaServicio es with(nolock) on es.IdEmpresa = e.IdEmpresa
	join dbo.Servicio s with(nolock) on s.IdServicio = es.IdServicio
	join dbo.ModuloEmpresaServicio mes with(nolock) on mes.IdEmpresaServicio = es.IdEmpresaServicio
	join dbo.Modulo m with(nolock) on m.IdModulo = mes.IdModulo
	where e.IdEmpresa = @prmIdEmpresa
		and e.Activo = 1
		and es.Activo = 1
		and mes.Activo = 1
		and (es.IdServicio = @prmIdServicio or @prmIdServicio is null)

end











