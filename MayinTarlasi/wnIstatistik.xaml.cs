using System;
using System.Windows;
using System.Windows.Input;
using System.Xml;

namespace MayinTarlasi
{
    /// <summary>
    /// Oyunun oynama ve kazanma istatistiklerinin gösterileceği window
    /// </summary>
    public partial class wnIstatistik : Window
    {
        public wnIstatistik()
        {
            InitializeComponent();
            lbloyunSayisi.Content = "Toplam oynama : " + oynananOyunSayisi().ToString();
            lblkazanma.Content = "Kazanma : " + galibiyetSayisi().ToString();
            lblyuzdelik.Content = "Kazanma Yüzdesi: %" + galibiyetYuzdesi().ToString();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private int oynananOyunSayisi()  //oyunun toplamda kaç kere oynandığı
        { 
            int toplamOyun = 0;
            try
            {
                XmlTextReader reader = new XmlTextReader("istatistikler.xml");
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element && reader.Name == "istatistik")
                    {
                        reader.MoveToAttribute("oyunSayisi");
                        toplamOyun = Convert.ToInt32(reader.Value);
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                clsMT.yeniXmlOlustur();
            }
            return toplamOyun;
        }
        
        private int galibiyetSayisi()  //oyunun kaz kere kazanıldığı
        { 
            int galibiyet = 0;
            XmlTextReader reader = new XmlTextReader("istatistikler.xml");
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Name == "istatistik")
                {
                    reader.MoveToAttribute("galibiyetSayisi");
                    galibiyet = Convert.ToInt32(reader.Value);
                }
            }
            reader.Close();
            return galibiyet;
        }

        private double galibiyetYuzdesi()  //oyunda ne kadar başarılı olunduğunun yüzdelik olarak ifade edilmesi
        { 
            if (galibiyetSayisi() == 0) return 0;

            return 100 * galibiyetSayisi() / oynananOyunSayisi();
        }

        private void btnCik_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
