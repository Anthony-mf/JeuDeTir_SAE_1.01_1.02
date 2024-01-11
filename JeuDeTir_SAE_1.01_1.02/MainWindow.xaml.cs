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
        //Liste des images pour le joueur
        private List<Image> animationJoueur = new List<Image>();
        //Liste des images pour les ennemis
        private List<Image> animationEnnemis = new List<Image>();
        //minuterie
        private DispatcherTimer minuterie = new DispatcherTimer();
        //rectangle joueur
        private Rect joueur;


        public MainWindow()
        {
            #if DEBUG
                Console.WriteLine(allerDroite);
            #endif
            InitializeComponent();
            /*ImageBrush fondWPF = new ImageBrush();
            fondWPF.ImageSource = new BitmapImage(new Uri("img\\statique\\FondCanvas.jpg"));
            monCanvas.Background = fondWPF;
            ImageBrush fondMenu = new ImageBrush();
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondMenu.png"));

            //MenuCanvas.Background = fondMenu;
            //Menu menu = new Menu();
            //menu.ShowDialog();*/
            minuterie.Tick += GameEngine;
            // rafraissement toutes les 16 milliseconds
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Start();
            CreationJoueur(1);
            CreationEnnemis(30);
        }

        //----------------------------------------------------------------------
        //---------------------------JOUEUR-------------------------------------

        private void CreationJoueur(int nbJoueur)
        {
            int gauche = 400;
            for (int i = 0; i < nbJoueur; i++)
            {
                ImageBrush joueurSkin = new ImageBrush();
                //joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Statique/tortue_statique.png"));
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


        //----------------------------------------------------------------------
        //--------------------------ENNEMIS-------------------------------------

        private void CreationEnnemis(int nbEnnemis)
        {
            Random rdm1 = new Random();
            Random rdm2 = new Random();
            double x, y;
            for (int i = 0; i < nbEnnemis; i++)
            {
                x = rdm1.Next();
                y = rdm2.Next();
                ImageBrush ennemiSkin = new ImageBrush();
                //joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Statique/tortue_statique.png"));
                Rectangle nouvelEnnemi = new Rectangle
                {
                    Tag = "ennemi",
                    Height = 15,
                    Width = 15,
                    Fill = Brushes.Blue,
                };
                Canvas.SetTop(nouvelEnnemi, x);
                Canvas.SetLeft(nouvelEnnemi, y);
                monCanvas.Children.Add(nouvelEnnemi);
            }
        }

        //---------------------------------------------------------------------------
        //-----------------------------TIRS------------------------------------------

        private void DeplacementsEtCollisionBalleJoueur(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "balleJoueur")
            {
                // si c’est un tir joueur on le déplace vers le haut
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);
                // création d’un tir joueur à base d’un rectangle Rect (nécessaire pour la collision)
                Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                // on vérifie que le tir a quitté le le haut du canvas (pas de collision avec un ennemie)
                if (Canvas.GetTop(x) < 10)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }
            }
        }
                private void GameEngine(object sender, EventArgs e)
        {
           
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                MouvementJoueur(x);
                DeplacementsEtCollisionBalleJoueur(x);
            }
        }

        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------

        private void MouvementJoueur(Rectangle x)
        {
            //Gauche
            if (x is Rectangle && (string)x.Tag == "joueur" && allerGauche && Canvas.GetLeft(x) > 0)
            {
                int angle = -90;
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
            //Droite
            else if (allerDroite && Canvas.GetLeft(x) + x.Width < Application.Current.MainWindow.ActualWidth)
            {
                int angle = 90;
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
            //Haut
            if (x is Rectangle && (string)x.Tag == "joueur" && allerHaut && Canvas.GetTop(x) > 0)
            {
                int angle = 360;
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseJoueur);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);

            }
            //Bas
            else if (allerBas && Canvas.GetTop(x) + x.Height < Application.Current.MainWindow.ActualHeight)
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
                // on vide la liste des items
                supprimerObjet.Clear();
                // création un nouveau tir
                Rectangle nouvelleBalle = new Rectangle
                {
                    Tag = "balleJoueur", //permet de tagger les rectangles
                    Height = 20,
                    Width = 5,
                    Fill = Brushes.White,
                    Stroke = Brushes.Red
                };
                // on place le tir à l’endroit du joueur
                Canvas.SetTop(nouvelleBalle, joueur.Height - nouvelleBalle.Height);
                Canvas.SetLeft(nouvelleBalle, joueur.Width + joueur.Width / 2);
                // on place le tir dans le canvas
                monCanvas.Children.Add(nouvelleBalle);
            }
        }

        private void CanvasToucheLevées(object sender, KeyEventArgs e)
        {
            //stop les déplacements si la touche est relevée
            if (e.Key == Key.Up)
            {
                allerGauche = false;
            }
            if (e.Key == Key.Up)
            {
                allerDroite = false;
            }
            if (e.Key == Key.Up)
            {
                allerHaut = false;
            }
            if (e.Key == Key.Up)
            {
                allerBas = false;
            }
        }
    }
}
