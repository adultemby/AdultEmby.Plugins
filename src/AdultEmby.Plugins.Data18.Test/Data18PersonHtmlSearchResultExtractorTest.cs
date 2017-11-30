using System.Collections.Generic;
using System.IO;
using AdultEmby.Plugins.Data18;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using Xunit;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.TestLogging;

namespace AdultEmby.Plugins.Data18.Test
{
    public class Data18PersonHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractResult()
        {
            Data18PersonHtmlSearchResultExtractor htmlSearchResultExtractor =
                new Data18PersonHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            Assert.Equal(71, results.Count);
            Assert.Equal("alexis_breeze", results[0].Id);
            Assert.Equal("Alexis Breeze", results[0].Name);
            Assert.Equal("http://www.data18.com/alexis_breeze/", results[0].Url);
            Assert.Equal("http://img.data18.com/images/stars/60/9563.jpg", results[0].ImageUrl);

        }

        private IHtmlDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\PersonSearchResponse.html");
            var parser = new HtmlParser();
            return parser.Parse(responseStream);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
