using Biblioteca.BDConexion;
using iText.IO.Font.Constants;
using iText.IO.Image;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Biblioteca.ModeloDeVista
{
    public class ReportesVM : INotifyPropertyChanged
    {
        private Conexion _conexion = new Conexion();
        public event PropertyChangedEventHandler PropertyChanged;

        private string _direccion;
        public string Direccion
        {
            get => _direccion;
            set
            {
                _direccion = value;
                OnPropertyChanged(nameof(Direccion));
            }
        }

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

        public async void GenerarReporte()
        {
            try
            {
                PdfWriter pdfWriter = new PdfWriter("C:/Users/HP/source/repos/Biblioteca/ReportesGenerados/ReporteNuevo.pdf");
                PdfDocument pdf = new PdfDocument(pdfWriter);
                Document documento = new Document(pdf);

                documento.SetMargins(60, 20, 55, 20);

                PdfFont fontColumnas = PdfFontFactory.CreateFont(StandardFonts.TIMES_BOLD);
                PdfFont fontContenido = PdfFontFactory.CreateFont(StandardFonts.TIMES_ROMAN);

                string[] columnas = { "ID prestamo", "fecha prestamo", "fecha devolucion", "fecha limite devolucion", "tipo prestamo", "ID transaccion" };

                float[] tamanios = { 1, 2, 2, 2, 2, 1 };
                var tabla = new iText.Layout.Element.Table(UnitValue.CreatePercentArray(tamanios));
                tabla.SetWidth(UnitValue.CreatePercentValue(100));

                foreach (string columna in columnas)
                {
                    tabla.AddHeaderCell(new Cell().Add(new iText.Layout.Element.Paragraph(columna).SetFont(fontColumnas).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                }

                string sql = "SELECT id_prestamo, fecha_prestamo, fecha_devolucion, fecha_max_devolucion, tipo_prestamo, id_transaccion FROM prestamo;";

                using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
                {
                    await cnx.OpenAsync();

                    using (MySqlCommand cmd = new MySqlCommand(sql, cnx))
                    {
                        MySqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["id_prestamo"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["fecha_prestamo"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["fecha_devolucion"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["fecha_max_devolucion"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["tipo_prestamo"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                            tabla.AddCell(new Cell().Add(new iText.Layout.Element.Paragraph(reader["id_transaccion"].ToString()).SetFont(fontContenido).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)));
                        }
                        await cnx.CloseAsync();
                    }
                }

                documento.Add(tabla);
                documento.Close();

                var logo = new iText.Layout.Element.Image(ImageDataFactory.Create("C:/Users/HP/source/repos/Biblioteca/Recursos/LOGO_SIS.png")).SetWidth(50);
                var plogo = new iText.Layout.Element.Paragraph("").Add(logo);

                var titulo = new iText.Layout.Element.Paragraph("BIBLIOTECA CARRERA DE ING. DE SISTEMAS E ING. INFORMATICA").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

                titulo.SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);
                titulo.SetFontSize(12);

                var dfecha = DateTime.Now.ToString("dd-MM-yyyy");
                var dhora = DateTime.Now.ToString("hh:mm:ss");
                var fecha = new iText.Layout.Element.Paragraph($"Fecha: {dfecha}\nHora: {dhora}");
                fecha.SetFontSize(12);

                PdfDocument pdfDoc = new PdfDocument(new PdfReader("C:/Users/HP/source/repos/Biblioteca/ReportesGenerados/ReporteNuevo.pdf"), new PdfWriter("C:/Users/HP/source/repos/Biblioteca/ReportesGenerados/ReporteGenerado.pdf"));
                Document doc = new Document(pdfDoc);

                int numeros = pdfDoc.GetNumberOfPages();

                for (int i = 1; i <= numeros; i++)
                {
                    PdfPage pagina = pdfDoc.GetPage(i);

                    float y = (pdfDoc.GetPage(i).GetPageSize().GetTop() - 15);
                    doc.ShowTextAligned(plogo, 40, y, i, iText.Layout.Properties.TextAlignment.CENTER, iText.Layout.Properties.VerticalAlignment.TOP, 0);
                    doc.ShowTextAligned(titulo, 150, y - 15, i, iText.Layout.Properties.TextAlignment.CENTER, iText.Layout.Properties.VerticalAlignment.TOP, 0);
                    doc.ShowTextAligned(fecha, 520, y - 15, i, iText.Layout.Properties.TextAlignment.CENTER, iText.Layout.Properties.VerticalAlignment.TOP, 0);

                    doc.ShowTextAligned(new iText.Layout.Element.Paragraph(String.Format($"Pagina {i} de {numeros}")), pdfDoc.GetPage(i).GetPageSize().GetWidth() / 2, pdfDoc.GetPage(i).GetPageSize().GetBottom() + 30, i, iText.Layout.Properties.TextAlignment.CENTER, iText.Layout.Properties.VerticalAlignment.TOP, 0);
                }

                doc.Close();

                //Direccion = "C:/Users/HP/source/repos/Biblioteca/ReportesGenerados/ReporteGenerado.pdf";

                ResultadoReporte = "Se ha generado el reporte";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //return Direccion;

        }


        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
