using API.Catalogos;
using Aplicacion.LogicaPrincipal.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Aplicacion.LogicaPrincipal.Correos
{
    public class EnviosEmails
    {
        //private readonly string server = "mail.gisconsultoria.com";
        //private readonly int port = 26;
        //private readonly string user = "facturas.xsa@gisconsultoria.com";
        //private readonly string pass = "Gisconsul+01";
        //private readonly string from = " facturas.xsa@gisconsultoria.com";

        public void SendEmail(EmailDto emailDto)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(emailDto.EmailEmisor);
            //emailDto.EmailsReceptores = new List<string> { "alexander.garcia@gisconsultoria.com", "eduardo.ayala@gisconsultoria.com" };
            //recorrer email receptores agregados
            foreach (var emailReceptor in emailDto.EmailsReceptores)
            {
                message.Bcc.Add(new MailAddress(emailReceptor));
            }
            message.Subject = emailDto.EncabezadoCorreo;
            message.Body = emailDto.CuerpoCorreo;
            //archivos adjuntos
            foreach (var file in emailDto.Archivos)
            {
                var attachment = new Attachment(file.Path)
                {
                    Name = file.NombreArchivo
                };
                message.Attachments.Add(attachment);
            }
            //se incializa conexion al servidor smtp
            SmtpClient client = new SmtpClient(emailDto.Servidor, (int)emailDto.Puerto);
            //uso de credenciales de authenticación
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(emailDto.User, emailDto.Contrasena);
            client.EnableSsl = false;

            try
            {
                client.Send(message);
                message.Dispose();
            }

            catch (Exception)
            {
                //throw new Exception(ex.Message);

            }
            //se eliminan los files 
            foreach (var file in emailDto.Archivos)
            {
                System.IO.File.Delete(file.Path);
            }
        }
        
        public EmailDto ObjectCorreo(Cliente cliente, List<String> archivos)
        {
            var envioEmailDto = new EmailDto
            {
                CuerpoCorreo = cliente.Sucursal.CuerpoCorreo,
                EncabezadoCorreo = cliente.Sucursal.EncabezadoCorreo,
                EmailEmisor = cliente.Sucursal.MailEmisor,
                NombreSucursal = cliente.Sucursal.Nombre,
                User = cliente.Sucursal.UserCorreo,
                Contrasena = cliente.Sucursal.PasswordCorreo,
                Servidor = cliente.Sucursal.Smtp,
                Puerto = (int)cliente.Sucursal.Puerto
            };

            //replace espacios en blanco y split cadena email
            if (cliente.Email != null)
            {
                envioEmailDto.EmailsReceptores = new List<string>();
                string replaceEspacios = Regex.Replace(cliente.Email, @"\s", "");
                string[] correos = replaceEspacios.Split(',');

                foreach (var correoArray in correos)
                {
                    envioEmailDto.EmailsReceptores.Add(correoArray);
                }
            }
            if (archivos.Count > 0)
            {
                var archivosAdjuntosDto = new List<ArchivosAdjuntosDto>();
                foreach (var archivo in archivos)
                {
                    archivosAdjuntosDto.Add(new ArchivosAdjuntosDto
                    {
                        Path = archivo,
                        NombreArchivo = String.Format("{0}_{1}{2}", cliente.Rfc, DateTime.Now.ToString("ddMMyyyyHHmmssffff"), Path.GetExtension(archivo))
                    });
                }

                envioEmailDto.Archivos = archivosAdjuntosDto;
            }
            return envioEmailDto;
           
        }

    }
}
