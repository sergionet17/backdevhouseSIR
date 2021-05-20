using System.Collections.Generic;
using SIR.Comun.Entidades.Abstracto;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;

namespace SIR.Comun.Entidades.FlujoDeTrabajo{
    public class MODGruposDeEjecucion:MODBase{
        public int Id {get;set;}
        public int IdCategoria{get;set;}
        public string Nombre{get;set;}
        public int Orden {get;set;}
        public List<MODTarea> Tareas{get;set;} 
        public List<MODRelGrupoEjecucion> relGrupoEjecucions{get;set;}
    }
}