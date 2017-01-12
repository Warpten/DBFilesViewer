using DBFilesClient.NET.Types;

namespace RelDBC.Utils
{
    public class Money : IObjectType<uint>
    {
        public Money(uint underlyingValue) : base(underlyingValue)
        {
            _gold   = Key / (100 * 100);
            _silver = (Key / 100) % 100;
            _copper = Key % 100;
        }

        private uint _gold;
        private uint _silver;
        private uint _copper;

        public override string ToString() => $"{_gold}g {_silver}s {_copper}c";
    }
}
