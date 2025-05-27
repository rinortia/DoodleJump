using System.Drawing;


public class Player
{
    public PointF Position { get; set; }
    public float VelocityY { get; set; }
    public const int Width = 40;
    public const int Height = 40;
    private const float Gravity = 0.5f;
    private const float JumpForce = -10f;
    private const float MoveSpeed = 5f;

    public Player(float x, float y)
    {
        Position = new PointF(x, y);
        VelocityY = 0;
    }

    public void Update()
    {
        VelocityY += Gravity;
        Position = new PointF(Position.X, Position.Y + VelocityY);
    }

    public void Jump()
    {
        VelocityY = JumpForce;
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