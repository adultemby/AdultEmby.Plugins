using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.TestLogging;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller;
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

namespace AdultEmby.Plugins.Simple.Test
{
    public class AdultEmbyMovieProviderBaseTest
    {
        private const string CacheRoot = "CacheRoot";
        private const string MovieId1 = "MOVIE1_ID";
        private const string SearchTerm = "MOVIE1_NAME";
        private MovieResult _serializedMovieResult = null;
        
        [Fact]
        public async void ShouldRetrieveMetadataFromSourceIfNotCachedLocally()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string testResponseFilename = @"TestResponses\CoreResponse.html";
            string cachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName + "\\movies\\M", MovieId1));
            string jsonCachePath = cachePath + "\\item.json";
            string htmlCachePath = cachePath + "\\item.html";
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\DummyMovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(MovieId1))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(testResponseFilename)));

            IFileSystem fileSystem = FileSystem(htmlCachePath, jsonCachePath, testResponseFilename);

            CancellationToken cancellationToken = new CancellationToken();
            TestSourceMovieProvider movieProvider = new TestSourceMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(jsonCachePath));

            MovieInfo movie = new MovieInfo()
            {
                Name = SearchTerm
            };
            //movie.ProviderIds.Add(TestSourceId.KeyName, MovieId1);

            MetadataResult<Movie> result = await movieProvider.GetMetadata(movie, cancellationToken);

            Assert.Equal("TITLE", result.Item.Name);
            //Assert.Equal("BIRTH_PLACE", result.Item.ProductionLocations[0]);
            Assert.Equal(2010, result.Item.ProductionYear);
            //Assert.Equal(new DateTime(2001, 1, 1), result.Item.PremiereDate);
            Assert.Equal(ImageType.Primary, result.Item.ImageInfos.First().Type);
            Assert.Equal("IMAGE_URL", result.Item.ImageInfos.First().Path);
            Assert.Equal("MOVIE1_ID", result.Item.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("SYNOPSIS", result.Item.Overview);
        }

        [Fact]
        public async void ShouldSearchForMovieByNameIfIdIsNotSupplied()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm)))
                .Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\DummyMovieSearchResponse.html")));
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            CancellationToken cancellationToken = new CancellationToken();

            TestSourceMovieProvider movieProvider = new TestSourceMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), jsonSerializer);

            MovieInfo searchInfo = new MovieInfo()
            {
                Name = SearchTerm
            };

            IEnumerable<RemoteSearchResult> searchResults = await movieProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("MOVIE1_NAME", searchResult.Name);
            Assert.Equal("MOVIE1_ID", searchResult.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("MOVIE1_IMAGE_URL", searchResult.ImageUrl);
            //Assert.Equal("MOVIE1_OVERVIEW", searchResult.Overview);
            Assert.Equal("TestSource", searchResult.SearchProviderName);
            Assert.Equal(2011, searchResult.ProductionYear);
        }

        [Fact]
        public async void SearchingShouldLookupItemInsteadOfSearchingIfIdIsSupplied()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string movieResultCachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName + "\\movies\\M", Path.Combine(MovieId1, "item.json")));
            IFileSystem fileSystem = FileSystemWithCachedItem(movieResultCachePath);
            CancellationToken cancellationToken = new CancellationToken();

            TestSourceMovieProvider movieProvider = new TestSourceMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(movieResultCachePath, MovieResult()));

            MovieInfo searchInfo = new MovieInfo();
            searchInfo.ProviderIds.Add(TestSourceId.KeyName, MovieId1);

            IEnumerable<RemoteSearchResult> searchResults = await movieProvider.GetSearchResults(searchInfo, cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("TITLE", searchResult.Name);
            Assert.Equal("MOVIE1_ID", searchResult.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("IMAGE_URL", searchResult.ImageUrl);
            //Assert.Equal("SYNOPSIS", searchResult.Overview);
            Assert.Equal("TestSource", searchResult.SearchProviderName);
            Assert.Equal(2001, searchResult.ProductionYear);
        }
        [Fact]
        public async void ShouldGetImagesForMovie()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string movieResultCachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName + "\\movies\\M", Path.Combine(MovieId1, "item.json")));
            IFileSystem fileSystem = FileSystemWithCachedItem(movieResultCachePath);
            CancellationToken cancellationToken = new CancellationToken();

            TestSourceMovieProvider movieProvider = new TestSourceMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(movieResultCachePath, MovieResult()));

            Movie movie = new Movie();
            movie.ProviderIds.Add(TestSourceId.KeyName, MovieId1);

            IEnumerable<RemoteImageInfo> imageInfos = await movieProvider.GetImages(movie,
                cancellationToken);

            RemoteImageInfo imageInfo = imageInfos.ElementAt(0);
            Assert.Equal("IMAGE_URL", imageInfo.Url);
            Assert.Equal("TestSource", imageInfo.ProviderName);
            Assert.Equal(ImageType.Primary, imageInfo.Type);
        }

        [Fact]
        public void ShouldSupportMovieType()
        {
            TestSourceMovieProvider movieProvider = new TestSourceMovieProvider(
                Substitute.For<IHttpClient>(),
                Substitute.For<IServerConfigurationManager>(),
                Substitute.For<IFileSystem>(),
                Substitute.For<ILogManager>(),
                Substitute.For<IJsonSerializer>());

            Assert.True(movieProvider.Supports(new Movie()));
        }

        private IJsonSerializer JsonSerializer(string jsonCacheFile)
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.SerializeToFile(Arg.Do<MovieResult>(x => _serializedMovieResult = x), jsonCacheFile);
            jsonSerializer.DeserializeFromFile<MovieResult>(jsonCacheFile).Returns(x => _serializedMovieResult);
            return jsonSerializer;
        }

        private IJsonSerializer JsonSerializer(string jsonCacheFile, MovieResult movieResult)
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.DeserializeFromFile<MovieResult>(jsonCacheFile).Returns(movieResult);
            return jsonSerializer;
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }

        private HttpResponseInfo HttpResponseInfo(string contentFilename)
        {
            Stream responseStream = File.OpenRead(contentFilename);
            return new HttpResponseInfo()
            {
                Content = responseStream
            };
        }

        private String UrlForMovie(string movieId)
        {
            return TestSourceConstants.BaseUrl + movieId;
        }

        private String UrlForSearch(string searchTerms)
        {
            //            "http://www.example.com/search.php?view_style=classic&page=1&search_string=MOVIE+ONE"
            //"http://www.example.com/search.php?view_style=classic&page=1&search_string=NAME"
            return TestSourceConstants.BaseUrl + "search.php?view_style=classic&page=1&search_string=" + WebUtility.UrlEncode(searchTerms);
        }

        private IServerConfigurationManager ConfigurationManager()
        {
            IServerApplicationPaths applicationPaths = Substitute.For<IServerApplicationPaths>();
            applicationPaths.CachePath.Returns(CacheRoot);
            IServerConfigurationManager serverConfigurationManager = Substitute.For<IServerConfigurationManager>();
            serverConfigurationManager.ApplicationPaths.Returns(applicationPaths);
            return serverConfigurationManager;
        }

        private IFileSystem FileSystem(string htmlCachePath, string jsonCachePath, string testResponseFilename)
        {
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetValidFilename(MovieId1).Returns(MovieId1);

            Stream htmlCacheStream = new MemoryStream();
            Stream httpResponseStream = File.OpenRead(testResponseFilename);

            fileSystem.GetFileStream(htmlCachePath, FileOpenMode.Create, FileAccessMode.Write, FileShareMode.Read,
                true).Returns(htmlCacheStream);
            fileSystem.GetFileStream(htmlCachePath, FileOpenMode.Open, FileAccessMode.Read, FileShareMode.Read).Returns(httpResponseStream);
            FileSystemMetadata fileInfo = new FileSystemMetadata()
            {
                Exists = false
            };
            fileSystem.GetFileInfo(jsonCachePath).Returns(fileInfo);
            return fileSystem;
        }

        private IFileSystem FileSystemWithCachedItem(string jsonCachePath)
        {
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetValidFilename(MovieId1).Returns(MovieId1);

            FileSystemMetadata fileInfo = new FileSystemMetadata()
            {
                Exists = true
            };
            fileSystem.GetFileInfo(jsonCachePath).Returns(fileInfo);
            fileSystem.GetLastWriteTimeUtc(fileInfo).Returns(DateTime.Now.AddDays(-1));
            return fileSystem;
        }

        private MovieResult MovieResult()
        {
            return new MovieResult()
            {
                HasMetadata = true,
                Title = "TITLE",
                Id = "ACTOR1_ID",
                Synopsis = "SYNOPSIS",
                PrimaryImageUrl = "IMAGE_URL",
                Genres = new List<string> {"HEIGHT"},
                ProductionYear = 2001
            };
        }
    }
}
