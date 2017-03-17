using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using System;
using System.IO;
using AdultEmby.Plugins.Core;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Iafd
{
    public class IafdMovieProvider : AdultEmbyMovieProviderBase, IRemoteMetadataProvider<Movie, MovieInfo>, IRemoteImageProvider
    {
        private readonly IHtmlMetadataExtractor _movieHtmlMetadataExtractor;

        public IafdMovieProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            _movieHtmlMetadataExtractor = new IafdMovieHtmlMetadataExtractor(logManager);
            SearchResultsHtmlExtractor = new IafdMovieHtmlSearchResultExtractor(logManager);
        }
        
        protected override string GetItemCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string movieId)
        {
            return GetMovieOrContentNumberCachePath(fileSystem, appPaths, movieId);
        }

        protected override IHtmlMetadataExtractor GetMovieHtmlExtractor(string movieId)
        {
            return _movieHtmlMetadataExtractor;
        }

        private static string GetMovieOrContentNumberCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string movieId)
        {
            //var letter = movieId.GetMD5().ToString().Substring(0, 1);
            //string[] parts = movieId.Split('/');
            //string type = parts[0];
            //string id = parts[1];
            var letter = movieId.Substring(0, 1);
            return Path.Combine(GetMovieOrContentCachePath(appPaths), letter, fileSystem.GetValidFilename(movieId));
        }

        private static string GetMovieOrContentCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(GetRootCachePath(appPaths), IafdConstants.MovieFileCacheName);
        }

        private static string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, IafdConstants.RootFileCacheName);
        }

        protected override string GetMovieExternalIdName()
        {
            return IafdMovieId.KeyName;
        }

        protected override string GetPersonExternalIdName()
        {
            return IafdPersonId.KeyName;
        }

        protected override string ProviderName => Name;

        public string Name => IafdConstants.ProviderName;

        protected override string SearchUrl => IafdConstants.BaseUrl + "search.php?view_style=classic&page=1&search_string={0}";

        protected override string GetExternalUrl(string id)
        {
            IafdMovieId providerId = new IafdMovieId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
