using SIR.Comun.Entidades.Auditoria;
using System.Collections.Generic;
using SIR.Comun.Entidades.Genericas;
using SIR.Comun.Enumeradores;
using System;

namespace SIR.Negocio.Interfaces.Auditoria
{
    public interface IAuditoriaNegocio
    {
        List<MODRastroAuditoria> ObtenerRastroAuditoriaPorId(int CampoClaveID, string ModeloDatos);
        MODResultado CrearRastroAuditoria(EnumTipoAccionAuditoria Accion, string CampoClaveID, string modeloDatos, Object AnteriorObject, Object NuevoObject, string Usuario, string ip);
        List<MODRastroAuditoria> ObtenerRastroAuditoriaPor(MODFiltroAuditoria filtro);
    }
}
