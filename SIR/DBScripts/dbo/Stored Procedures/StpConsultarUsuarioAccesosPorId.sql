-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2020-02-10>
-- Description:	<De acuerdo a un usuario de entrada, retorna objeto de usuario, modulos a los que tiene acceso de acuerdo a los servicios
			-- y empresas y los reportes permitidos.>
-- EXEC dbo.StpConsultarUsuarioAccesos 'elpropio'
-- =============================================

CREATE procedure [dbo].[StpConsultarUsuarioAccesosPorId]
(
	@prmIdUsuario int
)
as
begin
	if exists(select rtrim(ltrim(Usuario)) from dbo.Usuario with(nolock) where IdUsuario = @prmIdUsuario)
	begin

		--Información del usuario
		select 
			 u.IdUsuario
			, u.Usuario as UserName
			, u.FechaRegistro
			, u.FechaUltimoLogin 
			, u.EsAdministrador
			, u.Nombre
			, u.Apellido
			, u.Activo
		from dbo.Usuario u with(nolock)
		where u.IdUsuario = @prmIdUsuario

		--Información de los Modulos a los que tiene acceso. 
		select 
			  tp.Descripcion as DescripcionTipoPermiso
			, m.Descripcion as DescripcionModulo
			, e.RazonSocial 
			, s.Descripcion as DescripcionServicio
			, tp.IdTipoPermiso as TipoPermiso
			, m.IdModulo
			, e.IdEmpresa
			, s.IdServicio
			, mes.Activo
			, mes.IdModuloEmpresaServicio
		from dbo.PermisosUsuario pu with(nolock)
			join dbo.TipoPermiso tp with(nolock) on tp.IdTipoPermiso = pu.IdTipoPermiso
			join dbo.ModuloEmpresaServicio mes with(nolock) on mes.IdModuloEmpresaServicio = pu.IdModuloEmpresaServicio
			join dbo.Modulo m with(nolock) on m.IdModulo = mes.IdModulo
			join dbo.EmpresaServicio es with(nolock) on es.IdEmpresaServicio = mes.IdEmpresaServicio
			join dbo.Empresa e with(nolock) on e.IdEmpresa = es.IdEmpresa 
			join dbo.Servicio s with(nolock) on s.IdServicio = es.IdServicio
		where pu.IdUsuario = @prmIdUsuario
			and tp.Activo = 1 and mes.Activo = 1 and m.Activo = 1
		group by 
			  tp.Descripcion
			, m.Descripcion 
			, e.RazonSocial 
			, s.Descripcion 
			, tp.IdTipoPermiso
			, m.IdModulo
			, e.IdEmpresa
			, s.IdServicio
			, mes.Activo
			,mes.IdModuloEmpresaServicio

	--Información de reportes permitidos
		select 
				r.IdReporte
			  , r.Descripcion
			  , e.IdEmpresa
			, s.IdServicio
		from dbo.PermisosUsuario pu with(nolock)
			join dbo.ModuloEmpresaServicio mes with(nolock) on mes.IdModuloEmpresaServicio = pu.IdModuloEmpresaServicio
			join dbo.EmpresaServicio es with(nolock) on es.IdEmpresaServicio = mes.IdEmpresaServicio
			join dbo.ReporteEmpresaServicio res with(nolock) on res.IdEmpresaServicio = es.IdEmpresaServicio
			join dbo.Reporte r with(nolock) on r.IdReporte = res.IdReporte
			join dbo.Empresa e with(nolock) on e.IdEmpresa = es.IdEmpresa 
			join dbo.Servicio s with(nolock) on s.IdServicio = es.IdServicio
		where pu.IdUsuario = @prmIdUsuario
			and mes.Activo = 1 and es.Activo = 1 and res.Activo = 1 and r.Activo = 1
		group by 
			  r.IdReporte
			, r.Descripcion 
			, e.IdEmpresa
			, s.IdServicio
	end
end