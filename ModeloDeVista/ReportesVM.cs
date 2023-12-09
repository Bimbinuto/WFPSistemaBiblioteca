using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Biblioteca.ModeloDeVista
{
    public class ReportesVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _resultadoReporte;
        public string ResultadoReporte
        {
            get => _resultadoReporte;
            set
            {
                _resultadoReporte = value;
                OnPropertyChanged(nameof(ResultadoReporte));
            }
        }

        public void GenerarReporte()
        {
            ResultadoReporte = "Se ha generado el reporte";
        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
