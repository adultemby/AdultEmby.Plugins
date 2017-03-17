using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.JacquieEtMichel
{
    class JacquieEtMichel : BasePlugin<JacquieEtMichelConfiguration>
    {
        public JacquieEtMichel(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        public override string Name => JacquieEtMichelConstants.ProviderName;

        public override string Description => "Gets metadata for adult movies from jacquieetmicheltv.net";
    }

    public class JacquieEtMichelConfiguration : BasePluginConfiguration
    {
    }

    public class JacquieEtMichelConstants
    {
        public const string BaseUrl = "http://www.jacquieetmicheltv.net/";

        public const string ProviderName = "Jacquie Et Michel";

        public const string RootFileCacheName = "jacquieetmichel";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
