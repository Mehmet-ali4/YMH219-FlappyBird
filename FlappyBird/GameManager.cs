namespace FlappyBird;

/// <summary>
/// Oyunun kurallarını ve durumunu yönetir: kuş, borular, skor, seviye, hız, çarpışma.
/// Form'dan bağımsızdır; Form yalnızca olayları (event) dinleyip arayüzü günceller.
/// </summary>
public class GameManager
{
    // --- Sabitler ---
    public const int PipeCount = 3;        // Aynı anda kullanılan boru sayısı
    public const int PipeSpacing = 240;    // İki boru arasındaki yatay mesafe
    public const int BirdStartX = 80;      // Kuşun sabit yatay konumu
    public const int InitialSpeed = 4;     // Başlangıç boru hızı (piksel/kare)
    public const int SpeedIncrease = 1;    // Her seviyede hız artışı
    public const int MaxSpeed = 8;         // Hızın üst sınırı
    public const int ScorePerLevel = 5;    // Kaç skorda bir seviye atlanacağı

    private readonly int playAreaWidth;
    private readonly int playAreaHeight;
    private readonly HighScoreStorage highScoreStorage;
    private readonly Random random = new Random();
    private readonly List<Pipe> pipes = new List<Pipe>();

    // --- Olaylar (Form bu olayları dinleyerek ekranı günceller) ---
    public event EventHandler? ScoreChanged;
    public event EventHandler? LevelChanged;
    public event EventHandler? StateChanged;
    public event EventHandler<string>? ErrorOccurred;

    // --- Oyun durumu özellikleri (dışarıdan yalnızca okunabilir) ---
    public int Score { get; private set; }
    public int HighScore { get; private set; }
    public int Level { get; private set; }
    public int Speed { get; private set; }
    public GameState State { get; private set; }
    public bool IsNewHighScore { get; private set; }

    public Bird Bird { get; }
    public IReadOnlyList<Pipe> Pipes => pipes;

    /// <param name="playAreaWidth">Oyun alanı genişliği.</param>
    /// <param name="playAreaHeight">Oyun alanı yüksekliği (zeminin üst kenarı).</param>
    public GameManager(int playAreaWidth, int playAreaHeight, HighScoreStorage highScoreStorage)
    {
        this.playAreaWidth = playAreaWidth;
        this.playAreaHeight = playAreaHeight;
        this.highScoreStorage = highScoreStorage;

        Bird = new Bird(BirdStartX, playAreaHeight / 2);
        for (int i = 0; i < PipeCount; i++)
        {
            pipes.Add(new Pipe(playAreaHeight, random));
        }

        HighScore = highScoreStorage.Load();
        Restart();
    }

    /// <summary>Form'a eklenecek tüm oyun kontrollerini (kuş ve borular) döndürür.</summary>
    public IEnumerable<Control> GetControls()
    {
        yield return Bird.View;
        foreach (Pipe pipe in pipes)
        {
            yield return pipe.TopPart;
            yield return pipe.BottomPart;
        }
    }

    /// <summary>Hazır durumdaki oyunu başlatır.</summary>
    public void Start()
    {
        if (State != GameState.Ready)
        {
            return;
        }

        SetState(GameState.Playing);
        Bird.Flap(); // İlk SPACE aynı zamanda ilk zıplamadır.
    }

    /// <summary>SPACE tuşuna basıldığında çağrılır.</summary>
    public void Flap()
    {
        if (State == GameState.Ready)
        {
            Start();
        }
        else if (State == GameState.Playing)
        {
            Bird.Flap();
        }
        // GameOver durumunda SPACE bir şey yapmaz (R ile yeniden başlanır).
    }

    /// <summary>Oyun döngüsünün bir adımı (Timer her tetiklendiğinde çağrılır).</summary>
    public void Update()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        Bird.Update();
        foreach (Pipe pipe in pipes)
        {
            pipe.Update();
        }

        RepositionOffScreenPipes();
        UpdateScore();

        if (CheckCollision())
        {
            EndGame();
        }
    }

    /// <summary>Oyunu başlangıç durumuna döndürür (Ready).</summary>
    public void Restart()
    {
        Score = 0;
        Level = 1;
        IsNewHighScore = false;
        SetSpeed(InitialSpeed);
        Bird.Reset();

        // Borular ekranın sağından başlayarak eşit aralıklarla dizilir.
        for (int i = 0; i < pipes.Count; i++)
        {
            pipes[i].Reposition(playAreaWidth + i * PipeSpacing);
        }

        ScoreChanged?.Invoke(this, EventArgs.Empty);
        LevelChanged?.Invoke(this, EventArgs.Empty);
        SetState(GameState.Ready);
    }

    /// <summary>Oyunu bitirir; gerekiyorsa en yüksek skoru günceller ve kaydeder.</summary>
    public void EndGame()
    {
        if (State != GameState.Playing)
        {
            return;
        }

        IsNewHighScore = Score > HighScore;
        if (IsNewHighScore)
        {
            HighScore = Score;
        }

        // Önce durum değişir (Form timer'ı durdurur), sonra dosya işlemi yapılır.
        SetState(GameState.GameOver);

        if (IsNewHighScore && !highScoreStorage.Save(HighScore))
        {
            ErrorOccurred?.Invoke(this, highScoreStorage.LastError ?? "En yüksek skor kaydedilemedi.");
        }
    }

    /// <summary>Ekranın solundan çıkan boruyu en sağdaki borunun arkasına taşır.</summary>
    private void RepositionOffScreenPipes()
    {
        foreach (Pipe pipe in pipes)
        {
            if (pipe.IsOffScreen)
            {
                int rightmostX = pipes.Max(p => p.X);
                pipe.Reposition(rightmostX + PipeSpacing);
            }
        }
    }

    /// <summary>Kuşun sol kenarını geçen her boru için skoru bir kez artırır.</summary>
    private void UpdateScore()
    {
        foreach (Pipe pipe in pipes)
        {
            if (!pipe.Passed && pipe.Right < Bird.Bounds.Left)
            {
                pipe.Passed = true;
                Score++;
                ScoreChanged?.Invoke(this, EventArgs.Empty);
                UpdateLevel();
            }
        }
    }

    /// <summary>Her ScorePerLevel skorda seviyeyi ve hızı artırır (MaxSpeed sınırıyla).</summary>
    private void UpdateLevel()
    {
        int newLevel = Score / ScorePerLevel + 1;
        if (newLevel == Level)
        {
            return;
        }

        Level = newLevel;
        SetSpeed(Math.Min(InitialSpeed + (Level - 1) * SpeedIncrease, MaxSpeed));
        LevelChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Kuş tavana, zemine veya bir boruya çarptı mı?</summary>
    private bool CheckCollision()
    {
        Rectangle birdBounds = Bird.Bounds;

        if (birdBounds.Top <= 0 || birdBounds.Bottom >= playAreaHeight)
        {
            return true; // Tavan veya zemin
        }

        foreach (Pipe pipe in pipes)
        {
            if (pipe.CollidesWith(birdBounds))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>Oyun hızını ve tüm boruların hızını birlikte ayarlar.</summary>
    private void SetSpeed(int speed)
    {
        Speed = speed;
        foreach (Pipe pipe in pipes)
        {
            pipe.Speed = speed;
        }
    }

    private void SetState(GameState newState)
    {
        State = newState;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}
