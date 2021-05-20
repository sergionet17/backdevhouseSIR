using Microsoft.Extensions.Configuration;
using System;

namespace SIR.Comun.Funcionalidades
{
    public class Configuraciones
    {
        public static string ObtenerConfiguracion(string nombreSeccion, string NombreSubseccion)
        {
            string result = "";
            var seccion = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection(nombreSeccion);
            if (!string.IsNullOrEmpty(NombreSubseccion))
            {
                seccion = seccion.GetSection(NombreSubseccion);
            }

            result = seccion.Value;
            return result;
        }

        public static string SMTP
        {
            get
            {
                return ObtenerConfiguracion("MailConfiguration", "SMTP");
            }
        }
        public static int Puerto
        {
            get
            {
                return Convert.ToInt32(ObtenerConfiguracion("MailConfiguration", "Puerto"));
            }
        }
        public static string De
        {
            get
            {
                return ObtenerConfiguracion("MailConfiguration", "De");
            }
        }
        public static string NombreDe
        {
            get
            {
                return ObtenerConfiguracion("MailConfiguration", "NombreDe");
            }
        }
        
        public static object URLSitio
        {
            get
            {
                return ObtenerConfiguracion("URLSitio", "URLSitio");
            }
        }
        public static object RutaLog
        {
            get
            {
                return ObtenerConfiguracion("FileRoutes", "Url:RutaBaseLogErrores");
            }
        }
    }
}
