using System.Linq;
using DBFilesViewer.Data.IO.Files;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// This foreign key describes a "complex" key, where the actual key does not point to the
    /// index column in the target record, but rather another column.
    /// </summary>
    public sealed class ZoneMusicSoundKitKey : ForeignKey<SoundKitEntryEntry>
    {
        public ZoneMusicSoundKitKey(uint underlyingValue) :
            base(underlyingValue)
        {
        }

        /// Legacy code kept as documentation.
        // protected override Func<uint, SoundKitEntryEntry> ValueGetter { get; } = key =>
        // {
        //     try {
        //         return Database.GetStore<SoundKitEntryEntry>()?.First(kv => kv.Value.ZoneMusicID == key).Value;
        //     } catch (Exception) {
        //         return null;
        //     }
        // };

        private uint? _cachedKey;

        public override uint Key
        {
            get
            {
                if (_cachedKey.HasValue)
                    return _cachedKey.Value;

                _cachedKey = DBC.Get<SoundKitEntryEntry>()?.First(kv => kv.Value.ZoneMusicID == base.Key).Key ?? 0;
                return _cachedKey.Value;
            }
        }
    }

    /// <remarks>
    /// Simca: and you need the values from another DB2 to decide which entry to pick when there are multiple with same TextureFileDataID
    /// Simca: ComponentTextureFileData.db2
    /// </remarks>
    public sealed class CreatureDisplayInfoTextureKey : ForeignKey<TextureFileDataEntry>
    {
        public CreatureDisplayInfoTextureKey(uint underlyingValue) : base(underlyingValue)
        {
        }

        private uint? _cachedKey;

        public override uint Key
        {
            get
            {
                if (_cachedKey.HasValue)
                    return _cachedKey.Value;

                _cachedKey = DBC.Get<TextureFileDataEntry>()?.FirstOrDefault(kv => kv.Value.TextureFileDataID == base.Key).Key ?? 0;
                return _cachedKey.Value;
            }
        }
    }

    /// <summary>
    /// This is actually mapping a TextureFileDataID to an array of FileDataID.
    /// Selection is done through ComponentModelFileData.
    /// 
    /// This type is incomplete and kept as documentation.
    /// </summary>
    public sealed class TextureFileDataKey : ForeignKey<TextureFileDataEntry>
    {
        public TextureFileDataKey(uint underlyingValue) : base(underlyingValue)
        {
        }

        private uint? _cachedKey;

        public override uint Key
        {
            get
            {
                if (_cachedKey.HasValue)
                    return _cachedKey.Value;

                _cachedKey = DBC.Get<TextureFileDataEntry>()?.FirstOrDefault(kv => kv.Value.TextureFileDataID == base.Key).Key ?? 0;
                return _cachedKey.Value;
            }
        }
    }
}
