using SIR.Comun.Entidades.Abstracto;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.Reportes
{
    [DataContract]
    [Serializable]
    public class MODReporte : MODBase
    {
        [DataMember]
        public int IdReporteEmpresaServicio { get; set; }
        [DataMember] 
        public int Id { get; set; }
        [DataMember]
        public string Nombre { get; set; }
        [DataMember]
        public string Descripcion { get; set; }
        [DataMember]
        public int IdServicio { get; set; }
        [DataMember]
        public int IdEmpresa { get; set; }
        [DataMember]
        public string NombreEmpresa { get; set; }
        [DataMember]
        public bool Activo { get; set; }
        [DataMember]
        public bool ActivoEmpresa { get; set; }
        [DataMember]
        public List<MODCampos> campos { get; set; }
        [DataMember]
        public int IdTarea { get; set; }
        [DataMember]
        public int IdCategoria { get; set; }
        [DataMember]
        public string Categoria { get; set; }
        [DataMember]
        public bool EsReporte { get; set; }
    }
}
