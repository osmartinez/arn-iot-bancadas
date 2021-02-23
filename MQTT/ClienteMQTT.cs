using Entidades;
using SesionManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;

namespace MQTT
{
    public static class ClienteMQTT
    {
        public static List<Topic> Topics { get; private set; } = new List<Topic>();
        private static MqttClient client;
        private static string topicConsumo = "";
        public static bool Conectado { get; set; }
        static ClienteMQTT()
        {

        }

        public static void Iniciar()
        {
            client = new MqttClient("192.168.0.104");
            string clientId = string.Format("{0}_{1}", "arn-monitor-bancada", Sesion.Operario!=null?Sesion.Operario.CodigoObrero:Guid.NewGuid().ToString());
            client.Connect(clientId, "", "", false, 10);
            client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
            Conectado = true;
            //Topics.Add(new Topic(1, "/moldeado/plc/+/asociarTarea", new Regex(@"^\/moldeado\/plc\/\s?[0-9]+\/asociarTarea$"), 3, 1, qos: 1));
            //Topics.Add(new Topic(2, "/moldeado/plc/+/normal", new Regex(@"^\/moldeado\/plc\/\s?[0-9]+\/normal$"), 3, 1, qos: 2));
            //Topics.Add(new Topic(3, "/pegado/plc/+/asociarTarea", new Regex(@"^\/pegado\/plc\/\s?[0-9]+\/asociarTarea$"), 3, 1, qos: 1));
            //Topics.Add(new Topic(4, "/pegado/plc/+/normal", new Regex(@"^\/pegado\/plc\/\s?[0-9]+\/normal$"), 3, 1, qos: 2));
            //Topics.Add(new Topic(5, "/moldes/normal/sinProducir", new Regex(@"^\/moldes\/normal\/sinProducir"), -1, -1, qos: 0));
            //Topics.Add(new Topic(6, "/moldes/calentar/sinProducir", new Regex(@"^\/moldes\/calentar\/sinProducir"), -1, -1, qos: 0));
            //Topics.Add(new Topic(7, "/bancada/+/maquina/+/programacion", new Regex(@"^\/bancada\/\s?[0-9]+\/maquina\/\s?[0-9]+\/programacion"), -1, -1, qos: 1));
            //Topics.Add(new Topic(8, "/operario/+/imprimir/hojaProduccion", new Regex(@"^\/operario\/\s?[0-9]+\/imprimir\/hojaProduccion"), 2, -1, qos: 1));
            Topics.Add(new Topic(1, "/moldeado/plc/+/asociarTarea", new Regex(@"^\/moldeado\/plc\/\s?[0-9]+\/asociarTarea$"), 3, 1, qos: 1));
            Topics.Add(new Topic(2, "/moldeado/plc/+/normal", new Regex(@"^\/moldeado\/plc\/\s?[0-9]+\/normal$"), 3, 1, qos: 2));

            Suscribir();
        }

        public static void Publicar(string topic, string msg)
        {
            if (Conectado)
            {
                client.Publish(topic, System.Text.Encoding.UTF8.GetBytes(msg));
            }
        }

        public static void Suscribir()
        {

            foreach (Topic topic in Topics)
            {
                client.Subscribe(new string[] { topic.Nombre }, new byte[] { topic.QOS });
            }
        }


        private static void Desuscribir()
        {
            foreach (Topic topic in Topics)
            {
                client.Unsubscribe(new string[] { topic.Nombre });
            }
        }

        public static void Cerrar()
        {
            try
            {
                Desuscribir();
                client.Disconnect();
            }
            catch (Exception ex)
            {

            }

        }

        private static void Client_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            foreach (Topic topic in Topics)
            {
                if (topic.Expresion.IsMatch(e.Topic))
                {
                    for (int i = 0; i < topic.Callbacks.Count; i++)
                    {
                        Action<string, string, Topic> callback = topic.Callbacks[i];
                        callback(System.Text.Encoding.UTF8.GetString(e.Message), e.Topic, topic);
                    }
                }
            }
        }

    }
}
