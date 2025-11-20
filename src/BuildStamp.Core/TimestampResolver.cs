// ============================================================================
// File:        TimestampResolver.cs
// Project:     Solcogito.BuildStamp.Core
// Description: Provides UTC timestamp resolution for stamping.
// ============================================================================

using System;

namespace Solcogito.BuildStamp.Core
{
    public static class TimestampResolver
    {
        public static DateTime ResolveUtcTimestamp()
        {
            // Pure deterministic UTC timestamp
            return DateTime.UtcNow;
        }
    }
}
