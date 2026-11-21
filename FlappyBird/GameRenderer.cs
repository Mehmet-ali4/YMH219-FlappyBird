using System.Drawing.Drawing2D;

namespace FlappyBird;

/// <summary>
/// Oyunun görsel hazırlama (kuş sprite'ı kırpma/şeffaflaştırma) ve GDI+ çizim işlemlerini yönetir.
/// </summary>
public static class GameRenderer
{
    // --- Sabit Renkler (magic number kullanmamak için) ---
    public static readonly Color SkyColor = Color.FromArgb(0x4E, 0xC0, 0xCA);           // #4EC0CA
    public static readonly Color CloudColor = Color.FromArgb(0xE8, 0xFC, 0xD8);         // Ufuk bulutları
    public static readonly Color CityFillColor = Color.FromArgb(0xA3, 0xE2, 0xC9);      // Şehir silueti
    public static readonly Color CityWindowColor = Color.FromArgb(0xC9, 0xF2, 0xDF);    // Bina pencereleri
    public static readonly Color BushLightColor = Color.FromArgb(0x83, 0xE0, 0x55);     // Açık çalı tepesi
    public static readonly Color BushMainColor = Color.FromArgb(0x5B, 0xC9, 0x38);      // Koyu çalı gövdesi

    public static readonly Color PipeBodyColor = Color.FromArgb(0x73, 0xBF, 0x2E);      // #73BF2E
    public static readonly Color PipeHighlightColor = Color.FromArgb(0x9C, 0xE6, 0x59); // #9CE659
    public static readonly Color PipeDarkColor = Color.FromArgb(0x55, 0x8C, 0x22);      // Boru gölge tonu
    public static readonly Color PipeBorderColor = Color.FromArgb(0x54, 0x38, 0x47);    // #543847

    public static readonly Color GrassLightColor = Color.FromArgb(0x9C, 0xE6, 0x59);    // Çimen üst vurgu
    public static readonly Color GrassMainColor = Color.FromArgb(0x73, 0xBF, 0x2E);     // Çimen ana yeşili
    public static readonly Color GrassStripeColor = Color.FromArgb(0x55, 0x8C, 0x22);   // Çimen eğik şerit
    public static readonly Color DirtTopShadowColor = Color.FromArgb(0xD2, 0xAA, 0x4E); // Çimen altı gölge
    public static readonly Color SandColor = Color.FromArgb(0xDE, 0xD8, 0x95);          // #DED895

    // --- Sabit Ölçüler ---
    public const int PipeCapHeight = 24;       // Boru ağzı (cap) yüksekliği
    public const int PipeCapInset = 3;         // Gövdenin ağızdan içeride kalma payı (her iki yandan)
    public const int PipeBorderWidth = 2;      // Boru çerçeve kalınlığı
    public const int GrassHeight = 18;         // Zemin üstündeki çimen şeridi yüksekliği
    private const int BackgroundThreshold = 215; // Dama tahtası arka plan gri/beyaz alt sınırı
    private const int GrayTolerance = 10;        // R, G, B kanalları arası maksimum fark

    /// <summary>
    /// Ham kuş görselindeki dış arka planı (dama tahtası/beyaz) köşelerden flood-fill ile
    /// şeffaflaştırır ve kenar boşluklarını kırpar. Kuşun içindeki beyaz kanat/göz korunur.
    /// </summary>
    public static Bitmap PrepareBirdSprite(Bitmap raw)
    {
        int width = raw.Width;
        int height = raw.Height;
        Bitmap clean = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        using (Graphics g = Graphics.FromImage(clean))
        {
            g.DrawImage(raw, 0, 0, width, height);
        }

        // 4 köşeden başlayarak yalnızca dışarıdan erişilebilen açık gri/beyaz pikselleri şeffaf yap.
        bool[,] visited = new bool[width, height];
        Queue<Point> queue = new Queue<Point>();

        TryEnqueueCorner(clean, visited, queue, 0, 0);
        TryEnqueueCorner(clean, visited, queue, width - 1, 0);
        TryEnqueueCorner(clean, visited, queue, 0, height - 1);
        TryEnqueueCorner(clean, visited, queue, width - 1, height - 1);

        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };

