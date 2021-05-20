CREATE procedure [dbo].[StpInsertarUsuario]
(
	  @prmUsuario varchar(30)
	, @prmNombre varchar(50)
	, @prmApellido varchar(50)
	, @prmEsAdministrador bit = 0
	, @prmIdUsuario int output
)
as
begin
	if not exists(select Usuario from dbo.Usuario with(nolock) where Usuario = rtrim(ltrim(@prmUsuario)))
	begin
		insert into dbo.Usuario(Usuario, FechaRegistro, EsAdministrador, Nombre, Apellido)
		values(@prmUsuario, GETDATE(), @prmEsAdministrador, @prmNombre, @prmApellido)

		select @prmIdUsuario = SCOPE_IDENTITY()
	end
	else 
	begin
		select @prmIdUsuario = IdUsuario from dbo.Usuario with(nolock) where Usuario = rtrim(ltrim(@prmUsuario))
	end
end
