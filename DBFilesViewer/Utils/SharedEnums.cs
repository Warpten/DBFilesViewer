using System;
using System.ComponentModel;
using System.Drawing.Design;
using DBFilesViewer.UI.Controls;
using EnumConverter = DBFilesViewer.UI.Controls.EnumConverter;

namespace DBFilesViewer.Utils
{
    [TypeConverter(typeof (EnumConverter))]
    public enum SpecRole : byte
    {
        DPS = 2,
        Healer = 1,
        Tank = 0
    }

    [TypeConverter(typeof (EnumConverter))]
    public enum Classes : byte
    {
        None         = 0,
        Warrior      = 1,
        Paladin      = 2,
        Hunter       = 3,
        Rogue        = 4,
        Priest       = 5,
        DeathKnight  = 6,
        Shaman       = 7,
        Mage         = 8,
        Warlock      = 9,
        Monk         = 10,
        Druid        = 11,
        DemonHunter  = 12
    }

    [Editor(typeof (BitEnumEditor), typeof (UITypeEditor))]
    [Flags]
    public enum ClassMask : int
    {
        Warrior      = 1 << 0,
        Paladin      = 1 << 1,
        Hunter       = 1 << 2,
        Rogue        = 1 << 3,
        Priest       = 1 << 4,
        DeathKnight  = 1 << 5,
        Shaman       = 1 << 6,
        Mage         = 1 << 7,
        Warlock      = 1 << 8,
        Monk         = 1 << 9,
        Druid        = 1 << 10,
        DemonHunter  = 1 << 11
    }

    [TypeConverter(typeof (EnumConverter))]
    public enum Genders : int
    {
        Any = -1,
        Male = 0,
        Female = 1,
        Neutral = 2
    }

    [TypeConverter(typeof (EnumConverter))]
    public enum Races : uint
    {
        None               = 0,
        Human              = 1,
        Orc                = 2,
        Dwarf              = 3,
        Nightelf           = 4,
        Undead             = 5,
        Tauren             = 6,
        Gnome              = 7,
        Troll              = 8,
        Goblin             = 9,
        Bloodelf           = 10,
        Draenei            = 11,
        FelOrc             = 12,
        Naga               = 13,
        Broken             = 14,
        Skeleton           = 15,
        Vrykul             = 16,
        Tuskarr            = 17,
        ForestTroll        = 18,
        Taunka             = 19,
        NorthrendSkeleton  = 20,
        IceTroll           = 21,
        Worgen             = 22,
        Gilnean            = 23,
        PandarenNeutral    = 24,
        PandarenAlliance   = 25,
        PandarenHorde      = 26
    }

    [Editor(typeof (BitEnumEditor), typeof (UITypeEditor))]
    [Flags]
    public enum RaceMask : int
    {
        Human              = 1 << (1 - 1),
        Orc                = 1 << (2 - 1),
        Dwarf              = 1 << (3 - 1),
        Nightelf           = 1 << (4 - 1),
        Undead             = 1 << (5 - 1),
        Tauren             = 1 << (6 - 1),
        Gnome              = 1 << (7 - 1),
        Troll              = 1 << (8 - 1),
        Goblin             = 1 << (9 - 1),
        Bloodelf           = 1 << (10 - 1),
        Draenei            = 1 << (11 - 1),
        FelOrc             = 1 << (12 - 1),
        Naga               = 1 << (13 - 1),
        Broken             = 1 << (14 - 1),
        Skeleton           = 1 << (15 - 1),
        Vrykul             = 1 << (16 - 1),
        Tuskarr            = 1 << (17 - 1),
        ForestTroll        = 1 << (18 - 1),
        Taunka             = 1 << (19 - 1),
        NorthrendSkeleton  = 1 << (20 - 1),
        IceTroll           = 1 << (21 - 1),
        Worgen             = 1 << (22 - 1),
        Gilnean            = 1 << (23 - 1),
        PandarenNeutral    = 1 << (24 - 1),
        PandarenAlliance   = 1 << (25 - 1),
        PandarenHorde      = 1 << (26 - 1)
    }
    
}
