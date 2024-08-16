using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdvancedLib.Types;
using BinarySerializer;
using BinarySerializer.Nintendo.GBA;

namespace AdvancedLib.Serialize;


/// <summary>
/// A class to serialize tracks from binary data
/// </summary>
public class Track : BinarySerializable
{
#pragma warning disable
    ushort magic;

    #region Variables
    public ushort TrackType { get; set; }
    public byte TrackWidth { get; set; }
    public byte TrackHeight { get; set; }
    public sbyte TilesetLookback { get; set; }
    public Pointer LayoutPointer { get; set; }
    public Layout Layout { get; set; }
    public Pointer TilesetPartsPointer { get; set; }
    public Tileset Tileset { get; set; }
    public Pointer PalettePointer { get; set; }
    public AdvancedLib.Types.Palette Palette { get; set; }
    public Pointer TileBehaviorsPointer { get; set; }
    public byte[] TileBehaviors { get; set; } // TODO implement behaviors type
    public Pointer GameObjectsPointer { get; set; }
    public GameObjectGroup GameObjects {get; set;}
    public Pointer OverlayPointer { get; set; }
    public GameObjectGroup Overlay { get; set; }
    public Pointer ItemBoxesPointer { get; set; }
    public GameObjectGroup ItemBoxes { get; set; }
    public GameObjectGroup FinishLine { get; set; }
    public Pointer FinishLinePointer { get; set; }
    private uint data0 { get; set; }
    public uint TrackRoutine { get; set; }
    public Pointer MinimapPointer { get; set; }
    public Minimap Minimap { get; set; }
    public Pointer TrackAIPointer { get; set; }
    public TrackAI TrackAI { get; set; }
    public Pointer ObjectGfxPointer { get; set; }
    public ObjectGfx? ObjectGfx { get; set; }
    public MultiObjectGfx? MultiObjectGfx { get; set; }
    public Pointer ObjectPalettePointer { get; set; }
    public AdvancedLib.Types.Palette ObjectPalette { get; set; }

    private byte[] unk0;
    private byte[] unk1;
    private byte[] unk2;
    private byte[] unk3;
    private byte[] unk4;
    private byte[] unk5;
    #endregion

