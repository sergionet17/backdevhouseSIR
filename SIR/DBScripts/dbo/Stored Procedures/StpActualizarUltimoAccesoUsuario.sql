
-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2020-02-15>
-- Description:	<Actualiza la ultima fecha de autenticación exitosa de un usuario>
-- =============================================

create procedure [dbo].[StpActualizarUltimoAccesoUsuario]
(
	@prmUsuario varchar(30)
)
as
begin
	update dbo.Usuario 
	set FechaUltimoLogin = GETDATE()
	where Usuario = @prmUsuario and Activo = 1
end