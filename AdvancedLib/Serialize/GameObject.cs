using BinarySerializer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AdvancedLib.Serialize.GameObjectGroup;

namespace AdvancedLib.Serialize
{
    public class GameObjectGroup : BinarySerializable
    {
        public GameObject[] GameObjects { get; set; }
        public override void SerializeImpl(SerializerObject s)
        {
            GameObjects = s.SerializeObjectArrayUntil(GameObjects, o => (o.Id | o.X | o.Y | o.Zone) == 0, name: nameof(GameObjects));
            if (GameObjects.Length > 0)
                if (GameObjects.Last().Id == 0)
                    GameObjects = GameObjects[0..(GameObjects.Length - 1)];
        }
    }
    public class GameObject : BinarySerializable
    {
        public byte Id;
        public byte X;
        public byte Y;
        public byte Zone;
        public GameObject() { }
        public GameObject(byte id, byte x, byte y, byte zone)
        {
            Id = id;
            X = x;
            Y = y;
            Zone = zone;
        }
        public override void SerializeImpl(SerializerObject s)
        {
            Id = s.Serialize(Id, nameof(Id));
            X = s.Serialize(X, nameof(Id));
            Y = s.Serialize(Y, nameof(Id));
            Zone = s.Serialize(Zone, nameof(Id));
        }
    }
}
