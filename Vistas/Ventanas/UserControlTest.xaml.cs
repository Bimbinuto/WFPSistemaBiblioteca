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
using System.Security.Cryptography;
using System.IO;

namespace Biblioteca.Vistas.Ventanas
{
    /// <summary>
    /// Lógica de interacción para UserControlTest.xaml
    /// </summary>
    public partial class UserControlTest : UserControl
    {
        public UserControlTest()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string original = txtDatos.Text;

            using (Aes myAes = Aes.Create())
            {
                // Encriptar
                //byte[] encrypted = EncryptStringToBytes_Aes(original, myAes.Key, myAes.IV);
                //string encryptedString = BitConverter.ToString(encrypted); //.Replace("-", "");

                // Desencriptar
                //string roundtrip = DecryptStringFromBytes_Aes(encrypted, myAes.Key, myAes.IV);

                string hashed = ComputeSha256Hash(original);


                // Imprimir
                lbCripto.Content = hashed;
                //lbDescifrada.Content = roundtrip;
            }
        }

        public byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            return encrypted;
        }

        public string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;
        }

        static public string ComputeSha256Hash(string rawData)
        {
            // Crear un objeto SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // CalcularHash   
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convertir byte array a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
