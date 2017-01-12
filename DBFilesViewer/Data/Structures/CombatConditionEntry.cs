using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CombatCondition")]
    public sealed class CombatConditionEntry
    {
        public uint WorldStateExpressionID;
        public ForeignKey<UnitConditionEntry> SelfCondition;
        public ForeignKey<UnitConditionEntry> TargetCondition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ForeignKey<UnitConditionEntry>[] FriendCondition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] FriendConditionOp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] FriendConditionCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Unk;
        public uint FriendConditionLogic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ForeignKey<UnitConditionEntry>[] EnemyCondition;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] EnemyConditionOp;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] EnemyConditionCount;
        // public uint EnemyConditionLogic;
    }
}
