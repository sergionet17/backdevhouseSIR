using Newtonsoft.Json;
using SIR.Comun.Entidades.Archivos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;

namespace SIR.Comun.Funcionalidades
{
    public class Util
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptSHA1(string text)
        {
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(text));
            return string.Concat(hash.Select(b => b.ToString("x2")));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="para"></param>
        /// <param name="asunto"></param>
        /// <param name="cuerpo"></param>
        /// <param name="archivos"></param>
        /// <param name="plantilla"></param>
        public static void SendMail(List<string> para, string asunto, string cuerpo, List<MODArchivo> archivos, string plantilla = "")
        {
            string smtp = Configuraciones.SMTP;
            int puerto = Configuraciones.Puerto;
            string de = Configuraciones.De;
            string nombre = Configuraciones.NombreDe;

            var email = new MailMessage();
            email.From = new MailAddress(de, nombre);

            try
            {
                if (archivos != null)
                {
                    if (archivos.Count > 0)
                    {
                        for (int i = 0; i < archivos.Count; i++)
                        {
                            Attachment attach = new Attachment(archivos[i].Contenido, archivos[i].Nombre);
                            email.Attachments.Add(attach);
                        }
                    }
                }

                email.Sender = new MailAddress(de, nombre);

                foreach (string to in para)
                {
                    email.To.Add(new MailAddress(to));
                }

                string correoPlantilla = cuerpo;
                if (!string.IsNullOrEmpty(plantilla))
                {
                    try
                    {
                        var filePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location) + "\\Correos\\" + plantilla;
                        
                        using (StreamReader sr = new StreamReader(filePath))
                        {
                            correoPlantilla = sr.ReadToEnd();
                        }
                        correoPlantilla = correoPlantilla.Replace("[textoCuerpo]", cuerpo);
                    }
                    catch (Exception ex)
                    {
                        Log.WriteLog(ex, "Util.SendMail Plantilla", $"Plantilla: {plantilla}", ErrorType.Error);
                        correoPlantilla = cuerpo;
                    }

                }

                email.Subject = asunto;
                email.Body = correoPlantilla;
                email.IsBodyHtml = true;

                SmtpClient mailServer = new SmtpClient(smtp);
                mailServer.Credentials = CredentialCache.DefaultNetworkCredentials;

                mailServer.Send(email);
                email.Dispose();
            }
            catch (Exception ex)
            {
                var obj = new { para = para, asunto = asunto, cuerpo = cuerpo };
                Log.WriteLog(ex, "Util.SendMail", JsonConvert.SerializeObject(obj), ErrorType.Error);
            }
        }
    }
}
