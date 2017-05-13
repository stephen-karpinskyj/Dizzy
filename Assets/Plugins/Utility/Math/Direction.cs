public enum Direction
{
    CW = 0,
    CCW,
}

public static class DirectionExtensions
{
    public static Direction Opposite(this Direction dir)
    {
        return dir == Direction.CW ? Direction.CCW : Direction.CW;
    }
}