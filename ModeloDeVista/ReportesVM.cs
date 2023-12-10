using Biblioteca.BDConexion;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System.IO;
using Microsoft.Win32;
using MySql.Data.MySqlClient;

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

        public async Task<bool> GenerarReporte()
        {
            SaveFileDialog guardar = new SaveFileDialog();
            guardar.FileName = "C:\\Users\\HP\\Downloads\\TestReportes\\" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".pdf";

            Direccion = guardar.FileName;

            string paginahtml_texto = Properties.Resources.plantilla.ToString();

            paginahtml_texto = paginahtml_texto.Replace("@FECHA", DateTime.Now.ToString("dd-MM-yyyy"));
            paginahtml_texto = paginahtml_texto.Replace("@HORA", DateTime.Now.ToString("HH:mm:ss"));
            paginahtml_texto = paginahtml_texto.Replace("@CODIGO", "SISINF" + DateTime.Now.ToString("ddMMyyyyHHmmss"));

            //string consulta = "SELECT * FROM prestamo";
            //string total = "SELECT COUNT(*) FROM prestamo"

            string mesActual = DateTime.Now.ToString("yyyy-MM");
            string consulta = $"SELECT * FROM prestamo WHERE DATE_FORMAT(fecha_prestamo, '%Y-%m') = '{mesActual}'";

            paginahtml_texto = paginahtml_texto.Replace("@MES", mesActual);

            //PRESTAMOS DEL MES
            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consulta, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                StringBuilder filas = new StringBuilder();

                while (reader.Read())
                {
                    string fechaDevolucion = reader["fecha_devolucion"] == DBNull.Value ? "sala" : Convert.ToDateTime(reader["fecha_devolucion"]).Date.ToString("dd/MM/yyyy");
                    string fechaMaxDevolucion = reader["fecha_max_devolucion"] == DBNull.Value ? "-" : Convert.ToDateTime(reader["fecha_max_devolucion"]).Date.ToString("dd/MM/yyyy");

                    filas.Append("<tr>");
                    filas.Append("<td>" + reader["id_prestamo"] + "</td>");
                    filas.Append("<td>" + reader["fecha_prestamo"] + "</td>");
                    filas.Append("<td>" + fechaDevolucion + "</td>");
                    filas.Append("<td>" + fechaMaxDevolucion + "</td>");
                    filas.Append("<td>" + reader["tipo_prestamo"] + "</td>");
                    filas.Append("<td>" + reader["id_transaccion"] + "</td>");
                    filas.Append("</tr>");
                }

                reader.Close();

                paginahtml_texto = paginahtml_texto.Replace("@FILAS", filas.ToString());

                await cnx.CloseAsync();
            }

            //CANTIDAD DE LIBROS PRESTADOS DEL MES (COMPLEMENTARIO AL ANTERIOR)
            string totales = $"SELECT COUNT(*) as total FROM prestamo WHERE DATE_FORMAT(fecha_prestamo, '%Y-%m') = '{mesActual}'";
            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(totales, cnx);
                await cnx.OpenAsync();

                MySqlDataReader totalMes = cmd.ExecuteReader();

                totalMes.Read();

                string totalMesActual = totalMes["total"].ToString();


                paginahtml_texto = paginahtml_texto.Replace("@TOTAL", totalMesActual.ToString());

                totalMes.Close();

                await cnx.CloseAsync();
            }

            //LISTA DE LOS LIBROS MAS PRESTADOS:
            string librosMasPrestados = "SELECT L.titulo as titulo, COUNT(*) AS veces_prestado\r\nFROM prestamo P\r\nJOIN transaccion T ON P.id_transaccion = T.id_transaccion\r\nJOIN ejemplar E ON T.id_ejemplar = E.id_ejemplar\r\nJOIN libro L ON E.id_libro = L.id_libro\r\nGROUP BY L.titulo\r\nORDER BY veces_prestado DESC;\r\n";
            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(librosMasPrestados, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                StringBuilder filas = new StringBuilder();

                while (reader.Read())
                {
                    filas.Append("<tr>");
                    filas.Append("<td>" + reader["titulo"] + "</td>");
                    filas.Append("<td>" + reader["veces_prestado"] + "</td>");
                    filas.Append("</tr>");
                }

                reader.Close();

                paginahtml_texto = paginahtml_texto.Replace("@LIBROS", filas.ToString());

                await cnx.CloseAsync();
            }

            //LISTA DE LAS MULTA:
            string consultaMultas = "SELECT CONCAT(U.nombres, ' ', U.ap_paterno, ' ', U.ap_materno) AS nombre_completo, M.fecha_multa ,M.monto\r\nFROM multa M\r\nJOIN lector L ON M.id_lector = L.id_lector\r\nJOIN usuario U ON L.id_usuario = U.id_usuario\r\nWHERE M.fecha_cancelada IS NOT NULL;";

            using (MySqlConnection cnx = new MySqlConnection(_conexion.cadenaConexion))
            {
                MySqlCommand cmd = new MySqlCommand(consultaMultas, cnx);
                await cnx.OpenAsync();

                MySqlDataReader reader = cmd.ExecuteReader();

                StringBuilder filas = new StringBuilder();

                while (reader.Read())
                {
                    filas.Append("<tr>");
                    filas.Append("<td>" + reader["nombre_completo"] + "</td>");
                    filas.Append("<td>" + Convert.ToDateTime(reader["fecha_multa"]).Date.ToString("dd/MM/yyyy") + "</td>");
                    filas.Append("<td>" + reader["monto"] + "</td>");
                    filas.Append("</tr>");
                }

                reader.Close();

                paginahtml_texto = paginahtml_texto.Replace("@MULTAS", filas.ToString());
                paginahtml_texto = paginahtml_texto.Replace("@MES", DateTime.Now.ToString("MMMM yyyy"));

                await cnx.CloseAsync();
            }



            //LEER GUARDAR Y CREAR REPORTE PDF
            if (guardar.ShowDialog() == true)
            {
                using (FileStream stream = new FileStream(guardar.FileName, FileMode.Create))
                {
                    Document pdfDoc = new Document(PageSize.LETTER, 25, 25, 25, 25);

                    PdfWriter writer = PdfWriter.GetInstance(pdfDoc, stream);

                    pdfDoc.Open();

                    pdfDoc.Add(new Phrase(""));

                    //CREACION DEL LOGO SUPERIOR
                    iTextSharp.text.Image img = iTextSharp.text.Image.GetInstance(Properties.Resources.LOGO_SIS, System.Drawing.Imaging.ImageFormat.Png);
                    img.ScaleToFit(60, 60);
                    img.Alignment = iTextSharp.text.Image.UNDERLYING;
                    img.SetAbsolutePosition(pdfDoc.Right - 100, pdfDoc.Top - 75);
                    pdfDoc.Add(img);

                    iTextSharp.text.Image img2 = iTextSharp.text.Image.GetInstance(Properties.Resources.LOGO_INF, System.Drawing.Imaging.ImageFormat.Png);
                    img2.ScaleToFit(44, 44);
                    img2.Alignment = iTextSharp.text.Image.UNDERLYING;
                    img2.SetAbsolutePosition(pdfDoc.Right - 40, pdfDoc.Top - 74);
                    pdfDoc.Add(img2);

                    iTextSharp.text.Image img3 = iTextSharp.text.Image.GetInstance(Properties.Resources.FNI, System.Drawing.Imaging.ImageFormat.Png);
                    img3.ScaleToFit(60, 60);
                    img3.Alignment = iTextSharp.text.Image.UNDERLYING;
                    img3.SetAbsolutePosition(pdfDoc.LeftMargin + 40, pdfDoc.Top - 80);
                    pdfDoc.Add(img3);

                    using (StringReader sr = new StringReader(paginahtml_texto))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);
                    }

                    pdfDoc.Close();

                    stream.Close();

                    ResultadoReporte = "Se ha generado el reporte";
                }
            }
            return true;
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}




// NO USAR, EN CONFUSO
/*
 
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

 
 
 */
