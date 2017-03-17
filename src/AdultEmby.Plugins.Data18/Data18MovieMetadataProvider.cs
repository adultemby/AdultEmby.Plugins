using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Model.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Data18
{
    public class Data18MovieMetadataProvider : AdultEmbyMovieProviderBase, IRemoteMetadataProvider<Movie, MovieInfo>, IRemoteImageProvider
    {
        private readonly IHtmlMetadataExtractor _movieHtmlMetadataExtractor;
        private readonly IHtmlMetadataExtractor _contentHtmlMetadataExtractor;

        public Data18MovieMetadataProvider(IHttpClient httpClient, IServerConfigurationManager configurationManager, IFileSystem fileSystem, ILogManager logManager, IJsonSerializer jsonSerializer)
            : base(httpClient, configurationManager, fileSystem, logManager, jsonSerializer)
        {
            _movieHtmlMetadataExtractor = new Data18MovieHtmlMetadataExtractor(logManager);
            _contentHtmlMetadataExtractor = new Data18ContentHtmlMetadataExtractor(logManager);
            SearchResultsHtmlExtractor = new Data18MovieHtmlSearchResultExtractor(logManager);
        }

        protected override async Task<HttpResponseInfo> HandleSecurityPassthrough(HttpResponseInfo httpResponseInfo, string url, CancellationToken cancellationToken)
        {
            if (httpResponseInfo.ContentType.Contains("text/html"))
            {
                string html = null;
                using (var reader = new StreamReader(httpResponseInfo.Content, Encoding.UTF8))
                {
                    html = reader.ReadToEnd();
                }
                if (html.Contains("Security page: [data18.com]"))
                {
                    Logger.Info("Found security page for data 18; attempting to bypass");
                    IDocument htmlDocument = HtmlParser.Parse(html);
                    IHtmlAnchorElement bypassAnchorElement = (IHtmlAnchorElement) htmlDocument.QuerySelector("a");
                    if (bypassAnchorElement != null)
                    {
                        //HttpRequestOptions options = this.CreateHttpRequestOptions(url, cancellationToken);
                        //return await HttpClient.GetResponse(options);
                        Logger.Info("Invoking bypass url [{0}]", bypassAnchorElement.Href);
                        await InternalGetResponse(bypassAnchorElement.Href, false, cancellationToken);
                        Logger.Info("Invoked bypass url [{0}], requesting original url [{1}]", bypassAnchorElement.Href, url);
                        return await InternalGetResponse(url, false, cancellationToken);
                    }
                }
                httpResponseInfo.Content = new MemoryStream(Encoding.UTF8.GetBytes(html ?? ""));
            }
            else
            {
                await Task.FromResult(true);
            }
            return httpResponseInfo;
        }

        protected override string GetItemCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string movieId)
        {
            return GetMovieOrContentNumberCachePath(fileSystem, appPaths, movieId);
        }

        private static string GetMovieOrContentNumberCachePath(IFileSystem fileSystem, IApplicationPaths appPaths, string movieId)
        {
            //var letter = movieId.GetMD5().ToString().Substring(0, 1);
            string[] parts = movieId.Split('/');
            string type = parts[0];
            string id = parts[1];
            var letter = id.Substring(0, 1);
            return Path.Combine(GetMovieOrContentCachePath(appPaths, type), letter, fileSystem.GetValidFilename(id));
        }

        private static string GetMovieOrContentCachePath(IApplicationPaths appPaths, string type)
        {
            if (type == "movies")
            {
                return Path.Combine(GetRootCachePath(appPaths), Data18Constants.MovieFileCacheName);
            }
            else if (type == "content")
            {
                return Path.Combine(GetRootCachePath(appPaths), Data18Constants.ContentFileCacheName);
            }
            throw new Exception("Unsupported type: " + type);
        }

        protected override IHtmlMetadataExtractor GetMovieHtmlExtractor(string movieId)
        {
            IHtmlMetadataExtractor extractor = null;
            if (movieId.StartsWith("movies"))
            {
                extractor = _movieHtmlMetadataExtractor;
            }
            else if (movieId.StartsWith("content"))
            {
                extractor = _contentHtmlMetadataExtractor;
            }
            return extractor;
        }

        private static string GetRootCachePath(IApplicationPaths appPaths)
        {
            return Path.Combine(appPaths.CachePath, Data18Constants.RootFileCacheName);
        }

        protected override string GetMovieExternalIdName()
        {
            return Data18MovieId.KeyName;
        }

        protected override string GetPersonExternalIdName()
        {
            return Data18PersonId.KeyName;
        }

        protected override string ProviderName => Name;

        public string Name => Data18Constants.ProviderName;

        protected override string SearchUrl => Data18Constants.BaseUrl + "search/?k={0}";

        protected override string GetExternalUrl(string id)
        {
            Data18MovieId providerId = new Data18MovieId();
            return string.Format(providerId.UrlFormatString, id);
        }
    }
}
