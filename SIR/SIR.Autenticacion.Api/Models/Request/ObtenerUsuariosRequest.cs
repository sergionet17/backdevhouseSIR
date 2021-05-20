using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace SIR.Autenticacion.Api.Models.Request
{
    [DataContract]
    public class ObtenerUsuariosRequest
    {
        public bool? Activo { get; set; }
        public string Usuario { get; set; }
        public int? IdEmpresa { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int? Desde { get; set; }
        public int? Hasta { get; set; }
    }
}
