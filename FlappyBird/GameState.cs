namespace FlappyBird;

/// <summary>
/// Oyunun içinde bulunabileceği temel durumlar.
/// </summary>
public enum GameState
{
    Ready,    // Oyun başlamayı bekliyor (SPACE ile başlar)
    Playing,  // Oyun oynanıyor
    GameOver  // Çarpışma oldu, oyun bitti
}
