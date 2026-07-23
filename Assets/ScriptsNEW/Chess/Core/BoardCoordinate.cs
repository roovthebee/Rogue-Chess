using System;

public readonly struct BoardCoordinate : IEquatable<BoardCoordinate>
{
    // Public Properties

    public int X { get; }

    public int Y { get; }

    // Constructors

    public BoardCoordinate(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Public Methods

    public bool Equals(BoardCoordinate other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object obj)
    {
        return obj is BoardCoordinate other && Equals(other);
    }

    public override string ToString()
    {
        return $"({X}, {Y})";
    }

    // Operators

    public static bool operator ==(BoardCoordinate left, BoardCoordinate right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BoardCoordinate left, BoardCoordinate right)
    {
        return !left.Equals(right);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
}