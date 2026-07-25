using Unity.Netcode;

public struct NetworkCardPlay : INetworkSerializable
{
    // Public Fields

    public int CardId;

    public TargetType TargetType;

    public BoardCoordinate Coordinate;

    public Team Owner;

    public bool EndsTurn;

    // Public Methods

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref CardId);
        serializer.SerializeValue(ref TargetType);
        serializer.SerializeValue(ref Coordinate);
        serializer.SerializeValue(ref Owner);
        serializer.SerializeValue(ref EndsTurn);
    }
}