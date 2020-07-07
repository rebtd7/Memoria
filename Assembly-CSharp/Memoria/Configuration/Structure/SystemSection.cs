using System;
using Memoria.Prime.Ini;

namespace Memoria
{
    public sealed partial class Configuration
    {
        private sealed class SystemSection : IniSection
        {

            public readonly IniValue<String> StreamingAssets;

            public SystemSection() : base(nameof(SystemSection), true)
            {

                StreamingAssets = BindPath(nameof(StreamingAssets), "./StreamingAssets");
            }
        }
    }
}