using System.IO;
using AdultEmby.Plugins.Core;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdPersonProvider : AdultEmbyPersonProviderBase, IRemoteMetadataProvider<Person, PersonLookupInfo>, IRemoteImageProvider
    {

        public IafdPersonProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            PersonHtmlExtractor = new IafdPersonHtmlExtractor(logManager);
            SearchResultsHtmlExtractor = new IafdPersonHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), IafdConstants.PeopleFileCacheName);
        }

        private string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, IafdConstants.RootFileCacheName);
        }

        protected override string GetExternalIdName()
        {
            return IafdPersonId.KeyName;
        }

        protected override string GetProviderName()
        {
            return Name; 
        }

        public string Name
        {
            get { return IafdConstants.ProviderName; }
        }

        protected override string SearchUrl => IafdConstants.BaseUrl + "search/?t=1&k={0}";

        protected override string GetExternalUrl(string id)
        {
            IafdPersonId providerId = new IafdPersonId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
