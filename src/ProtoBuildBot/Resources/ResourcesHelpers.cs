using System;
using System.Collections.Generic;
using System.Text;

namespace ProtoBuildBot.Resources
{
    public static class ResourcesHelpers
    {
        /// <summary>
        /// Dictionary of | Language Code - Language |
        /// </summary>
        public static Dictionary<string, string> GetAvailableLanguages { get; } = new Dictionary<string, string>
        {
            { "en-US", "🌎 English" },
            { "it-IT", "🇮🇹 Italiano" },
            { "de-DE", "🇩🇪 Deutsch" },
            { "bem", "🥖 Baguette (DON'T)" }
        };
    }
}
