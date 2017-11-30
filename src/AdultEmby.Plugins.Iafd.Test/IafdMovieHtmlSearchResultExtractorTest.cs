using System.Collections.Generic;
using System.IO;
using MediaBrowser.Model.Logging;
using Xunit;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.Iafd;
using AdultEmby.Plugins.TestLogging;
using AngleSharp;
using AngleSharp.Dom;

namespace AdultEmby.Plugins.Iafd.Test
{
    public class IafdMovieHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractMovie()
        {
            IHtmlSearchResultExtractor htmlSearchResultExtractor =
                new IafdMovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(LoadDocument());

            Assert.Equal("title=Anal+Lessons+1/year=2012", results[0].Id);
            Assert.Equal("Anal Lessons 1", results[0].Name);
            Assert.Equal("http://www.iafd.com/title.rme/title=Anal+Lessons+1/year=2012/anal-lessons-1.htm", results[0].Url);
            Assert.Equal(2012, results[0].Year);
            Assert.Equal(null, results[0].ImageUrl);

        }

        private IDocument LoadDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(IafdConstants.BaseUrl)).Result;
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
