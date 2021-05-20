using SIR.Comun.Entidades.Abstracto;

namespace SIRBackend.SIR.SIR.Comun.Entidades.FlujoDeTrabajo
{
    public class MODProcTipoOpcion : MODBase
    {
        public int id {get;set;}
        public int id_tipo_proceso {get;set;}
        public int id_tipo {get;set;}
        public int id_opcion {get;set;}
    }
}