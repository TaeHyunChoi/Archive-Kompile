namespace DataStruct
{
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using UnityEngine;
    using CMathf;
    using CManipulator;
    using static Index.IDxTile;
    using System;

    [Serializable]
    public struct STile
    {
        //total 12 bytes
        private byte   mMapCode;     // 1 bit
        private byte   mMaskInfo;    // 1 bit
        private ushort mMaskTrigger; // 2 bits
        private ulong  mMaskMove;    // 8 bits

        public byte Code { get => (byte)mMapCode; }
        public bool IsMovable(int indexTriangle)
        {
            int mask = (int)(mMaskMove & 0xFFFF);
            mask &= (1 << indexTriangle);
            return 0 != mask;
        }
        public bool HasTrigger(ETileTriggerType type, out int value)
        {
            value = 0;
            bool hasTrigger = (0 != (mMaskTrigger & (ushort)type));

            switch (type)
            {
                case ETileTriggerType.Scale:
                    value = (int)((mMaskTrigger >> SHIFT_TRIGGER_SCALE_VALUE) & 0b_0001);
                    break;
                case ETileTriggerType.Layer:
                    value = (int)((mMaskTrigger >> SHIFT_TRIGGER_LAYER_VALUE) & 0b_1111);
                    break;
                case ETileTriggerType.Event:
                    value = (int)((mMaskTrigger >> SHIFT_TRIGGER_INTERACT_VALUE) & 0b_0011_1111_1111);
                    break;
            }

            return hasTrigger;
        }
        public float GetScale(ETileSizeType type = ETileSizeType.Default)
        {
            int   mask  = mMaskInfo >> SHIFT_INFO_SCALE;
            float scale = 0 != (mask & 1) ? 0.5f : 1f;

            return TileUtility.GetScale(type, scale);
        }
        public float GetYValue(int keyMy, Vector3 point)
        {
            Vector3 pivot = TileUtility.GetPivotByKey(keyMy, GetScale());
            float scale_half = GetScale(ETileSizeType.Half);
            int triangle = TileUtility.GetTriangleIndex(point - pivot, scale_half);

            long height = (long)(mMaskMove >> 16);
            TileUtility.GetTrianglePoints(pivot, GetScale(), height, triangle, out Vector3 p0, out Vector3 p1, out Vector3 p2);

            Vector3 normal = Vector3.Cross(p1 - p0, p2 - p0);
            normal.Normalize();
            normal = CMath.FloorToVector(normal, 3);
            float d = Vector3.Dot(normal, p0);

            return -(normal.x * point.x + normal.z * point.z - d) / normal.y;
        }

        //Only for Tile Map Sampling

#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
        public int Info { get => mMaskInfo; }
        public int Move { get => (int)(mMaskMove & 0xFFFF); }
        public long Height { get => (long)(mMaskMove >> 16); }
        public int Trigger { get => mMaskTrigger; }
        public STile(int code, int info, int trigger, int move, long height)
        {
            mMapCode     = (byte)code;
            mMaskInfo    = (byte)info;
            mMaskTrigger = (ushort)trigger;
            mMaskMove    = (ulong)((height << 16) | (long)move);
        }
#endif
    }

    public struct UnitData : IDataSetter<UnitData>
    {
        private static readonly int BIT_COUNT = 10;

        public byte Code { get; set; }
        public byte Group { get; set; }
        public byte Tier { get; set; }
        public string Name { get; set; }
        public string RcsCode { get; set; }
        /// <summary>
        /// 스탯값 범위: 0 ~ 1023 (10 bits, 포켓몬 이론상 최대 HP가 750 내외, 스탯이 400~450)
        /// bytes[13]이며 manipulator 사용해야 한다.
        /// </summary>
        public byte[] StatBaseBits { get; set; }


        /* interface: IDataSetter */
#if UNITY_EDITOR || UNITY_EDITOR_64 || UNITY_EDITOR_WIN
        public void SetByCSV(Dictionary<string, string> dataCSV)
        {
            Code  = byte.Parse(dataCSV["Code"]);
            Name  = dataCSV["Name"];
            Group = byte.Parse(dataCSV["Group"]);
            Tier  = byte.Parse(dataCSV["Tier"]);

            int length = Mathf.CeilToInt((float)EStat.Length * BIT_COUNT / 8);
            StatBaseBits = new byte[length];

            SetStatAsBinary(EStat.HP,  StatBaseBits, int.Parse(dataCSV["HP"]));
            SetStatAsBinary(EStat.MP,  StatBaseBits, int.Parse(dataCSV["MP"]));
            SetStatAsBinary(EStat.BP,  StatBaseBits, int.Parse(dataCSV["BP"]));
            SetStatAsBinary(EStat.STR, StatBaseBits, int.Parse(dataCSV["STR"]));
            SetStatAsBinary(EStat.CON, StatBaseBits, int.Parse(dataCSV["CON"]));
            SetStatAsBinary(EStat.INT, StatBaseBits, int.Parse(dataCSV["INT"]));
            SetStatAsBinary(EStat.WIS, StatBaseBits, int.Parse(dataCSV["WIS"]));
            SetStatAsBinary(EStat.DEX, StatBaseBits, int.Parse(dataCSV["DEX"]));
            SetStatAsBinary(EStat.AGI, StatBaseBits, int.Parse(dataCSV["AGI"]));
            SetStatAsBinary(EStat.LUK, StatBaseBits, int.Parse(dataCSV["LUK"]));
            SetStatAsBinary(EStat.CHA, StatBaseBits, int.Parse(dataCSV["CHA"]));

            RcsCode = dataCSV["RcsCode"];
        }
#endif
        public void WriteAsBinary(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Group);
            writer.Write(Tier);

            writer.Write(Name);
            writer.Write(RcsCode);

            writer.Write(StatBaseBits.Length);
            writer.Write(StatBaseBits);
        }
        public void ReadAsBinary(BinaryReader reader)
        {
            Code         = reader.ReadByte();
            Group        = reader.ReadByte();
            Tier         = reader.ReadByte();
            Name         = reader.ReadString();
            RcsCode      = reader.ReadString();
            StatBaseBits = reader.ReadBytes(reader.ReadInt32());
        }


        /* set stat in battle */
        public byte[] SetBattleStat(int level)
        {
            byte[] statBitArray = new byte[StatBaseBits.Length];

            int bitIndex, iv, baseStatValue, statValue;

            for (int statIndex = 0; statIndex < (int)EStat.Length; ++statIndex)
            {
                bitIndex = statIndex * BIT_COUNT;
                /*개체값*/
                iv = UnityEngine.Random.Range(1, 32);

                /*종족값*/
                baseStatValue = BitManipulator.ReadBits(StatBaseBits, bitIndex, BIT_COUNT);

                /*결과값*/
                statValue = (int)((2 * baseStatValue + iv) * level * 0.01f);
                switch (statIndex)
                {
                    case (int)EStat.HP:
                    case (int)EStat.MP:
                        statValue += (level + 10);
                        break;
                    case (int)EStat.BP:
                        statValue = baseStatValue;
                        break;
                    default:
                        statValue += 5;
                        break;
                }

                SetStatAsBinary((EStat)statIndex, statBitArray, statValue);
            }

            return statBitArray;
        }
        public void SetStatAsBinary(EStat statType, byte[] data, int value)
        {
            int bitIndex = (int)statType * BIT_COUNT;
            BitManipulator.WriteBits(data, bitIndex, BIT_COUNT, value);
        }
    }
    public struct MapData : IDataSetter<MapData>
    {
        public ushort Code { get; set; }
        public string Name { get; set; }
        public byte[] EnemyTypeArray { get; set; }
        public byte MaxENMCount { get; set; }

        /* interface: IDataSetter */
        public void SetByCSV(Dictionary<string, string> data)
        {
            Code = ushort.Parse(data["Code"]);
            Name = data["Name"];
            MaxENMCount = byte.Parse(data["MaxENMCount"]);

            StringBuilder builder = new StringBuilder();
            builder.Append("ENM");
            List<byte> listType = new List<byte>();
            byte unitCode;

            int index = 0;
            while (true)
            {
                builder.Append(index++);
                unitCode = byte.Parse(data[builder.ToString()]);
                if (0 == unitCode)
                {
                    break;
                }
                listType.Add(unitCode);

                int delete = (int)(index * 0.1f) + 1;
                builder.Remove(builder.Length - delete, delete);
            }

            if (listType.Count > 0)
            {
                EnemyTypeArray = listType.ToArray();
            }
        }
        public void WriteAsBinary(BinaryWriter writer)
        {
            writer.Write(Code);
            writer.Write(Name);
            int lengthEnemyType = EnemyTypeArray != null ? EnemyTypeArray.Length : 0;
            writer.Write(lengthEnemyType);
            if (0 < lengthEnemyType)
            {
                writer.Write(EnemyTypeArray);
            }

            writer.Write(MaxENMCount);
        }
        public void ReadAsBinary(BinaryReader reader)
        {
            Code = reader.ReadUInt16();
            Name = reader.ReadString();

            int lengthEnemyType = reader.ReadInt32();
            EnemyTypeArray = (0 < lengthEnemyType) ? reader.ReadBytes(lengthEnemyType) : null;
            MaxENMCount = reader.ReadByte();
        }
    }
}