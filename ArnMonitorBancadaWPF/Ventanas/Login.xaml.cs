using Entidades;
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
using System.Windows.Shapes;

namespace ArnMonitorBancadaWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window, INotifyPropertyChanged
    {
        public string CodigoOperario { get; set; } = "";

        private EF.Select.SelectOperarios selectOperarios = new EF.Select.SelectOperarios();
        private SQLite.Select.SelectOperarios localSelectOperarios = new SQLite.Select.SelectOperarios();
        private SQLite.Add.AddOperarios localAddOperarios = new SQLite.Add.AddOperarios();
        public Login()
        {
            InitializeComponent();
            this.DataContext = this; ;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Notificar(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BtNumeroClick(object sender, RoutedEventArgs e)
        {
            Button bt = sender as Button;
            string text = bt.Name.Replace("Bt", "");
            this.CodigoOperario += text;
            Notificar("CodigoOperario");
        }

        private void BtBorrar_Click(object sender, RoutedEventArgs e)
        {
            if (this.CodigoOperario.Length > 0)
            {
                this.CodigoOperario = this.CodigoOperario.Substring(0, this.CodigoOperario.Length - 1);
                Notificar("CodigoOperario");
            }
        }

        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            bool error = false;
            Operarios op = null;

            try
            {
                LocalOperario l_op = localSelectOperarios.BuscarPorCodigo(this.CodigoOperario);
                if (l_op != null)
                {
                    op = new Operarios { Nombre = l_op.Nombre, Apellidos = l_op.Apellidos, CodigoObrero = l_op.Codigo, Id = (int)l_op.Id };
                    SesionManager.Sesion.Operario = op;
                }
                else
                {
                    op = selectOperarios.BuscarPorCodigo(this.CodigoOperario);
                    if (op != null)
                    {
                        localAddOperarios.Insertar(new LocalOperario { Codigo = op.CodigoObrero, Nombre = op.Nombre, Apellidos = op.Apellidos, Id = op.Id });
                    }
                    else
                    {
                        error = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                error = true;
            }

            this.CodigoOperario = "";
            Notificar("CodigoOperario");

            if (error)
            {

            }
            else
            {

                SesionManager.Sesion.Operario = op;
                this.Close();
            }
        }
    }
}
