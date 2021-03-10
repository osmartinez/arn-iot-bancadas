using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logs
{
    public static class Log
    {
        private const string ruta = "errores.log";

        public static void Write(Exception ex)
        {
            File.AppendAllText(ruta, string.Format("\n\n[ERROR {0}] {1} - {2}",DateTime.Now.ToString(), ex.Message, ex.StackTrace));
        }
    }
}
