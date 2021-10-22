using RabbitMQ.Client;
using System;
using System.IO;
using System.Text;

namespace API.BUS
{
    public class Mensaje
    {
        public String Comando;
        public String Parametros;


        public static Mensaje FroMensaje(String xml)
        {
            var reader = new System.Xml.Serialization.XmlSerializer(typeof(Mensaje));
            TextReader sr = new StringReader(xml);

            return (Mensaje)reader.Deserialize(sr);
        }

        public override string ToString()
        {
            var x = new System.Xml.Serialization.XmlSerializer(GetType());
            Stream stream = new MemoryStream();
            x.Serialize(stream, this);

            stream.Position = 0;
            var sr = new StreamReader(stream);
            var myStr = sr.ReadToEnd();
            return myStr;

        }

        public static void Send(String message)
        {
            try
            {
                const string cola = "envio";

                var connectionFactory = new ConnectionFactory();
                connectionFactory.HostName = ("localhost");
                // connectionFactory.UserName = "guest";
                //connectionFactory.Password = "guest";
                //   connectionFactory.VirtualHost = "/";
                using (var connection = connectionFactory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(cola, true, false, false, null);

                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish("", cola, null, body);

                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Error en bus {0}", ex.Message));
            }

        }
    }
}
