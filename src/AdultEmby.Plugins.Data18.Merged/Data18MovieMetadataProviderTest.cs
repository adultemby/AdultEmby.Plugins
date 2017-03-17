using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.Data18;
using AdultEmby.Plugins.Test.Logging;
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

namespace AdultEmby.Plugins.Test.Data18
{
    public class Data18MovieMetadataProviderTest
    {
        private const string AnalLessons1Id = "movies/1105977";
        private const int AnalLessons1Year = 2012;
        private const string SearchTerm = "Anal Lessons";

        [Fact]
        public async void ShouldRetrieveMetadataViaSearchIfIdIsUnknown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\Data18\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();

            Data18MovieMetadataProvider metadataProvider =
                new Data18MovieMetadataProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.Name = SearchTerm;
            movieInfo.Year = AnalLessons1Year;

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Anal Lessons #1", metadataResult.Item.Name);
            ValidatePerson("Christy Mack", "christy_mack", PersonType.Actor, metadataResult.People[0]);
            ValidatePerson("Gabriella Paltrova", "gabriella_paltrova", PersonType.Actor, metadataResult.People[1]);
            ValidatePerson("Jessie Rogers", "jessie_rogers", PersonType.Actor, metadataResult.People[2]);
            ValidatePerson("Melina Mason", "melina_mason", PersonType.Actor, metadataResult.People[3]);
            ValidatePerson("Mike Adriano", "mike_adriano", PersonType.Actor, metadataResult.People[4]);
            ValidatePerson("Penny Pax", "penny_pax", PersonType.Actor, metadataResult.People[5]);
            ValidatePerson("Scarlett Wild", "scarlett_wild", PersonType.Actor, metadataResult.People[6]);
            ValidatePerson("Sheena Shaw", "sheena_shaw", PersonType.Actor, metadataResult.People[7]);
            ValidatePerson("Mike Adriano", null, PersonType.Director, metadataResult.People[8]);
            Assert.Equal(new List<string> { "Anal", "Gonzo" }, metadataResult.Item.Genres);
            Assert.Equal(new List<string> { "Evil Angel" }, metadataResult.Item.Studios);
            Assert.Equal(2012, metadataResult.Item.ProductionYear);
            Assert.Equal("http://img.data18.com/covers/1/1/22604_front.jpg", metadataResult.Item.ImageInfos[0].Path);
            Assert.Equal(new DateTime(2012, 7, 1), metadataResult.Item.PremiereDate);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Director and stud"));
            Assert.Equal("746183005806", metadataResult.Item.GetProviderId(UpcCodeId.KeyName));
        }

        [Fact]
        public async void ShouldRetrieveMetadataDirectlyIfIdIsKnown()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\Data18\MovieResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            Data18MovieMetadataProvider metadataProvider =
                new Data18MovieMetadataProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo movieInfo = new MovieInfo();
            movieInfo.ProviderIds.Add(Data18MovieId.KeyName, AnalLessons1Id);
            movieInfo.Name = "Anal Lessons #1";

            MetadataResult<Movie> metadataResult = await metadataProvider.GetMetadata(movieInfo, cancellationToken);

