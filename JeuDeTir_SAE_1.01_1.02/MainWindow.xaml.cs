// Importations
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
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
        // boolean pour la pause
        private bool enPause = false;
        // boolean pour les codes triches
        private bool codeTriche = false;
        // vitesse de l'ennemi
        private double vitesseEnnemi;
        private double positionX;
        private double positionY;
        //vitesse du joueur
        private int vitesseJoueur = 10;
        // vitesse du tir du joueur
        private int vitesseBallesJoueurs = 10;
        // vitesse balle ennemi
        private int vitesseBallesEnnemis;
        // liste des objets à supprimer
        private List<Rectangle> supprimerObjet = new List<Rectangle>();
        // liste des ennemis
        private List<Rectangle> ennemis = new List<Rectangle>();
        // liste des munitions ennemis
        List<Rectangle> munitionsEnnemi = new List<Rectangle>();
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
        private MediaPlayer audioTir = new MediaPlayer();
        // audio fond
        private MediaPlayer audioFond = new MediaPlayer();
        // minuteur
        private DispatcherTimer minuteur = new DispatcherTimer();
        // direction
        private string direction;
        // minuteur de tir ennemi
        private int minuteurTir;
        private int pourcentageMinuteurTir;
        // minuteur images marche joueur
        private int minuteurImagesMarcheJoueur;
        // minuteur deplacements ennemis
        private int minuteurDeplacementsEnnemis;
        private int pourcentageMinuteurDeplacementsEnnemis;
        // minuteur apparition ennemis
        private int minuteurApparitionsEnnemis;
        private int pourcentageMinuteurApparitionsEnnemis;
        // angle
        private int angle;
        // menu principal
        private Menu menu;
        // menu rejouer
        private MenuRejouer menuRejouer;
        // menu pause
        private MenuPause menuPause;
        // volume du jeu
        double volumeJeu = 0.5;
        // rectangle joueur
        private Rect rectJoueur = new Rect();
        // rectangle ennemi
        private Rect rectEnnemi = new Rect();
        // rectangle x
        private Rectangle x = new Rectangle();
        // nombre d'ennemis
        private int nbEnnemis;
        // minuteur temps passe
        private int minuteurTempsPasse;
        // temps passé
        private int tempsPasse;
        // random
        Random rdm = new Random();
        // position du joueur
        private Point positionJoueur;
        //
        private static readonly DependencyProperty VelocityXProperty =
            DependencyProperty.RegisterAttached("VelocityX", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        //
        private static readonly DependencyProperty VelocityYProperty =
            DependencyProperty.RegisterAttached("VelocityY", typeof(double), typeof(MainWindow), new PropertyMetadata(0.0));
        private string toucheGauche = "Left";
        private string toucheDroite = "Right";
        private string toucheHaut = "Up";
        private string toucheBas = "Down";
        private string toucheTir = "Space";

        public MainWindow()
        {
            InitializeComponent();
            InitialisationImage();
            InitialisationAudio();
            ApparitionMenu(perdu, enPause);
            monCanvas.Focus();
            minuteur.Tick += MoteurDeJeu;
            minuteur.Interval = TimeSpan.FromMilliseconds(16);
            minuteur.Start();
        }

        private void MoteurDeJeu(object sender, EventArgs e)
        {
            AffichageLabel();
            MouvementJoueur();
            MinuteurApparitionsEnnemis();
            MinuteurDeTirEnnemi();

            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                DeplacementsBallesJoueur(x);
                Collisions(x, codeTriche);
                DeplacementsMunitions(x);
                MinuteurDeplacementsEnnemis(x);
            }

            SupprimerObjet();
        }

        private void InitialisationImage()
        {
            // affectation skin fond
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/sol.jpg"));
            monCanvas.Background = fondWPF;
            // affectation skin joueur
            joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/tortue_statique.png"));
            joueur.Fill = joueurSkin;
            // on affecte le skin des ennemis
            ennemiSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/Statique/escargot_statique.png"));
            // on affecte le skin des munitions
            munitionsSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/munitions.png"));
            animationMarcheJoueur = new ImageBrush[22];
            for (int i = 0; i < animationMarcheJoueur.Length; i++)
            {
                animationMarcheJoueur[i] = new ImageBrush();
                animationMarcheJoueur[i].ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/sprite/tortue/marche/tortue_marche" + i + ".png"));
            }
        }

        private void InitialisationAudio()
        {
            // on affecte l'audio du tir
            audioTir.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "audio/bruitDeTir.mp3"));
            // on affecte l'audio de fond
            audioFond.Open(new Uri(AppDomain.CurrentDomain.BaseDirectory + "audio/musiqueDeFond.mp3"));
            // lancement de la musique de fond
            audioFond.Play();
            // lecture en boucle
            audioFond.MediaEnded += (sender, e) =>
            {
                audioFond.Position = TimeSpan.Zero;
                audioFond.Play();
            };
        }

        private void ApparitionMenu(bool perdu, bool enPause)
        {
            if (enPause)
            {
                menuPause = new MenuPause(toucheGauche, toucheDroite, toucheBas, toucheHaut, toucheTir, volumeJeu);
                menuPause.ShowDialog();

                if (menuPause.DialogResult == true)
                {
                    volumeJeu = menuPause.volumeValue;
                    audioFond.Volume = volumeJeu;
                    audioTir.Volume = volumeJeu;
                    ConfigurationsTouches();
                }
                else
                    this.Close();
            }
            else
            {
                if (!perdu)
                {
                    menu = new Menu();
                    menu.ShowDialog();

                    if (menu.DialogResult == true)
                    {
                        menu.Close();
                        AjusterDifficulte();
                    }
                    else
                        this.Close();
                }
                else
                {
                    menuRejouer = new MenuRejouer();
                    menuRejouer.ShowDialog();

                    if (menuRejouer.DialogResult == false)
                        this.Close();
                }
            }
        }

        private void ConfigurationsTouches()
        {
            toucheGauche = menuPause.toucheGauche;
            toucheDroite = menuPause.toucheDroite;
            toucheHaut = menuPause.toucheHaut;
            toucheBas = menuPause.toucheBas;
            toucheTir = menuPause.toucheTir;
        }

        private void ReinitialisationJeu(Rectangle x)
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
                perdu = false;
                enPause = false;

                // remise à zéro de la musique

                InitialisationAudio();

                // Remise à zéro de la liste des objets à supprimer

                supprimerObjet.Clear();

                foreach (Rectangle ennemi in ennemis)
                    supprimerObjet.Add(ennemi);
                foreach (Rectangle munitionsEnnemi in munitionsEnnemi)
                    supprimerObjet.Add(munitionsEnnemi);

                // Remise à zéro des autres listes

                ennemis.Clear();
                munitionsEnnemi.Clear();

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

        private void AjusterDifficulte()
        {
            if (menu.niveauDifficulte == "Facile")
            {
                vitesseEnnemi = 30;
                vitesseBallesEnnemis = 2;
                pourcentageMinuteurApparitionsEnnemis = 100;
                pourcentageMinuteurTir = 150;
                pourcentageMinuteurDeplacementsEnnemis = 100;
            }
            else if (menu.niveauDifficulte == "Moyen")
            {
                vitesseEnnemi = 40;
                vitesseBallesEnnemis = 5;
                pourcentageMinuteurApparitionsEnnemis = 75;
                pourcentageMinuteurTir = 75;
                pourcentageMinuteurDeplacementsEnnemis = 75;
            }
            else if (menu.niveauDifficulte == "Difficile")
            {
                vitesseEnnemi = 50;
                vitesseBallesEnnemis = 10;
                pourcentageMinuteurApparitionsEnnemis = 50;
                pourcentageMinuteurTir = 50;
                pourcentageMinuteurDeplacementsEnnemis = 50;
            }
        }

        //----------------------------------------------------------------------
        //--------------------------ENNEMIS-------------------------------------

        private void CreationEnnemis()
        {
            int irdm;
            irdm = rdm.Next(0, 5);
            Rectangle nouvelEnnemi = new Rectangle
            {
                Tag = "ennemi",
                Height = 50,
                Width = 50,
                Fill = ennemiSkin,
            };

            switch (irdm)
            {
                // gauche
                case 1:
                    positionX = Canvas.GetLeft(rectGauche) + rectGauche.Width;
                    positionY = Canvas.GetTop(rectGauche) + rdm.Next((int)(rectGauche.Height - rectHaut.Width), (int)rectGauche.Height);
                    break;
                // droite
                case 2:
                    positionX = Canvas.GetLeft(rectGauche) - nouvelEnnemi.Width;
                    positionY = Canvas.GetTop(rectGauche) + rdm.Next((int)rectGauche.Height);
                    break;
                // haut
                case 3:
                    positionX = Canvas.GetLeft(rectGauche) + rdm.Next((int)(rectHaut.Width - rectGauche.Width), (int)rectHaut.Width);
                    positionY = Canvas.GetTop(rectGauche) + rectEnnemi.Height;
                    break;
                // bas
                case 4:
                    positionX = Canvas.GetLeft(rectGauche) + rdm.Next((int)rectGauche.Width);
                    positionY = Canvas.GetTop(rectGauche) - nouvelEnnemi.Height;
                    break;
            }

            Canvas.SetLeft(nouvelEnnemi, positionX);
            Canvas.SetTop(nouvelEnnemi, positionY);
            monCanvas.Children.Add(nouvelEnnemi);
            ennemis.Add(nouvelEnnemi);
#if DEBUG
            Console.WriteLine("Nombre d'ennemis : " + ennemis.Count);
#endif
        }

        //---------------------------------------------------------------------------
        //-----------------------------TIRS------------------------------------------

        private void DeplacementsBallesJoueur(Rectangle x)
        {
            if ((string)x.Tag == "ballesJoueursH")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);//monte
                if (Canvas.GetTop(x) < 0)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }
            }
            if ((string)x.Tag == "ballesJoueursB")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseBallesJoueurs);//descend
                if (Canvas.GetTop(x) > 1000)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if ((string)x.Tag == "ballesJoueursD")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseBallesJoueurs);//droite
                if (Canvas.GetLeft(x) > 1000)
                {
                    // si c’est le cas on l’ajoute à la liste des éléments à supprimer
                    supprimerObjet.Add(x);
                }

            }
            if ((string)x.Tag == "ballesJoueursG")
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
                Height = 20,
                Width = 20,
                Fill = munitionsSkin
            };
            Canvas.SetTop(nouvelleMunitionEnnemi, y);
            Canvas.SetLeft(nouvelleMunitionEnnemi, x);
            // Ajoutez des propriétés au rectangle pour le mouvement
            nouvelleMunitionEnnemi.SetValue(Canvas.LeftProperty, x);
            nouvelleMunitionEnnemi.SetValue(Canvas.TopProperty, y);
            nouvelleMunitionEnnemi.SetValue(VelocityXProperty, directionX * vitesseBallesEnnemis);
            nouvelleMunitionEnnemi.SetValue(VelocityYProperty, directionY * vitesseBallesEnnemis);
            monCanvas.Children.Add(nouvelleMunitionEnnemi);
            munitionsEnnemi.Add(nouvelleMunitionEnnemi);
