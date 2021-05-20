using System.ComponentModel;

namespace SIR.Comun.Enumeradores.FlujoDeTrabajo
{
    public enum EnumTipoReemplazo
    {
        [Description("No aplica formula y se reemplaza el valor directamente")]
        Constante = 1,
        [Description("Es por medio de una ecuacion definida por el usuario y tiene dos posibles respuestas.")]
        Variable = 2
    }
}
