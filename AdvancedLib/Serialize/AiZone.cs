using System.Reflection.Metadata.Ecma335;
using BinarySerializer;

namespace AdvancedLib.Serialize;

public class AiZone : BinarySerializable{
    public byte Shape { get; set; }
    public ushort HalfX { get; set; }
    public ushort HalfY { get; set; }
    public int X {
        get => HalfX * 2;
        set => HalfX = (ushort)(value/2);
    }
    public int Y
    {
        get => HalfY * 2;
        set => HalfY = (ushort)(value / 2);
    }
    public ushort HalfWidth { get; set; }
    public ushort HalfHeight {get; set; }
    public int Width
    {
        get => HalfWidth * 2;
        set => HalfWidth = (ushort)(value / 2);
    }
    public int Height
    {
        get => HalfHeight * 2;
        set => HalfHeight = (ushort)(value / 2);
    }
    public int DisplayHeight { get => (Shape == 0) ? Height:Width; }
    public override void SerializeImpl(SerializerObject s)
    {
        Shape = s.Serialize<byte>(Shape, nameof(Shape)); //shape
        HalfX = s.Serialize<ushort>(HalfX, nameof(Shape)); //xpos
        HalfY = s.Serialize<ushort>(HalfY, nameof(Shape)); //ypos
        HalfWidth = s.Serialize<ushort>(HalfWidth, nameof(Shape)); //width
        HalfHeight = s.Serialize<ushort>(HalfHeight, nameof(Shape)); //height (0 if triangle)
        s.SerializePadding(3);
    }
}
public enum ZoneShape {
    Rectange = 0x00,
    TriangleTopLeft = 0x01,
    TriangleTopRight = 0x02,
    TriangleBottomRight = 0x03,
    TriangleBottomLeft = 0x04,
}