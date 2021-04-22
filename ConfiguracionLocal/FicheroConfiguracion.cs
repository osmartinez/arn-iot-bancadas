using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfiguracionLocal
{
    public class FicheroConfiguracion
    {
        private static string ruta = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.ApplicationData), "config_bancada.json");


        public void GuardarConfiguracion(Configuracion config)
        {
            File.WriteAllText(ruta,JsonConvert.SerializeObject(config));
        }

        public Configuracion LeerConfiguracion()
        {
            if (!File.Exists(ruta))
            {
                GuardarConfiguracion(Configuracion.Default);
            }   
            string text = File.ReadAllText(ruta);
            Configuracion config = JsonConvert.DeserializeObject<Configuracion>(text);
            return config;
        }
    }
}
