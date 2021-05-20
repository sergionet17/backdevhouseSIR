using System.ComponentModel;

namespace SIR.Comun.Enumeradores
{
    public enum EnumTipoPermiso : byte
    {
        [Description("SINPERMISO")]
        SINPERMISO,

        [Description("LECTURA")]
        LECTURA,

        [Description("ESCRITURA")]
        ESCRITURA
    }
}