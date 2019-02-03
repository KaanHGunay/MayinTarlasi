using System.Windows;
using System.Windows.Input;

namespace MayinTarlasi
{
    /// <summary>
    /// Oyunun kaybedilmesi üzerine gelecek window
    /// </summary>
    public partial class wnKaybetme : Window
    {
        public wnKaybetme()
        {
            InitializeComponent();
        }

        private static bool sonuc;

        public static bool sonucDondur()
        {
            wnKaybetme w = new wnKaybetme();
            w.ShowDialog();
            return sonuc;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ImgTekrar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sonuc = true;
            this.Close();
        }

        private void ImgAyniTekrar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sonuc = false;
            this.Close();
        }

        private void ImgCik_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
