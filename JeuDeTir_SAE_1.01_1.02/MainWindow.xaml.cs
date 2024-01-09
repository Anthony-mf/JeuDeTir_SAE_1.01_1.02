using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using System.Windows.Threading;

namespace JeuDeTir_SAE_1._01_1._02
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool allerGauche, allerDroite = false;
        private bool allerHaut, allerBas = false;
        private int vitesseJoueur = 7;
        private Rect joueur;
        private DispatcherTimer minuterie = new DispatcherTimer();

        private int totalJoueur;
        public MainWindow()
        {
            InitializeComponent();
            /*ImageBrush fondWPF = new ImageBrush();
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/FondCanvas.jpg"));
            monCanvas.Background = fondWPF;
            ImageBrush fondMenu = new ImageBrush();
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/FondMenu.png"));
            //MenuCanvas.Background = fondMenu;*/
            Menu menu = new Menu();
            menu.ShowDialog();
            minuterie.Tick += GameEngine;
            // rafraissement toutes les 16 milliseconds
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Start();
        }

        private void CreationJoueur(int nbJoueur)
        {
            int gauche = 400;
            totalJoueur = nbJoueur;
            for (int i = 0; i < nbJoueur; i++)
            {
                ImageBrush joueurSkin = new ImageBrush();
                joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/Statique/tortue_statique.png"));
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
        private void GameEngine(object sender, EventArgs e)
        {
            CreationJoueur(1);
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                //DeplacementsJoueur(x, joueur);
            }
        }
        /*private void DeplacementsJoueur(Rectangle x, Rect joueur)
        {
            if (allerGauche && Canvas.GetLeft() > 0)
            {
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - playerSpeed);
            }
            else if (allerDroite && Canvas.GetLeft(player) + player1.Width < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }
            else if (allerHaut && Canvas.GetTop(player) + player1.Width < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }
            else if (allerBas && Canvas.GetBottom(player) + player1.Width < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(player, Canvas.GetLeft(player) + playerSpeed);
            }*/

        //}
        private void CanvasToucheBaissée(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                allerGauche = true;
            }
            if (e.Key == Key.Right)
            {
                allerDroite = true;
            }
            if (e.Key == Key.Up)
            {
                allerHaut = true;
            }
            if (e.Key == Key.Down)
            {
                allerBas = true;
            }
        }
    }
}
