using System;
using System.Media;
using System.Xml;
using System.Xml.Linq;

namespace MayinTarlasi
{
    public class clsMT
    {
        public static int skor = 0, verilenMayinSayisi = 20, verilenboyut = 10, mayinPuan = 2;
        public static SoundPlayer sesBomba = new SoundPlayer(Properties.Resources.bomba);
        public static SoundPlayer sesClick = new SoundPlayer(Properties.Resources.click);
        public static SoundPlayer sesTada = new SoundPlayer(Properties.Resources.tada);
        public static bool ilkTiklamaMi = false, ayniOyunOyna = false, ses = true;
        public static int[] mayinlar;

        public static bool sagiVarmi(int index) //tıklanan bölgenin mayın bölgesinin en sağında olup olmadığının kontrolü
        {
            if ((index + 1) % clsMT.verilenboyut == 0)
            {
                return false;
            }
            return true;
        }

        public static bool soluVarmi(int index) //tıklanan bölgenin mayın bölgesinin en solunda olup olmadığının kontrolü
        {
            if (index % clsMT.verilenboyut == 0)
            {
                return false;
            }
            return true;
        }

        public static bool ustVarmi(int index)  //tıklanan bölgenin mayın bölgesinin en üstünde olup olmadığının kontrolü
        {
            if (index < clsMT.verilenboyut)
            {
                return false;
            }
            return true;
        }

        public static bool altVarmi(int index)  //tıklanan bölgenin mayın bölgesinin en altında olup olmadığının kontrolü
        {
            if (index >= ((clsMT.verilenboyut * clsMT.verilenboyut) - clsMT.verilenboyut) && index < clsMT.verilenboyut * clsMT.verilenboyut)
            {
                return false;
            }
            return true;
        }

        public static void verilecekPuanBelirle()  //mayın olmayan her bölgeye tıklandığında verilecek puanın mayın sayısına göre değişmesini sağlar
        {
            if (verilenMayinSayisi >= Convert.ToInt32(verilenboyut * verilenboyut * 0.15) && verilenMayinSayisi < Convert.ToInt32(verilenboyut * verilenboyut * 0.3))
            {
                mayinPuan = 2;
            }

            else if (verilenMayinSayisi >= Convert.ToInt32(verilenboyut * verilenboyut * 0.3) && verilenMayinSayisi < Convert.ToInt32(verilenboyut * verilenboyut * 0.5))
            {
                mayinPuan = 4;
            }

            else if (verilenMayinSayisi >= Convert.ToInt32(verilenboyut * verilenboyut * 0.5) && verilenMayinSayisi < Convert.ToInt32(verilenboyut * verilenboyut * 0.8))
            {
                mayinPuan = 8;
            }

            else
            {
                mayinPuan = 15;
            }
        }

        public static void oynananOyunEkle() //oyun her başlatıldığında oynanan oyun sayısını istatistiklere ekler
        {
            try //istatistiklerin tutulduğu xml dosyasının silinmesi durumunda tekrar oluşturulması
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("istatistikler.xml");
                XmlNode node = doc.SelectSingleNode("istatistik");
                node.Attributes["oyunSayisi"].Value = (Convert.ToInt32(node.Attributes["oyunSayisi"].Value) + 1).ToString();
                doc.Save("istatistikler.xml");
            }
            catch (Exception)
            {
                yeniXmlOlustur();
                oynananOyunEkle();
            }
        }

        public static void galibiyetEkle() //oyunun kazanılmasından sonra galibiyete bir eklenmesi
        {
            try  //oyun bitirildiği anda istatistik xml ine ulaşılamaması durumunda tekrar oluşturulması
            {
                XmlDocument doc = new XmlDocument();
                doc.Load("istatistikler.xml");
                XmlNode node = doc.SelectSingleNode("istatistik");
                node.Attributes["galibiyetSayisi"].Value = (Convert.ToInt32(node.Attributes["galibiyetSayisi"].Value) + 1).ToString();
                doc.Save("istatistikler.xml");
            }
            catch (Exception)
            {
                yeniXmlOlustur();
                oynananOyunEkle();
                galibiyetEkle();
            }
        }

        public static void yeniXmlOlustur()  //istatistikler.xml dosyasının bulunamaması durumunda tekrar oluşturulması
        {
            XDocument doc = new XDocument(
                new XElement("istatistik",
                    new XAttribute("oyunSayisi", "0"),
                    new XAttribute("galibiyetSayisi", "0")));
            doc.Save("istatistikler.xml");
        }
    }
}
