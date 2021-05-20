using SIR.Comun.Entidades.Archivos;
using System;
using System.Collections.Generic;

namespace SIR.Negocio.Interfaces.Archivos
{
    public interface IArchivoNegocio
    {
        List<MODTipoArchivo> ObtenerTipoArchivos();
        List<MODArchivo> ObtenerArchivos(MODArchivoFiltro prmFiltro);
        long CrearArchivo(MODArchivo prmArchivo);
        bool ActualizarArchivo(MODArchivo prmArchivo);
        bool EliminarArchivo(long prmIdArchivo);
        List<MODSeparadorArchivo> ObtenerSeparadorArchivos();
        long CrearLogGeneracionArchivo(MODLogGeneracionArchivo prmLogGeneracionArchivo);
        List<MODLogGeneracionArchivo> ConsultarLogGeneracionArchivos(int prmIdArchivo);
        List<MODCamposArchivo> ConsultarCamposArchivos(int prmIdArchivo);
        bool InsertarCamposArchivo(List<MODCamposArchivo> prmCamposArchivo);
        bool GenerarArchivo(int prmIdArchivo, ref List<string> mensajesError, DateTime prmPeriodo);
        bool GenerarArchivo(MODArchivo prmArchivo, ref List<string> mensajesError);
        bool CrearArchivo(MODArchivo prmArchivo, List<IDictionary<string, object>> prmDatos, ref string rutaDestino);
    }
}