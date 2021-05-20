
-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-03-01>
-- Description:	<Actualiza el estado, nombre y apellido de usuario de acuerdo el Id.>
-- =============================================
CREATE procedure [dbo].[StpActualizarUsuario]
(
	  @prmIdUsuario int
	, @prmActivo bit
	, @prmNombre varchar(50)
	, @prmApellido varchar(50)
)
as 
begin

	update 
		dbo.Usuario
	set   Activo = @prmActivo
		, Nombre = @prmNombre
		, Apellido = @prmApellido
		, FechaModificacion = GETDATE()
	where
		IdUsuario = @prmIdUsuario
end