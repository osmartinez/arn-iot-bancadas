using Entidades;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite
{
    internal static class Fichero
    {
        private const string ruta = "mydb.json";

        public static void EscribirConfiguracion(LocalConfiguracion cfg)
        {
            string json = JsonConvert.SerializeObject(cfg, Formatting.Indented);
            File.WriteAllText(ruta, json);
        }
        public static LocalConfiguracion LeerConfiguracion()
        {
            if (!File.Exists(ruta))
            {
                LocalConfiguracion cfg = LocalConfiguracion.Default;
                string json = JsonConvert.SerializeObject(cfg,Formatting.Indented);
                File.WriteAllText(ruta, json);
                return cfg;
            }
            else
            {
                string json = File.ReadAllText(ruta);
                LocalConfiguracion cfg = JsonConvert.DeserializeObject<LocalConfiguracion>(json);

                if (cfg == null)
                {
                    cfg = LocalConfiguracion.Default;
                    EscribirConfiguracion(cfg);
                }
                
                return cfg;
            }
        }
    }
}
