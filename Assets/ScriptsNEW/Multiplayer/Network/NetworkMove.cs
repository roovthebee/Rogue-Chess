using Unity.Netcode;

public struct NetworkMove : INetworkSerializable
{
    // Public Properties

    public int StartX;
    public int StartY;
    public int TargetX;
    public int TargetY;

    // Constructors

    public NetworkMove(BoardCoordinate start,  BoardCoordinate target)
    {
        StartX = start.X;
        StartY = start.Y;
        TargetX = target.X;
        TargetY = target.Y;
    }

    // Public Methods

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref StartX);
        serializer.SerializeValue(ref StartY);
        serializer.SerializeValue(ref TargetX);
        serializer.SerializeValue(ref TargetY);
    }
}