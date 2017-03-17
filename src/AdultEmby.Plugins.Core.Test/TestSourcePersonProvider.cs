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

namespace AdultEmby.Plugins.Core.Test
{
    public class TestSourcePersonProvider : AdultEmbyPersonProviderBase, IRemoteMetadataProvider<Person, PersonLookupInfo>, IRemoteImageProvider
    {

        public TestSourcePersonProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            PersonHtmlExtractor = new TestSourcePersonHtmlExtractor(logManager);
            SearchResultsHtmlExtractor = new TestSourcePersonHtmlSearchResultExtractor(logManager);
        }

        protected override string GetItemCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string personId)
        {
            return Path.Combine(appPaths.CachePath, Path.Combine(TestSourceConstants.RootFileCacheName, fileSystem.GetValidFilename(personId)));
        }

        protected override string GetExternalIdName()
        {
            return TestSourceId.KeyName;
        }

        protected override string GetProviderName()
        {
            return Name; 
        }

        public string Name
        {
            get { return TestSourceConstants.ProviderName; }
        }

        protected override string SearchUrl => TestSourceConstants.BaseUrl + "search/?t=1&k={0}";

        protected override string GetExternalUrl(string id)
        {
            TestSourceId providerId = new TestSourceId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
