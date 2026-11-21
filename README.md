# Flappy Bird Benzeri Masaüstü Oyunu (YMH 219)

Bu proje, **Nesne Tabanlı Programlama (YMH 219)** dersi kapsamında C# ve Windows Forms kullanılarak geliştirilmiş, Flappy Bird benzeri 2 boyutlu bir masaüstü oyunudur. Oyuncu, `SPACE` tuşuyla zıplattığı kuşu sağdan sola doğru hareket eden boru engellerinin arasından geçirerek en yüksek skoru elde etmeye çalışır. Kuş görseli gömülü kaynak (`Properties.Resources`) olarak projeye dahil edilmiş; borular, arka plan ve zemin ise GDI+ ile çalışma zamanında çizilmiştir.

---

## Kullanılan Teknolojiler ve NuGet Paketleri

- **Programlama Dili:** C#
- **Çatı (Framework):** .NET 10.0 (`net10.0-windows`)
- **Arayüz Teknolojisi:** Windows Forms (WinForms)
- **Geliştirme Ortamı:** Visual Studio 2022 / .NET SDK
- **NuGet Paketleri:** Harici paket yok (yalnızca standart .NET / Windows Forms kütüphaneleri kullanılmıştır).

---

## Kurulum ve Çalıştırma Adımları

Projede harici dosya, görsel veya mutlak dosya yolu (`C:\...`) bulunmadığından başka bir bilgisayara klonlandığında ek ayar gerektirmeden doğrudan çalışır.

### 1. Depoyu Klonlama
```bash
git clone https://github.com/Mehmet-ali4/YMH219-FlappyBird.git
cd YMH219-FlappyBird
```

### 2. Visual Studio ile Çalıştırma (Önerilen)
1. Klasör içindeki **`YMH219-FlappyBird.sln`** çözüm dosyasını Visual Studio ile açın.
2. Klavyeden **`F5`** tuşuna basın (veya üst menüden **Start / Başlat** butonuna tıklayın).

### 3. Komut Satırı (.NET CLI) ile Çalıştırma
```bash
dotnet build YMH219-FlappyBird.sln
dotnet run --project FlappyBird/FlappyBird.csproj
```

---

## Uygulamanın Temel Özellikleri

- **Fizik ve Kuş Kontrolü (`Bird`):** `SPACE` tuşuyla yukarı yönlü anlık hız (`FlapStrength`) kazanan ve her karede yer çekimi (`Gravity`) etkisiyle aşağı hızlanan kuş mekaniği.
- **Sonsuz Engel Döngüsü (`Pipe`):** Sağdan sola hareket eden 3 adet üst-alt boru çifti. Ekranın solundan çıkan her boru, en sağdaki borunun arkasına eşit yatay aralıkla (`PipeSpacing`) ve oynanabilir sınırlar içinde rastgele boşluk yüksekliğiyle yeniden konumlandırılır.
- **Çarpışma Kontrolü ve Game Over:** Kuşun borulara, zemine veya tavana çarpması `Rectangle.IntersectsWith` ve sınır kontrolleriyle tespit edilir; çarpışma anında oyun döngüsü durur ve **GAME OVER** ekranı gösterilir.
- **Skor, Seviye ve Hız Artışı (`GameManager`):** Geçilen her boru `Passed` bayrağı sayesinde skoru yalnızca bir kez artırır. Her **5 skorda bir** seviye (`Level`) yükselir ve boru hızı (`Speed`) üst sınıra (`MaxSpeed`) kadar kademeli olarak artar.
- **En Yüksek Skor Kaydı (`HighScoreStorage`):** En yüksek skor, uygulamanın çalıştığı dizindeki `highscore.txt` dosyasına kaydedilir ve açılışta okunur. Dosya yoksa veya bozuksa uygulama `try/catch` ile çökmeden `0` skoruyla devam eder.
- **Yeniden Başlatma:** Oyun herhangi bir anda **`R`** kısayol tuşu veya ekrandaki **"Yeni Oyun (R)"** butonu ile sıfırlanabilir.
- **Titremesiz Arayüz ve Odak Yönetimi:** Form üzerinde `DoubleBuffered = true` ve `KeyPreview = true` aktif edilmiş, buton `TabStop = false` yapılarak `SPACE` tuşunun butonu tetiklemesi engellenmiştir.



## Görseller ve Kaynaklar

- **Kuş Sprite'ı (`Resources/bird.png`):** `Properties/Resources.resx` üzerinden **Embedded Resource (Gömülü Kaynak)** olarak derlemeye dahil edilmiştir (`Properties.Resources.bird`). Mutlak dosya yolu veya harici klasör bağımlılığı yoktur; proje başka bilgisayarda klonlandığında doğrudan çalışır. Görseldeki dış arka plan rengi köşelerden flood-fill yöntemiyle şeffaflaştırılıp kenar boşlukları kırpılır ve özel `PixelPictureBox` kontrolüyle (`InterpolationMode.NearestNeighbor`, `PixelOffsetMode.Half`) bulanıklaşmadan çizilir.
- **Borular, Arka Plan ve Zemin (`GameRenderer`):** Harici resim dosyası kullanılmadan GDI+ (`Graphics`, `LinearGradientBrush`) ile kodla çizilmiştir:
  - **Borular:** Yatay açık-koyu yeşil degrade gövde (`#73BF2E`, `#9CE659`), boşluğa bakan uçlarda daha geniş boru ağzı (cap) ve koyu çerçeve (`#543847`).
  - **Arka Plan:** Gökyüzü mavisi (`#4EC0CA`) üzerinde kodla çizilmiş ufuk bulutları, basit şehir silueti ve çalı şeridi.
  - **Zemin:** Üstte diyagonal şeritli yeşil çimen kuşağı, altta kum rengi (`#DED895`) taban.
- **Ses Efektleri:** Ödev kapsamında harici ses dosyası kullanılmamıştır.

---

## Bilinen Sınırlamalar ve Geliştirme Notları

- Harici ses dosyası kullanılmamıştır; kuş görseli gömülü kaynak olarak eklenmiş, diğer görsel öğeler GDI+ ile kodla çizilmiştir.
- Çarpışma kontrolü dikdörtgen tabanlıdır (`Rectangle.IntersectsWith`).
- Form boyutu, oyun alanının ve boru aralıklarının tutarlı kalması için `480x650` piksel olarak sabitlenmiştir (`FormBorderStyle.FixedSingle`, `MaximizeBox = false`).
- `highscore.txt` dosyası çalışma zamanında otomatik oluşturulur ve `.gitignore` ile sürüm kontrolü dışında tutulur.

---

## Öğrenci Bilgileri

- **Ad Soyad:** [Mehmet Ali KIRAÇÇAKALI]
- **Öğrenci Numarası:** [250541069]
- **Ders:** YMH 219 - Nesne Tabanlı Programlama
