using SIR.Comun.Entidades.Abstracto;
using SIRBackend.SIR.SIR.Comun.Entidades.FlujoDeTrabajo;

namespace SIR.Comun.Entidades.FlujoDeTrabajo{
    public class MODOpciones : MODBase{
        public int id {get;set;}
        public string descripcion {get;set;}
        public string marca {get;set;}
        public string campo {get;set;}
        public MODProcTipoOpcion procTipoOpcion {get;set;}
    }
}