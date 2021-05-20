using SIR.Comun.Entidades;
using SIR.Comun.Entidades.Archivos;
using System;
using System.Collections.Generic;

namespace SIR.Datos.Interfaces.Archivo
{
    public interface IArchivoDatos
    {
        List<MODTipoArchivo> ObtenerTipoArchivos();
        List<Comun.Entidades.Archivos.MODArchivo> ObtenerArchivos(MODArchivoFiltro prmFiltro);
        long CrearArchivo(Comun.Entidades.Archivos.MODArchivo prmArchivo);
        bool ActualizarArchivo(Comun.Entidades.Archivos.MODArchivo prmArchivo);
        bool EliminarArchivo(long prmIdArchivo);
        List<MODSeparadorArchivo> ObtenerSeparadorArchivos();
        long CrearLogGeneracionArchivo(MODLogGeneracionArchivo prmLogGeneracionArchivo);
        List<MODLogGeneracionArchivo> ConsultarLogGeneracionArchivos(int prmIdArchivo);
        List<MODCamposArchivo> ConsultarCamposArchivo(int prmIdArchivo);
        bool InsertarCamposArchivo(List<MODCamposArchivo> prmCamposArchivo);
        List<IDictionary<string, object>> ObtenerInformacionArchivo(string prmSql, List<MODCampos> prmCampos);
    }
}
