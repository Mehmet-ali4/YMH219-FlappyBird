namespace FlappyBird;

internal static class Program
{
    /// <summary>
    /// Uygulamanın giriş noktası.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // Yakalanmamış bir hata olursa uygulama kapanmasın, kullanıcıya mesaj gösterilsin.
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (sender, e) =>
            MessageBox.Show(
                "Beklenmeyen bir hata oluştu:\n" + e.Exception.Message,
                "Hata",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);

        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
    }
}
