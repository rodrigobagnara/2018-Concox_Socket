using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
namespace concox_v3.Concox.Manager
{
    public class Rabbit_MQ
    {
        // (RabbitMQ) ---------------------------------------------------------- //
        private IConnection Connect() {
            ConnectionFactory factory = new ConnectionFactory();
            factory.UserName = Config.RMQ_UserName;
            factory.Password = Config.RMQ_Password;
            factory.VirtualHost = Config.RMQ_VirtualHost;
            factory.HostName = Config.RMQ_HostName;
            return factory.CreateConnection();

        }

        public void Publish(String jsonTrack)
        {
            var conn = Connect();
            IModel channel = conn.CreateModel();
            byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jsonTrack);
            channel.BasicPublish("bilhetes", "concox", null, messageBodyBytes);
            channel.Close();
            conn.Close();

        }
    }
}
