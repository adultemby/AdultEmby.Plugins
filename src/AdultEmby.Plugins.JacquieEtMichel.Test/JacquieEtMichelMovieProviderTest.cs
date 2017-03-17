using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.JacquieEtMichel;
using AdultEmby.Plugins.TestLogging;
using MediaBrowser.Common;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.JacquieEtMichel.Test
{
    public class JacquieEtMichelMovieProviderTest
    {
        private const string AnalLessons1Id = "2265/majalyss";
        private const int AnalLessons1Year = 2016;
        private const string SearchTerm = "Majalyss !";

        [Fact]
        public async void ShouldRetrieveMetadataViaSearchIfIdIsUnknown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();

            JacquieEtMichelMovieProvider metadataProvider =
                new JacquieEtMichelMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.Name = SearchTerm;
            movieInfo.Year = AnalLessons1Year;

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Alessandra, 22ans, prof de fitness russe !", metadataResult.Item.Name);
            //Assert.Equal(8, metadataResult.People.Count);
            Assert.Equal(3, metadataResult.Item.Genres.Count);
            Assert.Equal(1, metadataResult.Item.Studios.Count);
            Assert.Equal(2016, metadataResult.Item.ProductionYear);
            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/4a/db/4adb0409c4db46aa539e1c74cc8f06f5.jpg", metadataResult.Item.ImageInfos[0].Path);
            //Assert.Equal(Release date);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Venant tout droit de Moscou"));
            //Assert.Equal("746183005806", metadataResult.Item.GetProviderId(UpcCodeId.KeyName));
        }

        [Fact]
        public async void ShouldDiscardResultIfNotRelevantWhenRetrieveMetadataViaSearchIfIdIsUnknown()
        {
            string searchTerm = "FOO";
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(searchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();

            JacquieEtMichelMovieProvider metadataProvider =
                new JacquieEtMichelMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.Name = searchTerm;
            movieInfo.Year = 1900;

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            //Assert.True(metadataResult.Item.Name);
        }

        [Fact]
        public async void ShouldRetrieveMetadataDirectlyIfIdIsKnown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            JacquieEtMichelMovieProvider metadataProvider =
                new JacquieEtMichelMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.ProviderIds.Add(JacquieEtMichelId.KeyName, AnalLessons1Id);
            movieInfo.Name = "Anal Lessons #1";

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Alessandra, 22ans, prof de fitness russe !", metadataResult.Item.Name);
            Assert.Equal(0, metadataResult.People.Count);
            Assert.Equal(3, metadataResult.Item.Genres.Count);
            Assert.Equal(1, metadataResult.Item.Studios.Count);
            Assert.Equal(2016, metadataResult.Item.ProductionYear);
            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/4a/db/4adb0409c4db46aa539e1c74cc8f06f5.jpg", metadataResult.Item.ImageInfos[0].Path);
            //Assert.Equal(Release date);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Venant tout droit de Moscou"));
            //Assert.Equal("746183005806", metadataResult.Item.GetProviderId(UpcCodeId.KeyName));
        }

        [Fact]
        public async void ShouldSearchUsingIdIfSupplied()
        {
            IHttpClient httpClient = HttpClient();
            //httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();
            JacquieEtMichelMovieProvider metadataProvider =
                new JacquieEtMichelMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            searchInfo.SetProviderId(JacquieEtMichelId.KeyName, AnalLessons1Id);

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Alessandra, 22ans, prof de fitness russe !", searchResult1993.Name);
            Assert.Equal("2265/majalyss", searchResult1993.GetProviderId(JacquieEtMichelId.KeyName));
            Assert.Equal(2016, searchResult1993.ProductionYear);
            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/4a/db/4adb0409c4db46aa539e1c74cc8f06f5.jpg", searchResult1993.ImageUrl);
        }

        [Fact]
        public async void ShouldSearchUsingParamsIfIdNotSupplied()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));

            JacquieEtMichelMovieProvider metadataProvider =
                new JacquieEtMichelMovieProvider(httpClient, ConfigurationManager(), FileSystem(), LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            CancellationToken cancellationToken = new CancellationToken();

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Majalyss !", searchResult1993.Name);
            Assert.Equal("2265/majalyss", searchResult1993.GetProviderId(JacquieEtMichelId.KeyName));
            Assert.Equal(0, searchResult1993.ProductionYear);
            Assert.Equal("http://m.tv1.cdn.jetm-tech.net/cache/e9/be/e9bedde4135f796e47607888f5a1244d.jpg", searchResult1993.ImageUrl);
        }

        private string UrlForMovie(string movieId)
        {
            return JacquieEtMichelConstants.BaseUrl + "videos/show/" + movieId + ".html";
            // "http://www.jacquieetmicheltv.net/videos/show/2265/majalyss.html"
        }

        private string UrlForSearch(string searchTerms)
        {
            //return Data18MovieConstants.BaseUrl + "search/?t=2&k=" + WebUtility.UrlEncode(searchTerms);

            // "http://www.jacquieetmicheltv.net/recherche/Anal Lessons.html"
            return string.Format(JacquieEtMichelConstants.BaseUrl + "recherche/{0}.html", searchTerms);
        }

        private IServerConfigurationManager ConfigurationManager()
        {
            IApplicationPaths applicationPaths = Substitute.For<IApplicationPaths>();
            IServerConfigurationManager serverConfigurationManager = Substitute.For<IServerConfigurationManager>();
            applicationPaths.CachePath.Returns("CacheRoot");
            return serverConfigurationManager;
        }

        private IApplicationHost ApplicationHost()
        {
            IApplicationHost applicationHost = Substitute.For<IApplicationHost>();
            applicationHost.ApplicationVersion.Returns(new Version("0.0"));
            return applicationHost;
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }

        private IFileSystem FileSystem()
        {
            return Substitute.For<IFileSystem>();
        }

        private IFileSystem FileSystem(string cacheFilename, string contentFilename)
        {
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            Stream streamCache = new MemoryStream();
            Stream processStream = File.OpenRead(contentFilename);
            fileSystem.GetValidFilename("title=Debbie+Does+Dallas+Again/year=1993").Returns("title=Debbie+Does+Dallas+Again year=1993");
            fileSystem.GetFileStream(Arg.Any<string>(), FileOpenMode.Create, FileAccessMode.Write, FileShareMode.Read,
                true).Returns(streamCache);
            fileSystem.GetFileStream(Arg.Any<string>(), FileOpenMode.Open, FileAccessMode.Read, FileShareMode.Read).Returns(processStream);
            FileSystemMetadata fileInfo = new FileSystemMetadata();
            fileInfo.Exists = false;
            fileSystem.GetFileInfo(Arg.Any<string>()).Returns(fileInfo);
            return fileSystem;
        }

        private IJsonSerializer JsonSerializer()
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.DeserializeFromFile<MovieResult>(Arg.Any<string>()/*"cachePath"*/).Returns(new MovieResult()
            {
                HasMetadata = true,
                Title = "Alessandra, 22ans, prof de fitness russe !",
                PrimaryImageUrl = "http://m.tv1.cdn.jetm-tech.net/cache/4a/db/4adb0409c4db46aa539e1c74cc8f06f5.jpg",
                Genres = new List<string>() { "69", "Blonde", "Débutante" },
                Studio = "Jacquie Et Michel",
                ProductionYear = 2016,
                ReleaseDate = new DateTime(2016, 10, 19),
                Synopsis = "Venant tout droit de Moscou, Alessandra est professeur de fitness dans la capitale russe"
            });
            return jsonSerializer;
        }

        private IHttpClient HttpClient()
        {
            return Substitute.For<IHttpClient>();
        }

        private HttpResponseInfo HttpResponseInfo(string contentFilename)
        {
            Stream responseStream = File.OpenRead(contentFilename);
            return new HttpResponseInfo()
            {
                Content = responseStream
            };
        }
    }
}