    public override void SerializeImpl(SerializerObject s)
    {
        Pointer basePointer = s.CurrentPointer;

        #region Header
        magic = s.Serialize<ushort>(
            magic,
            nameof(magic)
        );
        TrackType = s.Serialize<ushort>(
            TrackType,
            nameof(TrackType)
        );
        TrackWidth = s.Serialize<byte>(
            TrackWidth,
            nameof(TrackWidth)
        );
        TrackHeight = s.Serialize<byte>(
            TrackHeight,
            nameof(TrackHeight)
        );

        unk0 = s.SerializeArray(unk0, 42, "Padding - unk");
        //s.SerializePadding(42);

        TilesetLookback = s.Serialize<sbyte>(
            TilesetLookback,
            nameof(TilesetLookback)
        );

        unk1 = s.SerializeArray(unk1, 15, "Padding - unk"); // 12 + 3 for extra space from sbyte
        //s.SerializePadding(12);

        LayoutPointer = s.SerializePointer(
            LayoutPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(LayoutPointer)
        );

        unk2 = s.SerializeArray(unk2, 60, "Padding - unk");
        //s.SerializePadding(60);

        TilesetPartsPointer = s.SerializePointer(
            TilesetPartsPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(TilesetPartsPointer)
        );
        PalettePointer = s.SerializePointer(
            PalettePointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(PalettePointer)
        );
        TileBehaviorsPointer = s.SerializePointer(
            TileBehaviorsPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(TileBehaviorsPointer)
        );
        GameObjectsPointer = s.SerializePointer(
            GameObjectsPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(GameObjectsPointer)
        );
        OverlayPointer = s.SerializePointer(
            OverlayPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(OverlayPointer)
        );
        ItemBoxesPointer = s.SerializePointer(
            ItemBoxesPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(ItemBoxesPointer)
        );
        FinishLinePointer = s.SerializePointer(
            FinishLinePointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(FinishLinePointer)
        );

        data0 = s.Serialize<uint>(
            data0,
            name: nameof(data0)
        );

        unk3 = s.SerializeArray(unk3, 32, "Padding - unk");
        //s.SerializePadding(32);

        TrackRoutine = s.Serialize<uint>(
            TrackRoutine,
            nameof(TrackRoutine)
        );

        MinimapPointer = s.SerializePointer(
            MinimapPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(MinimapPointer)
        );
        
        s.SerializePadding(4);

        TrackAIPointer = s.SerializePointer(
            TrackAIPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(TrackAIPointer)
        );

        unk4 = s.SerializeArray(unk4, 20, "Padding - unk");
        //s.SerializePadding(20);

        ObjectGfxPointer = s.SerializePointer(
            ObjectGfxPointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(ObjectGfxPointer)
        );

        ObjectPalettePointer = s.SerializePointer(
            ObjectPalettePointer,
            PointerSize.Pointer32,
            basePointer,
            name: nameof(ObjectPalettePointer)
        );

        unk5 = s.SerializeArray(unk5, 20, "Padding - unk");
        #endregion

        #region Serialize objects
        s.Goto(PalettePointer);
        Palette = s.SerializeObject(
            Palette,
            onPreSerialize: x => x.paletteLength = 64,
            name: nameof(Palette)
        );

        s.Goto(LayoutPointer);
        Layout = s.SerializeObject<Layout>(
            Layout,
            onPreSerialize: x => x.size = new(TrackWidth, TrackHeight),
            name: nameof(Layout)
        );

        // TODO: Implement tileset lookback
        if (TilesetLookback == 0)
        {
            s.Goto(TilesetPartsPointer);
            Tileset = s.SerializeObject<Tileset>(
                Tileset,
                onPreSerialize: x => x.lookback = false,
                name: nameof(Tileset)
            );
        }
        else {
            s.Goto(TilesetPartsPointer);
            Tileset = s.SerializeObject<Tileset>(
                Tileset,
                onPreSerialize: x => x.lookback = true,
                name: nameof(Tileset)
            );
        }

        s.Goto(MinimapPointer);
        Minimap = s.SerializeObject<Minimap>(
            Minimap,
            name: nameof(Minimap)
        );

        s.Goto(TileBehaviorsPointer);
        TileBehaviors = s.SerializeArray(
            TileBehaviors,
            256,
            name: nameof(TileBehaviors)
        );
        if (ObjectGfxPointer.SerializedOffset != 0)
        {
            switch (TrackType)
            {
                case 0x200:
                case 0x300:
                    {
                        s.Goto(ObjectGfxPointer);
                        ObjectGfx = s.SerializeObject<ObjectGfx>(
                            ObjectGfx,
                            name: nameof(ObjectGfx)
                        );
                        MultiObjectGfx = null;
                        break;
                    }
                case 0x700:
                    {
                        s.Goto(ObjectGfxPointer);
                        MultiObjectGfx = s.SerializeObject<MultiObjectGfx>(
                            MultiObjectGfx,
                            name: nameof(MultiObjectGfx)
                        );
                        break;
                    }
                default:
                    {
                        throw new NotImplementedException("edge case");
                        break;
                    }
            }
        }
        if (ObjectPalettePointer.SerializedOffset != 0)
        {
            s.Goto(ObjectPalettePointer);
            ObjectPalette = s.SerializeObject<AdvancedLib.Types.Palette>(
                ObjectPalette,
                onPreSerialize: x => x.paletteLength = 16,
                name: nameof(Layout)
            );
        }

        s.Goto(TrackAIPointer);
        TrackAI = s.SerializeObject<TrackAI>(
            TrackAI,
            name: nameof(Layout)
        );

        s.Goto(GameObjectsPointer);
        GameObjects = s.SerializeObject<GameObjectGroup>(
            GameObjects,
            name: nameof(GameObjects)
        );

        s.Goto(OverlayPointer);
        Overlay = s.SerializeObject<GameObjectGroup>(
            Overlay,
            name: nameof(Overlay)
        );

        s.Goto(ItemBoxesPointer);
        ItemBoxes = s.SerializeObject<GameObjectGroup>(
            ItemBoxes,
            name: nameof(ItemBoxes)
        );

        s.Goto(FinishLinePointer);
        FinishLine = s.SerializeObject<GameObjectGroup>(
            FinishLine,
            name: nameof(FinishLine)
        );
        #endregion
    }
    public override void RecalculateSize()
    {
        int currentSize = 0;

        #region Update Pointers
        // Header
        currentSize += 256;

        LayoutPointer = new Pointer(currentSize, LayoutPointer.File, LayoutPointer.Anchor, LayoutPointer.Size);
        Layout.RecalculateSize();
        currentSize += (int)Layout.SerializedSize;

        MinimapPointer = new Pointer(currentSize, MinimapPointer.File, MinimapPointer.Anchor, MinimapPointer.Size);
        Minimap.RecalculateSize();
        currentSize += (int)Minimap.SerializedSize;
        if (TilesetLookback == 0)
        {
            TilesetPartsPointer = new Pointer(currentSize, TilesetPartsPointer.File, TilesetPartsPointer.Anchor, TilesetPartsPointer.Size);
            Tileset.RecalculateSize();
            currentSize += (int)Tileset.SerializedSize;
        } else {
            // implement lookback
        }

        PalettePointer = new Pointer(currentSize, PalettePointer.File, PalettePointer.Anchor, PalettePointer.Size);
        Palette.RecalculateSize();
        currentSize += (int)Palette.SerializedSize;

        TileBehaviorsPointer = new Pointer(currentSize, TileBehaviorsPointer.File, TileBehaviorsPointer.Anchor, TileBehaviorsPointer.Size);
        currentSize += TileBehaviors.Length;

        TrackAIPointer = new Pointer(currentSize, TrackAIPointer.File, TrackAIPointer.Anchor, TrackAIPointer.Size);
        TrackAI.RecalculateSize();
        currentSize += (int)TrackAI.SerializedSize;

        if (ObjectGfxPointer.SerializedOffset != 0)
        {
            switch (TrackType)
            {
                case 0x200:
                case 0x300:
                    ObjectGfxPointer = new Pointer(currentSize, ObjectGfxPointer.File, ObjectGfxPointer.Anchor, ObjectGfxPointer.Size);
                    ObjectGfx.RecalculateSize();
                    currentSize += (int)ObjectGfx.SerializedSize;
                    break;
                case 0x700:
                    ObjectGfxPointer = new Pointer(currentSize, ObjectGfxPointer.File, ObjectGfxPointer.Anchor, ObjectGfxPointer.Size);
                    MultiObjectGfx.RecalculateSize();
                    currentSize += (int)MultiObjectGfx.SerializedSize;
                    break;
            }
        }

        if (ObjectPalettePointer.SerializedOffset != 0)
        {
            ObjectPalettePointer = new Pointer(currentSize, ObjectPalettePointer.File, ObjectPalettePointer.Anchor, ObjectPalettePointer.Size);
            ObjectPalette.RecalculateSize();
            currentSize += (int)ObjectPalette.SerializedSize;
        }

        GameObjectsPointer = new Pointer(currentSize, GameObjectsPointer.File, GameObjectsPointer.Anchor, GameObjectsPointer.Size);
        GameObjects.RecalculateSize();
        currentSize += (int)GameObjects.SerializedSize;

        OverlayPointer = new Pointer(currentSize, OverlayPointer.File, OverlayPointer.Anchor, OverlayPointer.Size);
        Overlay.RecalculateSize();
        currentSize += (int)Overlay.SerializedSize;

        ItemBoxesPointer = new Pointer(currentSize, ItemBoxesPointer.File, ItemBoxesPointer.Anchor, ItemBoxesPointer.Size);
        ItemBoxes.RecalculateSize();
        currentSize += (int)ItemBoxes.SerializedSize;

        FinishLinePointer = new Pointer(currentSize, FinishLinePointer.File, FinishLinePointer.Anchor, FinishLinePointer.Size);
        FinishLine.RecalculateSize();
        currentSize += (int)FinishLine.SerializedSize;
        #endregion

        SerializedSize = currentSize;
    }
}