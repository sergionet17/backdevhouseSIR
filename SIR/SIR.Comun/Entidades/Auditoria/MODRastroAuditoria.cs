using System;
using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores;

namespace SIR.Comun.Entidades.Auditoria
{
    public class MODRastroAuditoria : MODBase
    {
        public EnumTipoAccionAuditoria TipoAccionAuditoria { get; set; }
        public string TipoAccionAuditoriaNombre { get{ return Enum.GetName(typeof(EnumTipoAccionAuditoria), this.TipoAccionAuditoria);} }
        public string ModeloDatos { get; set; }
        public DateTime FechaAccion { get; set; }
        public string CampoClaveID { get; set; }
        public string ValorAntes { get; set; }
        public string ValorDespues { get; set; }
        public string Cambios { get; set; }
    }
}