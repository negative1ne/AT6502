// ============================================================================
// FIX BANNER: CCFormatConfig.cs - CENTRAL ENGINE DECOUPLING CONFIGURATION [v0.95]
// ============================================================================
using System;

namespace cSharpRaylib
{
    public static class CCFormatConfig
    {
        // Central single source of truth for the active system version registry specification
        public const string VersionTag = "0.95";

        // Helper property to return uniform file identification token strings dynamically
        public static string MasterEngineHeaderSpec => $"v{VersionTag} MASTER GROUND-TRUTH ENGINE SPECIFICATION";
    }
}