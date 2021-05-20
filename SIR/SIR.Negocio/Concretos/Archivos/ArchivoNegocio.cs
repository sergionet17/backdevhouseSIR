using SIR.Comun.Entidades.Archivos;
using SIR.Comun.Enumeradores.FlujoDeTrabajo;
using SIR.Comun.Funcionalidades;
using SIR.Datos.Fabrica;
using SIR.Datos.Interfaces.Archivo;
using SIR.Negocio.Fabrica;
using SIR.Negocio.Interfaces.Archivos;
using SIR.Negocio.Interfaces.Reporte;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIR.Negocio.Concretos.Archivos
{
    public class ArchivoNegocio : IArchivoNegocio
    {
        public List<MODTipoArchivo> ObtenerTipoArchivos()
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.ObtenerTipoArchivos();
        }

        public List<MODArchivo> ObtenerArchivos(MODArchivoFiltro prmFiltro)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            var archivos = archivoDatos.ObtenerArchivos(prmFiltro);
            if (archivos != null)
            {
                archivos.ForEach(x => {
                    x.Campos = this.ConsultarCamposArchivos(x.IdArchivo);
                });
            }
            return archivos;
        }

        public long CrearArchivo(MODArchivo prmArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            long idArchivo = archivoDatos.CrearArchivo(prmArchivo);

            if (idArchivo > 0)
            {
                prmArchivo.Campos.ForEach(x => 
                {
                    x.IdArchivo = (int)idArchivo;
                });

                this.InsertarCamposArchivo(prmArchivo.Campos);
            }
                

            return idArchivo;
        }

        public long CrearLogGeneracionArchivo(MODLogGeneracionArchivo prmLogGeneracionArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.CrearLogGeneracionArchivo(prmLogGeneracionArchivo);
        }

        public bool ActualizarArchivo(MODArchivo prmArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            bool actualizar = archivoDatos.ActualizarArchivo(prmArchivo);

            if (actualizar)
                this.InsertarCamposArchivo(prmArchivo.Campos);

            return actualizar;
        }

        public bool EliminarArchivo(long prmIdArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.EliminarArchivo(prmIdArchivo);
        }

        public List<MODSeparadorArchivo> ObtenerSeparadorArchivos()
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.ObtenerSeparadorArchivos();
        }

        public List<MODLogGeneracionArchivo> ConsultarLogGeneracionArchivos(int prmIdArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            var archivos = archivoDatos.ConsultarLogGeneracionArchivos(prmIdArchivo);

            if (archivos != null) 
            {
                archivos.ForEach(x => {

                    if (File.Exists(x.RutaDestino))
                    {
                        x.Contenido = File.ReadAllBytes(x.RutaDestino);
                        x.NombreArchivo = Path.GetFileName(x.RutaDestino);
                    }
                });
            }
           
            return archivos; 
        }

        public List<MODCamposArchivo> ConsultarCamposArchivos(int prmIdArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.ConsultarCamposArchivo(prmIdArchivo);
        }

        public bool InsertarCamposArchivo(List<MODCamposArchivo> prmCamposArchivo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;
            return archivoDatos.InsertarCamposArchivo(prmCamposArchivo);
        }

        public bool GenerarArchivo(MODArchivo prmArchivo, ref List<string> mensajesError) 
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            bool resultado = false;

            if (prmArchivo != null)
            {
                var campos = prmArchivo.Campos;
                string sql = this.ConstruirConsultaArchivo(campos, prmArchivo.IdArchivo, prmArchivo.IdReporte, prmArchivo.Periodo, prmArchivo.Periodicidad, (int)prmArchivo.IdFlujo, prmArchivo.DatoPeriodo, prmArchivo.IdElementoFlujo);

                var datos = archivoDatos.ObtenerInformacionArchivo(sql, campos.Select(x => new Comun.Entidades.MODCampos() { Id = x.IdCampo, Nombre = x.Nombre }).ToList());
                string rutaDestino = String.Empty;

                if (this.CrearArchivo(prmArchivo, datos, ref rutaDestino))
                {
                    this.CrearLogGeneracionArchivo(new MODLogGeneracionArchivo()
                    {
                        IdArchivo = prmArchivo.IdArchivo,
                        FechaGeneracion = DateTime.Now,
                        RutaDestino = rutaDestino,
                        IdFlujo = prmArchivo.IdFlujo
                    });

                    resultado = true;
                }
                else
                {
                    resultado = false;
                    mensajesError.Add($"Ha ocurrido un error al intentar crear el archivo");
                }
            }
            else
            {
                mensajesError.Add($"El archivo con id {prmArchivo.IdArchivo} no se encuentra en nuestros registros.");
                resultado = false;
            }

            return resultado;
        }

        public bool GenerarArchivo(int prmIdArchivo, ref List<string> mensajesError, DateTime prmPeriodo)
        {
            IArchivoDatos archivoDatos = FabricaDatos.CrearArchivoDatos;

            bool resultado = false;

            var archivo = this.ObtenerArchivos(new MODArchivoFiltro() { IdArchivo = prmIdArchivo });

            if(archivo != null || archivo.Count == 0)
            {
                archivo.First().Periodo = prmPeriodo;
                resultado = this.GenerarArchivo(archivo.First(), ref mensajesError);
            }
            else
            {
                mensajesError.Add($"El archivo con id {prmIdArchivo} no se encuentra en nuestros registros.");
                resultado = false;
            }

            return resultado;
        }

        private string ConstruirConsultaArchivo(List<MODCamposArchivo> prmCampos, int prmIdArchivo, int prmIdReporte, DateTime prmPeriodo, EnumPeriodicidad prmPeriodicidad, int prmIdFlujo, int prmDatoPeriodo, int? prmIdElemento = 0) 
        {
            IReporteNegocio reporteNegocio = FabricaNegocio.CrearReporteNegocio;

            string sql = $"SELECT {prmIdArchivo} AS IdArchivo";

            prmCampos.OrderBy(x => x.Orden).ToList().ForEach(x => {
                sql += $", {x.Nombre} ";
            });

            var reporte = reporteNegocio.Obtener(new Comun.Entidades.Reportes.MODReporteFiltro() { Id = prmIdReporte });

            string nombreTabla = String.Empty;

            if (prmIdElemento != null && prmIdElemento != 0)
            {
                var elemento = reporteNegocio.Obtener(new Comun.Entidades.Reportes.MODReporteFiltro() { Id = (int)prmIdElemento });
                nombreTabla = $"TB_{elemento.First().Nombre.ToUpper().Replace(" ", "_")}_{reporte.First().NombreEmpresa.ToUpper().Replace(" ", "_")}_{reporte.First().Nombre.ToUpper().Replace(" ", "_")}";
            }
            else
                nombreTabla = $"TB_{reporte.First().Nombre.ToUpper().Replace(" ", "_")}_{reporte.First().NombreEmpresa.ToUpper().Replace(" ", "_")}_{reporte.First().Nombre.ToUpper().Replace(" ", "_")}";

            sql += $" FROM {nombreTabla}";

            string periodo = ObtenerPeriodoPorPeriodicidad(prmPeriodo, prmPeriodicidad, prmDatoPeriodo);

            sql += $" WHERE VERSION_SIR = (SELECT MAX(VERSION_SIR) FROM {nombreTabla} WHERE PERIODO_SIR = '{periodo}' AND IDFLUJO_SIR = {prmIdFlujo}) AND PERIODO_SIR = '{periodo}' AND IDFLUJO_SIR = {prmIdFlujo}";

            return sql;
        }

        public bool CrearArchivo(MODArchivo prmArchivo, List<IDictionary<string, object>> prmDatos, ref string rutaDestino) 
        {
            var rutaBase = Configuraciones.ObtenerConfiguracion("FileRoutes", "Url:RutaBaseArchivosSalida");
            rutaBase += $"{DateTime.Now.Year}\\\\{DateTime.Now.Month}\\\\{DateTime.Now.Day}";

            if (!Directory.Exists(rutaBase))
                Directory.CreateDirectory(rutaBase);

            var rutaFinal = rutaBase + $"\\\\{prmArchivo.Nombre}_{prmArchivo.Periodo.ToString("yyyyMM")}_{DateTime.Now.ToString("ddMMyyyyHHmmss")}.{prmArchivo.Extension}";

            using (FileStream fs = File.Create(rutaFinal))
            {
                using (var sr = new StreamWriter(fs))
                {
                    string firstLine = String.Join(prmArchivo.ValorSeparador, prmArchivo.Campos.Select(x => x.Nombre.ToUpper()).ToArray());
                    sr.WriteLine(firstLine);

                    foreach (var item in prmDatos)
                    {
                        string line = String.Empty;

                        foreach (var campo in prmArchivo.Campos)
                        {
                            line += item.Where(x => x.Key.ToUpper() == campo.Nombre.ToUpper()).Select(x => x.Value.ToString().Replace(",", ".")).FirstOrDefault() + ",";
                        }

                        line = line.Remove(line.Length - 1);

                        sr.WriteLine(line);
                    }
                }
            }

            rutaDestino = rutaFinal;

            return true;
        }

        public string ObtenerPeriodoPorPeriodicidad(DateTime periodo, EnumPeriodicidad periodicidad, int datoPeriodo) 
        {
            string resultado = string.Empty;

            DateTime fecha = periodo;

            if (periodicidad == EnumPeriodicidad.trimestral)
            {
                switch (datoPeriodo)
                {
                    case 1:
                        fecha = new DateTime(periodo.Year, 1, 1);
                        break;
                    case 2:
                        fecha = new DateTime(periodo.Year, 4, 1);
                        break;
                    case 3:
                        fecha = new DateTime(periodo.Year, 7, 1);
                        break;
                    case 4:
                        fecha = new DateTime(periodo.Year, 10, 1);
                        break;
                    default:
                        break;
                }
            }

            switch (periodicidad)
            {
                case EnumPeriodicidad.diario:
                    resultado = fecha.ToString("yyyyMMdd");
                    break;
                case EnumPeriodicidad.mensual:
                    resultado = fecha.ToString("yyyyMM");
                    break;
                case EnumPeriodicidad.trimestral:
                    int trimester = (fecha.Month - 1) / 3 + 1;
                    resultado = fecha.ToString("yyyy") + trimester.ToString();
                    break;
                case EnumPeriodicidad.semestral:
                    int semestre = (fecha.Month - 1) / 3 + 1;
                    resultado = semestre > 2 ? "2" : "1";
                    break;
                case EnumPeriodicidad.anual:
                    resultado = fecha.ToString("yyyy");
                    break;
                default:
                    break;
            }

            return resultado;
        }
    }
}
