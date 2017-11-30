using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using System;
using System.IO;
using AdultEmby.Plugins.Base;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.HotMovies
{
    public class HotMoviesMovieProvider : AdultEmbyMovieProviderBase, IRemoteMetadataProvider<Movie, MovieInfo>, IRemoteImageProvider
    {
        private readonly IHtmlMetadataExtractor _movieHtmlMetadataExtractor;

        public HotMoviesMovieProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            _movieHtmlMetadataExtractor = new HotMoviesMovieHtmlMetadataExtractor(logManager);
            SearchResultsHtmlExtractor = new HotMoviesMovieHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), HotMoviesConstants.MovieFileCacheName);
        }

        private static string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, HotMoviesConstants.RootFileCacheName);
        }

        protected override string GetMovieExternalIdName()
        {
            return HotMoviesMovieId.KeyName;
        }

        protected override string GetPersonExternalIdName()
        {
            return HotMoviesPersonId.KeyName;
        }

        protected override string ProviderName => Name;

        public string Name => HotMoviesConstants.ProviderName;

        protected override string SearchUrl => HotMoviesConstants.BaseUrl + "search.php?view_style=classic&page=1&search_string={0}";

        protected override string GetExternalUrl(string id)
        {
            HotMoviesMovieId providerId = new HotMoviesMovieId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
