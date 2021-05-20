using SIR.Comun.Enumeradores;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades
{
    [DataContract]
    [Serializable]
    public class MODCampos
    {
        [DataMember]
        [Column(Order = 1, TypeName = "Id")]
        public int Id { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "IdEmpresa")]
        public int IdEmpresa { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "IdServicio")]
        public int IdServicio { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "IdReporte")]
        public int IdReporte { get; set; }
        [DataMember]
        [Column(Order = 5, TypeName = "Nombre")]
        public string Nombre { get; set; }
        [DataMember]
        [Column(Order = 6, TypeName = "Tipo")]
        public EnumTipoDato Tipo { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "Largo")]
        public string Largo { get; set; }
        [DataMember]
        [Column(Order = 8, TypeName = "Ordinal")]
        public int Ordinal { get; set; }
        [DataMember]
        [Column(Order = 9, TypeName = "esVersionable")]
        public bool esVersionable { get; set; }
        [DataMember]
        [Column(Order = 10, TypeName = "esEditable")]
        public bool esEditable { get; set; }
        [DataMember]
        [Column(Order = 11, TypeName = "nombreVisible")]
        public string nombreVisible { get; set; }

    }
}
