
-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-03-02>
-- Description:	<Retorna los modulos asociados a una empresa por IdEmpresa y opcional por IdServicio.>
-- =============================================
CREATE procedure [dbo].[StpConsultarModulosxEmpresaServicio]
(
	  @prmIdEmpresa int
	, @prmIdServicio int = null
)
as 
begin
	select distinct
		  m.IdModulo
		, m.Descripcion as DescripcionModulo
		, m.Activo 
	from 
		dbo.EmpresaServicio es with(nolock) 
		join dbo.ModuloEmpresaServicio mes with(nolock) on mes.IdEmpresaServicio = es.IdEmpresaServicio
		join dbo.Modulo m with(nolock) on m.IdModulo = mes.IdModulo
	where es.IdEmpresa = @prmIdEmpresa and es.IdServicio = ISNULL(@prmIdServicio, es.IdServicio) 
	order by m.IdModulo
end