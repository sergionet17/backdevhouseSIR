using System.ComponentModel;

namespace SIR.Comun.Enumeradores.FlujoDeTrabajo
{
    public enum EnumProceso
    {
        [Description("Proceso para solicitar información externa al SIR")]
        Obtener = 1,
        [Description("Proceso para almacenar información en SIR")]
        Registrar = 2,
        [Description("Proceso de comprobación de estructura y data")]
        Validar = 3,
        [Description("Proceso de ajuste de información según normas del usuario")]
        Homologar = 4,
        [Description("Proceso para consultas como las validaciones en origenes externos al SIR")]
        Ejecutar = 5,
        [Description("Proceso que espera la confirmación del usuario")]
        Confirmar = 6,
        [Description("Proceso de notificacion por correo electronico")]
        Notificar = 7,
        [Description("Proceso para generar archivo de salida")]
        Archivo = 8,
        [Description("Proceso para obtener información temporal y ejecutar script")]
        Combinacion = 9,
        [Description("Cerrar la ejecucion del flujo")]
        Finalizar = 10
    }
}
