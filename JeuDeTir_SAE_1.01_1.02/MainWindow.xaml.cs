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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int totalJoueur;
        public MainWindow()
        {
            InitializeComponent();
            CreationJoueur(1);
        }

        private void CreationJoueur(int nbJoueur)
        {
            int gauche = 400;
            totalJoueur = nbJoueur;
            for (int i = 0; i < nbJoueur; i++)
            {
                ImageBrush joueurSkin = new ImageBrush();
                joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Statique/tortue_statique.png"));

                Rectangle nouveauJoueur = new Rectangle
                {
                    Tag = "joueur",
                    Height = 150,
                    Width = 150,
                    Fill = joueurSkin,
                };
                Canvas.SetTop(nouveauJoueur, 350);
                Canvas.SetLeft(nouveauJoueur, gauche);
                monCanvas.Children.Add(nouveauJoueur);
                gauche -= 100;
            }
        }
    }
}
