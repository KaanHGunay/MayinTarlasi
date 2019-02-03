using System.Windows;
using System.Windows.Input;

namespace MayinTarlasi
{
    /// <summary>
    /// Oyunun başarı ile tamamlanmasından sonra gelecek ekran
    /// </summary>
    public partial class wnKazandiniz : Window
    {
        public wnKazandiniz()
        {
            InitializeComponent();
        }

        private static bool sonuc;

        public static bool sonucDondur()
        {
            wnKazandiniz w = new wnKazandiniz();
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

        private void ImgAyniOyun_MouseDown(object sender, MouseButtonEventArgs e)
        {
            sonuc = false;
            this.Close();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
