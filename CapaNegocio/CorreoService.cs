using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapaNegocio
{
    public class CorreoService
    {
        private readonly string _remitente = "melissabolivarp@gmail.com";
        private readonly string _password = "gdsg pima naug icyd";

        public async Task<bool> EnviarCorreoAsync(string destinatario, string asunto, string cuerpoMensaje)
        {
            try
            {
                var mensaje = new System.Net.Mail.MailMessage();
                mensaje.To.Add(destinatario);
                mensaje.Subject = asunto;
                mensaje.Body = cuerpoMensaje;
                mensaje.IsBodyHtml = false;
                mensaje.From = new System.Net.Mail.MailAddress(_remitente);

                using (var smtp = new System.Net.Mail.SmtpClient("smtp.gmail.com", 587))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(_remitente, _password);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mensaje);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
