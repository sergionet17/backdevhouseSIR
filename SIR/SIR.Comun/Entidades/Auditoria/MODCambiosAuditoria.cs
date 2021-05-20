using System;
using System.Collections.Generic;
using SIR.Comun.Enumeradores;

namespace SIR.Comun.Entidades.Auditoria
{
    public class MODCambiosAuditoria
    {
        public string DateTimeStamp { get; set; }
        public EnumTipoAccionAuditoria TipoAccionAuditoria { get; set; }
        public string TipoAccionAuditoriaNombre { get{ return Enum.GetName(typeof(EnumTipoAccionAuditoria), this.TipoAccionAuditoria);} }
        public string NombreUsuario {get;set;}
        public string ModeloDatos {get;set;}
        public List<DeltaAuditoria> Cambios { get; set; }
        public MODCambiosAuditoria()
        {
            Cambios = new List<DeltaAuditoria>();
        }
    }
}
