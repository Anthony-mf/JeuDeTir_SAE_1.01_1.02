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

namespace JeuDeTir_SAE_1._01_1._02
{
    /// <summary>
    /// Logique d'interaction pour Menu.xaml
    /// </summary>
    public partial class Menu : Window
    {
        public string niveauDifficulte = "Facile";
        public Menu()
        {
            InitializeComponent();
            // on affecte le skin du menu
            ImageBrush fondMenu = new ImageBrush();
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondMenu.png"));
            menuCanvas.Background = fondMenu;
            menuCanvas.Background.Opacity = 0.5;
        }

        public void butFacile_Click(object sender, RoutedEventArgs e)
        {
            niveauDifficulte = "Facile";
            this.DialogResult = true;
        }
        public void butMoyen_Click(object sender, RoutedEventArgs e)
        {
            niveauDifficulte = "Moyen";
            this.DialogResult = true;
        }
        public void butDifficile_Click(object sender, RoutedEventArgs e)
        {
            niveauDifficulte = "Difficile";
            this.DialogResult = true;
        }
        public void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
