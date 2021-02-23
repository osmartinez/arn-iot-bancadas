using ArnMonitorBancadaWPF.Util;
using ConfiguracionLocal;
using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArnMonitorBancadaWPF.Controles
{
    /// <summary>
    /// Lógica de interacción para LayoutPrensas.xaml
    /// </summary>
    public partial class LayoutPrensas : UserControl
    {
        private EF.Select.SelectBancada selectBancadas = new EF.Select.SelectBancada();
        private SQLite.Add.AddPrensa localAddPrensas = new SQLite.Add.AddPrensa();
        private SQLite.Select.SelectPrensas localSelectPrensas = new SQLite.Select.SelectPrensas();
        private SQLite.Update.UpdatePrensas localUpdatePrensas = new SQLite.Update.UpdatePrensas();
        private FicheroConfiguracion ficheroConfig = new FicheroConfiguracion();

        public LayoutPrensas()
        {
            InitializeComponent();
            List<LocalPrensa> prensas = new List<LocalPrensa>();
            try
            {
                prensas = localSelectPrensas.BuscarTodas();
                Configuracion cfg = ficheroConfig.LeerConfiguracion();
                Store.Bancada = selectBancadas.BuscarPorId(cfg.Bancada.Id);
                foreach (var maquina in Store.Bancada.Maquinas)
                {
                    var prensaLocal = prensas.FirstOrDefault(x => x.Id == maquina.ID);
                    if (prensaLocal == null)
                    {
                        prensaLocal = new LocalPrensa
                        {
                            Id = maquina.ID,
                            Nombre = maquina.Nombre,
                            IpAutomata = maquina.IpAutomata,
                            Posicion = maquina.Posicion,
                            Left = 0,
                            Top = 0,
                        };
                        localAddPrensas.Insertar(prensaLocal);
                        prensas.Add(prensaLocal);
                    }
                }
                prensas.RemoveAll(x => !Store.Bancada.Maquinas.Select(y => y.ID).Contains(x.Id));
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "Excepción", MessageBoxButton.OK, MessageBoxImage.Error) ;
            }

            int numFilas = prensas.Count / 2;
            int numColumnas = 2;
            for(int i = 0;i<numFilas; i++)
            {
                this.grid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < numColumnas; i++)
            {
                this.grid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            foreach (var prensa in prensas)
            {
                PrensaLayout pl = new PrensaLayout(prensa, Store.Bancada.Maquinas.FirstOrDefault(x=>x.ID == prensa.Id));
                this.grid.Children.Add(pl);
            }
        }


    }
}
