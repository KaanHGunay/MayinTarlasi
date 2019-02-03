using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MayinTarlasi
{
    /// <summary>
    /// Mayın Tarlası Oyunu / Kaan Han Günay 
    /// Oyunun Özellikleri;
    ///     -Oyun her defasında mayınları rastgele atamaktadır.
    ///     -Mayın alanının büyüklüğü ve mayın sayısı (Mayın Alanının %15 inden az, %90 ından fazla olamaz.) kullanıcı tarafından değiştirilebilmektedir.
    ///     -Çevresinde mayın bulunmayan bölgeye tıklandığında kendisi otomatik olarak çevresini açar.
    ///     -Çevresinde mayın bulunan bölge, ne kadar mayın bulunduğunu üstünde gösterir.
    ///     -Mayın olduğu düşünülen bölgeye sağ tık ile işaretleme yapılabilir.
    ///     -Mayına basma, boş kısma tıklama ve kazanma durumunda ses eklenmiştir.
    ///     -Sesler kullanıcı tarafından açılıp kapatılabilmektedir.
    ///     -Oyununda ilk tıklanan bölgede mayın bulunmaz (Ayarlardan kapatılabilir.)
    ///     -Oyun xml dosyası içine oyunun oynanma ve kazanma sayılarının istatistiklerini tutar.
    ///     -Oyunda bütün mayınları göster seçeneği bulunur (Varsayılan olarak kapalı gelir, ayarlardan açılabilir.).
    ///     -Puan sistemi oyunun boyutu ve mayın sayısına göre değişmektedir.
    ///     -Oyun kazanıldığında veya kaybedildiğinde aynı oyun (mayınların yerlerinin aynı olduğu oyun) tekrar oynanabilir.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) //başla butonuna tıklanması
        {
            clsMT.oynananOyunEkle();
            MayinDoldur(clsMT.verilenMayinSayisi, clsMT.verilenboyut);
        }

        private void MayinDoldur(int mayinSayisi, int boyut) //mayinlari seç ve paneli butonlarla doldur
        {
            if (txtMayinSayisi.Visibility == Visibility.Visible) gorunurlukDegistir();
            if (cbIlkMayin.IsChecked ?? true && !clsMT.ilkTiklamaMi) clsMT.ilkTiklamaMi = true;
            MayinAlani.Children.Clear();
            lblSkor.Content = "Skor: 0";
            clsMT.skor = 0;

            if (!clsMT.ayniOyunOyna)
            {
                clsMT.mayinlar = new int[mayinSayisi];
                Random rnd = new Random();

                for (int i = 0; i < mayinSayisi; i++)  //hangi butonlarda mayın olacağının belirlenmesi
                {
                    int secilen = rnd.Next(0, boyut * boyut);
                    if (clsMT.mayinlar.Contains(secilen))
                    {
                        i--;
                        continue;
                    }
                    clsMT.mayinlar[i] = secilen;
                }
            }

            clsMT.ayniOyunOyna = false;

            for (int i = 0; i < boyut * boyut; i++)
            {
                Button btn = new Button {
                     Background = Brushes.LightBlue,
                     Height = MayinAlani.Height / boyut,
                     Width = MayinAlani.Height / boyut,
                     Margin = new Thickness(0),
                     Tag = clsMT.mayinlar.Contains(i)
                };
                btn.Click += Btn_Click;
                btn.MouseDown += Btn_MouseDown;
                MayinAlani.Children.Add(btn);
            }
        }

        private void Btn_MouseDown(object sender, MouseButtonEventArgs e) //sağtık yapıldığında bölgeyi işaretleme ve kaldırma
        {
            Button tiklanan = (Button)sender;
            if (e.ChangedButton == MouseButton.Right)
            {
                if (tiklanan.Background != Brushes.Gray)
                {
                    tiklanan.Background = Brushes.Gray;
                    btnResimEkle(tiklanan, "bayrak");
                }
                else
                {
                    tiklanan.Background = Brushes.LightBlue;
                    tiklanan.Content = null;
                }
            }
        }

        private void Btn_Click(object sender, RoutedEventArgs e) //mayın bölgesine tıklandığında
        {
            Button tiklanan = (Button)sender;
            if (clsMT.ilkTiklamaMi && (bool)tiklanan.Tag)  //ilk tıklanan bölgede mayın çıkmaması için
            {
                tiklanan.Tag = false;

                Random rnd = new Random();
                int yeniSecilen = rnd.Next(0, clsMT.verilenboyut * clsMT.verilenboyut);
                while (clsMT.mayinlar.Contains(yeniSecilen) || yeniSecilen == MayinAlani.Children.IndexOf(tiklanan))
                {
                    yeniSecilen = rnd.Next(0, clsMT.verilenboyut * clsMT.verilenboyut);
                }
                ((Button)MayinAlani.Children[yeniSecilen]).Tag = true;
                for (int i = 0; i < clsMT.mayinlar.Length; i++)
                {
                    if (clsMT.mayinlar[i] == MayinAlani.Children.IndexOf(tiklanan))
                    {
                        clsMT.mayinlar[i] = yeniSecilen;
                    }
                }
            }

            if ((bool)tiklanan.Tag)
            {
                //tiklanan.Background = Brushes.Red;
                btnResimEkle(tiklanan, "mayin");
                if (clsMT.ses) clsMT.sesBomba.Play();
                oyunBitir();
            }
            else
            {
                //tiklanan.Background = Brushes.Green;
                if (clsMT.ilkTiklamaMi) clsMT.ilkTiklamaMi = false;
                clsMT.skor += clsMT.mayinPuan;
                lblSkor.Content = "Skor: " + clsMT.skor.ToString();
                tiklanan.IsEnabled = false;
                if (cevreMayinSay(tiklanan) > 0) tiklanan.Content = (cevreMayinSay(tiklanan)).ToString();
                if (clsMT.skor == (((clsMT.verilenboyut * clsMT.verilenboyut) - clsMT.verilenMayinSayisi) * clsMT.mayinPuan))
                {
                    if (clsMT.ses) clsMT.sesTada.Play();
                    clsMT.galibiyetEkle();
                    if (wnKazandiniz.sonucDondur())
                    {
                        Button_Click(null, null);
                    }
                    else
                    {
                        ayniOyunuTekrarBaslat();
                    }
                }
                else
                {
                    if (clsMT.ses) clsMT.sesClick.Play();
                    bosMayinlariAc(tiklanan);
                }
            }
        }

        private void oyunBitir() //mayına tıklanınca oyunu bitirir
        {
            foreach (Button item in MayinAlani.Children)
            {
                if ((bool)item.Tag)
                {
                    //item.Background = Brushes.LightBlue;
                    btnResimEkle(item, "mayin");
                }
                else
                {
                    //item.Background = Brushes.LightBlue;
                }
            }
            
            if (wnKaybetme.sonucDondur())
            {
                Button_Click(null, null);
            }
            else
            {
                ayniOyunuTekrarBaslat();
            }
        }
        
        int cevreMayinSay(Button btn)  //mayınsız bir bölgeye tıklandığında çevresinde bulunan mayın sayısını döndürür
        {
            int index = MayinAlani.Children.IndexOf(btn), cevreMayin = 0;
            if (clsMT.sagiVarmi(index) && (bool)((Button)MayinAlani.Children[index + 1]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.soluVarmi(index) && (bool)((Button)MayinAlani.Children[index - 1]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.ustVarmi(index) && (bool)((Button)MayinAlani.Children[index - clsMT.verilenboyut]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.altVarmi(index) && (bool)((Button)MayinAlani.Children[index + clsMT.verilenboyut]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.ustVarmi(index) && clsMT.soluVarmi(index) && (bool)((Button)MayinAlani.Children[index - clsMT.verilenboyut - 1]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.ustVarmi(index) && clsMT.sagiVarmi(index) && (bool)((Button)MayinAlani.Children[index - clsMT.verilenboyut + 1]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.altVarmi(index) && clsMT.soluVarmi(index) && (bool)((Button)MayinAlani.Children[index + clsMT.verilenboyut - 1]).Tag)
            {
                cevreMayin++;
            }
            if (clsMT.altVarmi(index) && clsMT.sagiVarmi(index) && (bool)((Button)MayinAlani.Children[index + clsMT.verilenboyut + 1]).Tag)
            {
                cevreMayin++;
            }
            return cevreMayin;
        }

        private void bosMayinlariAc(Button btn)  //tıklanan bölgedenin çevresinde hiç mayın olmayan bölümlerin otomatik açılmasını sağlar
        {
            int index = MayinAlani.Children.IndexOf(btn);
            if (cevreMayinSay(btn) == 0)
            {
                if (clsMT.ustVarmi(index) && clsMT.soluVarmi(index) && !(bool)(((Button)MayinAlani.Children[index - clsMT.verilenboyut - 1]).Tag) && MayinAlani.Children[index - clsMT.verilenboyut - 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index - clsMT.verilenboyut - 1], null);
                }
                if (clsMT.ustVarmi(index) && !(bool)(((Button)MayinAlani.Children[index - clsMT.verilenboyut]).Tag) && MayinAlani.Children[index - clsMT.verilenboyut].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index - clsMT.verilenboyut], null);
                }
                if (clsMT.ustVarmi(index) && clsMT.sagiVarmi(index) && !(bool)(((Button)MayinAlani.Children[index - clsMT.verilenboyut + 1]).Tag) && MayinAlani.Children[index - clsMT.verilenboyut + 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index - clsMT.verilenboyut + 1], null);
                }
                if (clsMT.soluVarmi(index) && !(bool)(((Button)MayinAlani.Children[index - 1]).Tag) && MayinAlani.Children[index - 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index - 1], null);
                }
                if (clsMT.sagiVarmi(index) && !(bool)(((Button)MayinAlani.Children[index + 1]).Tag) && MayinAlani.Children[index + 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index + 1], null);
                }
                if (clsMT.altVarmi(index) && clsMT.soluVarmi(index) && !(bool)(((Button)MayinAlani.Children[index + clsMT.verilenboyut - 1]).Tag) && MayinAlani.Children[index + clsMT.verilenboyut - 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index + clsMT.verilenboyut - 1], null);
                }
                if (clsMT.altVarmi(index) && !(bool)(((Button)MayinAlani.Children[index + clsMT.verilenboyut]).Tag) && MayinAlani.Children[index + clsMT.verilenboyut].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index + clsMT.verilenboyut], null);
                }
                if (clsMT.altVarmi(index) && clsMT.sagiVarmi(index) && !(bool)(((Button)MayinAlani.Children[index + clsMT.verilenboyut + 1]).Tag) && MayinAlani.Children[index + clsMT.verilenboyut + 1].IsEnabled)
                {
                    Btn_Click(MayinAlani.Children[index + clsMT.verilenboyut + 1], null);
                }
            }
        }
        
        void btnResimEkle(Button btn, string resimAd) //istenilen butonun içine resim atama
        {
            Image img = new Image();
            img.Stretch = Stretch.Fill;
            img.Source = new BitmapImage(new Uri("resimler/" + resimAd + ".png", UriKind.Relative));
            StackPanel stackPnl = new StackPanel();
            stackPnl.Orientation = Orientation.Horizontal;
            stackPnl.Margin = new Thickness(5);
            stackPnl.Children.Add(img);
            btn.Content = stackPnl;
        }
        
        private void txtMayinSayisi_PreviewTextInput(object sender, TextCompositionEventArgs e)  //Mayin sayısı ve boyut textboxlarına sadece sayı girişi yapılabilmesi için
        {
            if (!char.IsDigit(e.Text,e.Text.Length-1))
            {
                e.Handled = true;
            }
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)  //ayarlara tıklanılması
        {
            gorunurlukDegistir();
        }

        private void imgGoz_MouseDown(object sender, MouseButtonEventArgs e)  //bombaları göster
        {
            if (MayinAlani.Children.Count > 0)
            {
                for (int i = 0; i < clsMT.mayinlar.Length; i++)
                {
                    btnResimEkle(((Button)MayinAlani.Children[clsMT.mayinlar[i]]), "mayin");
                } 
            }
        }

        private void imgGoz_MouseUp(object sender, MouseButtonEventArgs e) //gözüken bombaları gizle
        {
            if (MayinAlani.Children.Count > 0)
            {
                for (int i = 0; i < clsMT.mayinlar.Length; i++)
                {
                    if (((Button)MayinAlani.Children[clsMT.mayinlar[i]]).Background != Brushes.Gray)
                    {
                        ((Button)MayinAlani.Children[clsMT.mayinlar[i]]).Content = null; 
                    }
                    else
                    {
                        btnResimEkle(((Button)MayinAlani.Children[clsMT.mayinlar[i]]), "bayrak");
                    }
                } 
            }
        }

        private void imgGoz_MouseLeave(object sender, MouseEventArgs e) //bombalar gözükmesinden sonra mouseup eventi tetiklenmeden mousenin image den ayrılması durumunda mayınların gözükür durumda kalmaması için
        {
            imgGoz_MouseUp(null, null); 
        }

        private void cbMayinGoster_Checked(object sender, RoutedEventArgs e) //mayin gösterme image ının görünür olmasını sağlar
        {
            imgGoz.Visibility = Visibility.Visible;
        }

        private void cbMayinGoster_Unchecked(object sender, RoutedEventArgs e)  //mayin gösterme image ının gizli olmasını sağlar
        {
            imgGoz.Visibility = Visibility.Hidden;
        }

        private void imgIst_MouseDown(object sender, MouseButtonEventArgs e)  //istatistik ekranının açılması
        {
            wnIstatistik ist = new wnIstatistik();
            ist.ShowDialog();
        }

        private void imgSes_MouseDown(object sender, MouseButtonEventArgs e)  //ses açıp kapama
        {
            if (clsMT.ses)
            {
                imgSes.Source = new BitmapImage(new Uri("resimler/ses_kapali.png", UriKind.Relative));
                clsMT.ses = false;
            }
            else
            {
                imgSes.Source = new BitmapImage(new Uri("resimler/ses_acik.png", UriKind.Relative));
                clsMT.ses = true;
            }
        }

        private void Tamam_Click(object sender, RoutedEventArgs e)  //ayarların yapıldıktan sonra yapılan ayarlarla oyunun tekrar başlatılması
        {
            if (txtBoyut.Text != "" && txtMayinSayisi.Text != "")
            {
                int boyut = Int32.Parse(txtBoyut.Text);
                int mayin = Int32.Parse(txtMayinSayisi.Text);

                if (boyut >= 10 && boyut <= 30)
                {
                    clsMT.verilenboyut = boyut;
                }
                else if (boyut < 10)
                {
                    clsMT.verilenboyut = 10;
                }
                else
                {
                    clsMT.verilenboyut = 30;
                }

                if (mayin >= boyut * boyut * 0.15 && mayin <= boyut * boyut * 0.9)
                {
                    clsMT.verilenMayinSayisi = mayin;
                }
                else if (mayin < boyut * boyut * 0.15)
                {
                    clsMT.verilenMayinSayisi = Convert.ToInt32(boyut * boyut * 0.15);
                }
                else
                {
                    clsMT.verilenMayinSayisi = Convert.ToInt32(boyut * boyut * 0.9);
                }

                txtBoyut.Text = null;
                txtMayinSayisi.Text = null;

                gorunurlukDegistir();
                clsMT.verilecekPuanBelirle();

                Button_Click(null, null); 
            }

            else if (txtBoyut.Text == "" && txtMayinSayisi.Text != "") //sadece mayin sayısı verilirse boyut aynı kalarak sadece mayın sayısını değiştir
            {
                txtBoyut.Text = clsMT.verilenboyut.ToString();
                Tamam_Click(null, null);
            }

            else if (txtBoyut.Text != "" && txtMayinSayisi.Text == "")  //sadece boyut verilirse mayın sayısı aynı kalarak sadece boyutu değiştir
            {
                txtMayinSayisi.Text = clsMT.verilenMayinSayisi.ToString();
                Tamam_Click(null, null);
            }

            else
            {
                gorunurlukDegistir();
            }
        }

        void gorunurlukDegistir() //ayarlar kısmı görünüyorsa görünmemesini, görünmüyorsa görünmesini sağlar
        {
            if (txtMayinSayisi.Visibility == Visibility.Visible)
            {
                txtMayinSayisi.Visibility = txtBoyut.Visibility = lblBoyut.Visibility = lblMayinSayisi.Visibility = btnTamam.Visibility = cbMayinGoster.Visibility = cbIlkMayin.Visibility = Visibility.Hidden;
                txtBoyut.Text = txtMayinSayisi.Text = null;
            }
            else
            {
                txtMayinSayisi.Visibility = txtBoyut.Visibility = lblBoyut.Visibility = lblMayinSayisi.Visibility = btnTamam.Visibility = cbMayinGoster.Visibility = cbIlkMayin.Visibility = Visibility.Visible;
                txtMayinSayisi.Focus();
            }
        }
        
        void ayniOyunuTekrarBaslat() //mayın bölgeleri değişmeksizin aynı oyunu tekrar başlatma
        {
            clsMT.ayniOyunOyna = true;
            Button_Click(null, null);
        }
    }
}
