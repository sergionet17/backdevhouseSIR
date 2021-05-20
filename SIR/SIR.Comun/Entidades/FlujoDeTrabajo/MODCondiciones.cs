using SIR.Comun.Enumeradores;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SIR.Comun.Entidades.FlujoDeTrabajo
{
    [DataContract]
    [Serializable]
    public class MODCondiciones
    {
        [DataMember]
        [Column(Order = 1, TypeName = "ID")]
        public int Id { get; set; }
        [DataMember]
        [Column(Order = 2, TypeName = "IdCampo")]
        public int IdCampo { get; set; }
        [DataMember]
        public string Campo { get; set; }
        [DataMember]
        public EnumTipoDato TipoCampo { get; set; }
        [DataMember]
        [Column(Order = 4, TypeName = "Valor")]
        public string Valor { get; set; }
        [DataMember]
        [Column(Order = 5, TypeName = "Condicion")]
        public EnumCondiciones Condicion { get; set; }
        [DataMember]
        [Column(Order = 6, TypeName = "Conector")]
        public EnumConectores Conector { get; set; }
        [DataMember]
        [Column(Order = 7, TypeName = "Nivel")]
        public int Nivel { get; set; }
        [DataMember]
        [Column(Order = 3, TypeName = "IdHomologacion")]
        public int IdHomologacion { get; set; }
        [DataMember]
        [Column(Order = 8, TypeName = "codigo_Externo_Homologacion")]
        public Guid codigo_Externo_Homologacion { get; set; }
    }
}
