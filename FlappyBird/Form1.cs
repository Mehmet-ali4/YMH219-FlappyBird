namespace FlappyBird;

/// <summary>
/// Ana form. Yalnızca arayüzü günceller ve olayları GameManager'a bağlar;
/// oyun kuralları GameManager sınıfındadır.
/// </summary>
public partial class Form1 : Form
{
    private readonly HighScoreStorage highScoreStorage;
    private readonly GameManager game;
    private bool isSpacePressed; // SPACE basılı tutulunca sürekli zıplamayı önler

    public Form1()
    {
        InitializeComponent();

        // Arka plan ve zemin GDI+ çizim olaylarını GameRenderer'a bağla.
        Paint += (sender, e) => GameRenderer.DrawSkyBackground(e.Graphics, ClientSize.Width, groundPanel.Top);
        groundPanel.Paint += (sender, e) => GameRenderer.DrawGround(e.Graphics, groundPanel.ClientSize);

        // Oyun alanı: formun genişliği x zeminin üst kenarına kadar olan yükseklik.
        highScoreStorage = new HighScoreStorage();
        game = new GameManager(ClientSize.Width, groundPanel.Top, highScoreStorage);

        // Kuş ve borular kodla oluşturulup forma eklenir.
        foreach (Control control in game.GetControls())
        {
            Controls.Add(control);
        }

        game.ScoreChanged += (sender, e) => UpdateHud();
        game.LevelChanged += (sender, e) => UpdateHud();
        game.StateChanged += Game_StateChanged;
        game.ErrorOccurred += Game_ErrorOccurred;

        Game_StateChanged(this, EventArgs.Empty);
    }

    private void Form1_Load(object? sender, EventArgs e)
    {
        // Dosya okunurken bir sorun olduysa kullanıcıyı bilgilendir (uygulama çalışmaya devam eder).
        if (highScoreStorage.LastError != null)
        {
            MessageBox.Show(highScoreStorage.LastError, "Uyarı",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // --- Oyun döngüsü ---
    private void GameTimer_Tick(object? sender, EventArgs e)
    {
        try
        {
            game.Update();
        }
        catch (Exception ex)
        {
            gameTimer.Stop();
            MessageBox.Show("Oyun sırasında beklenmeyen bir hata oluştu:\n" + ex.Message +
                            "\nOyun yeniden başlatılıyor.", "Hata",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
            game.Restart();
        }
    }

    // --- Klavye ---
    private void Form1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            if (!isSpacePressed)
            {
                isSpacePressed = true;
                game.Flap();
            }
            // SPACE'in odaktaki butonu tetiklemesini engelle.
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
        else if (e.KeyCode == Keys.R)
        {
            game.Restart();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void Form1_KeyUp(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Space)
        {
            isSpacePressed = false;
            e.Handled = true;
        }
    }

    // --- Buton ---
    private void BtnNewGame_Click(object? sender, EventArgs e)
    {
        game.Restart();
    }

    // --- GameManager olayları ---
    private void Game_StateChanged(object? sender, EventArgs e)
    {
        switch (game.State)
        {
            case GameState.Ready:
                gameTimer.Stop();
                lblGameOver.Visible = false;
                lblStatus.Text = "Başlamak için SPACE'e bas";
                break;

            case GameState.Playing:
                lblGameOver.Visible = false;
                lblStatus.Text = "Oynanıyor";
                gameTimer.Start();
                break;

            case GameState.GameOver:
                gameTimer.Stop();
                lblStatus.Text = "Game Over - Yeniden başlamak için R";
                lblGameOver.Text = "GAME OVER\n" +
                                   $"Skor: {game.Score}\n" +
                                   $"En Yüksek: {game.HighScore}" +
                                   (game.IsNewHighScore ? "\nYeni rekor!" : "");
                lblGameOver.Visible = true;
                lblGameOver.BringToFront();
                break;
        }

        UpdateHud();
    }

    private void Game_ErrorOccurred(object? sender, string message)
    {
        MessageBox.Show(message, "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    /// <summary>Skor, en yüksek skor, seviye ve hız etiketlerini günceller.</summary>
    private void UpdateHud()
    {
        lblScore.Text = $"Skor: {game.Score}";
        lblHighScore.Text = $"En Yüksek Skor: {game.HighScore}";
        lblLevel.Text = $"Seviye: {game.Level}   |   Hız: {game.Speed}";
    }
}
