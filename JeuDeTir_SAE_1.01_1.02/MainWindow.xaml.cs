using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading;
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
        //déplacement gauche/droite/haut/bas
        private bool allerGauche, allerDroite, allerHaut, allerBas = false;
        //vitesse de l'ennemi
        private int vitesseEnnemi = 2;
        //vitesse du joueur
        private int vitesseJoueur = 7;
        // vitesse du tir du joueur
        private int vitesseBallesJoueurs = 10;
        //vitesse balle ennemi
        private int vitesseBallesEnnemis = 2;
        //liste des objets à supprimer
        private List<Rectangle> supprimerObjet = new List<Rectangle>();
        //Liste des images de marche pour le joueur
        private ImageBrush[] skinJoueur;
        //minuterie
        private DispatcherTimer minuterie = new DispatcherTimer();
        //direction
        private string direction;
        //minuterie de tir ennemi
        private int minuterieTir;
        //limite de la frequence du tir ennemi
        private int limiteMinuterieTir = 90;

        

        public MainWindow()
        {
            InitializeComponent();
            //montrer le menu
            Menu menu = new Menu();
            menu.ShowDialog();
            //menu.Owner = this;
            if (menu.DialogResult == false)
                Application.Current.Shutdown();
            else
                menu.Close();
            monCanvas.Focus();
            // on affecte le skin du MainWindow
            ImageBrush fondWPF = new ImageBrush();
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/FondCanvas.jpg"));
            monCanvas.Background = fondWPF;
            ImageBrush joueurSkin = new ImageBrush();
            joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/tortue_statique.png"));
            minuterie.Tick += MoteurDeJeu;
            // rafraissement toutes les 16 milliseconds
            minuterie.Interval = TimeSpan.FromMilliseconds(16);
            minuterie.Start();
            CreationEnnemis(30);
        }
        //----------------------------------------------------------------------
        //--------------------------ENNEMIS-------------------------------------

        private void CreationEnnemis(int nbEnnemis)
        {
            Random rdm1 = new Random();
            Random rdm2 = new Random();
            double x, y;
            // on affecte le skin des ennemis
            ImageBrush ennemiSkin = new ImageBrush();
            ennemiSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Statique/escargot_statique.png"));
            for (int i = 0; i < nbEnnemis; i++)
            {
                x = rdm1.Next(150, 550);
                y = rdm2.Next(150, 1250);
                Rectangle nouvelEnnemi = new Rectangle
                {
                    Tag = "ennemi",
                    Height = 50,
                    Width = 50,
                    Fill = ennemiSkin,
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
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_H")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);//monte
                if (Canvas.GetTop(x) < 0)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_B")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseBallesJoueurs);//descend
                if (Canvas.GetTop(x) > 800)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_D")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseBallesJoueurs);//droite
                if (Canvas.GetLeft(x) > 1600)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_G")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseBallesJoueurs);//gauche
                if (Canvas.GetLeft(x) < 0)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }
            }

        }

        // Creation munitions ennemi
        private void MunitionsEnnemis(double x, double y)
        {
            // création des tirs ennemies tirant vers l'objet joueur
            // x et y position du tir
            Rectangle nouvelleMunitionEnnemi = new Rectangle
            {
                Tag = "munitionsEnnemis",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5
            };
            Canvas.SetTop(nouvelleMunitionEnnemi, y);
            Canvas.SetLeft(nouvelleMunitionEnnemi, x);
            monCanvas.Children.Add(nouvelleMunitionEnnemi);
        }

        private void MinuterieDeTirEnnemi()
        {
            minuterieTir -= 2;
            if (minuterieTir == 0)
            { 
                MunitionsEnnemis((Canvas.GetLeft(joueur)) + joueur.Width / 2, 10);
                minuterieTir = limiteMinuterieTir;
            }
        }
        private void CollisionJoueur(Rectangle x)
        {
            if ((string)x.Tag == "ennemi")
            {
                //création d’un rectangle joueur pour la détection de collision
                Rect joueurRect = new Rect(Canvas.GetLeft(joueur), Canvas.GetTop(joueur), joueur.Width, joueur.Height);
                Rect ennemiRect = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                if (joueurRect.IntersectsWith(ennemiRect))
                {
                    // collision avec le joueur et fin de la partie
                    minuterie.Stop();
                }
            }
        }
        private void DeplacementsEnnemis(Rectangle x)
        {
            if ((string)x.Tag == "ennemi")
            {
                if (direction == "D" && Canvas.GetLeft(x) + x.Width < Application.Current.MainWindow.Width)
                {
                    int angle = -90;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemi);
                    x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                }
                if (direction == "G" && Canvas.GetLeft(x) > 0)
                {
                    int angle = 90;
                    Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemi);
                    x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                }
                if (direction == "H" && Canvas.GetTop(x) > 0)
                {
                    int angle = 180;
                    Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemi);
                    x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                }
                if (direction == "B" && Canvas.GetTop(x) + x.Height < Application.Current.MainWindow.Height)
                {
                    int angle = 360;
                    Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemi);
                    x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                }
                }
        }
        
        private void DeplacementsTirEnnemis()
        {

        }

        private void MoteurDeJeu(object sender, EventArgs e)
        {
            MouvementJoueur();
            MinuterieDeTirEnnemi();
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                DeplacementsEtCollisionBalleJoueur(x);
                DeplacementsEnnemis(x);
            }
            SupprimerObjet();
        }

        private void SupprimerObjet()
        {
            foreach (Rectangle y in supprimerObjet)
            {
                monCanvas.Children.Remove(y);
            }
        }

        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------

        private void MouvementJoueur()
        {
            //Gauche
            if (allerGauche && Canvas.GetLeft(joueur) > 0)
            {
                direction = "G";
                int angle = -90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
            }
            //Droite
            if (allerDroite && Canvas.GetLeft(joueur) + joueur.Width < Application.Current.MainWindow.Width)
            {
                direction = "D";
                int angle = 90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
            }
            //Haut
            if (allerHaut && Canvas.GetTop(joueur) > 0)
            {
                direction = "H";
                int angle = 360;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);

            }
            //Bas
            if (allerBas && Canvas.GetTop(joueur) + joueur.Height < Application.Current.MainWindow.Height)
            {
                direction="B";
                int angle = -180;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
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
                if (direction == "G")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs_G",
                        Height = 5,
                        Width = 20,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height / 2);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
                else if (direction == "D")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs_D",
                        Height = 5,
                        Width = 20,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height / 2);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
                else if (direction == "H")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs_H",
                        Height = 20,
                        Width = 5,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
                else if(direction == "B")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs_B",
                        Height = 20,
                        Width = 5,
                        Fill = Brushes.White,
                        Stroke = Brushes.Gray
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
            }
        }

        private void CanvasToucheLevées(object sender, KeyEventArgs e)
        {
            //stop les déplacements si la touche est relevée
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
