YemekSepeti Projesi - Kurulum ve Çalıştırma Kılavuzu
Proje Adı: Yemek Sepeti – Veritabanı Projesi

1. Proje Hakkında Genel Bilgi: Bu proje, C# programlama dili, Katmanlı Mimari ve Entity Framework Database First yaklaşımı kullanılarak geliştirilmiş bir online yemek sipariş sistemidir. Sistem; Admin, Restoran ve Kullanıcı olmak üzere 3 ana panelden oluşmaktadır.

2. Gerekli Yazılımlar; Projenin sorunsuz çalışması için sisteminizde aşağıdaki yazılımların kurulu olması gerekmektedir:
-Visual Studio 2022 
-SQL Server
-SQL Server Management Studio (SSMS)
-.NET Framework, .NET 8.0 SDK

3.Kullanılan Teknolojiler:
-ASP.NET Core MVC(.NET 8)
-C#
-SQL Server
-Entity Framework
-Bootstrap

4.VERİ TABANI KURULUMU
1-Scripts klasöründeki script dosyalarını sırasıyla çalıştırarak veri tabanını kurabilirsiniz:
Önce SonKezTemizTablolar.sql sonrasında SP_Func_view_trigger.sql (Tek sorgu olarak yaptıklarım hata verdiği için 2 sorgu dosyası olarak ayırdım.)
2-SQL Server Management Studio üzerinden çalıştırın.
3-Veritabanı YemekSepetiProje adıyla oluşturulacaktır.
4-Tüm tablolar, Stored Procedure’ler, Trigger’lar, Fonksiyonlar ve View otomatik olarak oluşur.

5.CONNECTİON STRİNG AYARI
Projedeki bağlantı ayarı appsettings.json dosyasında yapılmaktadır.
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=YemekSepetiProje;Trusted_Connection=True;TrustServerCertificate=True;"
}
Eğer farklı bir SQL Server kullanıyorsanız lütfen Server kısmını güncelleyin.

6.EĞER BAĞLANTI HATASI ALIRSANIZ:
Connection String içinde Database adının doğru olduğundan emin olun,
“TrustServerCertificate=True” parametresinin olduğunu kontrol edin.

7.PROJENİN ÇALIŞTIRILMASI
1-Visual Studio’da projeyi açın.
2-Çözüm Gezgini üzerinden YemekSepeti.WebUI projesini Startup Project yapın.
3-Run/https (F5) tuşuna basarak projeyi çalıştırın.
4-Sistem tarayıcıda açılacaktır.

8.SİSTEM ROLLERİ VE PANELLERİ

-Admin Paneli
Ürünler, restoranlar, kullanıcılar ve kategorilerin yönetimi yapılır.
EMAİL:Admin@sistem.com
ŞİFRE:123456

-Restoran Sahibi Paneli
Gelen siparişler, Ürünlerim, Restoran Bilgilerim, Rapor ve yorumlarım kısımlarından oluşur ve üzerlerinde işlemler yapılabilir.
Her restorana özel email ve şifre vardır, bir örnek panel için:
EMAİL:BurgerKing@rest.com
ŞİFRE:123456

-Kullanıcı Paneli
1.ANASAYFA: Burada kullanıcı ana sayfa da mutfakları görüntüleyebilir seçtiği mutfağa göre restoranlar listelenir veya mutfak seçmeden tüm restoranları görüntüleyebilir. Her restoran üzerinde kalp sembolu vardır eğer içi dolu değilse favoriye ekleyebilir doluysa zaten favorilerdedir. 
2.NAVBAR(üST MENÜ): Navbar kısmında kullanıcı email kısmı vardır orda kullanıcı hesap bilgilerini görebilir değiştirebilir, siparişlerini takip edebilir detaylı bir şekilde açabilir. Yine navbar da Kalp simgesi ve Sepet simgesi bulunur. Kalp simgesinde kullanıcının eklediği favori restoranlar listelenir. Sepet simgesinde ise kullanıcının sepete eklediği ürünler listelenir burda ürün birim fiyatı, adet arttırımı, toplam fiyatı, birim ürün silme veya tamamen sepeti temizleme ve siparişi tamamlama butonları bulunur.
SİPARİŞ TAMAMLAMA: Siparişi tamamlaya basılınca eğer kullanıcı giriş yapmamışsa ilk giriş yapması istenir sonrasında kullanıcıyı teslimat ve ödeme sayfasına yönlendirilir ve kullanıcıdan adres ve kart bilgisi istenir ve ödeme yapılır. 
SİPARİS TAKİBİ: Kullanıcı verdiği siparişi, siparişlerim kısmından takip edebilir durum kolonu altında siparişin ne durumda olduğu gösterilir eğer kullanıcı sipariş iptali isterse bu durumu sadece "onay bekleniyor" durumundayken yapabilir.
YORUM YAPMA: Sipariş "teslimEdildi" durumuna geçtiğinde ise kullanıcı yine siparişlerim kısmında Yorum yap butonuna sahip olur ve bu sayede restorana yorum yazabilir ve puan verebilir.


