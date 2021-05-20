-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2020-02-22>
-- Description:	<Retorna todos los tipos de permisos que se encuentran activos>
-- =============================================

create procedure [dbo].[StpConsultarTipoPermisos]
as
begin
	select IdTipoPermiso
		 , Descripcion as DescripcionTipoPermiso
	from dbo.TipoPermiso with(nolock)
	where Activo = 1
end

exec [StpConsultarTipoPermisos]