#if DEBUG
            Console.WriteLine("Nombre de tirs ennemis : " + munitionsEnnemi.Count);
#endif
        }

        private void Collisions(Rectangle x, bool codeTriche)
        {
            if (!codeTriche)
            {
                rectJoueur = new Rect(Canvas.GetLeft(joueur), Canvas.GetTop(joueur), joueur.Width, joueur.Height);
                if ((string)x.Tag == "ennemi")
                {
                    rectEnnemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                    //création d’un rectangle joueur pour la détection de collision
                    if (rectJoueur.IntersectsWith(rectEnnemi))
                    {
                        // collision avec le joueur et fin de la partie
                        minuteur.Stop();
                        perdu = true;
                        ApparitionMenu(perdu, enPause);
                        ReinitialisationJeu(x);
                    }
                }

                if ((string)x.Tag == "munitionEnnemi")
                {
                    Rect munitionEnnemi = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

                    if (munitionEnnemi.IntersectsWith(rectJoueur))
                    {
                        perdu = true;
                        ApparitionMenu(perdu, enPause);
                        ReinitialisationJeu(x);
                    }
                }
            }

            if (x is Rectangle && (string)x.Tag == "ballesJoueurs" + direction)
            {
                Rect rectBalleJoueur = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                foreach (Rectangle y in monCanvas.Children.OfType<Rectangle>())
                {
                    if ((string)y.Tag == "ennemi")
                    {
                        // création d’un rectangle correspondant à l’ennemi
                        Rect rectEnnemi = new Rect(Canvas.GetLeft(y), Canvas.GetTop(y), y.Width, y.Height);
                        // on vérifie la collision
                        // appel à la méthode IntersectsWith pour détecter la collision
                        if (rectBalleJoueur.IntersectsWith(rectEnnemi))
                        {
                            supprimerObjet.Add(x);
                            supprimerObjet.Add(y);
                            ennemis.Remove(y);
                            nbEnnemis += 1;
                        }
                    }
                }
            }
        }

        private void MinuteurApparitionsEnnemis()
        {
            minuteurApparitionsEnnemis++;
            if (minuteurApparitionsEnnemis % pourcentageMinuteurApparitionsEnnemis == 0)
            {
                CreationEnnemis();
            }
        }

        private void MinuteurDeplacementsEnnemis(Rectangle x)
        {
            minuteurDeplacementsEnnemis++;
            if (minuteurDeplacementsEnnemis % pourcentageMinuteurDeplacementsEnnemis == 0)
            {
                DeplacementsEnnemis(x);
            }
        }
        private void MinuteurDeTirEnnemi()
        {
            minuteurTir++;
            if (minuteurTir % pourcentageMinuteurTir == 0)
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
            }
        }
        private void DeplacementsEnnemis(Rectangle x)
        {
            if (x.Tag is string && (string)x.Tag == "ennemi")
            {
                double joueurX = Canvas.GetLeft(joueur);
                double joueurY = Canvas.GetTop(joueur);
                double ennemiX = Canvas.GetLeft(x);
                double ennemiY = Canvas.GetTop(x);
                double directionX = joueurX - ennemiX;
                double directionY = joueurY - ennemiY;
                double norme = Math.Sqrt(directionX * directionX + directionY * directionY);
                // Normalisez la direction pour assurer une vitesse constante
                directionX /= norme;
                directionY /= norme;

                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseEnnemi * directionX);
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseEnnemi * directionY);
                // Mettez à jour l'angle en fonction de la direction
                angle = (int)(Math.Atan2(directionY, directionX) * 180);
                x.RenderTransform = new RotateTransform(angle, x.Width / 2, x.Height / 2);
            }
        }

        private void DeplacementsMunitions(Rectangle x)
        {
            if ((string)x.Tag == "munitionEnnemi")
            {
                double velocityX = (double)x.GetValue(VelocityXProperty);
                double velocityY = (double)x.GetValue(VelocityYProperty);
                Canvas.SetLeft(x, Canvas.GetLeft(x) + velocityX);
                Canvas.SetTop(x, Canvas.GetTop(x) + velocityY);

                if (Canvas.GetTop(x) > ActualHeight || Canvas.GetTop(x) < 0 || Canvas.GetLeft(x) > ActualWidth || Canvas.GetLeft(x) < 0)
                {
                    supprimerObjet.Add(x);
                    munitionsEnnemi.Remove(x);
                }
            }
        }

        private void SupprimerObjet()
        {
            foreach (Rectangle x in supprimerObjet)
            { 
#if DEBUG
                Console.WriteLine("Nombre d'objets dans la liste supprimerObjet : " + supprimerObjet.Count);
#endif
                monCanvas.Children.Remove(x);
            }


            supprimerObjet.Clear();
        }

        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------
        private void MouvementJoueur()
        {
            if (minuteurImagesMarcheJoueur > 20)
                minuteurImagesMarcheJoueur++;

            if (allerGauche && Canvas.GetLeft(joueur) > rectGauche.Width)
            {
                direction = "G";
                angle = -90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
            }
            //Droite
            if (allerDroite && Canvas.GetLeft(joueur) + joueur.Width < Application.Current.MainWindow.Width - rectDroit.Width)
            {
                direction = "D";
                angle = 90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
            }
            //Haut
            if (allerHaut && Canvas.GetTop(joueur) > rectHaut.Height)
            {
                direction = "H";
                angle = 360;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];

            }
            //Bas
            if (allerBas && Canvas.GetTop(joueur) + joueur.Height < Application.Current.MainWindow.Height - rectBas.Height)
            {
                direction = "B";
                angle = -180;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
                joueur.Fill = animationMarcheJoueur[minuteurImagesMarcheJoueur];
            }
        positionJoueur = new Point(Canvas.GetLeft(joueur), Canvas.GetTop(joueur));
        }

        //----------------------------------------------------------------------
        //--------------------------TOUCHES-------------------------------------

        private void CanvasToucheBaissée(object sender, KeyEventArgs e)
        {
            //touche pour déplacement à gauche
            if (e.Key.ToString() == toucheGauche)
            {
                allerGauche = true;
            }
            //touche pour déplacement à droite
            if (e.Key.ToString() == toucheDroite)
            {
                allerDroite = true;
            }
            //touche pour déplacement en haut
            if (e.Key.ToString() == toucheHaut)
            {
                allerHaut = true;
            }
            //touche pour déplcement en bas
            if (e.Key.ToString() == toucheBas)
            {
                allerBas = true;
            }
            // touche pour mettre en pause
            if (e.Key == Key.Escape)
            {
                enPause = true;
                minuteur.Stop();
                ApparitionMenu(perdu, enPause);
                if (menuPause.DialogResult == true)
                {
                    enPause = false;
                    menuPause.Close();
                    minuteur.Start();
                }
            }
            if (e.Key == Key.NumPad1)
            {
                codeTriche = true;
                Collisions(x, codeTriche);
            }
        }

        private void CanvasToucheLevées(object sender, KeyEventArgs e)
        {
            if (e.Key.ToString() == toucheGauche)
                allerGauche = false;
            if (e.Key.ToString() == toucheDroite)
                allerDroite = false;
            if (e.Key.ToString() == toucheHaut)
                allerHaut = false;
            if (e.Key.ToString() == toucheBas)
                allerBas = false;

            if (e.Key.ToString() == toucheTir)
                GestionTir();
        }

        private void GestionTir()
        {
            audioFond.Pause();
            audioTir.Position = TimeSpan.Zero;
            audioTir.Play();
            audioFond.Play();

            supprimerObjet.Clear();

            if (direction == "G" || direction == "D" || direction == "H" || direction == "B")
                CreerNouvelleBalle();
        }

        private void CreerNouvelleBalle()
        {
            Rectangle nouvelleBalle = new Rectangle
            {
                Tag = "ballesJoueurs" + direction,
                Height = 20,
                Width = 20,
                Fill = munitionsSkin
            };

            Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) + joueur.Height / 2 - nouvelleBalle.Height / 2);
            Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);

            monCanvas.Children.Add(nouvelleBalle);
        }
    }
}
