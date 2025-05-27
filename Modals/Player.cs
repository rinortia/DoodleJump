using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Player
{
    public PointF Position { get; set; }
    public bool IsOnGround { get; set; }
    public float VelocityY { get; set; }
    public float VelocityX { get; set; } // Добавлено для плавного движения

    public const int Width = 70;
    public const int Height = 70;
    private const float Gravity = 0.5f;
    private const float JumpForce = 14f;
    private const float MoveSpeed = 5f; // Уменьшено для более плавного управления


    private static Image playerImage = LoadImage();

    private static Image LoadImage()
    {
        string path = Path.Combine(Application.StartupPath, "Resources", "kitten.png");

        if (!File.Exists(path))
        {
            MessageBox.Show($"Файл не найден: {path}");
            return null;
        }
        return Image.FromFile(path);
    }


    public Player(float x, float y)
    {
        Position = new PointF(x, y);
        VelocityY = 0;

        if (playerImage == null)
        {
            string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "player.png");
            if (File.Exists(imagePath))
                playerImage = Image.FromFile(imagePath);
        }
    }


    public void Update()
    {
        if (!IsOnGround) // Только если не на земле
        {
            VelocityY += Gravity;
            Position = new PointF(Position.X, Position.Y + VelocityY);
        }
    }

    public void Jump()
    {
        if (IsOnGround) // Можно прыгать только с земли
        {
            VelocityY = -JumpForce; // Отрицательное значение, так как Y увеличивается вниз
            IsOnGround = false;
        }

    }

    public void MoveLeft()
    {
        Position = new PointF(Position.X - MoveSpeed, Position.Y);
    }

    public void MoveRight()
    {
        Position = new PointF(Position.X + MoveSpeed, Position.Y);
    }

    public void Draw(Graphics g)
    {
        if (playerImage != null)
            g.DrawImage(playerImage, Position.X, Position.Y, Width, Height);
        else
            g.FillEllipse(Brushes.Green, Position.X, Position.Y, Width, Height);
    }


    public RectangleF GetBounds()
    {
        return new RectangleF(Position.X, Position.Y, Width, Height);
    }


}