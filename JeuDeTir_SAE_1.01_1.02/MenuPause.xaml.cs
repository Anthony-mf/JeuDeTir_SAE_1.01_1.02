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
    /// Logique d'interaction pour MenuPause.xaml
    /// </summary>
    public partial class MenuPause : Window
    {
        public double volumeValue;
        public string toucheGauche, toucheDroite, toucheBas, toucheHaut, toucheTir;
        public MenuPause(string toucheGaucheActuelle, string toucheDroiteActuelle, string toucheBasActuelle, string toucheHautActuelle, string toucheTirActuelle, double volumeActuelle)
        {
            InitializeComponent();
            ImageBrush fondMenuPause = new ImageBrush();
            fondMenuPause.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondMenu.png"));
            canvasMenuPause.Background = fondMenuPause;
            canvasMenuPause.Background.Opacity = 0.5;
            txtToucheGauche.Text = toucheGaucheActuelle;
            txtToucheDroite.Text = toucheDroiteActuelle;
            txtToucheHaut.Text = toucheHautActuelle;
            txtToucheBas.Text = toucheBasActuelle;
            txtToucheTir.Text = toucheTirActuelle;
            slideVolume.Value = volumeActuelle;
        }

        private void ButReprendre_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            toucheGauche = txtToucheGauche.Text;
            toucheDroite = txtToucheDroite.Text;
            toucheBas = txtToucheBas.Text;
            toucheHaut = txtToucheHaut.Text;
            toucheTir = txtToucheTir.Text;

        }
        private void butQuitter_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void slideVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            volumeValue = slideVolume.Value;
        }
    }
}
