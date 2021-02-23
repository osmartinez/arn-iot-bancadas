using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Entidades
{
    public class Topic
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public Regex Expresion { get; set; }
        public int IndiceIdTopic { get; set; }
        public int IndiceTipoBancada { get; set; }
        public List<Action<string, string, Topic>> Callbacks { get; set; } = new List<Action<string, string, Topic>>();
        public byte QOS { get; set; }
        public Topic(int id, string nombre, Regex expresion, int indiceIdTopic, int indiceTipoBancada)
        {
            Id = id;
            Nombre = nombre;
            Expresion = expresion;
            IndiceIdTopic = indiceIdTopic;
            IndiceTipoBancada = indiceTipoBancada;
            QOS = (byte)2;
        }
        public Topic(int id, string nombre, Regex expresion, int indiceIdTopic, int indiceTipoBancada, byte qos)
        {
            Id = id;
            Nombre = nombre;
            Expresion = expresion;
            IndiceIdTopic = indiceIdTopic;
            IndiceTipoBancada = indiceTipoBancada;
            this.QOS = qos;
        }

        public Topic(int id, string nombre, Regex expresion, int indiceIdTopic, int indiceTipoBancada, byte qos, Action<string, string, Topic> callback)
        {
            Id = id;
            Nombre = nombre;
            Expresion = expresion;
            IndiceIdTopic = indiceIdTopic;
            IndiceTipoBancada = indiceTipoBancada;
            this.QOS = qos;
            this.Callbacks.Add(callback);
        }
    }
}
