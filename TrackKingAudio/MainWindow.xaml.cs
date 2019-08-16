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

namespace TrackKingAudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TrackButton_Click(object sender, RoutedEventArgs e)
        {
            SolidColorBrush btnPressedBrush = new SolidColorBrush(Color.FromArgb(221, 221, 221, 100));
            btnTrack.Foreground = btnPressedBrush;
            lblMiddle.Content = "Track Admin";
            lblTypeTitle.Content = "Tracks";
        }

        private void TxtCatagory_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
