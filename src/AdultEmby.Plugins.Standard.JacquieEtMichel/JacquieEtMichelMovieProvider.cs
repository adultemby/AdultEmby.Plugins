using System;
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

namespace AdultEmby.Plugins.JacquieEtMichel
{
    public class JacquieEtMichelMovieProvider : AdultEmbyMovieProviderBase, IRemoteMetadataProvider<Movie, MovieInfo>, IRemoteImageProvider
    {
        private readonly IHtmlMetadataExtractor _movieHtmlMetadataExtractor;

        public JacquieEtMichelMovieProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            _movieHtmlMetadataExtractor = new JacquieEtMichelMovieHtmlMetadataExtractor(logManager);
            SearchResultsHtmlExtractor = new JacquieEtMichelMovieHtmlSearchResultExtractor(logManager);
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
            return Path.Combine(GetRootCachePath(appPaths), JacquieEtMichelConstants.MovieFileCacheName);
        }

        private static string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, JacquieEtMichelConstants.RootFileCacheName);
        }

        protected override string GetMovieExternalIdName()
        {
            return JacquieEtMichelId.KeyName;
        }

        protected override string GetPersonExternalIdName()
        {
            throw new NotSupportedException();
        }

        protected override string ProviderName => Name;

        public string Name => JacquieEtMichelConstants.ProviderName;

        protected override string SearchUrl => JacquieEtMichelConstants.BaseUrl + "recherche/{0}.html";

        protected override string GetExternalUrl(string id)
        {
            JacquieEtMichelId providerId = new JacquieEtMichelId();
            return string.Format(providerId.UrlFormatString, id);
        }

        protected override bool EncodeSearchTerm =>  false;
    }
}
