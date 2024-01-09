﻿using System;
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
        //déplacement gauche/droite
        private bool allerGauche, allerDroite = false;
        //déplacement haut/bas
        private bool allerHaut, allerBas = false;
        //vitesse du joueur
        private int VitesseJoueur = 7;
        // vitesse du tir du joueur
        private int vitesseBallesJoueurs = 10;

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
                //joueurSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "images/Statique/tortue_statique.png"));
                Rectangle nouveauJoueur = new Rectangle
                {
                    Tag = "joueur",
                    Height = 150,
                    Width = 150,
                    Fill = Brushes.Red,
                };
                Canvas.SetTop(nouveauJoueur, 350);
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
                Rect bullet = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);
            }
            if ((string)x.Tag == "ballesJoueurs" && )
            {

            }
        }
        private void GameEngine(object sender, EventArgs e)
        {
            CreationJoueur(1);
            foreach (Rectangle x in monCanvas.Children.OfType<Rectangle>())
            {
                MouvementJoueur(x, joueur);
            }
        }
        //------------------------------------------------------------------------
        //-------------------------MOUVEMENTS-------------------------------------

        private void MouvementJoueur(Rectangle x, Rect joueur)
        {
            if (x is Rectangle && (string)x.Tag == "joueur" && allerGauche && Canvas.GetLeft(x) > 0)
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) - vitesseJoueur);
            }
            else if (x is Rectangle && (string)x.Tag == "joueur" && allerDroite && Canvas.GetLeft(x) + joueur.Width < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) + vitesseJoueur);
            }
            /*if (x is Rectangle && (string)x.Tag == "joueur" && allerHaut && Canvas.GetTop(x) > 0)
            {
                Canvas.SetTop(x, Canvas.GetTop(x) - vitesseJoueur);
            }
            else if (x is Rectangle && (string)x.Tag == "joueur" && allerBas && Canvas.GetTop(x) + joueur.Height < Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(x, Canvas.GetTop(x) + vitesseJoueur);
            }*/
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
