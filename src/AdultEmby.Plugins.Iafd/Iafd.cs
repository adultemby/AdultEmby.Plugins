using System;
using System.Linq;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Iafd
{
    public class Iafd : BasePlugin<IafdConfiguration>
    {
        private ILogger _logger;

        public Iafd(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogManager logManager) : base(applicationPaths, xmlSerializer)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
            var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            var descriptionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();
            var description = descriptionAttribute != null ? descriptionAttribute.Description : "UNKNOWN";
            _logger.Info("Starting plugin {0}, version: {1}, revision: {2}", this.GetType().Name, version, description);
        }

        public override string Name=> IafdConstants.ProviderName;

        public override string Description => "Gets metadata for adult movies from iafd.com";
    }

    public class IafdConfiguration : BasePluginConfiguration
    {
    }

    public class IafdConstants
    {
        public const string BaseUrl = "http://www.iafd.com/";

        public const string ProviderName = "IAFD";

        public const string RootFileCacheName = "iafd";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
