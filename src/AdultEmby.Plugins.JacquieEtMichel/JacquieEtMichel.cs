using System.Linq;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.JacquieEtMichel
{
    class JacquieEtMichel : BasePlugin<JacquieEtMichelConfiguration>
    {
        private ILogger _logger;

        public JacquieEtMichel(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogManager logManager)
            : base(applicationPaths, xmlSerializer)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var descriptionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();
            var description = descriptionAttribute != null ? descriptionAttribute.Description : "UNKNOWN";
            _logger.Info("Starting plugin {0}, version: {1}, revision: {2}", this.GetType().Name, version, description);
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
