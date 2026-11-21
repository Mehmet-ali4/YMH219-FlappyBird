namespace FlappyBird;

/// <summary>
/// Oyuncunun kontrol ettiği kuş. Yer çekimiyle düşer, Flap() ile yukarı zıplar.
/// </summary>
public class Bird
{
    // --- Sabitler (magic number kullanmamak için) ---
    public const int BirdWidth = 38;         // Kuşun ekran genişliği (sprite en-boy oranına göre)
    public const int BirdHeight = 28;        // Kuşun ekran yüksekliği
    public const int BirdSize = BirdHeight;  // Dikey boyut sabiti
    public const int HitboxInset = -3;       // Adil çarpışma için hitbox daraltma payı (piksel)
    public const float Gravity = 0.6f;       // Her karede dikey hıza eklenen yer çekimi
    public const float FlapStrength = -8f;   // Zıplamada verilen dikey hız (negatif = yukarı)
    public const float MaxFallSpeed = 10f;   // Düşüş hızının üst sınırı

    // --- Eğim (görsel efekt) sabitleri ---
    private const float TiltFactor = 3.0f;   // Hıza bağlı eğim çarpanı
    private const float MaxUpTilt = -22f;    // Yükselirken en fazla yukarı eğim (derece)
    private const float MaxDownTilt = 35f;   // Düşerken en fazla aşağı eğim (derece)

    private readonly int startX;
    private readonly int startY;
    private readonly PixelPictureBox pixelView;
    private float positionY;                 // Hassas konum (piksel küsuratları için float)

    /// <summary>Dikey hız (piksel/kare). Pozitif değer aşağı yönü gösterir.</summary>
    public float Velocity { get; private set; }

    /// <summary>Kuşu ekranda gösteren piksel-duyarlı kontrol (kodla oluşturulur).</summary>
    public PictureBox View => pixelView;

    /// <summary>Çarpışma kontrolünde kullanılan daraltılmış dikdörtgen (hitbox).</summary>
    public Rectangle Bounds
    {
        get
        {
            Rectangle hitbox = pixelView.Bounds;
            hitbox.Inflate(HitboxInset, HitboxInset);
            return hitbox;
        }
    }

    /// <param name="startX">Kuşun sabit yatay konumu.</param>
    /// <param name="centerY">Başlangıçta kuşun merkezinin dikey konumu.</param>
    public Bird(int startX, int centerY)
    {
        this.startX = startX;
        startY = centerY - BirdHeight / 2;

        pixelView = new PixelPictureBox
        {
            Size = new Size(BirdWidth, BirdHeight),
            Image = GameRenderer.PrepareBirdSprite(Properties.Resources.bird)
        };

        Reset();
    }

    /// <summary>Kuşu yukarı zıplatır.</summary>
    public void Flap()
    {
        Velocity = FlapStrength;
        UpdateTilt();
    }

    /// <summary>Her karede çağrılır: yer çekimini uygular ve kuşu hareket ettirir.</summary>
    public void Update()
    {
        Velocity = Math.Min(Velocity + Gravity, MaxFallSpeed);
        positionY += Velocity;
        pixelView.Top = (int)Math.Round(positionY);
        UpdateTilt();
    }

    /// <summary>Kuşu başlangıç konumuna, hızına ve düz açıya döndürür.</summary>
    public void Reset()
    {
        Velocity = 0;
        positionY = startY;
        pixelView.Location = new Point(startX, startY);
        UpdateTilt();
    }

    private void UpdateTilt()
    {
        pixelView.TiltAngle = Math.Clamp(Velocity * TiltFactor, MaxUpTilt, MaxDownTilt);
        pixelView.Invalidate();
    }
}
