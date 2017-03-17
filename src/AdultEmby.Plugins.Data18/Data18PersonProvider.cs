using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Core;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Data18
{
    public class Data18PersonProvider : AdultEmbyPersonProviderBase, IRemoteMetadataProvider<Person, PersonLookupInfo>, IRemoteImageProvider
    {

        public Data18PersonProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            PersonHtmlExtractor = new Data18PersonHtmlExtractor(logManager);
            SearchResultsHtmlExtractor = new Data18PersonHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), Data18Constants.PeopleFileCacheName);
        }

        private string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, Data18Constants.RootFileCacheName);
        }

        protected override string GetExternalIdName()
        {
            return Data18PersonId.KeyName;
        }

        protected override string GetProviderName()
        {
            return Name; 
        }

        public string Name
        {
            get { return Data18Constants.ProviderName; }
        }

        protected override string SearchUrl => Data18Constants.BaseUrl + "search/?t=1&k={0}";

        protected override string GetExternalUrl(string id)
        {
            Data18PersonId providerId = new Data18PersonId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
