using Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ArnMonitorBancadaWPF.Controles
{
    public class PrensaLayout1:Border
    {
        private bool moviendo = false;
        private LocalPrensa prensa;
        public event EventHandler OnPrensaLayoutMovida;
        public PrensaLayout1(LocalPrensa prensa)
        {
            this.prensa = prensa;
            Grid.SetRow(this, (int)prensa.Top);
            Grid.SetColumn(this, (int)prensa.Left);

            this.BorderBrush = Brushes.Black;
            this.BorderThickness = new System.Windows.Thickness(1);
            this.Background = Brushes.White;

            Grid grd = new Grid();
            grd.RowDefinitions.Add(new RowDefinition());
            grd.RowDefinitions.Add(new RowDefinition());
            grd.RowDefinitions.Add(new RowDefinition());
            TextBlock tbNombre = new TextBlock { Text = prensa.Nombre };
            Grid.SetRow(tbNombre, 0);
            grd.Children.Add(tbNombre);
            this.Child = grd;

            /*this.MouseLeftButtonDown += PrensaLayout_MouseLeftButtonDown;
            this.MouseLeftButtonUp += PrensaLayout_MouseLeftButtonUp;
            this.MouseMove += PrensaLayout_MouseMove;*/
        }

        public void PrensaLayoutMovida()
        {
            if (this.OnPrensaLayoutMovida != null)
            {
                this.OnPrensaLayoutMovida(this.prensa, null);
            }
        }
        private void PrensaLayout_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (moviendo)
            {
                double left = Canvas.GetLeft(this);
                double top = Canvas.GetTop(this);

                Canvas.SetLeft(this, left + e.GetPosition((IInputElement)this).X - this.Width/2);
                Canvas.SetTop(this, top + e.GetPosition((IInputElement)this).Y - this.Height/2);
            }
        }

        private void PrensaLayout_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            moviendo = false;
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            this.prensa.Left = left;
            this.prensa.Top = top;
            this.PrensaLayoutMovida();
        }

        private void PrensaLayout_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            moviendo = true;
        }
    }
}
