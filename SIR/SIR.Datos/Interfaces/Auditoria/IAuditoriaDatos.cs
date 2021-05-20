using System.Collections.Generic;
using SIR.Comun;
using SIR.Comun.Entidades.Auditoria;
using SIR.Comun.Entidades.Genericas;

namespace SIR.Datos.Interfaces.Auditoria{
    public interface IAuditoriaDatos{
        List<MODRastroAuditoria> ObtenerRastroAuditoria();
        MODResultado CrearRastroAuditoria(MODRastroAuditoria rastro);
        List<MODRastroAuditoria> ObtenerRastroAuditoriaPorId(int CampoClaveID,string modeloDatos);
        List<MODRastroAuditoria> ObtenerRastroAuditoriaPor(MODFiltroAuditoria filtro);
    }
}