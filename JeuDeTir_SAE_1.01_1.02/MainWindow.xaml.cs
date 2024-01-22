using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Media;
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
        // déplacement gauche/droite/haut/bas
        private bool allerGauche, allerDroite, allerHaut, allerBas = false;
        // boolean pour savoir si le joueur a perdu ou non
        private bool perdu = false;
        // vitesse de l'ennemi
        private int vitesseEnnemi = 30;
        //vitesse du joueur
        private int vitesseJoueur = 10;
        // vitesse du tir du joueur
        private int vitesseBallesJoueurs = 10;
        // vitesse balle ennemi
        private int vitesseBallesEnnemis = 2;
        // liste des objets à supprimer
        private List<Rectangle> supprimerObjet = new List<Rectangle>();
        // liste des ennemis
        private List<Rectangle> ennemis = new List<Rectangle>();
        // Liste des images de marche pour le joueur
        private ImageBrush[] animationMarcheJoueur;
        // Liste des images de mort pour l'ennemi
        private ImageBrush[] animationMortEnnemi;
        // skin du MainWindow
        ImageBrush fondWPF = new ImageBrush();
        // skin joueur
        ImageBrush joueurSkin = new ImageBrush();
        // skin ennemi
        ImageBrush ennemiSkin = new ImageBrush();
        // skin munitions
        ImageBrush munitionsSkin = new ImageBrush();
        // skin buisson
        ImageBrush buissonSkin = new ImageBrush();
        // skin buisson tourné
        ImageBrush buissonTourneSkin = new ImageBrush();
        // audio tir
        private SoundPlayer audioTir = new SoundPlayer();
        // audio fond
        private SoundPlayer audioFond = new SoundPlayer();
        // minuteur
        private DispatcherTimer minuteur = new DispatcherTimer();
        // direction
        private string direction;
        // minuteur de tir ennemi
        private int minuteurTir;
        // minuteur images marche joueur
        private int minuteurImagesMarcheJoueur;
        // minuteur images mort ennemi
        private int minuteurImagesMortEnnemi;
        // minuteur deplacements ennemis
        private int minuteurDeplacementsEnnemis;
        // minuteur apparition ennemis
        private int minuteurApparitionsEnnemis;
        //limite de la frequence du tir ennemi
        private int limiteMinuterieTir = 90;
        // angle
        private int angle = 0;
        // menu principal
        private Menu menu;
        // menu rejouer
        private MenuRejouer menuRejouer;
        // rectangle joueur
        private Rect rectJoueur = new Rect();
        // rectangle ennemi
        private Rect rectEnnemi = new Rect();
        // nombre d'ennemis
        private int nbEnnemis;
        // minuteur temps passe
        private int minuteurTempsPasse;
        // temps passé
        private int tempsPasse;
        // random
        Random rdm = new Random();

        public MainWindow()
        {
            InitializeComponent();
            // initialisation des images
            InitialisationImage();
            // initialisation des audios
            InitialisationAudio();
            // montrer le menu
            ApparitionMenu(perdu);
            monCanvas.Focus();
            // affectation minuteur au moteur de jeu
            minuteur.Tick += MoteurDeJeu;
            // rafraissement toutes les 16 milliseconds
            minuteur.Interval = TimeSpan.FromMilliseconds(16);
            // demarrage du minuteur
            minuteur.Start();
        }
        public static readonly DependencyProperty VelocityXProperty =
            DependencyProperty.RegisterAttached("VelocityX", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));

        public static readonly DependencyProperty VelocityYProperty =
            DependencyProperty.RegisterAttached("VelocityY", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        
        private void MoteurDeJeu(object sender, EventArgs e)
        {
            AffichageLabel();
            MouvementJoueur();
            MinuteurApparitionsEnnemis();
            //MinuteurDeTirEnnemi();
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                DeplacementsBallesJoueur(x);
                Collisions(x);
                DéplacementMunitions(x);
                MinuteurDeplacementsEnnemis(x);
            }
            SupprimerObjet();
        }

        private void InitialisationImage()
        {
            // affectation skin fond
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/FondCanvas"));
            monCanvas.Background = fondWPF;
            // affectation skin joueur
            joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/tortue_statique"));
            joueur.Fill = joueurSkin;
            // on affecte le skin des ennemis
            ennemiSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/Statique/escargot_statique"));
            // on affecte le skin des munitions
            munitionsSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/munitions"));
            // on affecte le skin du buisson en haut et en bas
            buissonSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/buissonNormal.png"));
            // on affecte le skin du buisson des côtés
            buissonTourneSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/buissonTourne.png"));
            buisson1.Fill = buissonTourneSkin;
            buisson2.Fill = buissonTourneSkin;
            buisson3.Fill = buissonSkin;
            buisson4.Fill = buissonSkin;
            buisson5.Fill = buissonSkin;
            buisson6.Fill = buissonSkin;
            animationMarcheJoueur = new ImageBrush[22];
            for (int i = 0; i < animationMarcheJoueur.Length; i++)
            {
                animationMarcheJoueur[i] = new ImageBrush();
                animationMarcheJoueur[i].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/sprite/tortue/marche/tortue_marche" + i + ".png"));
            }
            animationMortEnnemi = new ImageBrush[8];
            for (int i = 0; i < animationMortEnnemi.Length; i++)
            {
                animationMortEnnemi[i] = new ImageBrush();
                animationMortEnnemi[i].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/sprite/escargot/mort/escargot_mort" + i + ".png"));
            }
        }

        private void InitialisationAudio()
        {            
            // on affecte l'audio du tir
            audioTir = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "audio/bruitDeTir.wav");
            // on affecte l'audio de fond
            audioFond = new SoundPlayer(AppDomain.CurrentDomain.BaseDirectory + "audio/musiqueDeFond.wav");
            // on le joue en boucle
            audioFond.PlayLooping();
        }

        private void ApparitionMenu(bool perdu)
        {
            if (!perdu)
            {
                menu = new Menu();
                menu.ShowDialog();

                if (menu.DialogResult == true)
                    menu.Close();
                else
                    this.Close();
            }
            else
            {
                menuRejouer = new MenuRejouer();
                menuRejouer.ShowDialog();
            }
        }

        private void ReinitialisationJeu()
        {
            if (menuRejouer.DialogResult == true)
            {
                // Remise à zéro vos compteurs, ennemis, etc.
                nbEnnemis = 0;
                tempsPasse = 0;
                minuteurTempsPasse = 0;

                // Remise à zéro des booleans
                allerBas = false;
                allerHaut = false;
                allerDroite = false;
                allerGauche = false;

                foreach (Rectangle x in ennemis)
                {
                    monCanvas.Children.Remove(x);
                }

                // Remise à zéro vos listes et éléments de jeu si nécessaire
                supprimerObjet.Clear();
                ennemis.Clear();

                // Redemarrage du minuteur
                minuteur.Start();

                // Remise en place du joueur
                Canvas.SetTop(joueur, 305);
                Canvas.SetLeft(joueur, 660);
            }
        }

        private void AffichageLabel()
        {
            labEnnemisTués.Content = "Ennemis tués : " + nbEnnemis;
            minuteurTempsPasse++;
            if (minuteurTempsPasse % 30 == 0)
            {
                tempsPasse++;
                labTempsPassé.Content = "Temps passé : " + tempsPasse + " secondes";
            }
        }

        //----------------------------------------------------------------------
        //--------------------------ENNEMIS-------------------------------------

        private void CreationEnnemis()
        {
            int irdm;
            irdm = rdm.Next(0, 7);
            Rectangle nouvelEnnemi = new Rectangle
            {
                Tag = "ennemi",
                Height = 50,
                Width = 50,
                Fill = ennemiSkin,
            };
            // Sélection d'un buisson aléatoire
            Rectangle buisson;
            switch (irdm)
            {
                case 1:
                    buisson = buisson1;
                    break;
                case 2:
                    buisson = buisson2;
                    break;
                case 3:
                    buisson = buisson3;
                    break;
                case 4:
                    buisson = buisson4;
                    break;
                case 5:
                    buisson = buisson5;
                    break;
                case 6:
                    buisson = buisson6;
                    break;
                default:
                    // Valeur par défaut
                    buisson = buisson1;
                    break;
            }

            // Positionnement aléatoire à l'intérieur du buisson

            double positionX = rdm.Next((int)Canvas.GetLeft(buisson), (int)(Canvas.GetLeft(buisson) + buisson.Width - nouvelEnnemi.Width));
            double positionY = rdm.Next((int)Canvas.GetTop(buisson), (int)(Canvas.GetTop(buisson) + buisson.Height - nouvelEnnemi.Height));

            // Création des rect pour les ennemis et les buissons

            Rect ennemiRect = new Rect(positionX, positionY, nouvelEnnemi.Width, nouvelEnnemi.Height);
            Rect buissonRect = new Rect(Canvas.GetLeft(buisson), Canvas.GetTop(buisson), buisson.Width, buisson.Height);

            // Vérifiez le buisson et ajustez la position en conséquence
            if (buisson == buisson1)
            {
                // Si l'ennemi est dans le buisson de gauche, il sort par la droite
                positionX = Canvas.GetLeft(buisson) + buisson.Width;
                positionY = rdm.Next((int)Canvas.GetTop(buisson), (int)(Canvas.GetTop(buisson) + buisson.Height - nouvelEnnemi.Height));
            }
            else if (buisson == buisson2)
            {
                // Si l'ennemi est dans le buisson de droite, il sort par la gauche
                positionX = Canvas.GetLeft(buisson) - nouvelEnnemi.Width;
                positionY = rdm.Next((int)Canvas.GetTop(buisson), (int)(Canvas.GetTop(buisson) + buisson.Height - nouvelEnnemi.Height));
            }
            else if (buisson == buisson3)
            {
                // Si l'ennemi est dans le buisson du haut, il sort par le bas
                positionX = rdm.Next((int)Canvas.GetLeft(buisson), (int)(Canvas.GetLeft(buisson) + buisson.Width - nouvelEnnemi.Width));
                positionY = Canvas.GetTop(buisson) + buisson.Height;
            }
            else if (buisson == buisson4)
            {
                // Si l'ennemi est dans le buisson du haut, il sort par le bas
                positionX = rdm.Next((int)Canvas.GetLeft(buisson), (int)(Canvas.GetLeft(buisson) + buisson.Width - nouvelEnnemi.Width));
                positionY = Canvas.GetTop(buisson) + buisson.Height;
            }
            else if (buisson == buisson5)
            {
                // Si l'ennemi est dans le buisson en bas à gauche, il sort par le haut à droite
                positionX = rdm.Next((int)Canvas.GetLeft(buisson), (int)(Canvas.GetLeft(buisson) + buisson.Width - nouvelEnnemi.Width));
                positionY = Canvas.GetTop(buisson) - nouvelEnnemi.Height;
            }
            else
            {
                // Si l'ennemi est dans le buisson en bas à droite, il sort par le haut à gauche
                positionX = rdm.Next((int)Canvas.GetLeft(buisson), (int)(Canvas.GetLeft(buisson) + buisson.Width - nouvelEnnemi.Width));
                positionY = Canvas.GetTop(buisson) - nouvelEnnemi.Height;
            }

            // Vérifiez si la nouvelle position est à l'intérieur des limites du canvas
            positionX = Math.Max(0, Math.Min(positionX, monCanvas.ActualWidth - nouvelEnnemi.Width));
            positionY = Math.Max(0, Math.Min(positionY, monCanvas.ActualHeight - nouvelEnnemi.Height));

            Canvas.SetLeft(nouvelEnnemi, positionX);
            Canvas.SetTop(nouvelEnnemi, positionY);
            monCanvas.Children.Add(nouvelEnnemi);
            ennemis.Add(nouvelEnnemi);
        }

        //---------------------------------------------------------------------------
        //-----------------------------TIRS------------------------------------------

        private void DeplacementsBallesJoueur(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "ballesJoueursH")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);//monte
                if (Canvas.GetTop(x) < 0)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }
            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueursB")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseBallesJoueurs);//descend
                if (Canvas.GetTop(x) > 800)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueursD")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseBallesJoueurs);//droite
                if (Canvas.GetLeft(x) > 1600)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueursG")
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
        private void MunitionsEnnemis(double x, double y, double joueurX, double joueurY)
        {
            double directionX = joueurX - x;
            double directionY = joueurY - y;
            double norme = Math.Sqrt(directionX * directionX + directionY * directionY);

            // Normalisez la direction pour assurer une vitesse constante
            directionX /= norme;
            directionY /= norme;

            Rectangle nouvelleMunitionEnnemi = new Rectangle
            {
                Tag = "munitionEnnemi",
                Height = 40,
                Width = 15,
                Fill = Brushes.Yellow,
                Stroke = Brushes.Black,
                StrokeThickness = 5
            };

            Canvas.SetTop(nouvelleMunitionEnnemi, y);
            Canvas.SetLeft(nouvelleMunitionEnnemi, x);

            // Ajoutez des propriétés au rectangle pour le mouvement
            nouvelleMunitionEnnemi.SetValue(Canvas.LeftProperty, x);
            nouvelleMunitionEnnemi.SetValue(Canvas.TopProperty, y);
            nouvelleMunitionEnnemi.SetValue(VelocityXProperty, directionX * vitesseBallesEnnemis);
            nouvelleMunitionEnnemi.SetValue(VelocityYProperty, directionY * vitesseBallesEnnemis);

            monCanvas.Children.Add(nouvelleMunitionEnnemi);
        }


        private void DéplacementMunitions(Rectangle x)
        {
            if (x is Rectangle && x.Tag is string && (string)x.Tag == "munitionEnnemi")
            {
                double velocityX = (double)x.GetValue(VelocityXProperty);
                double velocityY = (double)x.GetValue(VelocityYProperty);

                Canvas.SetLeft(x, Canvas.GetLeft(x) + velocityX);
                Canvas.SetTop(x, Canvas.GetTop(x) + velocityY);

                // Vérifier si le tir sort de l'écran et l'ajouter à la liste à supprimer
                if (Canvas.GetTop(x) > ActualHeight + x.ActualHeight)
                    supprimerObjet.Add(x);
            }
        }

        private void MinuterieDeTirEnnemi()
        {
            minuteurTir -= 2;

            if (minuteurTir < 0)
            {
                foreach (Rectangle ennemi in ennemis.ToList())
                {
                    // Assurez-vous que l'ennemi est toujours présent
                    if (monCanvas.Children.Contains(ennemi))
                    {
                        MunitionsEnnemis(Canvas.GetLeft(ennemi), Canvas.GetTop(ennemi), positionJoueur.X, positionJoueur.Y);
                    }
                    else
                    {
                        // Supprimez l'ennemi de la liste s'il n'est plus présent
                        ennemis.Remove(ennemi);
                    }
                }

                minuteurTir = limiteMinuterieTir;
            }
        }

        private System.Windows.Point positionJoueur;


        private void Collisions(Rectangle x)
        {
            rectJoueur = new Rect(Canvas.GetLeft(joueur), Canvas.GetTop(joueur), joueur.Width, joueur.Height);


            if (x.Tag is string && (string)x.Tag == "ennemi")
            {
                rectEnnemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //création d’un rectangle joueur pour la détection de collision
                if (rectJoueur.IntersectsWith(rectEnnemi))
                {
                    // collision avec le joueur et fin de la partie
                    minuteur.Stop();
                }
            }

            foreach (var y in monCanvas.Children.OfType<Rectangle>())
            {
                rectJoueur = new Rect(Canvas.GetLeft(joueur), Canvas.GetTop(joueur), joueur.Width, joueur.Height);
                Rect munitionEnnemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                // Si le rectangle est une munition ennemie
                if (x.Tag is string && x is Rectangle && (string)x.Tag == "munitionEnnemi")
                {
                    // Création d'un rectangle correspondant à la munition ennemie

                    if (munitionEnnemi.IntersectsWith(rectJoueur))
                    {

                        minuteur.Stop();
                    }

                }

                // si le rectangle est une balle du joueur
                else if (y.Tag is string && y is Rectangle && ((string)y.Tag).StartsWith("ballesJoueurs"))
                {
                    Rect balleJoueur = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);

                    foreach (var ennemi in ennemis)
                    {
                        Rect rectEnnemi = new Rect(Canvas.GetLeft(ennemi), Canvas.GetTop(ennemi), ennemi.Width, ennemi.Height);

                        if (balleJoueur.IntersectsWith(rectEnnemi))
                        {
                            // collision avec un ennemi, ajoutez l'ennemi à la liste des objets à supprimer
                            supprimerObjet.Add(ennemi);
                            // ajoutez également la balle à la liste des objets à supprimer
                            supprimerObjet.Add(y);
                            // incrémente le nombre d'ennemis détruits

                        }
                    }
                }
            }
        }
        private void MinuteurApparitionsEnnemis()
        {
            minuteurApparitionsEnnemis ++;
            if (minuteurApparitionsEnnemis % 10 == 0)
            {
                CreationEnnemis();
            }
        }

        private void MinuteurDeplacementsEnnemis(Rectangle x)
        {
            minuteurDeplacementsEnnemis ++;
            if (minuteurDeplacementsEnnemis % 50 == 0)
            {
                DeplacementsEnnemis(x);
            }
        }
        
        private void DeplacementsEnnemis(Rectangle x)
        {
            int irdm = rdm.Next(0, 5);
            if ((string)x.Tag == "ennemi")
            {
                switch (irdm)
                {
                    case 1:
                        {
                            // Gauche
                            if (Canvas.GetLeft(x) > buisson1.Width)
                            {
                                angle = -90;
                                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseEnnemi);
                                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                            }
                            break;
                        }
                    case 2:
                        {
                            // Droite
                            if (Canvas.GetLeft(x) + x.Width < Application.Current.MainWindow.Width - buisson2.Width)
                            { 
                            angle = 90;
                            Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemi);
                            x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                            }
                            break;
                        }
                    case 3:
                        {
                            // Bas
                            if (Canvas.GetTop(x) + x.Height < Application.Current.MainWindow.Height - buisson5.Height && Canvas.GetTop(x) + x.Height < Application.Current.MainWindow.Height - buisson6.Height)
                            angle = 180;
                            Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemi);
                            x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                            break;
                        }
                    case 4:
                        {
                            // Haut
                            if(Canvas.GetTop(x) > buisson3.Height && Canvas.GetTop(x) > buisson4.Height)
                            { 
                            angle = 360;
                            Canvas.SetTop(x, Canvas.GetTop(x) - vitesseEnnemi);
                            x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
                            }
                            break;
                        }
                }
            }
        }
        
        private void DeplacementsTirEnnemis()
        {

        }

        private void SupprimerObjet()
        {
            foreach (Rectangle x in supprimerObjet)
            {
                monCanvas.Children.Remove(x);
            }
        }

        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------

        private void MouvementJoueur()
        {
            if (minuteurImagesMarcheJoueur > 20)
            {
                minuteurImagesMarcheJoueur = 0;
            }
            minuteurImagesMarcheJoueur++;
            //Gauche
            if (allerGauche && Canvas.GetLeft(joueur) > buisson1.Width)
            {
                direction = "G";
                angle = -90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
            }
            //Droite
            if (allerDroite && Canvas.GetLeft(joueur) + joueur.Width < Application.Current.MainWindow.Width - buisson2.Width)
            {
                direction = "D";
                angle = 90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
            }
            //Haut
            if (allerHaut && Canvas.GetTop(joueur) > buisson3.Height && Canvas.GetTop(joueur) > buisson4.Height)
            {
                direction = "H";
                angle = 360;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];

            }
            //Bas
            if (allerBas && Canvas.GetTop(joueur) + joueur.Height < Application.Current.MainWindow.Height - buisson5.Height && Canvas.GetTop(joueur) + joueur.Height < Application.Current.MainWindow.Height - buisson6.Height)
            {
                direction="B";
                angle = -180;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
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
            //touche tir
            if (e.Key == Key.Space)
            {
                // on lance l'audio à chaque tir
                audioTir.Play();
                // on vide la liste des items
                supprimerObjet.Clear();
                // création un nouveau tir
                if (direction == "G")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs" + direction,
                        Height = 20,
                        Width = 20,
                        Fill = munitionsSkin
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
                        Tag = "ballesJoueurs" + direction,
                        Height = 20,
                        Width = 20,
                        Fill = munitionsSkin
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
                        Tag = "ballesJoueurs" + direction,
                        Height = 20,
                        Width = 20,
                        Fill = munitionsSkin
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
                else if (direction == "B")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs" + direction,
                        Height = 20,
                        Width = 20,
                        Fill = munitionsSkin
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }
            }
        }
    }
}