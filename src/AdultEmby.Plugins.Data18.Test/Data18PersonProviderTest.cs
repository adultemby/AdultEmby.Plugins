using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.Data18;
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
//using NLog;

namespace AdultEmby.Plugins.Data18.Test
{
    public class Data18PersonProviderTest
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
            Data18PersonProvider personProvider = new Data18PersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo person = new PersonLookupInfo();
            person.ProviderIds.Add(Data18PersonId.KeyName, BreeOlsonId);

            MetadataResult<Person> result = await personProvider.GetMetadata(person, cancellationToken);

            //Assert.Equal("", result.It);
            Assert.Equal("Bree Olson", result.Item.Name);
            Assert.Equal("Houston, TX", result.Item.PlaceOfBirth);
            Assert.Equal(1986, result.Item.ProductionYear);
            //Assert.Equal(new DateTime(1986, 10, 7), result.Item.PremiereDate);
            //Assert.Equal(ImageType.Primary, images.First().Type);
            Assert.Equal("bree_olson", result.Item.GetProviderId(Data18PersonId.KeyName));
            Assert.Equal("5ft 3in 161.5 cm.<br/>124 lbs 56 kg<br/>34C-28-36<br/>United States<br/>Caucasian<br/>Libra", result.Item.Overview);
        }

        [Fact]
        public async void ShouldParsePersonSearchResponse()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonSearchResponse.html")));

            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            CancellationToken cancellationToken = new CancellationToken();

            Data18PersonProvider personProvider = new Data18PersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            PersonLookupInfo searchInfo = new PersonLookupInfo()
            {
                Name = SearchTerm
            };

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("Alexis Breeze", searchResult.Name);
            Assert.Equal("alexis_breeze", searchResult.GetProviderId(Data18PersonId.KeyName));
            Assert.Equal("http://img.data18.com/images/stars/60/9563.jpg", searchResult.ImageUrl);
        }

        [Fact]
        public async void ShouldRetrievePersonImages()
        {
            IHttpClient httpClient = HttpClient();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(BreeOlsonId))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\PersonResponse.html")));

            IFileSystem fileSystem = FileSystem("title=Debbie+Does+Dallas+Again/year=1993", @"TestResponses\PersonResponse.html");

            CancellationToken cancellationToken = new CancellationToken();
            Data18PersonProvider personProvider = new Data18PersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer());

            Person person = new Person();
            person.ProviderIds.Add(Data18PersonId.KeyName, BreeOlsonId);

            IEnumerable<RemoteImageInfo> result = await personProvider.GetImages(person, cancellationToken);

            Assert.Equal("http://img.data18.com/images/stars/120/255.jpg", result.ElementAt(0).Url);
        }

        //[Fact]
       /* public ILogManager LogManager()
        {
            //string logdir = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\Logz";
            //NlogManager logManager = new NlogManager();
            return NlogManager.Instance;
            //ILogger logger = logManager.GetLogger("FOO");
            //logger.Error("WWWWWWW");
//            NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
            //var logger = LogManager.GetCurrentClassLogger();
            //LogManager.GetCurrentClassLogger()
            //logger.Debug("This is a debug message!");
        }*/

        private String UrlForPerson(string personId)
        {
            return Data18Constants.BaseUrl + personId;
        }

        private String UrlForSearch(string searchTerms)
        {
            return Data18Constants.BaseUrl + "search/?t=1&k=" + WebUtility.UrlEncode(searchTerms);
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
                PrimaryImageUrl = "http://img.data18.com/images/stars/120/255.jpg",
                Birthplace = "Houston, TX",
                Birthdate = new DateTime(1986, 10, 7),
                Height = "5ft 3in 161.5 cm.",
                Weight = "124 lbs 56 kg",
                Measurements = "34C-28-36",
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
