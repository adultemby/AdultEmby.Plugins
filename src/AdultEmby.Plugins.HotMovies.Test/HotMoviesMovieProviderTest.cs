using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.HotMovies;
using AdultEmby.Plugins.TestLogging;
using MediaBrowser.Common;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Entities.Movies;
using MediaBrowser.Controller.Providers;
using MediaBrowser.Model.Entities;
using MediaBrowser.Model.IO;
using MediaBrowser.Model.Logging;
using MediaBrowser.Model.Providers;
using MediaBrowser.Model.Serialization;
using NSubstitute;
using Xunit;

namespace AdultEmby.Plugins.HotMovies.Test
{
    public class HotMoviesMovieProviderTest
    {
        private const string BaseUrl = "http://www.hotmovies.com/";
        private const string AnalLessons1Id = "215223";
        private const int AnalLessons1Year = 2012;
        private const string SearchTerm = "Anal Lessons";

        [Fact]
        public async void ShouldRetrieveMetadataViaSearchIfIdIsUnknown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();

            HotMoviesMovieProvider metadataProvider =
                new HotMoviesMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.Name = SearchTerm;
            movieInfo.Year = AnalLessons1Year;

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Anal Lessons (Disc 1)", metadataResult.Item.Name);
            ValidatePerson("Christy Mack", "156190", PersonType.Actor, metadataResult.People[0]);
            ValidatePerson("Jessie Rogers", "149060", PersonType.Actor, metadataResult.People[1]);
            ValidatePerson("Sheena Shaw", "151176", PersonType.Actor, metadataResult.People[2]);
            ValidatePerson("Gabriella Paltrova", "152228", PersonType.Actor, metadataResult.People[3]);
            ValidatePerson("Penny Pax", "154028", PersonType.Actor, metadataResult.People[4]);
            ValidatePerson("Scarlett Wild", "154341", PersonType.Actor, metadataResult.People[5]);
            ValidatePerson("Mike Adriano", "40925", PersonType.Actor, metadataResult.People[6]);
            ValidatePerson("Mike Adriano", null, PersonType.Director, metadataResult.People[7]);
            Assert.Equal(new List<string> { "Anal->M On F", "Gonzo->Anal", "Gonzo->Pov", "Appearance->Big Cocks", "Cum Shots->Facials", "Gonzo->Cumshots & Facials" }, metadataResult.Item.Genres);
            Assert.Equal(new List<string> { "Evil Angel" }, metadataResult.Item.Studios);
            Assert.Equal(new List<string> { "Evil Angel"}, metadataResult.Item.Studios);
            Assert.Equal(2012, metadataResult.Item.ProductionYear);
            Assert.Equal("https://img2.vod.com/image2/cover/215/215223.cover.0.jpg", metadataResult.Item.ImageInfos[0].Path);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("This movie features women that need a lesson in getting"));
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

