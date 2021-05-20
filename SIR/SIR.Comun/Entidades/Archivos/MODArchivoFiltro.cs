using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Comun.Entidades.Archivos
{
    [DataContract]
    public class MODArchivoFiltro
    {
        [DataMember]
        public int? IdArchivo { get; set; }
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