            Assert.Equal("Anal Lessons #1", metadataResult.Item.Name);
            ValidatePerson("Christy Mack", "christy_mack", PersonType.Actor, metadataResult.People[0]);
            ValidatePerson("Gabriella Paltrova", "gabriella_paltrova", PersonType.Actor, metadataResult.People[1]);
            ValidatePerson("Jessie Rogers", "jessie_rogers", PersonType.Actor, metadataResult.People[2]);
            ValidatePerson("Melina Mason", "melina_mason", PersonType.Actor, metadataResult.People[3]);
            ValidatePerson("Mike Adriano", "mike_adriano", PersonType.Actor, metadataResult.People[4]);
            ValidatePerson("Penny Pax", "penny_pax", PersonType.Actor, metadataResult.People[5]);
            ValidatePerson("Scarlett Wild", "scarlett_wild", PersonType.Actor, metadataResult.People[6]);
            ValidatePerson("Sheena Shaw", "sheena_shaw", PersonType.Actor, metadataResult.People[7]);
            ValidatePerson("Mike Adriano", null, PersonType.Director, metadataResult.People[8]);
            Assert.Equal(new List<string> { "Anal", "Gonzo" }, metadataResult.Item.Genres);
            Assert.Equal(new List<string> { "Evil Angel" }, metadataResult.Item.Studios);
            Assert.Equal(2012, metadataResult.Item.ProductionYear);
            Assert.Equal("http://img.data18.com/covers/1/1/22604_front.jpg", metadataResult.Item.ImageInfos[0].Path);
            Assert.Equal(new DateTime(2012, 7, 1), metadataResult.Item.PremiereDate);
            // Rating
            Assert.True(metadataResult.Item.Overview.StartsWith("Director and stud"));
            Assert.Equal("746183005806", metadataResult.Item.GetProviderId(UpcCodeId.KeyName));
        }

        [Fact]
        public async void ShouldSearchUsingIdIfSupplied()
        {
            IHttpClient httpClient = HttpClient();
            //httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieSearchResponse.html")));
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForMovie(AnalLessons1Id))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieResponse.html")));
            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\Data18\MovieResponse.html");
            CancellationToken cancellationToken = new CancellationToken();
            Data18MovieMetadataProvider metadataProvider =
                new Data18MovieMetadataProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            searchInfo.SetProviderId(Data18MovieId.KeyName, AnalLessons1Id);

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Anal Lessons #1", searchResult1993.Name);
            Assert.Equal("movies/1105977", searchResult1993.GetProviderId(Data18MovieId.KeyName));
            Assert.Equal(2012, searchResult1993.ProductionYear);
            Assert.Equal("http://img.data18.com/covers/1/1/22604_front.jpg", searchResult1993.ImageUrl);
        }

        [Fact]
        public async void ShouldSearchUsingParamsIfIdNotSupplied()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\Data18\MovieSearchResponse.html")));

            Data18MovieMetadataProvider metadataProvider =
                new Data18MovieMetadataProvider(httpClient, ConfigurationManager(), FileSystem(), LogManager(), JsonSerializer());

            MovieInfo searchInfo = new MovieInfo
            {
                Name = SearchTerm,
                Year = AnalLessons1Year
            };
            CancellationToken cancellationToken = new CancellationToken();

            IEnumerable<RemoteSearchResult> searchResults = await metadataProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult1993 = searchResults.ElementAt(0);
            Assert.Equal("Anal Lessons #1", searchResult1993.Name);
            Assert.Equal("movies/1105977", searchResult1993.GetProviderId(Data18MovieId.KeyName));
            Assert.Equal(2012, searchResult1993.ProductionYear);
            Assert.Equal("http://img.data18.com/covers/1/4/22604_front.jpg", searchResult1993.ImageUrl);
        }

        private void ValidatePerson(string name, string id, string type, PersonInfo person)
        {
            Assert.Equal(id, person.GetProviderId(Data18PersonId.KeyName));
            Assert.Equal(name, person.Name);
            Assert.Equal(type, person.Type);
        }

        private string UrlForMovie(string movieId)
        {
            return Data18Constants.BaseUrl + movieId;
        }

        private string UrlForSearch(string searchTerms)
        {
            //return Data18MovieConstants.BaseUrl + "search/?t=2&k=" + WebUtility.UrlEncode(searchTerms);
            return Data18Constants.BaseUrl + "search/?k=" + WebUtility.UrlEncode(searchTerms);
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
            fileSystem.GetValidFilename(cacheFilename);
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
                Title = "Anal Lessons #1",
                PrimaryImageUrl = "http://img.data18.com/covers/1/1/22604_front.jpg",
                Genres = new List<string>() {"Anal", "Gonzo"},
                Studio = "Evil Angel",
                ProductionYear = 2012,
                ReleaseDate = new DateTime(2012, 7, 1),
                Synopsis = "Director and stud Mike Adriano is not only an obsessive devotee of anal fucking",
                UpcCode = "746183005806",
                Director = new MoviePerson {Name = "Mike Adriano" },
                Actors = new List<MoviePerson>()
                {
                    new MoviePerson { Name = "Christy Mack", Id="christy_mack" },
                    new MoviePerson { Name = "Gabriella Paltrova", Id="gabriella_paltrova" },
                    new MoviePerson { Name = "Jessie Rogers", Id="jessie_rogers" },
                    new MoviePerson { Name = "Melina Mason", Id="melina_mason" },
                    new MoviePerson { Name = "Mike Adriano", Id="mike_adriano" },
                    new MoviePerson { Name = "Penny Pax", Id="penny_pax" },
                    new MoviePerson { Name = "Scarlett Wild", Id="scarlett_wild" },
                    new MoviePerson { Name = "Sheena Shaw", Id="sheena_shaw" }
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
