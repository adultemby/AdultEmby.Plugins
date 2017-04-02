using System;
using System.Linq;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Data18
{
    public class Data18 : BasePlugin<Data18Configuration>
    {
        private ILogger _logger;

        public Data18(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogManager logManager) : base(applicationPaths, xmlSerializer)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var descriptionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();
            var description = descriptionAttribute != null ? descriptionAttribute.Description : "UNKNOWN";
            _logger.Info("Starting plugin {0}, version: {1}, revision: {2}", this.GetType().Name, version, description);
        }

        public override string Name => Data18Constants.ProviderName;

        public override string Description => "Gets metadata for adult movies from data18.com";
    }

    public class Data18Configuration : BasePluginConfiguration
    {
    }

    public class Data18Constants
    {
        public const string BaseUrl = "http://www.data18.com/";

        public const string ProviderName = "Data 18";

        public const string RootFileCacheName = "data18";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
