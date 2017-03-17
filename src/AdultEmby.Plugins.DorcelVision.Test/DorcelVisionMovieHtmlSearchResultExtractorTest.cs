using System.Collections.Generic;
using System.IO;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Core;
using AdultEmby.Plugins.DorcelVision;
using AdultEmby.Plugins.TestLogging;
using AngleSharp;
using AngleSharp.Dom;

namespace AdultEmby.Plugins.DorcelVision.Test
{
    public class DorcelVisionMovieHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractMovie()
        {
            DorcelVisionMovieHtmlSearchResultExtractor htmlSearchResultExtractor =
                new DorcelVisionMovieHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadDocument());

            Assert.Equal("marc-dorcel/pornochic-20-anna-polina", results[0].Id);
            Assert.Equal("Pornochic 20 - Anna Polina", results[0].Name);
            Assert.Equal("https://www.dorcelvision.com/fr/films/marc-dorcel/pornochic-20-anna-polina", results[0].Url);
            //Assert.Equal(2017, results[0].Year);
            Assert.Equal("https://www.dorcelvision.com/images/endless3/8005.jpg", results[0].ImageUrl);

        }

        private IDocument loadDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(DorcelVisionConstants.BaseUrl)).Result;
            //Stream responseStream = File.OpenRead(@"TestResponses\MovieSearchResponse.html");
            //var parser = new HtmlParser();
            //return parser.Parse(document);
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}
