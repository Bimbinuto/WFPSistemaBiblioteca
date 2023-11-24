using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Biblioteca.Modelos;

namespace Biblioteca.ModeloDeVista
{
    public class BibliotecarioVM : INotifyPropertyChanged
    {
        private BibliotecarioM _biblibiotecario = new BibliotecarioM();
        public ICommand RegistrarCommand { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isNombresEmpty;

        // control de atributos de Bibliotecario.
        public long? CI
        {
            get => _biblibiotecario.ci;
            set
            {
                _biblibiotecario.ci = value;
                OnPropertyChanged(nameof(CI));

                Contrasena = _biblibiotecario.nombres + _biblibiotecario.ci.ToString();
            }
        }

        public string Nombres
        {
            get => _biblibiotecario.nombres;
            set
            {
                _biblibiotecario.nombres = value;
                //IsNombresEmpty = String.IsNullOrEmpty(value);
                OnPropertyChanged(nameof(Nombres));

                Cuenta = _biblibiotecario.nombres + "." + _biblibiotecario.apPaterno + "@sistenas.edu.bo";
                Contrasena = _biblibiotecario.nombres + _biblibiotecario.ci.ToString();
            }
        }

        public string ApPaterno
        {
            get => _biblibiotecario.apPaterno;
            set
            {
                _biblibiotecario.apPaterno = value;
                OnPropertyChanged(nameof(ApPaterno));

                Cuenta = _biblibiotecario.nombres + "." + _biblibiotecario.apPaterno + "@sistenas.edu.bo";
            }
        }

        public string ApMaterno
        {
            get => _biblibiotecario.apMaterno;
            set
            {
                _biblibiotecario.apMaterno = value;
                OnPropertyChanged(nameof(ApMaterno));
            }
        }

        public DateTime FechaNacimiento
        {
            get => _biblibiotecario.fechaNacimiento;
            set
            {
                _biblibiotecario.fechaNacimiento = value;
                OnPropertyChanged(nameof(FechaNacimiento));
            }
        }

        public string Direccion
        {
            get => _biblibiotecario.direccion;
            set
            {
                _biblibiotecario.direccion = value;
                OnPropertyChanged(nameof(Direccion));
            }
        }

        public long? Telefono
        {
            get => _biblibiotecario.telefono;
            set
            {
                _biblibiotecario.telefono = value;
                OnPropertyChanged(nameof(Telefono));
            }
        }

        public string Correo
        {
            get => _biblibiotecario.correo;
            set
            {
                _biblibiotecario.correo = value;
                OnPropertyChanged(nameof(Correo));
            }
        }

        public string Cuenta
        {
            get => _biblibiotecario.cuenta;
            set
            {
                _biblibiotecario.cuenta = value;
                OnPropertyChanged(nameof(Cuenta));
            }
        }

        public string Contrasena
        {
            get => _biblibiotecario.contrasena;
            set
            {
                _biblibiotecario.contrasena = value;
                OnPropertyChanged(nameof(Contrasena));
            }
        }

        public string Sexo
        {
            get => _biblibiotecario.sexo;
            set
            {
                _biblibiotecario.sexo = value;
                OnPropertyChanged(nameof(Sexo));
            }
        }

        public DateTime FechaContrato
        {
            get => _biblibiotecario.fechaContrato;
            set
            {
                _biblibiotecario.fechaContrato = value;
                OnPropertyChanged(nameof(FechaContrato));
            }
        }


        #region control de contenido del TextBox

        public bool IsNombresEmpty
        {
            get => _isNombresEmpty;
            set
            {
                _isNombresEmpty = value;
                OnPropertyChanged(nameof(IsNombresEmpty));
            }
        }


        #endregion control de contenido del TexBox

        public BibliotecarioVM()
        {
            FechaNacimiento = new DateTime(2000, 1, 1);
            FechaContrato = new DateTime(2000, 1, 1);
            RegistrarCommand = new RelayCommand(Registrar, PuedeRegistrar);
        }

        private void Registrar(object parameter)
        {
            if (Nombres == "user")
            {
                MessageBox.Show("Bienvenido");
            }
        }

        private bool PuedeRegistrar(object parameter)
        {
            return (CI > 0 && CI != 0 && CI.HasValue) &&
                   !String.IsNullOrEmpty(Nombres) &&
                   !String.IsNullOrEmpty(ApPaterno) &&
                   !String.IsNullOrEmpty(ApMaterno) &&
                   (FechaNacimiento != null && FechaNacimiento != default(DateTime)) &&
                   !String.IsNullOrEmpty(Direccion) &&
                   (Telefono > 0 && Telefono != 0 && Telefono.HasValue) &&
                   !String.IsNullOrEmpty(Correo) &&
                   !String.IsNullOrEmpty(Cuenta) &&
                   !String.IsNullOrEmpty(Contrasena) &&
                   !String.IsNullOrEmpty(Sexo) &&
                   (FechaContrato != null);
        }



        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
