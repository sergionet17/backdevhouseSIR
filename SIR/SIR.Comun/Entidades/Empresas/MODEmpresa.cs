using System;
using System.Collections.Generic;
using SIR.Comun.Entidades.Abstracto;
using Newtonsoft.Json;

namespace SIR.Comun.Entidades
{
    public class MODEmpresa : MODBase
    {
        [JsonProperty(PropertyName = "IdEmpresa")] 
        public int IdEmpresa { get; set; }
        [JsonProperty(PropertyName = "NumeroIdentificacion")] 
        public string NumeroIdentificacion { get; set; }
        [JsonProperty(PropertyName = "RazonSocial")]
        public string RazonSocial { get; set; }
        [JsonProperty(PropertyName = "Correo")]
        public string Correo{ get; set; }
        [JsonProperty(PropertyName = "Activo")]
        public bool Activo{ get; set; }
        [JsonProperty(PropertyName = "Descripcion")]
        public string Descripcion { get; set; }
        [JsonProperty(PropertyName = "FechaCreacion")]
        public DateTime FechaCreacion{ get; set; }
        [JsonProperty(PropertyName = "Servicios")]
        public List<MODServicio> Servicios { get; set; }
        public MODEmpresa()
        {
            Servicios = new List<MODServicio>();
        }

    }
}
