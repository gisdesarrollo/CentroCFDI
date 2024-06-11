using API.Catalogos;
using API.Enums;
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
            if (complementoPago.SucursalId == 49)
            {
                message.Bcc.Add(new MailAddress("Alejandro.Islas@rohlig.com"));
                message.Bcc.Add(new MailAddress("Erendira.Lopez@rohlig.com"));
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

        public EmailDto ObjectCorreo(SocioComercial sociocomercial, List<String> archivos)
        {
            var envioEmailDto = new EmailDto
            {
                CuerpoCorreo = sociocomercial.Sucursal.CuerpoCorreo,
                EncabezadoCorreo = sociocomercial.Sucursal.EncabezadoCorreo,
                EmailEmisor = sociocomercial.Sucursal.MailEmisor,
                NombreSucursal = sociocomercial.Sucursal.Nombre,
                User = sociocomercial.Sucursal.UserCorreo,
                Contrasena = sociocomercial.Sucursal.PasswordCorreo,
                Servidor = sociocomercial.Sucursal.Smtp,
                Puerto = (int)sociocomercial.Sucursal.Puerto
            };

            //replace espacios en blanco y split cadena email
            if (sociocomercial.Email != null)
            {
                envioEmailDto.EmailsReceptores = new List<string>();
                string replaceEspacios = Regex.Replace(sociocomercial.Email, @"\s", "");
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
                        NombreArchivo = String.Format("{0}_{1}{2}", sociocomercial.Rfc, DateTime.Now.ToString("ddMMyyyyHHmmssffff"), Path.GetExtension(archivo))
                    });
                }

                envioEmailDto.Archivos = archivosAdjuntosDto;
            }
            return envioEmailDto;
        }

        #region Notificaciones de Recepción de Facturas y Pagos

        public void NotificacionCambioEstadoComercial(Usuario usuario, API.Operaciones.OperacionesProveedores.DocumentoRecibido documentoRecibido, c_EstadoComercial EstadoComercial, int sucursalId)
        {
            //Obtener el correo del usuario
            string destinatario = usuario.Email;
            string asunto = null;
            string cuerpoCorreo = null;

            var sucursal = _db.Sucursales.Find(sucursalId);

            if (sucursal.MailEmisor != null)
            {
                var validaEmail = ComprobarFormatoEmail(sucursal.MailEmisor);

                if (validaEmail)
                {
                    switch (EstadoComercial)
                    {
                        case c_EstadoComercial.Aprobado:
                            break;

                        case c_EstadoComercial.Rechazado:
                            asunto = "CentroCFDi - Notificación de Rechazo de Factura";
                            cuerpoCorreo = $"Estimado {documentoRecibido.Usuario.Nombre},\n\n" +
                            "Esperamos que este mensaje le encuentre bien.Nos dirigimos a usted en relación con la factura\n\n" +
                            $"número {documentoRecibido.CfdiRecibidosSerie}-{documentoRecibido.CfdiRecibidosFolio} emitida el {documentoRecibido.FechaEntrega}.\n\n" +
                            "Lamentamos informarle que su factura ha sido rechazada en nuestro sistema de gestión CentroCFDi.\n" +
                            "Nuestro equipo ha revisado la factura y la ha rechazado por el siguiente motivo:\n\n" +
                            $"{documentoRecibido.AprobacionesDR.DetalleRechazo}.\n\n" +
                            "Entendemos que esta notificación puede generar inconvenientes y, por ello, le instamos a revisar el motivo de rechazo mencionado.\n\n" +
                            "Agradecemos su comprensión y colaboración para resolver cualquier problema pendiente.\n\n" +
                            "Atentamente,\n\n" +
                            $"{sucursal.Nombre}\n \n" +
                            "Equipo de CentroCFDi";
                            break;

                        default:
                            break;
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

        public void NotificacionPagoSocioComercial(int? usuario_id, int? sucursal_id, PagosDR pagos, int? socioComercial_id)
        {
            var usuario = _db.Usuarios.Find(usuario_id);
            var sucursal = _db.Sucursales.Find(sucursal_id);
            var socioComercial = _db.SociosComerciales.Find(socioComercial_id);

            string destinatario = usuario.Email;

            if (sucursal.MailEmisor != null)
            {
                var validaEmail = ComprobarFormatoEmail(sucursal.MailEmisor);

                if (validaEmail)
                {
                    string documentosPagados = null;

                    if (pagos.DocumentosPagados.Count > 0)
                    {
                        foreach (var documento in pagos.DocumentosPagados)
                        {
                            documentosPagados += $"- {documento.UUID}\n";
                        }
                    }
                    string asunto = "CentroCFDi - Notificación de Pago Realizado";
                    string cuerpoCorreo =
                        $"Estimado {usuario.NombreCompleto},\n\n" +
                        $"Nos complace informarte que hemos emitido un pago correspondiente a {socioComercial.RazonSocial}:\n\n" +
                        $"UUID de Factura: \n" +
                        $"Importe del pago: {pagos.Total.ToString("c")} {pagos.Moneda}\n" +
                        $"Fecha de Emisión:{pagos.FechaPago}\n\n" +
                        $"Referencia Bancaria: {pagos.ReferenciaBancaria}\n" +
                        $"Documens Pagados: \n" +
                        $"{documentosPagados}\n\n" +
                        $"Nos gustaría recordarte esta acción requiere que cargues el CFDi con Complemento de Recepción de Pagos correspondiente para completar el ciclo de facturación en CentroCFDi. Esto nos ayudará a mantener un registro preciso de todas las transacciones y garantizar una relación comercial fluida.\n\n" +
                        $"Por favor, accede a CentroCFDi en la sección de carga de Complementos de Pago. Si necesitas ayuda o tienes alguna pregunta, no dudes en ponerte en contacto con nuestro equipo de finanzas.\n\n" +
                        $"Agradecemos tu pronta atención a este asunto y esperamos seguir colaborando de manera fluuida.\n\n" +
                        $"Saludos,\n" +
                        $"{sucursal.Nombre}\n \n" +
                        "Equipo de CentroCFDi";

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

        public void NotificacionRevisionComercial(Usuario usuario, API.Operaciones.OperacionesProveedores.DocumentoRecibido documentoRecibido, int sucursalId)
        {
            //Obtener el correo del usuario
            string destinatario = usuario.Email;
            string asunto = null;
            string cuerpoCorreo = null;

            var sucursal = _db.Sucursales.Find(sucursalId);

            if (sucursal.MailEmisor != null)
            {
                var validaEmail = ComprobarFormatoEmail(sucursal.MailEmisor);

                if (validaEmail)
                {
                    asunto = "Revisión de Documento en CentroCFDi: Acción Requerida";
                    cuerpoCorreo = $"Estimado {usuario.NombreCompleto},\n\n" +
                                   "Quería informarte que uno de los Documentos Recibidos que aprobaste en el sistema CentroCFDi ha sido devuelto para revisión por parte del departamento de Cuentas por Pagar.\n\n" +
                                   "Detalle del Documento:\n\n" +
                                   $"Número de Documento: {documentoRecibido.CfdiRecibidosUUID}\n" +
                                   $"Fecha de Carga: {documentoRecibido.FechaEntrega}\n" +
                                   $"Proveedor: {documentoRecibido.SocioComercial.RazonSocial}\n\n" +
                                   "Observaciones de Cuentas por Pagar:\n\n" +
                                   $"{documentoRecibido.AprobacionesDR.Observaciones}\n\n" +
                                   "Por favor, te solicitamos que revises las observaciones proporcionadas por el departamento de Cuentas por Pagar y tomes las medidas necesarias para resolver cualquier problema o discrepancia que haya surgido.\n\n" +
                                   "Una vez que hayas realizado los ajustes pertinentes, por favor, vuelve a Aprobar el documento en el sistema CentroCFDi para su revisión adicional.\n\n" +
                                   "Si el problema no puede resolverse en este movimiento, Rechaza el Documento y podrán cargarlo de nuevo.\n\n" +
                                   "Si tienes alguna pregunta o necesitas asistencia adicional, no dudes en ponerte en contacto con Cuentas por Pagar.\n\n" +
                                   "Gracias por tu atención y pronta acción en este asunto.\n\n" +
                                   "Saludos,\n" +
                                   "Equipo de CentroCFDi";

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

        public void NotificacionNuevoUsuario(Usuario usuario, int sucursalId)
        {
            string destinatario = usuario.Email;
            string asunto;
            string cuerpoCorreo = null;
            var sucursal = _db.Sucursales.Find(sucursalId);

            if (sucursal.MailEmisor != null)
            {
                var validaEmail = ComprobarFormatoEmail(sucursal.MailEmisor);

                if (validaEmail)
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
                       "Equipo de Centro CFDI";
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
                            "Equipo de Centro CFDI";
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

        #endregion Notificaciones de Recepción de Facturas y Pagos

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