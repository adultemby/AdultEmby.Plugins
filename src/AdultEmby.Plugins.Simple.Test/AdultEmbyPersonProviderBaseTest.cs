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
using MediaBrowser.Controller.Entities;
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
    public class AdultEmbyPersonProviderBaseTest
    {
        private const string CacheRoot = "CacheRoot";
        private const string ActorId1 = "ACTOR1_ID";
        private const string SearchTerm = "ACTOR ONE";
        private PersonResult _serializedPersonResult = null;

        [Fact]
        public async void ShouldRetrieveMetadataFromSourceIfNotCachedLocally()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string testResponseFilename = @"TestResponses\CoreResponse.html";
            string cachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName, ActorId1));
            string jsonCachePath = cachePath + "\\item.json";
            string htmlCachePath = cachePath + "\\item.html";
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(ActorId1))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(testResponseFilename)));

            IFileSystem fileSystem = FileSystem(htmlCachePath, jsonCachePath, testResponseFilename);

            CancellationToken cancellationToken = new CancellationToken();
            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(jsonCachePath));

            PersonLookupInfo person = new PersonLookupInfo();
            person.ProviderIds.Add(TestSourceId.KeyName, ActorId1);

            MetadataResult<Person> result = await personProvider.GetMetadata(person, cancellationToken);

            Assert.Equal("NAME", result.Item.Name);
            Assert.Equal("BIRTH_PLACE", result.Item.ProductionLocations[0]);
            Assert.Equal(2001, result.Item.ProductionYear);
            Assert.Equal(new DateTime(2001, 1, 1), result.Item.PremiereDate);
            Assert.Equal(ImageType.Primary, result.Item.ImageInfos.First().Type);
            Assert.Equal("IMAGE_URL", result.Item.ImageInfos.First().Path);
            Assert.Equal("ACTOR1_ID", result.Item.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("HEIGHT<br/>WEIGHT<br/>MEASUREMENTS<br/>NATIONALITY<br/>ETHNICITY<br/>STARSIGN", result.Item.Overview);
        }

        [Fact]
        public async void ShouldSearchForPersonByNameIfIdIsNotSupplied()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm)))
                .Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\DummyPersonSearchResponse.html")));
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            CancellationToken cancellationToken = new CancellationToken();

            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), jsonSerializer);

            PersonLookupInfo searchInfo = new PersonLookupInfo()
            {
                Name = SearchTerm
            };

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo,
                cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("PERSON1_NAME", searchResult.Name);
            Assert.Equal("PERSON1_ID", searchResult.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("PERSON1_IMAGE_URL", searchResult.ImageUrl);
            Assert.Equal("PERSON1_OVERVIEW", searchResult.Overview);
            Assert.Equal("TestSource", searchResult.SearchProviderName);
            Assert.Equal(2011, searchResult.ProductionYear);
        }

        [Fact]
        public async void SearchShouldReturnEmptyListIfNameIsEmptyAndIdIsNotSupplied()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForSearch(SearchTerm)))
                .Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(@"TestResponses\DummyPersonSearchResponse.html")));
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            CancellationToken cancellationToken = new CancellationToken();

            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), jsonSerializer);

            PersonLookupInfo searchInfo = new PersonLookupInfo()
            {
                Name = ""
            };

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo,
                cancellationToken);

            Assert.False(searchResults.Any());
        }

        [Fact]
        public async void SearchingShouldLookupItemInsteadOfSearchingIfIdIsSupplied()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string personResultCachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName, Path.Combine(ActorId1, "item.json")));
            IFileSystem fileSystem = FileSystemWithCachedItem(personResultCachePath);
            CancellationToken cancellationToken = new CancellationToken();

            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(personResultCachePath, PersonResult()));

            PersonLookupInfo searchInfo = new PersonLookupInfo();
            searchInfo.ProviderIds.Add(TestSourceId.KeyName, ActorId1);

            IEnumerable<RemoteSearchResult> searchResults = await personProvider.GetSearchResults(searchInfo, cancellationToken);

            RemoteSearchResult searchResult = searchResults.ElementAt(0);
            Assert.Equal("NAME", searchResult.Name);
            Assert.Equal("ACTOR1_ID", searchResult.GetProviderId(TestSourceId.KeyName));
            Assert.Equal("IMAGE_URL", searchResult.ImageUrl);
            Assert.Equal("HEIGHT<br/>WEIGHT<br/>MEASUREMENTS<br/>NATIONALITY<br/>ETHNICITY<br/>STARSIGN", searchResult.Overview);
            Assert.Equal("TestSource", searchResult.SearchProviderName);
            Assert.Equal(2001, searchResult.ProductionYear);
        }

        [Fact]
        public async void ShouldGetImagesForPerson()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string personResultCachePath = Path.Combine(CacheRoot, Path.Combine(TestSourceConstants.RootFileCacheName, Path.Combine(ActorId1, "item.json")));
            IFileSystem fileSystem = FileSystemWithCachedItem(personResultCachePath);
            CancellationToken cancellationToken = new CancellationToken();

            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), fileSystem, LogManager(), JsonSerializer(personResultCachePath, PersonResult()));

            Person person = new Person();
            person.ProviderIds.Add(TestSourceId.KeyName, ActorId1);

            IEnumerable<RemoteImageInfo> imageInfos = await personProvider.GetImages(person,
                cancellationToken);

            RemoteImageInfo imageInfo = imageInfos.ElementAt(0);
            Assert.Equal("IMAGE_URL", imageInfo.Url);
            Assert.Equal("TestSource", imageInfo.ProviderName);
            Assert.Equal(ImageType.Primary, imageInfo.Type);
        }

        [Fact]
        public async void ShouldGetImageResponseForPerson()
        {
            IHttpClient httpClient = Substitute.For<IHttpClient>();
            string testResponseFilename = @"TestResponses\CoreResponse.html";
            httpClient.GetResponse(Arg.Is<HttpRequestOptions>(options => options.Url == UrlForPerson(ActorId1))).Returns(Task.FromResult<HttpResponseInfo>(HttpResponseInfo(testResponseFilename)));

            CancellationToken cancellationToken = new CancellationToken();
            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(httpClient, ConfigurationManager(), Substitute.For<IFileSystem>(), LogManager(), Substitute.For<IJsonSerializer>());

            Person person = new Person();
            person.ProviderIds.Add(TestSourceId.KeyName, ActorId1);

            HttpResponseInfo httpResponseInfo = await personProvider.GetImageResponse(UrlForPerson(ActorId1),
                cancellationToken);
            
            //Assert.Equal("IMAGE_URL", httpResponseInfo.ResponseUrl);
            //Assert.Equal("TestSource", httpResponseInfo..ProviderName);
            //Assert.Equal(ImageType.Primary, imageInfo.Type);
        }

        [Fact]
        public void ShouldSupportPersonType()
        {
            TestSourcePersonProvider personProvider = new TestSourcePersonProvider(
                Substitute.For<IHttpClient>(), 
                Substitute.For<IServerConfigurationManager>(), 
                Substitute.For<IFileSystem>(), 
                Substitute.For<ILogManager>(), 
                Substitute.For<IJsonSerializer>());

            Assert.True(personProvider.Supports(new Person()));
        }

        private String UrlForPerson(string personId)
        {
            return TestSourceConstants.BaseUrl + personId;
        }

        private String UrlForSearch(string searchTerms)
        {
            return TestSourceConstants.BaseUrl + "search/?t=1&k=" + WebUtility.UrlEncode(searchTerms);
        }

        private IServerConfigurationManager ConfigurationManager()
        {
            IServerApplicationPaths applicationPaths = Substitute.For<IServerApplicationPaths>();
            applicationPaths.CachePath.Returns(CacheRoot);
            IServerConfigurationManager serverConfigurationManager = Substitute.For<IServerConfigurationManager>();
            serverConfigurationManager.ApplicationPaths.Returns(applicationPaths);
            return serverConfigurationManager;
        }

        private IJsonSerializer JsonSerializer(string jsonCacheFile)
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.SerializeToFile(Arg.Do<PersonResult>(x => _serializedPersonResult = x), jsonCacheFile);
            jsonSerializer.DeserializeFromFile<PersonResult>(jsonCacheFile).Returns(x => _serializedPersonResult);
            return jsonSerializer;
        }

        private IJsonSerializer JsonSerializer(string jsonCacheFile, PersonResult personResult)
        {
            IJsonSerializer jsonSerializer = Substitute.For<IJsonSerializer>();
            jsonSerializer.DeserializeFromFile<PersonResult>(jsonCacheFile).Returns(personResult);
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

        private IFileSystem FileSystem(string htmlCachePath, string jsonCachePath, string testResponseFilename)
        {
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetValidFilename(ActorId1).Returns(ActorId1);

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
            fileSystem.GetValidFilename(ActorId1).Returns(ActorId1);

            FileSystemMetadata fileInfo = new FileSystemMetadata()
            {
                Exists = true
            };
            fileSystem.GetFileInfo(jsonCachePath).Returns(fileInfo);
            fileSystem.GetLastWriteTimeUtc(fileInfo).Returns(DateTime.Now.AddDays(-1));
            return fileSystem;
        }

        private IFileSystem FileSystem2(string jsonCachePath)
        {
            IFileSystem fileSystem = Substitute.For<IFileSystem>();
            fileSystem.GetValidFilename(ActorId1).Returns(ActorId1);
            
            FileSystemMetadata fileInfo = new FileSystemMetadata()
            {
                Exists = true
            };
            fileSystem.GetFileInfo(jsonCachePath).Returns(fileInfo);
            return fileSystem;
        }

        private PersonResult PersonResult()
        {
            return new PersonResult()
            {
                HasMetadata = true,
                Name = "NAME",
                Id = "ACTOR1_ID",
                PrimaryImageUrl = "IMAGE_URL",
                Height = "HEIGHT",
                Weight = "WEIGHT",
                Measurements = "MEASUREMENTS",
                Nationality = "NATIONALITY",
                Ethnicity = "ETHNICITY",
                StarSign = "STARSIGN",
                Birthdate = new DateTime(2001, 1, 1)
            };
        }
    }
}
