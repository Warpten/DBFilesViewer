using System.ComponentModel;
using System.Runtime.InteropServices;
using DBFilesClient.NET;
using EnumConverter = DBFilesViewer.UI.Controls.EnumConverter;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("UnitCondition")]
    public sealed class UnitConditionEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public uint[] Value;
        public uint Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UnitConditionVariableType[] Variable;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public UnitConditionOp[] Op;
    }

    [TypeConverter(typeof(EnumConverter))]
    public enum UnitConditionOp : uint
    {
        EQUAL = 1,
        DIFFERENT = 2,
        LESSER = 3,
        LESSER_OR_EQUAL = 4,
        GREATER = 5,
        GREATER_OR_EQUAL = 5
    }

    [TypeConverter(typeof(EnumConverter))]
    public enum UnitConditionVariableType : uint
    {
        UNIT_CONDITION_UNK_1                            = 1, 
        UNIT_CONDITION_UNK_2                            = 2, 
        UNIT_CONDITION_UNK_3                            = 3, 
        UNIT_CONDITION_UNK_4                            = 4, 
        UNIT_CONDITION_UNK_5                            = 5, 
        UNIT_CONDITION_UNK_6                            = 6, 
        UNIT_CONDITION_UNK_7                            = 7, 
        UNIT_CONDITION_CAN_ASSIST                       = 8,
        UNIT_CONDITION_CAN_ATTACK                       = 9,   
        UNIT_CONDITION_UNK_10                           = 10,
        UNIT_CONDITION_UNK_11                           = 11,
        UNIT_CONDITION_UNK_12                           = 12, // some percentage
        UNIT_CONDITION_UNK_13                           = 13, // something about power. same code as 14 and 15. executes if ( !*(*(a1 + 408) + 92LL) )
        UNIT_CONDITION_UNK_14                           = 14, // something about power. same code as 13 and 15. executes if ( *(*(a1 + 408) + 92LL) == 1 )
        UNIT_CONDITION_UNK_15                           = 15, // something about power. same code as 13 and 14. executes if ( *(*(a1 + 408) + 92LL) == 3 )
        UNIT_CONDITION_COMBO_POINTS                     = 16,
        UNIT_CONDITION_UNK_17                           = 17, // shared code with 20
        UNIT_CONDITION_UNK_18                           = 18, // shared code with 21 (something about spell categories)
        UNIT_CONDITION_UNK_19                           = 19, // shared code with 22 (something about spell categories and spell effect)
        UNIT_CONDITION_UNK_20                           = 20, // shared code with 17
        UNIT_CONDITION_UNK_21                           = 21, // shared code with 18 (something about spell categories)
        UNIT_CONDITION_UNK_22                           = 22, // shared code with 19 (something about spell categories and spell effect)
        UNIT_CONDITION_UNK_23                           = 23, // something about spell effect difficulty
        UNIT_CONDITION_DAMAGE_SCHOOL0_PERCENT           = 24,
        UNIT_CONDITION_DAMAGE_SCHOOL1_PERCENT           = 25,
        UNIT_CONDITION_DAMAGE_SCHOOL2_PERCENT           = 26,
        UNIT_CONDITION_DAMAGE_SCHOOL3_PERCENT           = 27,
        UNIT_CONDITION_DAMAGE_SCHOOL4_PERCENT           = 28,
        UNIT_CONDITION_DAMAGE_SCHOOL5_PERCENT           = 29,
        UNIT_CONDITION_DAMAGE_SCHOOL6_PERCENT           = 30,
        UNIT_CONDITION_UNK_31                           = 31,
        UNIT_CONDITION_UNK_32                           = 32,
        UNIT_CONDITION_UNK_33                           = 33,
        UNIT_CONDITION_UNK_34                           = 34,
        UNIT_CONDITION_UNK_35                           = 35,
        UNIT_CONDITION_UNK_36                           = 36,  
        UNIT_CONDITION_MELEE_ATTACKERS_COUNT            = 37,
        UNIT_CONDITION_UNK_38                           = 38,
        UNIT_CONDITION_UNK_39                           = 39, // float SIMD magic
        UNIT_CONDITION_IS_IN_MELEE_RANGE                = 40,
        UNIT_CONDITION_PURSUIT_TIME                     = 41,
        UNIT_CONDITION_HARMFUL_AURA_CANCELLED_BY_DAMAGE = 42,
        UNIT_CONDITION_UNK_43                           = 43,
        UNIT_CONDITION_UNK_44                           = 44,
        UNIT_CONDITION_NUM_FRIENDS                      = 45, // Party size
        UNIT_CONDITION_THREAT_SCHOOL0_PERCENT           = 46,
        UNIT_CONDITION_THREAT_SCHOOL1_PERCENT           = 47,
        UNIT_CONDITION_THREAT_SCHOOL2_PERCENT           = 48,
        UNIT_CONDITION_THREAT_SCHOOL3_PERCENT           = 49,
        UNIT_CONDITION_THREAT_SCHOOL4_PERCENT           = 50,
        UNIT_CONDITION_THREAT_SCHOOL5_PERCENT           = 51,
        UNIT_CONDITION_THREAT_SCHOOL6_PERCENT           = 52,
        UNIT_CONDITION_IS_INTERRUPTIBLE                 = 53,
        UNIT_CONDITION_UNK_54                           = 54,
        UNIT_CONDITION_RANGED_ATTACKERS_COUNT           = 55,
        UNIT_CONDITION_CREATURE_TYPE                    = 56, // Shapeshift form included
        UNIT_CONDITION_UNK_57                           = 57, // Something about melee ?
        UNIT_CONDITION_UNK_58                           = 58, // 
        UNIT_CONDITION_UNK_59                           = 59, // 
        UNIT_CONDITION_IS_SPELL_KNOWN                   = 60, // UNIT_CONDITION_NUM_FRIENDS in magic, but CGUnit_C::IsSpellKnown
        UNIT_CONDITION_UNK_61                           = 61,
        UNIT_CONDITION_IS_AREA_IMMUNE                   = 62,
        UNIT_CONDITION_UNK_63                           = 63, 
        UNIT_CONDITION_DAMAGE_MAGIC_PERCENT             = 64,
        UNIT_CONDITION_DAMAGE_PERCENT                   = 65,
        UNIT_CONDITION_THREAT_MAGIC_PERCENT             = 66,
        UNIT_CONDITION_THREAT_PERCENT                   = 67,
        UNIT_CONDITION_UNK_68                           = 68,
        UNIT_CONDITION_HAS_TOTEM1                       = 69,
        UNIT_CONDITION_HAS_TOTEM2                       = 70,
        UNIT_CONDITION_HAS_TOTEM3                       = 71,
        UNIT_CONDITION_HAS_TOTEM4                       = 72,
        UNIT_CONDITION_HAS_TOTEM5                       = 73, // Yes, five totems.
        UNIT_CONDITION_UNK_74                           = 74,
        UNIT_CONDITION_HAS_STRING_ID                    = 75,
        UNIT_CONDITION_HAS_AURA                         = 76,
        UNIT_CONDITION_IS_HOSTILE_TO                    = 77, // CGUnit_C::UnitReaction(a1, a2) < 2;
        UNIT_CONDITION_UNK_78                           = 78, // spec       (*(v137 + 40) & 4) >> 2;
        UNIT_CONDITION_ROLE_IS_TANK                     = 79,
        UNIT_CONDITION_UNK_80                           = 80, // spec still (*(v143 + 40) & 2) >> 1;
        UNIT_CONDITION_ROLE_IS_HEALER                   = 81,
        UNIT_CONDITION_UNK_82                           = 82,
        UNIT_CONDITION_IS_LOCAL_CLIENT_CONTROLLED       = 83,
        UNIT_CONDITION_PATH_FAIL_COUNT                  = 84
    }
}
