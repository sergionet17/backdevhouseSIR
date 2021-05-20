-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-03-12>
-- Description:	<Elimina un usuario del sistema, primero los permisos y luego el usuario dado el Id.>
-- =============================================
CREATE PROCEDURE dbo.StpEliminarUsuario (
	@prmIdUsuario int 
)
AS 
BEGIN
	DELETE FROM dbo.PermisosUsuario WHERE IdUsuario = @prmIdUsuario
	DELETE FROM dbo.Usuario WHERE IdUsuario = @prmIdUsuario
END