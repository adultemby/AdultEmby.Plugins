using System.Linq;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.DorcelVision
{
    class DorcelVision : BasePlugin<DorcelVisionConfiguration>
    {
        private ILogger _logger;

        public DorcelVision(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogManager logManager)
            : base(applicationPaths, xmlSerializer)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var descriptionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();
            var description = descriptionAttribute != null ? descriptionAttribute.Description : "UNKNOWN";
            _logger.Info("Starting plugin {0}, version: {1}, revision: {2}", this.GetType().Name, version, description);
        }

        public override string Name => DorcelVisionConstants.ProviderName;

        public override string Description => "Gets metadata for adult movies from dorcelvision.com";
    }

    public class DorcelVisionConfiguration : BasePluginConfiguration
    {
    }

    public class DorcelVisionConstants
    {
        public const string BaseUrl = "https://www.dorcelvision.com/";

        public const string ProviderName = "Dorcel Vision";

        public const string RootFileCacheName = "dorcelvision";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
