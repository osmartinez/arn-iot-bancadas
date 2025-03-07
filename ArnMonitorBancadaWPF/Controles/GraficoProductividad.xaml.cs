﻿using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace ArnMonitorBancadaWPF.Controles
{
    /// <summary>
    /// Lógica de interacción para GraficoProductividad.xaml
    /// </summary>
    public partial class GraficoProductividad : UserControl, INotifyPropertyChanged
    {
        private double _lastLecture;
        private double _trend;

        public event PropertyChangedEventHandler PropertyChanged;

        public SeriesCollection LastHourSeries { get; set; }
        public double LastLecture
        {
            get { return _lastLecture; }
            set
            {
                _lastLecture = value;
                Notificar("LastLecture");
            }
        }
        public GraficoProductividad()
        {
            InitializeComponent();
            this.DataContext = this;
            LastHourSeries = new SeriesCollection
            {
                new LineSeries
                {
                    AreaLimit = -10,
                    Values = new ChartValues<ObservableValue>
                    {
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8),
                        new ObservableValue(3),
                        new ObservableValue(5),
                        new ObservableValue(6),
                        new ObservableValue(7),
                        new ObservableValue(3),
                        new ObservableValue(4),
                        new ObservableValue(2),
                        new ObservableValue(5),
                        new ObservableValue(8)
                    }
                }
            };
            _trend = 8;


            Task.Factory.StartNew(() =>
            {
                var r = new Random();

                Action action = delegate
                {
                    LastHourSeries[0].Values.Add(new ObservableValue(_trend));
                    LastHourSeries[0].Values.RemoveAt(0);
                    SetLecture();
                };

                while (true)
                {
                    Thread.Sleep(500);
                    _trend += (r.NextDouble() > 0.3 ? 1 : -1) * r.Next(0, 5);
                    if (Application.Current != null)
                        Application.Current.Dispatcher.Invoke(DispatcherPriority.Normal, action);
                }
            });
        }
        private void SetLecture()
        {
            var target = ((ChartValues<ObservableValue>)LastHourSeries[0].Values).Last().Value;

            var step = (target - _lastLecture) / 4;
        }
        protected virtual void Notificar(string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
