using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SIR.Autenticacion.Api.Models.Request
{
    [DataContract]
    public class ObtenerArchivosRequestModel
    {
        [DataMember]
        public int? IdTipoArchivo { get; set; }
        [DataMember]
        public int? IdReporte { get; set; }
        [DataMember]
        public int? IdSeparador { get; set; }
        [DataMember]
        public string? Nombre { get; set; }
        [DataMember]
        public int? Desde { get; set; }
        [DataMember]
        public int? Hasta { get; set; }
    }
}
