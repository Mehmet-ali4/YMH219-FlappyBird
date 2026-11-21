namespace FlappyBird
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gameTimer = new System.Windows.Forms.Timer(components);
            groundPanel = new Panel();
            btnNewGame = new Button();
            lblStatus = new Label();
            lblLevel = new Label();
            lblHighScore = new Label();
            lblScore = new Label();
            lblGameOver = new Label();
            groundPanel.SuspendLayout();
            SuspendLayout();
            // 
            // gameTimer
            // 
            gameTimer.Interval = 20;
            gameTimer.Tick += GameTimer_Tick;
            // 
            // groundPanel
            // 
            groundPanel.BackColor = Color.FromArgb(222, 216, 149);
            groundPanel.Controls.Add(btnNewGame);
            groundPanel.Controls.Add(lblStatus);
            groundPanel.Controls.Add(lblLevel);
            groundPanel.Controls.Add(lblHighScore);
            groundPanel.Controls.Add(lblScore);
            groundPanel.Location = new Point(0, 540);
            groundPanel.Name = "groundPanel";
            groundPanel.Size = new Size(480, 110);
            groundPanel.TabIndex = 0;
            // 
            // btnNewGame
            // 
            btnNewGame.BackColor = Color.FromArgb(245, 240, 215);
            btnNewGame.FlatStyle = FlatStyle.Flat;
            btnNewGame.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNewGame.ForeColor = Color.FromArgb(84, 56, 71);
            btnNewGame.Location = new Point(345, 36);
            btnNewGame.Name = "btnNewGame";
            btnNewGame.Size = new Size(120, 44);
            btnNewGame.TabIndex = 4;
            btnNewGame.TabStop = false;
            btnNewGame.Text = "Yeni Oyun (R)";
            btnNewGame.UseVisualStyleBackColor = false;
            btnNewGame.Click += BtnNewGame_Click;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold | FontStyle.Italic);
            lblStatus.ForeColor = Color.FromArgb(180, 40, 30);
            lblStatus.Location = new Point(12, 86);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(180, 17);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Başlamak için SPACE'e bas";
            // 
            // lblLevel
            // 
            lblLevel.AutoSize = true;
            lblLevel.BackColor = Color.Transparent;
            lblLevel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblLevel.ForeColor = Color.FromArgb(84, 56, 71);
            lblLevel.Location = new Point(12, 65);
            lblLevel.Name = "lblLevel";
            lblLevel.Size = new Size(140, 17);
            lblLevel.TabIndex = 2;
            lblLevel.Text = "Seviye: 1   |   Hız: 4";
            // 
            // lblHighScore
            // 
            lblHighScore.AutoSize = true;
            lblHighScore.BackColor = Color.Transparent;
            lblHighScore.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            lblHighScore.ForeColor = Color.FromArgb(84, 56, 71);
            lblHighScore.Location = new Point(12, 44);
            lblHighScore.Name = "lblHighScore";
            lblHighScore.Size = new Size(130, 17);
            lblHighScore.TabIndex = 1;
            lblHighScore.Text = "En Yüksek Skor: 0";
            // 
            // lblScore
            // 
            lblScore.AutoSize = true;
            lblScore.BackColor = Color.Transparent;
            lblScore.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
            lblScore.ForeColor = Color.FromArgb(84, 56, 71);
            lblScore.Location = new Point(12, 21);
            lblScore.Name = "lblScore";
            lblScore.Size = new Size(70, 21);
            lblScore.TabIndex = 0;
            lblScore.Text = "Skor: 0";
            // 
            // lblGameOver
            // 
            lblGameOver.BackColor = Color.FromArgb(222, 216, 149);
            lblGameOver.BorderStyle = BorderStyle.FixedSingle;
            lblGameOver.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            lblGameOver.ForeColor = Color.FromArgb(84, 56, 71);
            lblGameOver.Location = new Point(90, 170);
            lblGameOver.Name = "lblGameOver";
            lblGameOver.Size = new Size(300, 160);
            lblGameOver.TabIndex = 1;
            lblGameOver.Text = "GAME OVER";
            lblGameOver.TextAlign = ContentAlignment.MiddleCenter;
            lblGameOver.Visible = false;
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(78, 192, 202);
            ClientSize = new Size(480, 650);
            Controls.Add(lblGameOver);
            Controls.Add(groundPanel);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Flappy Bird - YMH 219";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            groundPanel.ResumeLayout(false);
            groundPanel.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer gameTimer;
        private Panel groundPanel;
        private Button btnNewGame;
        private Label lblStatus;
        private Label lblLevel;
        private Label lblHighScore;
        private Label lblScore;
        private Label lblGameOver;
    }
}
