using System;
using Unity.Netcode;

public struct BoardCoordinate : IEquatable<BoardCoordinate>, INetworkSerializable
{
    // Private Fields

    private int x;

    private int y;

    // Public Properties

    public readonly int X => x;
    
    public readonly int Y => y;

    // Constructors

    public BoardCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
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

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref x);
        serializer.SerializeValue(ref y);
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