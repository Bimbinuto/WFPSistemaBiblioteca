﻿using System;
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
using Biblioteca.Vistas.Ventanas;
using Biblioteca.Vistas.Registros;
using Biblioteca.ModeloDeVista;

namespace Biblioteca.Vistas.Registros
{
    /// <summary>
    /// Lógica de interacción para GestionarDocente.xaml
    /// </summary>
    public partial class GestionarDocente : UserControl
    {
        ContentControl contentControl;
        DocenteVM docenteVM = new DocenteVM();

        public GestionarDocente(ContentControl ccDeAdministracion)
        {
            InitializeComponent();
            DataContext = docenteVM;
            contentControl = ccDeAdministracion;
        }

        private void btnVolverAdministracion(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new Administracion(contentControl);
        }

        private void btnEditarDocente(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new ModificarDocente(contentControl, docenteVM);
        }

        private void btnEliminarDocente(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new EliminarDocente(contentControl, docenteVM);
        }

        private void btnRegistrarNuevoDocente(object sender, RoutedEventArgs e)
        {
            contentControl.Content = new RegistrarDocente(contentControl);
        }
    }
}