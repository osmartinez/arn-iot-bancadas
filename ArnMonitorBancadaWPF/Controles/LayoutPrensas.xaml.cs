using ArnMonitorBancadaWPF.Util;
using ConfiguracionLocal;
using Entidades;
using Logs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        private SQLite.Add.AddPrensa localAddPrensas = new SQLite.Add.AddPrensa();
        private SQLite.Select.SelectPrensas localSelectPrensas = new SQLite.Select.SelectPrensas();
        private SQLite.Update.UpdatePrensas localUpdatePrensas = new SQLite.Update.UpdatePrensas();
        private List<LocalPrensa> prensas = new List<LocalPrensa>();
        
        public PrensaLayout PrensaSeleccionada { get; set; }

        public LayoutPrensas()
        {
            InitializeComponent();
            //this.Loaded += LayoutPrensas_Loaded;
            this.KeyUp += LayoutPrensas_KeyUp;
            Store.OnStoreIniciada += Store_OnStoreIniciada;

        }

        private void Store_OnStoreIniciada(object sender, EventArgs e)
        {
            BuscarBancada();

        }

        private void LayoutPrensas_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
            {

            }
        }

        public void MoverPrensa(Key key)
        {
            if (this.PrensaSeleccionada != null)
            {
                if (key == Key.Right)
                {
                    int col = Grid.GetColumn(this.PrensaSeleccionada) + 1;
                    Grid.SetColumn(this.PrensaSeleccionada, col);
                    this.PrensaSeleccionada.Prensa.Left = col;
                }
                else if (key == Key.Left)
                {
                    int col = Grid.GetColumn(this.PrensaSeleccionada) -1;
                    this.PrensaSeleccionada.Prensa.Left = col;
                    Grid.SetColumn(this.PrensaSeleccionada, col);
                }
                else if (key == Key.Down)
                {
                    int row = Grid.GetRow(this.PrensaSeleccionada) + 1;
                    Grid.SetRow(this.PrensaSeleccionada, row);
                    this.PrensaSeleccionada.Prensa.Top = row;

                }
                else if (key == Key.Up)
                {
                    int row = Grid.GetRow(this.PrensaSeleccionada) -1;
                    Grid.SetRow(this.PrensaSeleccionada, Grid.GetRow(this.PrensaSeleccionada) - 1);
                    this.PrensaSeleccionada.Prensa.Top = row;
                }
                localUpdatePrensas.ActualizarPrensa(PrensaSeleccionada.Prensa);
            }
            
        }


        private void BuscarBancada()
        {

            try
            {
                prensas = localSelectPrensas.BuscarTodas();
                
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
                Logs.Log.Write(ex);
            }

            try
            {
                int numFilas = prensas.Count / 2;
                int numColumnas = 2;
                for (int i = 0; i < numFilas; i++)
                {
                    this.grid.RowDefinitions.Add(new RowDefinition());
                }
                for (int i = 0; i < numColumnas; i++)
                {
                    this.grid.ColumnDefinitions.Add(new ColumnDefinition());
                }

                foreach (var prensa in prensas)
                {
                    PrensaLayout pl = new PrensaLayout(prensa, Store.Bancada.Maquinas.FirstOrDefault(x => x.ID == prensa.Id));
                    this.grid.Children.Add(pl);
                }
            }
            catch (Exception ex)
            {
                Logs.Log.Write(ex);
            }


        }
    }
}
