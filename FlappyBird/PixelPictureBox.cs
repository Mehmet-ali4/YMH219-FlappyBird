using System.ComponentModel;
using System.Drawing.Drawing2D;

namespace FlappyBird;

/// <summary>
/// Piksel sanatı (pixel art) görselleri bulanıklaştırmadan ve isteğe bağlı eğimle çizen PictureBox.
/// </summary>
public class PixelPictureBox : PictureBox
{
    /// <summary>Kuşun ekranda çizileceği eğim açısı (derece). Pozitif değer aşağı bakar.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public float TiltAngle { get; set; }

    public PixelPictureBox()
    {
        DoubleBuffered = true;
        BackColor = Color.Transparent;
        SizeMode = PictureBoxSizeMode.Normal;
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
        if (Image == null)
        {
            base.OnPaint(pe);
            return;
        }

        Graphics g = pe.Graphics;
        // Piksel sanatını keskin tutmak için en yakın komşu (NearestNeighbor) örneklemesi kullanılır.
        g.InterpolationMode = InterpolationMode.NearestNeighbor;
        g.PixelOffsetMode = PixelOffsetMode.Half;
        g.SmoothingMode = SmoothingMode.None;

        // En-boy oranını koruyarak kontrolün içine ortalanmış hedef dikdörtgeni hesapla.
        float scale = Math.Min((float)ClientSize.Width / Image.Width, (float)ClientSize.Height / Image.Height);
        float drawWidth = Image.Width * scale;
        float drawHeight = Image.Height * scale;
        float offsetX = (ClientSize.Width - drawWidth) / 2f;
        float offsetY = (ClientSize.Height - drawHeight) / 2f;
        RectangleF destRect = new RectangleF(offsetX, offsetY, drawWidth, drawHeight);

        if (Math.Abs(TiltAngle) > 0.1f)
        {
            float centerX = ClientSize.Width / 2f;
            float centerY = ClientSize.Height / 2f;
            g.TranslateTransform(centerX, centerY);
            g.RotateTransform(TiltAngle);
            g.TranslateTransform(-centerX, -centerY);
        }

        g.DrawImage(Image, destRect);
    }
}
