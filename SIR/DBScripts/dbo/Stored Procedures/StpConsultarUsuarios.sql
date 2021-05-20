-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-02-26>
-- Description:	<Retorna todos los usuarios por diferentes filtros y de manera paginada.>
-- EXEC [dbo].[StpConsultarUsuarios] null,'j',null,null,null,null, null
-- =============================================

CREATE procedure [dbo].[StpConsultarUsuarios]
(
	  @prmActivo bit = null
	, @prmUsuario varchar(30) = null
	, @prmIdEmpresa int = null
	, @prmNombre varchar(50) = null
	, @prmApellido varchar(50) = null
	, @prmDesdeRegitro int = null
	, @prmHastaRegistro int = null
)
as
begin
	select distinct
		  u.IdUsuario
		, u.Usuario as UserName
		, u.Activo
		, u.FechaRegistro
		, u.FechaUltimoLogin
		, u.FechaModificacion
		, u.EsAdministrador
		, u.Nombre
		, u.Apellido
	from dbo.Usuario u with(nolock)
		left join dbo.PermisosUsuario pu with(nolock) on pu.IdUsuario = u.IdUsuario
		left join dbo.ModuloEmpresaServicio mes with(nolock) on mes.IdModuloEmpresaServicio = pu.IdModuloEmpresaServicio
		left join dbo.EmpresaServicio es with(nolock) on es.IdEmpresaServicio = mes.IdEmpresaServicio
	where ((u.Activo = @prmActivo or @prmActivo is null)
		and (u.Usuario like '%' + @prmUsuario + '%' or @prmUsuario is null)
		and (es.IdEmpresa = @prmIdEmpresa or @prmIdEmpresa is null)
		and (u.Nombre like '%'+ @prmNombre +'%' or @prmNombre is null)
		and (u.Apellido like '%'+ @prmApellido +'%' or @prmApellido is null))
		and u.EsAdministrador = 0

	order by u.FechaRegistro desc

	OFFSET isnull(@prmDesdeRegitro - 1, 0) rows
	fetch next isnull((@prmHastaRegistro - (@prmDesdeRegitro - 1)), 100) rows ONLY;

end