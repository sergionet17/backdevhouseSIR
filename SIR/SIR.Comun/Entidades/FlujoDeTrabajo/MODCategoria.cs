using SIR.Comun.Entidades.Abstracto;

namespace SIR.Comun.Entidades.FlujoDeTrabajo{
    public class MODCategoria : MODBase{
        public int Id{get;set;}
        public string Nombre{get;set;}
        public bool Activo{get;set;}
    }
}