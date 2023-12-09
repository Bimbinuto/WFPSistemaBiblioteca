using iText.IO.Font.Constants;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

            try
            {
                PdfWriter pdfWriter = new PdfWriter("C:\\Users\\HP\\source\\repos\\Biblioteca\\ReportesGenerados\\ReporteNuevo.pdf");
                PdfDocument pdf = new PdfDocument(pdfWriter);
                Document documento = new Document(pdf);

                documento.SetMargins(60, 20, 55, 20);

                var parrafo = new Paragraph("Hola mundo");
                documento.Add(parrafo);
                documento.Close();



                //PdfFont fontColumnas = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                //PdfFont fontContenido = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                string[] columnas = { "ID prestamo", "fecha prestamo", "fecha devolucion", "fecha limite devolucion", "tipo prestamo", "ID transaccion" };





                ResultadoReporte = "Se ha generado el reporte";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
