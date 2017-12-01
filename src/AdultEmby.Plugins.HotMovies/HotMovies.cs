using System;
using System.Linq;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMovies : BasePlugin<HotMoviesConfiguration>
    {
        private ILogger _logger;

        public HotMovies(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer, ILogManager logManager) : base(applicationPaths, xmlSerializer)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
            //var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            //var descriptionAttribute = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false).OfType<AssemblyDescriptionAttribute>().FirstOrDefault();
            //var description = descriptionAttribute != null ? descriptionAttribute.Description : "UNKNOWN";
            //_logger.Info("Starting plugin {0}, version: {1}, revision: {2}", this.GetType().Name, version, description);
        }

        public override string Name=> HotMoviesConstants.ProviderName;

        public override string Description => "Gets metadata for adult movies from hotmovies.com";
    }

    public class HotMoviesConfiguration : BasePluginConfiguration
    {
    }

    public class HotMoviesConstants
    {
        public const string BaseUrl = "http://www.hotmovies.com/";

        public const string ProviderName = "HotMovies";

        public const string RootFileCacheName = "hotmovies";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
