using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
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
        //déplacement gauche/droite
        private bool allerGauche, allerDroite = false;
        //déplacement haut/bas
        private bool allerHaut, allerBas = false;
        //vitesse du joueur
        private int vitesseJoueur = 7;
        // vitesse du tir du joueur
        private int vitesseBallesJoueurs = 10;
        //liste des objets à suppr
        private List<Rectangle> supprimerObjet = new List<Rectangle>();
        //nb joueurs 
        private int totalJoueur;
        //minuterie
        private DispatcherTimer minuterie = new DispatcherTimer();
        

        public MainWindow()
        {
            InitializeComponent();
            ImageBrush fondWPF = new ImageBrush();
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondCanvas.jpg"));
            monCanvas.Background = fondWPF;
            ImageBrush fondMenu = new ImageBrush();
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondMenu.png"));

            //MenuCanvas.Background = fondMenu;
            Menu menu = new Menu();
            menu.ShowDialog();
            minuterie.Tick += GameEngine;
            // rafraissement toutes les 16 milliseconds
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Start();
            CreationJoueur(1);
            
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
                    Height = 15,
                    Width = 15,
                    Fill = Brushes.Red,
                };
                Canvas.SetTop(nouveauJoueur, 100);
                Canvas.SetLeft(nouveauJoueur, gauche);
                monCanvas.Children.Add(nouveauJoueur);
                gauche -= 100;
                
            }
        }
        //---------------------------------------------------------------------------
        //-----------------------------TIRS------------------------------------------

        private void TestBallesAllié(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "ballesJoueur")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);//monte
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseBallesJoueurs);//descend
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseBallesJoueurs);//gauche
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseBallesJoueurs);//droite
                //création rectangle pour une balle
                Rect balles = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            }
            //pour delete les balles jsp comment faire
            if ((string)x.Tag == "ballesJoueursjjjjjj" )//&& )
            {
                supprimerObjet.Add(x);

            }
        }
        private void GameEngine(object sender, EventArgs e)
        {
           
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                MouvementJoueur(x);
                TestBallesAllié(x);
                
            }
        }
        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------

        private void MouvementJoueur(Rectangle x)
        {
           
            if (x is Rectangle && (string)x.Tag == "joueur" && allerGauche && Canvas.GetLeft(x) > 0)
            {
                int angle = -90;
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
            //droite
            else if (allerDroite && Canvas.GetLeft(x) + x.Width < Application.Current.MainWindow.Width)
            {
                int angle = 90;
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
            if (x is Rectangle && (string)x.Tag == "joueur" && allerHaut && Canvas.GetTop(x) > 0)
            {
                int angle = 360;
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);

            }
            //bas
            else if (allerBas && Canvas.GetTop(x) + x.Height < Application.Current.MainWindow.Height)
            {
                int angle = -180;
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
        }

//----------------------------------------------------------------------
//--------------------------TOUCHES-------------------------------------

        private void CanvasToucheBaissée(object sender, KeyEventArgs e)
        {
            //touche pour déplacement à gauche
            if (e.Key == Key.Left)
            {
                allerGauche = true;
            }
            //touche pour déplacement à droite
            if (e.Key == Key.Right)
            {
                allerDroite = true;
            }
            //touche pour déplacement en haut
            if (e.Key == Key.Up)
            {
                allerHaut = true;
            }
            //touche pour déplcement en bas
            if (e.Key == Key.Down)
            {
                allerBas = true;
            }
            //touche tir
            if (e.Key == Key.Space)
            {
                // clear des objets à suppr
                supprimerObjet.Clear();
                // création tir
                Rectangle nouvelleBalle = new Rectangle
                {
                    Tag = "ballesJoueur",
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };
                // on place le tir à l’endroit du joueur
                //Canvas.SetTop(nouvelleBalle, Canvas.GetTop() - nouvelleBalle.Height);
                //Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + 75);
                // on place le tir dans le canvas
                monCanvas.Children.Add(nouvelleBalle);
            }
        }
        private void CanvasToucheRelachée(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                allerGauche = false;
            }
            if (e.Key == Key.Right)
            {
                allerDroite = false;
            }
            if (e.Key == Key.Up)
            {
                allerHaut = false;
            }
            if (e.Key == Key.Down)
            {
                allerBas = false;
            }
        }


    }
}
