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
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Net;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Controller.Entities;
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
    public class DorcelVisionPersonProviderTest
    {
        private const string BreeOlsonId = "bree_olson";
        private const string SearchTerm = "Bree Olson";

        [Fact]
        public async void ShouldRetrievePersonIfIdIsProvided()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(BreeOlsonId))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonResponse.html")));

            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\PersonResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            DorcelVisionPersonProvider personProvider = new DorcelVisionPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo person = new PersonLookupInfo();
            person.ProviderIds.Add(DorcelVisionPersonId.KeyName, BreeOlsonId);

            MetadataResult<Person> result = await personProvider.GetMetadata(person, cancellationToken);

            //Assert.Equal("", result.It);
            Assert.Equal("Nikita Bellucci", result.Item.Name);
            Assert.Equal(null, result.Item.PlaceOfBirth);
            Assert.Equal(null, result.Item.ProductionYear);
            //Assert.Equal(new DateTime(1986, 10, 7), result.Item.PremiereDate);
            //Assert.Equal(ImageType.Primary, images.First().Type);
            Assert.Equal("bree_olson", result.Item.GetProviderId(DorcelVisionPersonId.KeyName));
            Assert.Equal("Française", result.Item.Overview);
        }

        [Fact]
        public async void ShouldParsePersonSearchResponse()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonSearchResponse.html")));

            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            CancellationToken cancellationToken = new CancellationToken();

            DorcelVisionPersonProvider personProvider = new DorcelVisionPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo searchInfo = new PersonLookupInfo()
            {
                Name = SearchTerm
            };

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("Nikita (a)", searchResult.Name);
            Assert.Equal("actrices-x/nikita-a", searchResult.GetProviderId(DorcelVisionPersonId.KeyName));
            Assert.Equal("https://www.dorcelvision.com/images/actorslist/697412.jpg", searchResult.ImageUrl);
        }

        [Fact]
        public async void ShouldRetrievePersonImages()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(BreeOlsonId))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonResponse.html")));

            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\PersonResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            DorcelVisionPersonProvider personProvider = new DorcelVisionPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            Person person = new Person();
            person.ProviderIds.Add(DorcelVisionPersonId.KeyName, BreeOlsonId);

            IEnumerable<RemoteImageInfo> result = await personProvider.GetImages(person, cancellationToken);

            Assert.Equal("https://www.dorcelvision.com/images/actors/812774.jpg", result.ElementAt(0).Url);
        }

        private String UrlForPerson(string personId)
        {
            return DorcelVisionConstants.BaseUrl + personId;
        }

        private String UrlForSearch(string searchTerms)
        {
            return DorcelVisionConstants.BaseUrl + "fr/recherche?type=2&keyword=" + WebUtility.UrlEncode(searchTerms);
        }

        private IServerConfigurationManager ConfigurationManager()
        {
            IApplicationPaths applicationPaths = Substitute.For<IApplicationPaths>();
            IServerConfigurationManager serverConfigurationManager = Substitute.For<IServerConfigurationManager>();
            applicationPaths.CachePath.Returns("CacheRoot");
            return serverConfigurationManager;
        }

        private IJsonSerializer JsonSerializer()
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.DeserializeFromFile<PersonResult>(Arg.Any<string>()/*"cachePath"*/).Returns(new PersonResult()
            {
                HasMetadata = true,
                Name = "Nikita Bellucci",
                PrimaryImageUrl = "https://www.dorcelvision.com/images/actors/812774.jpg",
                Nationality = "Française"
            });
            return jsonSerializer;
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
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
    }
}
