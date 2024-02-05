using API.Catalogos;
using API.Operaciones.OperacionesProveedores;
using Aplicacion.Context;
using Aplicacion.LogicaPrincipal.Email;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Aplicacion.LogicaPrincipal.Correos
{
    public class EnviosEmails
    {
        private readonly AplicacionContext _db = new AplicacionContext();
        //private readonly string server = "mail.gisconsultoria.com";
        //private readonly int port = 26;
        //private readonly string user = "facturas.xsa@gisconsultoria.com";
        //private readonly string pass = "Gisconsul+01";
        //private readonly string from = " facturas.xsa@gisconsultoria.com";

        public void SendEmail(EmailDto emailDto, int complementoPagoId)
        {
            var complementoPago = _db.ComplementosPago.Find(complementoPagoId);
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
            var archivoZip = String.Format(AppDomain.CurrentDomain.BaseDirectory + "//Content//FileCfdiGenerados//{0} - {1} - {2}.zip", complementoPago.FacturaEmitida.Serie, complementoPago.FacturaEmitida.Folio, DateTime.Now.ToString("yyyyMMddHHmmssfff"));
            // Crear un archivo ZIP
            using (FileStream fs = new FileStream(archivoZip, FileMode.Create))
            using (ZipArchive zip = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                foreach (var file in emailDto.Archivos)
                {
                    // Agregar archivo a ZIP
                    zip.CreateEntryFromFile(file.Path, file.NombreArchivo);
                }

            }



            var attachment = new Attachment(archivoZip)
            {
                Name = Path.GetFileName(archivoZip)
            };
            //add attachments
            message.Attachments.Add(attachment);
            //se incializa conexion al servidor smtp
            SmtpClient client = new SmtpClient(emailDto.Servidor, (int)emailDto.Puerto);
            //uso de credenciales de authenticación
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(emailDto.User, emailDto.Contrasena);
            client.EnableSsl = false;

            try
            {
                client.Send(message);
                message.Dispose();
            }

            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
            }
            //se eliminan los files 
            foreach (var file in emailDto.Archivos)
            {
                System.IO.File.Delete(file.Path);
            }
            //se elimina el zip
            System.IO.File.Delete(archivoZip);
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

        public void SendEmailNotifications(Usuario usuario, DocumentosRecibidosDR documentoRecibido, bool rechazo, int sucursalId)
        {
            string destinatario = null;
            string asunto;
            string cuerpoCorreo = null;
            if (usuario == null)
            {
                destinatario = documentoRecibido.Usuario.Email;
            }
            else { destinatario = usuario.Email; }
            var sucursal = _db.Sucursales.Find(sucursalId);
            if (sucursal.MailEmisor != null)
            {
                var validaEmail = ComprobarFormatoEmail(sucursal.MailEmisor);
                if (validaEmail)
                {
                    if (rechazo)
                    {
                        asunto = "CentroCFDi - Notificación de Rechazo de Factura";
                        cuerpoCorreo = $"Estimado {documentoRecibido.Usuario.Nombre},\n\n" +
                        "Esperamos que este mensaje le encuentre bien.Nos dirigimos a usted en relación con la factura\n\n" +
                        $"número {documentoRecibido.CfdiRecibidos_Serie}-{documentoRecibido.CfdiRecibidos_Folio} emitida el {documentoRecibido.FechaEntrega}.\n\n" +
                        "Lamentamos informarle que su factura ha sido rechazada en nuestro sistema de gestión CentroCFDi.\n" +
                        "Nuestro equipo ha revisado la factura y la ha rechazado por el siguiente motivo:\n\n" +
                        $"{documentoRecibido.MotivoRechazo}.\n\n" +
                        "Entendemos que esta notificación puede generar inconvenientes y, por ello, le instamos a revisar el motivo de rechazo mencionado.\n\n" +
                        "Agradecemos su comprensión y colaboración para resolver cualquier problema pendiente.\n\n" +
                        "Atentamente,\n\n" +
                        $"{sucursal.Nombre}\n \n" +
                        "Equipo de CentroCFDi";
                    }
                    else
                    {
                        asunto = "Bienvenido a CentroCFDi";
                        if (usuario.esProveedor)
                        {
                            cuerpoCorreo = $"¡Hola {usuario.Nombre}!\n\n" +
                           "Te damos la bienvenida a nuestra plataforma. Estamos emocionados de tenerte con nosotros.\n\n" +
                           "Detalles de tu cuenta:\n" +
                           $"- Usuario: {usuario.NombreUsuario}\n" +
                           "- Contraseña: A12345 \n\n" +
                           "Por favor, accede a tu cuenta con el siguiente enlace: http://www.centrocfdi.com\n\n" +

                           "Si tienes alguna pregunta o necesitas asistencia, no dudes en ponerte en contacto con nuestro equipo de soporte en soporte@centrocfdi.com\n\n" +
                           "Como proveedor, tu participación es fundamental para el éxito de nuestra plataforma. Esperamos que encuentres todas las herramientas necesarias para gestionar tus servicios de manera efectiva. Si tienes alguna pregunta específica sobre cómo utilizar la plataforma como proveedor, no dudes en ponerte en contacto con nuestro equipo de soporte dedicado a proveedores.\n\n" +
                           "¡Gracias por unirte a nuestra red!\n\n" +
                           "Saludos cordiales,\n" +
                           "Equipo de GIS Consultoria\n" +
                           "Centro CFDI.";
                        }
                        else
                        {
                            cuerpoCorreo = $"¡Hola {usuario.Nombre}!\n\n" +
                                "Te damos la bienvenida a nuestra plataforma. Estamos emocionados de tenerte con nosotros.\n\n" +
                                "Detalles de tu cuenta:\n" +
                                $"- Usuario: {usuario.NombreUsuario}\n" +
                                "- Contraseña: A12345 \n\n" +
                                "Por favor, accede a tu cuenta con el siguiente enlace: http://www.centrocfdi.com\n\n" +

                                "Si tienes alguna pregunta o necesitas asistencia, no dudes en ponerte en contacto con nuestro equipo de soporte en soporte@centrocfdi.com\n\n" +
                                "Como miembro de nuestro equipo, tendrás acceso a recursos y herramientas que facilitarán tu trabajo diario. Estamos comprometidos a proporcionar un entorno en el que puedas crecer y tener éxito. Si tienes alguna pregunta sobre cómo aprovechar al máximo nuestra plataforma como trabajador, no dudes en ponerte en contacto con nuestro equipo de recursos humanos.\n\n" +
                                "¡Bienvenido a nuestro equipo!\n\n" +
                                "Saludos cordiales,\n" +
                                "Equipo de GIS Consultoria\n" +
                                "Centro CFDI.";
                        }
                    }

                    using (SmtpClient smtpClient = new SmtpClient("mail.gisconsultoria.com", 26))
                    {
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = new NetworkCredential("facturas.xsa@gisconsultoria.com", "Gisconsul+01");
                        smtpClient.EnableSsl = false;

                        MailMessage mail = new MailMessage();
                        mail.From = new MailAddress(sucursal.MailEmisor);
                        mail.To.Add(destinatario);
                        mail.Subject = asunto;
                        mail.Body = cuerpoCorreo;

                        smtpClient.Send(mail);
                    }
                }
            }
        }

        public bool ComprobarFormatoEmail(string email)
        {
            String sFormato;
            sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(email, sFormato))
            {
                if (Regex.Replace(email, sFormato, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

    }
}