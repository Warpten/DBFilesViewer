using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("PlayerCondition")]
    public sealed class PlayerConditionEntry
    {
        public RaceMask RaceMask;
        public uint SkillLogic;
        public uint ReputationLogic;
        public uint PrevQuestLogic;
        public uint CurrQuestLogic;
        public uint CurrentCompletedQuestLogic;
        public uint SpellLogic;
        public uint ItemLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Time;
        public uint AuraSpellLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<SpellEntry>[] AuraSpellID;
        public uint AchievementLogic;
        public uint AreaLogic;
        public uint QuestKillLogic;
        public string FailureDescription;
        public ushort MinLevel;
        public ushort MaxLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] SkillID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] MinSkill;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public short[] MaxSkill;
        public ushort MaxFactionID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] PrevQuestID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] CurrQuestID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ushort[] CurrentCompletedQuestID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[] Explored;
        public ushort WorldStateExpressionID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] Achievement; // ForeignKey<AchievementEntry>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<AreaTableEntry>[] AreaID;
        public ushort QuestKillID;
        public ushort PhaseID;
        public ushort MinAvgEquippedItemLevel;
        public ushort MaxAvgEquippedItemLevel;
        public ushort ModifierTreeID;
        public byte Flags;
        public sbyte Gender;
        public sbyte NativeGender;
        public byte MinLanguage;
        public byte MaxLanguage;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] MinReputation;
        public byte MaxReputation;
        public byte Unknown1;
        public byte MinPVPRank;
        public byte MaxPVPRank;
        public byte PvpMedal;
        public byte ItemFlags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] AuraCount;
        public byte WeatherID;
        public byte PartyStatus;
        public byte LifetimeMaxPVPRank;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] LfgStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] LfgCompare;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] CurrencyCount;
        public sbyte MinExpansionLevel;
        public sbyte MaxExpansionLevel;
        public sbyte MinExpansionTier;
        public sbyte MaxExpansionTier;
        public byte MinGuildLevel;
        public byte MaxGuildLevel;
        public byte PhaseUseFlags;
        public sbyte ChrSpecializationIndex;
        public sbyte ChrSpecializationRole;
        public sbyte PowerType;
        public sbyte PowerTypeComp;
        public sbyte PowerTypeValue;
        public ClassMask ClassMask;
        public uint LanguageID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] MinFactionID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<SpellEntry>[] SpellID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<ItemSparseEntry>[] ItemID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] ItemCount;
        public uint LfgLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] LfgValue;
        public uint CurrencyLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<CurrencyTypesEntry>[] CurrencyID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public uint[] QuestKillMonster;
        public uint PhaseGroupID;
        public uint MinAvgItemLevel;
        public uint MaxAvgItemLevel;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int Unknown700;
    }
}
