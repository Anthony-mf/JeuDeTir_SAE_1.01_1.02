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
using System.Windows.Shapes;

namespace JeuDeTir_SAE_1._01_1._02
{
    /// <summary>
    /// Logique d'interaction pour MenuRejouer.xaml
    /// </summary>
    public partial class MenuRejouer : Window
    {
        public MenuRejouer()
        {
            InitializeComponent();
            // on affecte le skin du menu rejouer
            ImageBrush fondMenuRejouer = new ImageBrush();
            fondMenuRejouer.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/FondMenu.png"));
            canvasMenuRejouer.Background = fondMenuRejouer;
            canvasMenuRejouer.Background.Opacity = 0.5;
        }

        private void butRejouer_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
