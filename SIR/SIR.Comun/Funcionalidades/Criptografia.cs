using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SIR.Comun.Funcionalidades
{
    public static class Criptografia
    {
        public static string Incriptar(string texto)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Configuraciones.ObtenerConfiguracion("Keys", "Llave"));
                aes.IV = iv;
                aes.Padding = PaddingMode.Zeros;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(texto);
                        }
                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string Desincriptar(string texto)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(texto);

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(Configuraciones.ObtenerConfiguracion("Keys", "Llave"));
                aes.IV = iv;
                aes.Padding = PaddingMode.Zeros;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            var dec = streamReader.ReadToEnd();
                            return dec.Replace("\0","");
                        }
                    }
                }
            }
        }
    }
}
