-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2021-02-23>
-- Description:	<Inserta un objeto tipo tabla para asignar los permisos de un usuario>
-- =============================================
CREATE PROCEDURE [dbo].[StpInsertarPermisosUsuario] 
(
	@prmPermisosUsuario tblPermisosUsuario readonly
)
AS
BEGIN
	
	MERGE dbo.PermisosUsuario AS TARGET
	USING @prmPermisosUsuario AS SOURCE 
	ON (TARGET.IdModuloEmpresaServicio = SOURCE.IdModuloEmpresaServicio AND TARGET.IdUsuario = SOURCE.IdUsuario) 
	
	WHEN MATCHED AND TARGET.IdTipoPermiso <> SOURCE.TipoPermiso
	THEN UPDATE SET TARGET.IdTipoPermiso = SOURCE.TipoPermiso
	
	WHEN NOT MATCHED BY TARGET 
	THEN INSERT(IdUsuario, IdModuloEmpresaServicio, IdTipoPermiso) VALUES (SOURCE.IdUsuario, SOURCE.IdModuloEmpresaServicio, SOURCE.TipoPermiso)
	
	WHEN NOT MATCHED BY SOURCE AND TARGET.IdUsuario IN (SELECT IdUsuario FROM @prmPermisosUsuario)
	THEN DELETE;

END
