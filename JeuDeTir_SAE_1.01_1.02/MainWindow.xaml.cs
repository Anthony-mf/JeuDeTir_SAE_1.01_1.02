﻿using System;
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
        //direction
        private string direction;
        

        public MainWindow()
        {
            InitializeComponent();
            //montrer le menu
            Menu menu = new Menu();
            menu.ShowDialog();
            //menu.Owner = this;
            //if (menu.DialogResult == false)
                //Application.Current.Shutdown();

            monCanvas.Focus();
            ImageBrush fondWPF = new ImageBrush();
            fondWPF.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondCanvas.jpg"));
            monCanvas.Background = fondWPF;
            ImageBrush fondMenu = new ImageBrush();
            fondMenu.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/statique/FondMenu.png"));
            ImageBrush joueurSkin = new ImageBrush();
            joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/Statique/tortue_statique.png"));
            joueur.Fill = joueurSkin;
            //MenuCanvas.Background = fondMenu;
            //Menu menu = new Menu();
            //menu.ShowDialog();
            
            
             minuterie.Tick += GameEngine;
             // rafraissement toutes les 16 milliseconds
             minuterie.Interval = TimeSpan.FromMilliseconds(16);
             minuterie.Start();
                
            
            
            
        }

        //---------------------------------------------------------------------------
        //-----------------------------TIRS------------------------------------------

        private void TestBallesAllié(Rectangle x)
        {
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_H")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseBallesJoueurs);//monte
                //création rectangle pour une balle
                Rect balles = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //pour delete les balles jsp comment faire
               
            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_B")
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseBallesJoueurs);//descend
                //création rectangle pour une balle
                Rect balles = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //pour delete les balles jsp comment faire
           
            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_D")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseBallesJoueurs);//droite
                //création rectangle pour une balle
                Rect balles = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //pour delete les balles jsp comment faire
               
            }
            if (x is Rectangle && (string)x.Tag == "ballesJoueurs_G")
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseBallesJoueurs);//gauche
                //création rectangle pour une balle
                Rect balles = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
                //pour delete les balles jsp comment faire
             
            }

        }
        private void GameEngine(object sender, EventArgs e)
        {
            Rect joueur1 = new Rect(Canvas.GetLeft(joueur), Canvas.GetTop(joueur),
            joueur.Width, joueur.Height);
            MouvementJoueur();

            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                TestBallesAllié(x);
              
            }
            SupprimerObjetASupprimer();
        }

        private void SupprimerObjetASupprimer()
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
           
            if (allerGauche && Canvas.GetLeft(joueur) > 0)
            {
                direction = "G";
                int angle = -90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
            }
            //droite
            else if (allerDroite && Canvas.GetLeft(joueur) + joueur.Width < Application.Current.MainWindow.Width)
            {
                direction = "D";
                int angle = 90;
                Canvas.SetLeft(joueur, Canvas.GetLeft(joueur) + vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);
            }
            if (allerHaut && Canvas.GetLeft(joueur) > 0)
            {
                direction = "H";
                int angle = 360;
                Canvas.SetTop(joueur, Canvas.GetTop(joueur) - vitesseJoueur);
                joueur.RenderTransform = new RotateTransform(angle, joueur.Width / 2, joueur.Height / 2);

            }
            //bas
            else if (allerBas && Canvas.GetTop(joueur) + joueur.Height < Application.Current.MainWindow.Height)
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
                // clear des objets à suppr
                supprimerObjet.Clear();
               

                if (direction == "G")
                {
                    Rectangle nouvelleBalle = new Rectangle
                    {
                        Tag = "ballesJoueurs_G",
                        Height = 5,
                        Width = 20,
                        Fill = Brushes.White,
                        Stroke = Brushes.Red
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) - nouvelleBalle.Height);
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
                        Stroke = Brushes.Red
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) - nouvelleBalle.Height);
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
                        Stroke = Brushes.Red
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) - nouvelleBalle.Height);
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
                        Stroke = Brushes.Red
                    };
                    // on place le tir à l’endroit du joueur
                    Canvas.SetTop(nouvelleBalle, Canvas.GetTop(joueur) - nouvelleBalle.Height);
                    Canvas.SetLeft(nouvelleBalle, Canvas.GetLeft(joueur) + joueur.Width / 2);
                    // on place le tir dans le canvas
                    monCanvas.Children.Add(nouvelleBalle);
                }

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
