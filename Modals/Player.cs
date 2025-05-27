using System.Drawing;


public class Player
{
    public PointF Position { get; set; }
    public bool IsOnGround { get; set; }
    public float VelocityY { get; set; }
    public float VelocityX { get; set; } // Добавлено для плавного движения

    public const int Width = 40;
    public const int Height = 40;
    private const float Gravity = 0.5f;
    private const float JumpForce = 12f;
    private const float MoveSpeed = 5f; // Уменьшено для более плавного управления

    public Player(float x, float y)
    {
        Position = new PointF(x, y);
        VelocityY = 0;
        IsOnGround = false;
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
        g.FillEllipse(Brushes.Green, Position.X, Position.Y, Width, Height);
    }

    public RectangleF GetBounds()
    {
        return new RectangleF(Position.X, Position.Y, Width, Height);
    }
}