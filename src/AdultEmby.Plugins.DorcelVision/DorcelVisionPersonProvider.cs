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

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionPersonProvider : AdultEmbyPersonProviderBase, IRemoteMetadataProvider<Person, PersonLookupInfo>, IRemoteImageProvider
    {
        public DorcelVisionPersonProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            PersonHtmlExtractor = new DorcelVisionPersonHtmlExtractor(logManager);
            SearchResultsHtmlExtractor = new DorcelVisionPersonHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), DorcelVisionConstants.PeopleFileCacheName);
        }

        private string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, DorcelVisionConstants.RootFileCacheName);
        }

        protected override string GetExternalIdName()
        {
            return DorcelVisionPersonId.KeyName;
        }

        protected override string GetProviderName()
        {
            return Name; 
        }

        public string Name => DorcelVisionConstants.ProviderName;

        protected override string SearchUrl => DorcelVisionConstants.BaseUrl + "fr/recherche?type=2&keyword={0}";

        protected override string GetExternalUrl(string id)
        {
            DorcelVisionPersonId providerId = new DorcelVisionPersonId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
