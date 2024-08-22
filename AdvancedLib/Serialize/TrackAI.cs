using BinarySerializer;

namespace AdvancedLib.Serialize;

public class TrackAI : BinarySerializable{
    byte zonesCount {get; set;}
    Pointer zonesPointer {get; set;}
    public AiZone[] Zones;
    Pointer targetsPointer {get; set;}
    public AiTarget[] Targets { get; set;}
    public override void SerializeImpl(SerializerObject s)
    {
        Pointer basePointer = s.CurrentPointer;

        zonesCount = s.Serialize<byte>(
            zonesCount,
            nameof(zonesCount)
        );
        zonesPointer = s.SerializePointer(
            zonesPointer,
            PointerSize.Pointer16,
            anchor: basePointer,
            name:nameof(zonesPointer)
        );
        targetsPointer = s.SerializePointer(
            targetsPointer,
            PointerSize.Pointer16,
            anchor: basePointer,
            name:nameof(targetsPointer)
        );

        s.Goto(zonesPointer);
        Zones = s.SerializeObjectArray(
            Zones,
            zonesCount,
            name: nameof(Zones)
        );
        s.Goto(targetsPointer);
        Targets = s.SerializeObjectArray(
            Targets,
            zonesCount * 3,
            name: nameof(Zones)
        );
        s.SerializePadding(3);
    }
    public override void RecalculateSize()
    {
        int position = 5;

        zonesPointer = new Pointer(position, zonesPointer.File, zonesPointer.Anchor, zonesPointer.Size);
        position += zonesCount * 12; // each zone is 12 bytes long.

        targetsPointer = new Pointer(position, targetsPointer.File, targetsPointer.Anchor, targetsPointer.Size);
        position += zonesCount * 3 * 8;

        base.RecalculateSize();
    }
}