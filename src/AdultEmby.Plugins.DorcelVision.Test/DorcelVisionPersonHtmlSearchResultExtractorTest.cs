using System.Collections.Generic;
using System.IO;
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
    public class DorcelVisionPersonHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractResult()
        {
            DorcelVisionPersonHtmlSearchResultExtractor htmlSearchResultExtractor =
                new DorcelVisionPersonHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(loadHtmlDocument());

            Assert.Equal(8, results.Count);
            Assert.Equal("actrices-x/nikita-a", results[0].Id);
            Assert.Equal("Nikita (a)", results[0].Name);
            Assert.Equal("https://www.dorcelvision.com/fr/actrices-x/nikita-a", results[0].Url);
            Assert.Equal("https://www.dorcelvision.com/images/actorslist/697412.jpg", results[0].ImageUrl);

        }

        private IDocument loadHtmlDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\PersonSearchResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(DorcelVisionConstants.BaseUrl)).Result;
        }

        private ILogManager LogManager()
        {
            return TestLogManager.Instance;
        }
    }
}