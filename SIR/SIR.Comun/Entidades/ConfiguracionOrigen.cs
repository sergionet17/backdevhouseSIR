using SIR.Comun.Entidades.FlujoDeTrabajo;
using System.Collections.Generic;

namespace SIR.Comun.Entidades
{
    public class ConfiguracionOrigen
    {
        public int id { get; set; }
        public Plantilla plantillaDestino { get; set; }
        public List<MODOrigenDatos> origenes { get; set; }
        public List<MODCampos> campos { get; set; }
        public IDictionary<int,List<Homologacion>> homologacion { get; set; }
        public List<RelacionOrigen> relacion;
        public ConfiguracionOrigen()
        {
            this.origenes = new List<MODOrigenDatos>();
        }
        public void AgregarOrigen(MODOrigenDatos origen)
        {
            this.origenes.Add(origen);
            this.homologacion.Add(origen.id, new List<Homologacion>());
        }
    }
}
