-- =============================================
-- Author:		<Rafael Alarcón>
-- Create date: <2020-02-22>
-- Description:	<Retorna todos los modulos de SIR que se encuentran activos>
-- =============================================

CREATE procedure [dbo].[StpConsultarModulos]
as
begin
	select IdModulo
		 , Descripcion as DescripcionModulo
		 , Activo
	from dbo.Modulo with(nolock)
	where Activo = 1
	order by IdModulo
end

