using System.ComponentModel;
using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.UI.Controls;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Item-sparse")]
    public sealed class ItemSparseEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3), TypeConverter(typeof(HexConverter))]
        public Flags<uint>[] Flags; // This is a hack, TypeConverter just doesn't want to work on arrays, i guess
        public float Unk1;
        public float Unk2;
        public Money BuyPrice;
        public Money SellPrice;
        public ClassMask AllowableClass;
        public RaceMask AllowableRace;
        public ForeignKey<SpellEntry> RequiredSpell;
        public uint MaxCount;
        public uint Stackable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public int[] ItemStatAllocation;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public float[] ItemStatSocketCostMultiplier;
        public float RangedModRange;
        public string Name;
        public string Name2;
        public string Name3;
        public string Name4;
        public string Description;
        public uint BagFamily;
        public float ArmorDamageModifier;
        public uint Duration;
        public float StatScalingFactor;
        public uint ItemLevel;
        public uint RequiredSkill;
        public uint RequiredSkillRank;
        public uint RequiredReputationFaction;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public uint[] ItemStatValue;
        public uint ScalingStatDistribution;
        public uint Delay;
        public uint PageText;
        public uint StartQuest;
        public uint LockID;
        public uint RandomProperty;
        public uint RandomSuffix;
        public uint ItemSet;
        public ForeignKey<MapEntry> Map;
        public ForeignKey<AreaTableEntry> Area;
        public uint SocketBonus;
        public uint GemProperties;
        public uint ItemLimitCategory;
        public ForeignKey<HolidaysEntry> Holiday;
        public uint ItemNameDescriptionID;
        public uint Quality;
        public uint BuyCount;
        public uint InventoryType;
        public uint RequiredLevel;
        public uint RequiredHonorRank;
        public uint RequiredCityRank;
        public uint RequiredReputationRank;
        public uint ContainerSlots;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public sbyte[] ItemStatType;
        public byte DamageType;
        public byte Bonding;
        public byte LanguageID;
        public byte PageMaterial;
        public sbyte Material;
        public byte Sheath;
        public byte TotemCategory;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] SocketColor;
        public byte CurrencySubstitutionID;
        public byte CurrencySubstitutionCount;
        public byte ArtifactID;
        public byte RequiredExpansion;
    }
}
