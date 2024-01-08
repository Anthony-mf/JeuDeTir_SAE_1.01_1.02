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
        private int totalPlayer;
        public MainWindow()
        {
            InitializeComponent();
            MakePlayer(1);
        }

        private void MakePlayer(int nbPlayer)
        {
            int left = 400;
            totalPlayer = nbPlayer;
            for (int i = 0; i < nbPlayer; i++)
            {
                ImageBrush playerSkin = new ImageBrush();
                playerSkin.ImageSource = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "img/statique/tortue_statique.png"));

                Rectangle newPlayer = new Rectangle
                {
                    Tag = "player",
                    Height = 65,
                    Width = 55,
                    Fill = playerSkin,
                };
                Canvas.SetTop(newPlayer, 350);
                Canvas.SetLeft(newPlayer, left);
                monCanvas.Children.Add(newPlayer);
                left -= 100;
            }
        }
    }
}
