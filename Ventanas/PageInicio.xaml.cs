﻿using Biblioteca.BDConexion;
using Biblioteca.ViewModel;
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

namespace Biblioteca.Ventanas
{
    /// <summary>
    /// Lógica de interacción para PageInicio.xaml
    /// </summary>
    public partial class PageInicio : Page
    {
        BaseDeDatosVM dbvm;

        public PageInicio()
        {
            InitializeComponent();
            dbvm = (BaseDeDatosVM)FindResource("BDVM");
        }

        private void btnConectar(object sender, RoutedEventArgs e)
        {
            dbvm.estadoDeLaConexion();
        }
    }
}
