using SIR.Comun.Entidades.Abstracto;

namespace SIR.Comun.Entidades.FlujoDeTrabajo{
    public class MODRelGrupoEjecucion : MODBase{
        public int Id{get;set;}
        public int IdEmpresa{get;set;}
        public int IdServicio{get;set;}
        public int IdReporte{get;set;}
        public int IdGrupoEjecucion{get;set;}
        public int IdRelGrupoEjecuicion {get{return Id;} set{Id = value;}}

    }
}