using SIR.Comun.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Datos.Interfaces.FlujoDeTrabajo
{
    public interface IConfiguracionDatos
    {
        List<MODServicio> ObtenerServicios();
        List<MODEmpresa> ObtenerEmpresas();
    }
}
