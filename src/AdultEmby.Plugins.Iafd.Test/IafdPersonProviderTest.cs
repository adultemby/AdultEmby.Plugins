using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Base;
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

namespace AdultEmby.Plugins.Iafd.Test
{
    public class IafdPersonProviderTest
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
            IafdPersonProvider personProvider = new IafdPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo person = new PersonLookupInfo();
            person.ProviderIds.Add(IafdPersonId.KeyName, BreeOlsonId);

            MetadataResult<Person> result = await personProvider.GetMetadata(person, cancellationToken);

            //Assert.Equal("", result.It);
            Assert.Equal("Bree Olson", result.Item.Name);
            Assert.Equal("Houston, Texas United States", result.Item.ProductionLocations[0]);
            Assert.Equal(1986, result.Item.ProductionYear);
            //Assert.Equal(new DateTime(1986, 10, 7), result.Item.PremiereDate);
            //Assert.Equal(ImageType.Primary, images.First().Type);
            Assert.Equal("bree_olson", result.Item.GetProviderId(IafdPersonId.KeyName));
            Assert.Equal("5ft 3in<br/>32D<br/>United States<br/>Caucasian<br/>Libra", result.Item.Overview);
        }

        [Fact]
        public async void ShouldParsePersonSearchResponse()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonSearchResponse.html")));

            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            CancellationToken cancellationToken = new CancellationToken();

            IafdPersonProvider personProvider = new IafdPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo searchInfo = new PersonLookupInfo()
            {
                Name = SearchTerm
            };

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("Jessie Andrews", searchResult.Name);
            Assert.Equal("perfid=jessieandrews/gender=f", searchResult.GetProviderId(IafdPersonId.KeyName));
            Assert.Equal("http://www.iafd.com/graphics/headshots/thumbs/th_jessieandrews_f_jessieandrews_na.jpg", searchResult.ImageUrl);
        }

        [Fact]
        public async void ShouldRetrievePersonImages()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(BreeOlsonId))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonResponse.html")));

            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\PersonResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            IafdPersonProvider personProvider = new IafdPersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            Person person = new Person();
            person.ProviderIds.Add(IafdPersonId.KeyName, BreeOlsonId);

            IEnumerable<RemoteImageInfo> result = await personProvider.GetImages(person, cancellationToken);

            Assert.Equal("https://img2.vod.com/image2/star/488/Bree_Olson-4887.10627212671415801793large.jpg", result.ElementAt(0).Url);
        }

        private String UrlForPerson(string personId)
        {
            return IafdConstants.BaseUrl + personId;
        }

        private String UrlForSearch(string searchTerms)
        {
            // "http://www.hotmovies.com/search/?t=1&k=Bree+Olson"
            return IafdConstants.BaseUrl + "search/?t=1&k=" + WebUtility.UrlEncode(searchTerms);
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
                Name = "Bree Olson",
                PrimaryImageUrl = "https://img2.vod.com/image2/star/488/Bree_Olson-4887.10627212671415801793large.jpg",
                Birthplace = "Houston, Texas United States",
                Birthdate = new DateTime(1986, 10, 7),
                Height = "5ft 3in",
                Measurements = "32D",
                Nationality = "United States",
                Ethnicity = "Caucasian",
                StarSign = "Libra"
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
