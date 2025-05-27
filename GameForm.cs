using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Doodle_Jump.Modals;

namespace Doodle_Jump
{
    public partial class GameForm : Form
    {
        private Player player;

        private const int ScrollTriggerY = 250;         // «полка», выше которой начинаем прокрутку
        private int score;                              // максимум достигнутой высоты
        private Random rnd = new Random();

        private List<IPlatform> platforms;

        private int platformsCreated = 0;               // счетчик созданных платформ
        private float nextPlatformY = -40;              // Y-координата для следующей платформы
        private float lastPlatformX = 150;              // X-координата последней платформы

        private float minPlatformSpacing = 40f;         // минимальное расстояние между платформами
        private float maxPlatformSpacing = 100f;        // максимальное расстояние между платформами
        private int minVisiblePlatforms = 12;            // минимальное количество платформ на экране
        private float maxJumpHeight = 230f;             // максимальная высота прыжка игрока

        public GameForm()
        {
            InitializeComponent();

            this.ClientSize = new Size(400, 600); // задаем одинаковый размер окна
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            game_timer.Interval = 16; // 60 кадров/сек

            player = new Player(100, 100);
            platforms = new List<IPlatform>();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true; // для сглаженной графики
            StartNewGame();
        }

        private void game_timer_Tick(object sender, EventArgs e)
        {
            player.IsOnGround = false; // по умолчанию игрок в воздухе
            player.Update();

            // телепортация по горизонтали
            if (player.Position.X < -Player.Width)
                player.Position = new PointF(ClientSize.Width, player.Position.Y);
            else if (player.Position.X > ClientSize.Width)
                player.Position = new PointF(-Player.Width, player.Position.Y);

            // обработка приземления
            foreach (var p in platforms.ToList())
            {
                if (IsPlayerLandingOn(p))
                {
                    bool stillExists = p.OnLand(player);
                    if (!stillExists) platforms.Remove(p);
                }
            }

            // прокрутка мира
            if (player.Position.Y < ScrollTriggerY)
            {
                float dy = ScrollTriggerY - player.Position.Y;
                ScrollWorld(dy);
                score += (int)dy;
            }

            // если упал за экран — конец игры
            if (player.Position.Y > ClientSize.Height)
                GameOver();

            Invalidate();
        }

        private void ScrollWorld(float dy)
        {
            // фиксируем игрока на ScrollTriggerY
            player.Position = new PointF(player.Position.X, ScrollTriggerY);

            // сдвигаем платформы вниз
            foreach (var p in platforms)
                p.Position = new PointF(p.Position.X, p.Position.Y + dy);

            // удаляем платформы, вышедшие за экран
            platforms.RemoveAll(p => p.Position.Y > ClientSize.Height + 50);

            // добавляем недостающие платформы, если их стало меньше минимума
            int visibleCount = platforms.Count(p => p.Position.Y >= 0 && p.Position.Y <= ClientSize.Height);
            while (visibleCount < minVisiblePlatforms)
            {
                AddRandomPlatform();
                visibleCount++;
            }
        }

        private void AddRandomPlatform()
        {
            float x = rnd.Next(50, ClientSize.Width - 50);
            float spacing;

            // ограничение генерации расстояния между платформами по максимальной высоте прыжка
            do
            {
                spacing = (float)(rnd.NextDouble() * (maxPlatformSpacing - minPlatformSpacing) + minPlatformSpacing);
            }
            while (spacing > maxJumpHeight);

            float y = nextPlatformY;

            IPlatform newPlatform;

            if (platformsCreated < 8)
                newPlatform = new NormalPlatform(x, y);
            else if (rnd.NextDouble() < 0.2)
                newPlatform = new BreakablePlatform(x, y);
            else
                newPlatform = new NormalPlatform(x, y);

            platforms.Add(newPlatform);
            platformsCreated++;

            nextPlatformY -= spacing;
        }

        private void StartNewGame()
        {
            platforms.Clear();
            platformsCreated = 0;
            nextPlatformY = -40;
            lastPlatformX = 150;

            // создаем стартовую платформу
            var startPlatform = new NormalPlatform(150, ClientSize.Height - 100);
            platforms.Add(startPlatform);
            platformsCreated++;

            // игрок на стартовой платформе
            player = new Player(
                startPlatform.Position.X + (startPlatform.Size.Width - Player.Width) / 2,
                startPlatform.Position.Y - Player.Height)
            {
                VelocityY = 0,
                IsOnGround = true
            };

            score = 0;

            // заполняем экран стартовыми платформами
            for (int i = 0; i < minVisiblePlatforms; i++)
            {
                float x = rnd.Next(50, ClientSize.Width - 100);
                float spacing;

                do
                {
                    spacing = (float)(rnd.NextDouble() * (maxPlatformSpacing - minPlatformSpacing) + minPlatformSpacing);
                }
                while (spacing > maxJumpHeight);

                float y = ClientSize.Height - 160 - i * spacing;

                platforms.Add(new NormalPlatform(x, y));
                platformsCreated++;
                lastPlatformX = x;
                nextPlatformY = y - spacing;
            }

            game_timer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Left || keyData == Keys.A)
                player.MoveLeft();
            else if (keyData == Keys.Right || keyData == Keys.D)
                player.MoveRight();

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;

            foreach (var p in platforms)
                p.Draw(g);

            player.Draw(g);

            e.Graphics.DrawString($"Score: {score}", this.Font, Brushes.Black, 10, 10);
        }

        private bool IsPlayerLandingOn(IPlatform p)
        {
            RectangleF pr = player.GetBounds();
            RectangleF pl = new RectangleF(p.Position, p.Size);

            bool verticalHit = player.VelocityY > 0 &&
                               pr.Bottom >= pl.Top &&
                               pr.Bottom <= pl.Top + player.VelocityY + 5;

            bool horizontalOverlap = pr.Right > pl.Left && pr.Left < pl.Right;

            if (verticalHit && horizontalOverlap)
            {
                player.IsOnGround = true;
                player.VelocityY = 0;
                player.Position = new PointF(player.Position.X, pl.Top - Player.Height);
                return true;
            }

            return false;
        }

        private void GameOver()
        {
            game_timer.Stop();
            MessageBox.Show($"Игра окончена!\nСчёт: {score}", "Doodle Jump");
            this.Close();
        }
    }
}
