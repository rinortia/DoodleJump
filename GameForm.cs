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

        // Для генерации платформ
        private int platformsCreated = 0;
        private float nextPlatformY = -40; // координата Y для следующей платформы сверху
        private float lastPlatformX = 150; // позиция последней платформы по X

        public GameForm()
        {
            InitializeComponent();

            // Инициализация в конструкторе
            player = new Player(100, 100);

            platforms = new List<IPlatform>();
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true; // важно для плавной графики
            StartNewGame();
        }

        private void game_timer_Tick(object sender, EventArgs e)
        {
            player.IsOnGround = false; // Сначала предполагаем, что игрок в воздухе

            player.Update();

            // Телепортация игрока при выходе за границы экрана
            if (player.Position.X < -Player.Width)
                player.Position = new PointF(ClientSize.Width, player.Position.Y);
            else if (player.Position.X > ClientSize.Width)
                player.Position = new PointF(-Player.Width, player.Position.Y);

            // 1) Приземление на платформы
            foreach (var p in platforms.ToList())
            {
                if (IsPlayerLandingOn(p))
                {
                    bool stillExists = p.OnLand(player);
                    if (!stillExists) platforms.Remove(p);
                }
            }

            // 2) Прокрутка: игрок выше порога?
            if (player.Position.Y < ScrollTriggerY)
            {
                float dy = ScrollTriggerY - player.Position.Y;   // сколько поднялись
                ScrollWorld(dy);
                score += (int)dy;                                // + очки
            }

            // 3) Упал ли игрок за нижний край?
            if (player.Position.Y > ClientSize.Height)
                GameOver();

            Invalidate();        // перерисовать
        }

        private void ScrollWorld(float dy)
        {
            // Сдвигаем игрока на ScrollTriggerY (не выше этого уровня)
            player.Position = new PointF(player.Position.X, ScrollTriggerY);

            // Сдвигаем все платформы вниз
            foreach (var p in platforms)
                p.Position = new PointF(p.Position.X, p.Position.Y + dy);

            // Удаляем платформы, которые вышли за нижнюю границу
            platforms.RemoveAll(p => p.Position.Y > ClientSize.Height + 50);

            // Генерируем новые платформы, пока верхняя часть не заполнена
            while (!TopIsFilled())
            {
                AddRandomPlatform();
            }
        }

        private float platformSpacing = 40f; // меньшее расстояние между платформами

        private void AddRandomPlatform()
        {
            float x = rnd.Next(50, ClientSize.Width - 50);
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

            nextPlatformY -= platformSpacing;
        }

        private void StartNewGame()
        {
            platforms.Clear();
            platformsCreated = 0;
            nextPlatformY = -40;
            lastPlatformX = 150;  // сброс последней X позиции

            // Добавляем стартовую платформу
            var startPlatform = new NormalPlatform(150, 500);
            platforms.Add(startPlatform);
            platformsCreated++;

            // Создаем игрока НА стартовой платформе
            player = new Player(
                startPlatform.Position.X + (startPlatform.Size.Width - Player.Width) / 2,
                startPlatform.Position.Y - Player.Height)
            {
                VelocityY = 0,
                IsOnGround = true
            };

            score = 0;

            // Создаём несколько платформ на экране сразу, поднимаясь вверх с шагом 60
            for (int i = 0; i < 7; i++)
            {
                float x = rnd.Next(50, ClientSize.Width - 100);
                float y = 440 - i * 60;
                platforms.Add(new NormalPlatform(x, y));
                platformsCreated++;
                lastPlatformX = x;
                nextPlatformY = y - 60; // для последовательной генерации сверху
            }

            game_timer.Start();
        }

        private bool TopIsFilled()
        {
            // Проверяем, есть ли платформа в верхних 10px экрана
            return platforms.Any(p => p.Position.Y < 80);
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
                // Корректируем позицию игрока, чтобы он стоял точно на платформе
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

            // Если есть меню, можно здесь вызвать:
            // MainMenuForm menu = new MainMenuForm();
            // menu.Show();
        }
    }
}