        while (queue.Count > 0)
        {
            Point p = queue.Dequeue();
            clean.SetPixel(p.X, p.Y, Color.Transparent);

            for (int i = 0; i < 4; i++)
            {
                int nx = p.X + dx[i];
                int ny = p.Y + dy[i];
                if (nx >= 0 && nx < width && ny >= 0 && ny < height && !visited[nx, ny])
                {
                    if (IsBackgroundPixel(clean.GetPixel(nx, ny)))
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue(new Point(nx, ny));
                    }
                }
            }
        }

        // Şeffaf olmayan piksellerin en dar çerçevesini bulup kırp.
        int minX = width;
        int minY = height;
        int maxX = -1;
        int maxY = -1;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (clean.GetPixel(x, y).A > 0)
                {
                    if (x < minX) minX = x;
                    if (x > maxX) maxX = x;
                    if (y < minY) minY = y;
                    if (y > maxY) maxY = y;
                }
            }
        }

        if (maxX < minX || maxY < minY)
        {
            return clean;
        }

        Rectangle cropArea = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        Bitmap cropped = clean.Clone(cropArea, clean.PixelFormat);
        clean.Dispose();
        return cropped;
    }

    private static void TryEnqueueCorner(Bitmap bmp, bool[,] visited, Queue<Point> queue, int x, int y)
    {
        if (!visited[x, y] && IsBackgroundPixel(bmp.GetPixel(x, y)))
        {
            visited[x, y] = true;
            queue.Enqueue(new Point(x, y));
        }
    }

    private static bool IsBackgroundPixel(Color c)
    {
        if (c.A < 128)
        {
            return true;
        }

        bool isLight = c.R >= BackgroundThreshold && c.G >= BackgroundThreshold && c.B >= BackgroundThreshold;
        bool isNeutral = Math.Abs(c.R - c.G) <= GrayTolerance && Math.Abs(c.G - c.B) <= GrayTolerance;
        return isLight && isNeutral;
    }

    /// <summary>
    /// Bir boru parçasını (gövde + boşluğa bakan uçta daha geniş ağız) yatay degrade ile çizer.
    /// </summary>
    public static void DrawPipePart(Graphics g, Rectangle rect, bool isTopPipe)
    {
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        int capHeight = Math.Min(PipeCapHeight, rect.Height);
        Rectangle bodyRect = new Rectangle(
            rect.X + PipeCapInset,
            rect.Y,
            Math.Max(1, rect.Width - PipeCapInset * 2),
            rect.Height);

        Rectangle capRect = isTopPipe
            ? new Rectangle(rect.X, rect.Bottom - capHeight, rect.Width, capHeight)
            : new Rectangle(rect.X, rect.Y, rect.Width, capHeight);

        // Gövde çizimi
        DrawPipeSegment(g, bodyRect);

        // Ağız (cap) çizimi
        DrawPipeSegment(g, capRect);
    }

    private static void DrawPipeSegment(Graphics g, Rectangle segment)
    {
        if (segment.Width <= 0 || segment.Height <= 0)
        {
            return;
        }

        using (LinearGradientBrush brush = new LinearGradientBrush(
            segment, PipeDarkColor, PipeDarkColor, LinearGradientMode.Horizontal))
        {
            brush.InterpolationColors = new ColorBlend
            {
                Colors = new[] { PipeBodyColor, PipeHighlightColor, PipeBodyColor, PipeDarkColor },
                Positions = new[] { 0.0f, 0.25f, 0.65f, 1.0f }
            };
            g.FillRectangle(brush, segment);
        }

        using Pen borderPen = new Pen(PipeBorderColor, PipeBorderWidth)
        {
            Alignment = PenAlignment.Inset
        };
        g.DrawRectangle(borderPen, segment.X, segment.Y, segment.Width - 1, segment.Height - 1);
    }

    /// <summary>
    /// Formun arka planına ufuk bulutları, basit şehir silueti ve çalı şeridi çizer.
    /// </summary>
    public static void DrawSkyBackground(Graphics g, int width, int groundTop)
    {
        g.Clear(SkyColor);

        // 1. Uzak ufuk bulutları
        int cloudBaseY = groundTop - 58;
        using (SolidBrush cloudBrush = new SolidBrush(CloudColor))
        {
            g.FillRectangle(cloudBrush, 0, cloudBaseY + 20, width, groundTop - (cloudBaseY + 20));
            for (int x = -15; x < width; x += 64)
            {
                g.FillEllipse(cloudBrush, x, cloudBaseY, 72, 56);
                g.FillEllipse(cloudBrush, x + 32, cloudBaseY + 8, 56, 48);
            }
        }

        // 2. Basit şehir silueti
        int[] buildingHeights = { 48, 66, 38, 72, 54, 44 };
        int buildingWidth = 40;
        using SolidBrush cityBrush = new SolidBrush(CityFillColor);
        using SolidBrush windowBrush = new SolidBrush(CityWindowColor);

        for (int i = 0; i * buildingWidth < width; i++)
        {
            int bx = i * buildingWidth;
            int bh = buildingHeights[i % buildingHeights.Length];
            int by = groundTop - bh - 12;
            g.FillRectangle(cityBrush, bx + 2, by, buildingWidth - 4, bh);

            // Basit pencere detayları
            for (int wy = by + 6; wy < groundTop - 22; wy += 12)
            {
                g.FillRectangle(windowBrush, bx + 8, wy, 8, 6);
                g.FillRectangle(windowBrush, bx + 22, wy, 8, 6);
            }
        }

        // 3. Ön plandaki yeşil çalı şeridi
        int bushBaseY = groundTop - 22;
        using (SolidBrush bushLight = new SolidBrush(BushLightColor))
        using (SolidBrush bushMain = new SolidBrush(BushMainColor))
        {
            g.FillRectangle(bushMain, 0, bushBaseY + 10, width, 12);
            for (int x = -10; x < width; x += 48)
            {
                g.FillEllipse(bushLight, x, bushBaseY, 44, 30);
                g.FillEllipse(bushMain, x + 18, bushBaseY + 4, 42, 26);
            }
        }
    }

    /// <summary>
    /// Zemin panelinin üstüne çizgili yeşil çimen şeridi, altına ise kum rengi zemin çizer.
    /// </summary>
    public static void DrawGround(Graphics g, Size size)
    {
        // Kum rengi alt taban
        using (SolidBrush sandBrush = new SolidBrush(SandColor))
        {
            g.FillRectangle(sandBrush, 0, 0, size.Width, size.Height);
        }

        // Çimen şeridi ana dolgusu
        using (SolidBrush grassBrush = new SolidBrush(GrassMainColor))
        {
            g.FillRectangle(grassBrush, 0, 0, size.Width, GrassHeight);
        }

        // Çimen üst parlak vurgu
        using (SolidBrush lightBrush = new SolidBrush(GrassLightColor))
        {
            g.FillRectangle(lightBrush, 0, 2, size.Width, 3);
        }

        // Çimen içi eğik desen şeritleri
        using (SolidBrush stripeBrush = new SolidBrush(GrassStripeColor))
        {
            for (int x = -16; x < size.Width + 16; x += 22)
            {
                Point[] stripe =
                {
                    new Point(x, GrassHeight - 2),
                    new Point(x + 9, 4),
                    new Point(x + 17, 4),
                    new Point(x + 8, GrassHeight - 2)
                };
                g.FillPolygon(stripeBrush, stripe);
            }
        }

        // Çimen altı toprak gölgesi
        using (SolidBrush shadowBrush = new SolidBrush(DirtTopShadowColor))
        {
            g.FillRectangle(shadowBrush, 0, GrassHeight, size.Width, 4);
        }

        // Üst ve alt koyu çerçeve çizgileri
        using Pen borderPen = new Pen(PipeBorderColor, PipeBorderWidth);
        g.DrawLine(borderPen, 0, 1, size.Width, 1);
        g.DrawLine(borderPen, 0, GrassHeight - 1, size.Width, GrassHeight - 1);
    }
}
