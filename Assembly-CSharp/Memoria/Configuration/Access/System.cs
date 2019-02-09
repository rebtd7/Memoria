using System;

namespace Memoria
{
    public sealed partial class Configuration
    {
        public static class System
        {
            public static Boolean Enabled => Instance._export.Enabled;
            public static String Path => Instance._system.StreamingAssets;
            
        }
    }
}