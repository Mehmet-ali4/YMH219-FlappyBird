namespace FlappyBird;

/// <summary>
/// Bir engel: üst boru + alt boru ve aralarındaki boşluk. Sağdan sola hareket eder.
/// </summary>
public class Pipe
{
    // --- Sabitler ---
    public const int Width = 60;           // Boru genişliği (ağız dahil)
    public const int GapHeight = 150;      // Üst ve alt boru arasındaki boşluk yüksekliği
    public const int MinPipeHeight = 120;  // Bir borunun en az yüksekliği (oynanabilirlik sınırı)

    private readonly int playAreaHeight;   // Oyun alanının yüksekliği (zeminin üst kenarı)
    private readonly Random random;

    /// <summary>Üst boru kontrolü.</summary>
    public PictureBox TopPart { get; }

    /// <summary>Alt boru kontrolü.</summary>
    public PictureBox BottomPart { get; }

    /// <summary>Borunun her karede sola kayma miktarı (piksel).</summary>
    public int Speed { get; set; }

    /// <summary>Kuş bu boruyu geçti mi? (Skorun bir kez artması için)</summary>
    public bool Passed { get; set; }

    /// <summary>Borunun sol kenarı.</summary>
    public int X => TopPart.Left;

    /// <summary>Borunun sağ kenarı.</summary>
    public int Right => TopPart.Right;

    /// <summary>Boru tamamen ekranın solundan çıktı mı?</summary>
    public bool IsOffScreen => Right < 0;

    public Pipe(int playAreaHeight, Random random)
    {
        this.playAreaHeight = playAreaHeight;
        this.random = random;

        TopPart = CreatePart(isTopPipe: true);
        BottomPart = CreatePart(isTopPipe: false);
    }

    /// <summary>GDI+ ile boyanan şeffaf arka planlı bir boru kontrolü oluşturur.</summary>
    private static PictureBox CreatePart(bool isTopPipe)
    {
        PictureBox part = new PictureBox
        {
            BackColor = Color.Transparent,
            BorderStyle = BorderStyle.None
        };

        part.Paint += (sender, e) =>
        {
            if (sender is Control control)
            {
                GameRenderer.DrawPipePart(e.Graphics, control.ClientRectangle, isTopPipe);
            }
        };

        return part;
    }

    /// <summary>Boruyu Speed kadar sola kaydırır.</summary>
    public void Update()
    {
        TopPart.Left -= Speed;
        BottomPart.Left -= Speed;
    }

    /// <summary>
    /// Boruyu verilen x konumuna taşır ve boşluğun yerini rastgele seçer.
    /// </summary>
    public void Reposition(int x)
    {
        // Boşluğun üst kenarı: hem üst hem alt boru en az MinPipeHeight kadar olacak şekilde.
        int maxGapTop = playAreaHeight - MinPipeHeight - GapHeight;
        int gapTop = random.Next(MinPipeHeight, maxGapTop + 1);
        int bottomTop = gapTop + GapHeight;

        TopPart.Bounds = new Rectangle(x, 0, Width, gapTop);
        BottomPart.Bounds = new Rectangle(x, bottomTop, Width, playAreaHeight - bottomTop);
        TopPart.Invalidate();
        BottomPart.Invalidate();

        Passed = false;
    }

    /// <summary>Verilen dikdörtgen (kuş) borulardan birine değiyor mu?</summary>
    public bool CollidesWith(Rectangle area)
    {
        return TopPart.Bounds.IntersectsWith(area) || BottomPart.Bounds.IntersectsWith(area);
    }
}