            HotMoviesMovieProvider metadataProvider =
                new HotMoviesMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

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
            HotMoviesMovieProvider metadataProvider =
                new HotMoviesMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.ProviderIds.Add(HotMoviesMovieId.KeyName, AnalLessons1Id);
            movieInfo.Name = "Anal Lessons #1";

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Anal Lessons (Disc 1)", metadataResult.Item.Name);
            ValidatePerson("Christy Mack", "156190", PersonType.Actor, metadataResult.People[0]);
            ValidatePerson("Jessie Rogers", "149060", PersonType.Actor, metadataResult.People[1]);
            ValidatePerson("Sheena Shaw", "151176", PersonType.Actor, metadataResult.People[2]);
            ValidatePerson("Gabriella Paltrova", "152228", PersonType.Actor, metadataResult.People[3]);
            ValidatePerson("Penny Pax", "154028", PersonType.Actor, metadataResult.People[4]);
            ValidatePerson("Scarlett Wild", "154341", PersonType.Actor, metadataResult.People[5]);
            ValidatePerson("Mike Adriano", "40925", PersonType.Actor, metadataResult.People[6]);
            ValidatePerson("Mike Adriano", null, PersonType.Director, metadataResult.People[7]);
            Assert.Equal(new List<string> { "Anal->M On F", "Gonzo->Anal", "Gonzo->Pov", "Appearance->Big Cocks", "Cum Shots->Facials", "Gonzo->Cumshots & Facials" }, metadataResult.Item.Genres);
            Assert.Equal(new List<string> { "Evil Angel" }, metadataResult.Item.Studios);
            Assert.Equal(new List<string> { "Evil Angel" }, metadataResult.Item.Studios);
            Assert.Equal(2012, metadataResult.Item.ProductionYear);
            Assert.Equal("https://img2.vod.com/image2/cover/215/215223.cover.0.jpg", metadataResult.Item.ImageInfos[0].Path);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("This movie features women that need a lesson in getting"));
        }

        [Fact]
        public async void ShouldSearchUsingIdIfSupplied()
        {
            IHttpClient httpClient = HttpClient();
            //httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();
            HotMoviesMovieProvider metadataProvider =
                new HotMoviesMovieProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            searchInfo.SetProviderId(HotMoviesMovieId.KeyName, AnalLessons1Id);

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Anal Lessons (Disc 1)", searchResult1993.Name);
            Assert.Equal("215223", searchResult1993.GetProviderId(HotMoviesMovieId.KeyName));
            Assert.Equal(2012, searchResult1993.ProductionYear);
            Assert.Equal("https://img2.vod.com/image2/cover/215/215223.cover.0.jpg", searchResult1993.ImageUrl);
        }

        [Fact]
        public async void ShouldSearchUsingParamsIfIdNotSupplied()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\MovieSearchResponse.html")));

            HotMoviesMovieProvider metadataProvider =
                new HotMoviesMovieProvider(httpClient, ConfigurationManager(), FileSystem(), LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            CancellationToken cancellationToken = new CancellationToken();

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Anal Lessons (Disc 1)", searchResult1993.Name);
            Assert.Equal("215223", searchResult1993.GetProviderId(HotMoviesMovieId.KeyName));
            Assert.Equal(2012, searchResult1993.ProductionYear);
            Assert.Equal("https://img3.vod.com/image2/cover/215/215223.cover.0.jpg", searchResult1993.ImageUrl);
        }

        private void ValidatePerson(string name, string id, string type, PersonInfo person)
        {
            Assert.Equal(id, person.GetProviderId(HotMoviesPersonId.KeyName));
            Assert.Equal(name, person.Name);
            Assert.Equal(type, person.Type);
        }

        private string UrlForMovie(string movieId)
        {
            return HotMoviesConstants.BaseUrl + "video/" + movieId;
        }

        private string UrlForSearch(string searchTerms)
        {
            //return Data18MovieConstants.BaseUrl + "search/?t=2&k=" + WebUtility.UrlEncode(searchTerms);
            return HotMoviesConstants.BaseUrl + "search.php?view_style=classic&page=1&search_string=" + WebUtility.UrlEncode(searchTerms);
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
                Title = "Anal Lessons (Disc 1)",
                PrimaryImageUrl = "https://img2.vod.com/image2/cover/215/215223.cover.0.jpg",
                Genres = new List<string>() { "Anal->M On F", "Gonzo->Anal", "Gonzo->Pov", "Appearance->Big Cocks", "Cum Shots->Facials", "Gonzo->Cumshots & Facials" },
                Studio = "Evil Angel",
                ProductionYear = 2012,
                Synopsis = "This movie features women that need a lesson in getting",
                UpcCode = "746183005806",
                Director = new MoviePerson { Name = "Mike Adriano" },
                Actors = new List<MoviePerson>()
                {
                    new MoviePerson { Name = "Christy Mack", Id="156190" },
                    new MoviePerson { Name = "Jessie Rogers", Id="149060" },
                    new MoviePerson { Name = "Sheena Shaw", Id="151176" },
                    new MoviePerson { Name = "Gabriella Paltrova", Id="152228" },
                    new MoviePerson { Name = "Penny Pax", Id="154028" },
                    new MoviePerson { Name = "Scarlett Wild", Id="154341" },
                    new MoviePerson { Name = "Mike Adriano", Id="40925" }
                }
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
