using System;
using System.IO;

namespace SIR.Comun.Funcionalidades
{
    public class Log
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="source"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        public static void WriteLog(Exception ex, string source, string data, ErrorType type)
        {
            var basepath = Convert.ToString(Configuraciones.RutaLog);

            string anioPath = Path.Combine(basepath, DateTime.Now.ToString("yyyy"));
            if (!Directory.Exists(anioPath))
            {
                Directory.CreateDirectory(anioPath);
            }

            string mesPath = Path.Combine(anioPath, DateTime.Now.ToString("MMM"));
            if (!Directory.Exists(mesPath))
            {
                Directory.CreateDirectory(mesPath);
            }

            var finalpath = Path.Combine(mesPath, String.Concat(DateTime.Now.ToString("dd"), "- Log - SIR.txt"));

            using (StreamWriter file = new StreamWriter(finalpath, true))
            {
                file.WriteLine("-----------------------------------------------------------");
                if (ex == null)
                {
                    file.WriteLine($"Tipo: {type}");
                    file.WriteLine($"Mensaje: {data}");
                    file.WriteLine($"Source: {source}");
                }
                else
                {

                    file.WriteLine($"Tipo: {type}");
                    file.WriteLine($"Hora: {DateTime.Now}");
                    file.WriteLine($"Source: {source}");
                    file.WriteLine($"Data: {data}");
                    file.WriteLine($"Error: {ex.Message}");
                    file.WriteLine($"StackTrace: {ex.StackTrace}");
                    file.WriteLine($"Inner exceptions: ");
                    var inner = ex.InnerException;

                    while (inner != null)
                    {
                        file.WriteLine($"-- Message: {inner.Message}");
                        file.WriteLine($"-- StackTrace: {inner.StackTrace}");

                        inner = inner.InnerException;
                    }
                }

                file.WriteLine("-----------------------------------------------------------");
            }
        }
    }

    public enum ErrorType
    {
        Error,
        Informacion
    }
}
