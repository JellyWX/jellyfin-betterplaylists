using System;
using System.Collections.Generic;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;


namespace Jellyfin.Plugin.BetterPlaylists
{
    public class Plugin : BasePlugin<BasePluginConfiguration>, IHasWebPages
    {
        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer
            ) : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }


        public override Guid Id => Guid.Parse("05E83046-1C9B-4749-8CCF-55FA6B174045");

        public override string Name => "BetterPlaylists";

        public override string Description => "Persist playlists between filesystem restructures.";

        public static Plugin Instance { get; private set; }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            return new[]
            {
                new PluginPageInfo
                {
                    //Name = "smartplaylist.html",
                    //EmbeddedResourcePath = string.Format("{0}.Configuration.smartplaylist.html", GetType().Namespace),
                }
            };
        }

    }
}
