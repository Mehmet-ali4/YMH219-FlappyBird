namespace FlappyBird;

/// <summary>
/// En yüksek skoru bir .txt dosyasına yazar ve okur.
/// Dosya yoksa veya bozuksa uygulama çökmez, 0 ile devam edilir.
/// </summary>
public class HighScoreStorage
{
    public const string DefaultFileName = "highscore.txt";

    /// <summary>Dosyanın tam yolu (uygulamanın çalıştığı klasörde, göreli olarak oluşturulur).</summary>
    public string FilePath { get; }

    /// <summary>Son işlemde oluşan hata mesajı; hata yoksa null.</summary>
    public string? LastError { get; private set; }

    public HighScoreStorage(string fileName = DefaultFileName)
    {
        // Mutlak yol yazılmaz: dosya, exe'nin bulunduğu klasörde tutulur.
        FilePath = Path.Combine(AppContext.BaseDirectory, fileName);
    }

    /// <summary>En yüksek skoru dosyadan okur. Sorun olursa 0 döndürür.</summary>
    public int Load()
    {
        LastError = null;
        try
        {
            if (!File.Exists(FilePath))
            {
                return 0; // İlk çalıştırma: dosya henüz yok, bu bir hata değil.
            }

            string text = File.ReadAllText(FilePath).Trim();
            if (int.TryParse(text, out int value) && value >= 0)
            {
                return value;
            }

            LastError = "En yüksek skor dosyasının içeriği geçersiz. Skor 0 olarak kabul edildi.";
            return 0;
        }
        catch (IOException ex)
        {
            LastError = "En yüksek skor dosyası okunamadı: " + ex.Message;
            return 0;
        }
        catch (UnauthorizedAccessException ex)
        {
            LastError = "En yüksek skor dosyasına erişim izni yok: " + ex.Message;
            return 0;
        }
    }

    /// <summary>En yüksek skoru dosyaya yazar. Başarılıysa true döndürür.</summary>
    public bool Save(int highScore)
    {
        LastError = null;
        try
        {
            File.WriteAllText(FilePath, highScore.ToString());
            return true;
        }
        catch (IOException ex)
        {
            LastError = "En yüksek skor kaydedilemedi: " + ex.Message;
            return false;
        }
        catch (UnauthorizedAccessException ex)
        {
            LastError = "En yüksek skor dosyasına yazma izni yok: " + ex.Message;
            return false;
        }
    }
}
