using BinarySerializer;

namespace AdvancedLib.Types
{
    /// <summary>
    /// GBA native BGR color format (Blue Green Red)
    /// where each color is 5 bits
    /// </summary>
    public class BgrColor : BinarySerializable
    {
        public byte b { get => (byte)((rawValue & 0b01111100_00000000) >> 10); }
        public byte g { get => (byte)((rawValue & 0b00000011_11100000) >> 5); }
        public byte r { get => (byte)((rawValue & 0b00000000_00011111) >> 0); }
        public ushort rawValue { get; set; }
        public BgrColor() { }
        /// <summary>
        /// Converts from a rgb888 color to BgrColor
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        public BgrColor(byte r, byte g, byte b)
        {
            ushort val = 0;
            val |= (ushort)(((b/8) << 10) & 0b01111100_00000000);
            val |= (ushort)(((g/8) << 5 ) & 0b00000011_11100000);
            val |= (ushort)(((r/8) << 0 ) & 0b00000000_00011111);
            rawValue = val;
        }
        public override void SerializeImpl(SerializerObject s)
        {
            rawValue = s.Serialize<ushort>(rawValue, "BGR Color");
        }
    }
}