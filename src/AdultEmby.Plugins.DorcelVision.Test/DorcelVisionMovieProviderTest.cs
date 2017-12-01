using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.DorcelVision;
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

namespace AdultEmby.Plugins.DorcelVision.Test
{
    public class DorcelVisionMovieProviderTest
    {
        private const string AnalLessons1Id = "marc-dorcel/pornochic-27-superstars";
        private const int AnalLessons1Year = 2016;
        private const string SearchTerm = "Pornochic 27 - Superstars";

        [Fact]
        public async void ShouldRetrieveMetadataViaSearchIfIdIsUnknown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();

            DorcelVisionMovieProvider metadataProvider =
                new DorcelVisionMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.Name = SearchTerm;
            movieInfo.Year = AnalLessons1Year;

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("La Transporteuse", metadataResult.Item.Name);
            Assert.Equal(7, metadataResult.People.Count);
            Assert.Equal(5, metadataResult.Item.Genres.Count);
            Assert.Equal(1, metadataResult.Item.Studios.Length);
            Assert.Equal(2015, metadataResult.Item.ProductionYear);
            Assert.Equal("https://www.dorcelvision.com/images/endless3/703161.jpg", metadataResult.Item.ImageInfos[0].Path);
            //Assert.Equal(Release date);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Kimber et son garde du corps Jason forment"));
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

            DorcelVisionMovieProvider metadataProvider =
                new DorcelVisionMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

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
            DorcelVisionMovieProvider metadataProvider =
                new DorcelVisionMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.ProviderIds.Add(DorcelVisionMovieId.KeyName, AnalLessons1Id);
            movieInfo.Name = "La Transporteuse";

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("La Transporteuse", metadataResult.Item.Name);
            Assert.Equal(7, metadataResult.People.Count);
            Assert.Equal(5, metadataResult.Item.Genres.Count);
            Assert.Equal(1, metadataResult.Item.Studios.Length);
            Assert.Equal(2015, metadataResult.Item.ProductionYear);
            Assert.Equal("https://www.dorcelvision.com/images/endless3/703161.jpg", metadataResult.Item.ImageInfos[0].Path);
            //Assert.Equal(Release date);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Kimber et son garde du corps Jason forment"));
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
            DorcelVisionMovieProvider metadataProvider =
                new DorcelVisionMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            searchInfo.SetProviderId(DorcelVisionMovieId.KeyName, AnalLessons1Id);

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("La Transporteuse", searchResult1993.Name);
            Assert.Equal("marc-dorcel/pornochic-27-superstars", searchResult1993.GetProviderId(DorcelVisionMovieId.KeyName));
            Assert.Equal(2015, searchResult1993.ProductionYear);
            Assert.Equal("https://www.dorcelvision.com/images/endless3/703161.jpg", searchResult1993.ImageUrl);
        }

        [Fact]
        public async void ShouldSearchUsingParamsIfIdNotSupplied()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));

            DorcelVisionMovieProvider metadataProvider =
                new DorcelVisionMovieProvider(httpClient, ConfigurationManager(), FileSystem(), LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            CancellationToken cancellationToken = new CancellationToken();

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Pornochic 27 - Superstars", searchResult1993.Name);
            Assert.Equal("marc-dorcel/pornochic-27-superstars", searchResult1993.GetProviderId(DorcelVisionMovieId.KeyName));
            Assert.Equal(0, searchResult1993.ProductionYear);
            Assert.Equal("https://www.dorcelvision.com/images/endless6/741966.jpg", searchResult1993.ImageUrl);
        }

        private string UrlForMovie(string movieId)
        {
            return DorcelVisionConstants.BaseUrl + "fr/films/" + movieId;
        }

        private string UrlForSearch(string searchTerms)
        {
            return string.Format(DorcelVisionConstants.BaseUrl + "fr/recherche?type=4&keyword={0}", WebUtility.UrlEncode(searchTerms));
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
                Title = "La Transporteuse",
                PrimaryImageUrl = "https://www.dorcelvision.com/images/endless3/703161.jpg",
                Genres = new List<string>() { "Anal", "Cunnilingus", "Levrette", "Levrette debout", "Missionnaire" },
                Studio = "Fred Coppula Prod",
                ProductionYear = 2015,
                Synopsis = "Kimber et son garde du corps Jason forment le duo de choc d'une agence sensée régler les problèmes sexuels des couples.",
                UpcCode = "746183005806",
                Director = new MoviePerson { Name = "D1" },
                Actors = new List<MoviePerson>() { new MoviePerson { Name = "1" }, new MoviePerson { Name = "2" }, new MoviePerson { Name = "3" }, new MoviePerson { Name = "4" }, new MoviePerson { Name = "5" }, new MoviePerson { Name = "6" } }
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
