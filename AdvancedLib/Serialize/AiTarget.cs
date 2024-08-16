using BinarySerializer;

namespace AdvancedLib.Serialize;

public class AiTarget : BinarySerializable{
    public ushort Intersection { get; set; }
    public ushort Speed { get; set; }
    public ushort X { get; set; }
    public ushort Y { get; set; }
    public override void SerializeImpl(SerializerObject s)
    {
        X = s.Serialize(X, nameof(X));
        Y = s.Serialize(Y, nameof(Y));
        Speed = s.Serialize(Speed, nameof(Speed));
        Intersection = s.Serialize(Intersection, nameof(Intersection));
    }
}