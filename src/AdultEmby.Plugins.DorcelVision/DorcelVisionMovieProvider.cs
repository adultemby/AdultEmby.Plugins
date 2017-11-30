using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using System.IO;
using AdultEmby.Plugins.Base;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.DorcelVision
{
    public class DorcelVisionMovieProvider : AdultEmbyMovieProviderBase, IRemoteMetadataProvider<Movie, MovieInfo>, IRemoteImageProvider
    {
        private readonly IHtmlMetadataExtractor _movieHtmlMetadataExtractor;

        public DorcelVisionMovieProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            _movieHtmlMetadataExtractor = new DorcelVisionMovieHtmlMetadataExtractor(logManager);
            SearchResultsHtmlExtractor = new DorcelVisionMovieHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), DorcelVisionConstants.MovieFileCacheName);
        }

        private static string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, DorcelVisionConstants.RootFileCacheName);
        }

        protected override string GetMovieExternalIdName()
        {
            return DorcelVisionMovieId.KeyName;
        }

        protected override string GetPersonExternalIdName()
        {
            return DorcelVisionPersonId.KeyName;
        }

        protected override string ProviderName => Name;

        public string Name => DorcelVisionConstants.ProviderName;

        protected override string SearchUrl => DorcelVisionConstants.BaseUrl + "fr/recherche?type=4&keyword={0}";

        protected override string GetExternalUrl(string id)
        {
            DorcelVisionMovieId providerId = new DorcelVisionMovieId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
