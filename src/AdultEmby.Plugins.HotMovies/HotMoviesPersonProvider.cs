using System.IO;
using AdultEmby.Plugins.Base;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesPersonProvider : AdultEmbyPersonProviderBase, IRemoteMetadataProvider<Person, PersonLookupInfo>, IRemoteImageProvider
    {

        public HotMoviesPersonProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            PersonHtmlExtractor = new HotMoviesPersonHtmlExtractor(logManager);
            SearchResultsHtmlExtractor = new HotMoviesPersonHtmlSearchResultExtractor(logManager);
        }

        protected override string GetItemCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string personId)
        {
            return GetPersonLetterCachePath(fileSystem, appPaths, personId);
        }

        private string GetPersonLetterCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string personId)
        {
            //var letter = personId.GetMD5().ToString().Substring(0, 1);
            var letter = personId.Substring(0, 1);
            return Path.Combine(GetPeopleCachePath(appPaths), letter, fileSystem.GetValidFilename(personId));
        }

        private string GetPeopleCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(GetRootCachePath(appPaths), HotMoviesConstants.PeopleFileCacheName);
        }

        private string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, HotMoviesConstants.RootFileCacheName);
        }

        protected override string GetExternalIdName()
        {
            return HotMoviesPersonId.KeyName;
        }

        protected override string GetProviderName()
        {
            return Name; 
        }

        public string Name
        {
            get { return HotMoviesConstants.ProviderName; }
        }

        protected override string SearchUrl => HotMoviesConstants.BaseUrl + "search/?t=1&k={0}";

        protected override string GetExternalUrl(string id)
        {
            HotMoviesPersonId providerId = new HotMoviesPersonId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
