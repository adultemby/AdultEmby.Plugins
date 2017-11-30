using System.Collections.Generic;
using System.IO;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using MediaBrowser.Model.Logging;
using NSubstitute;
using Xunit;
using AdultEmby.Plugins.Base;
using AdultEmby.Plugins.Iafd;
using AdultEmby.Plugins.TestLogging;
using AngleSharp;
using AngleSharp.Dom;

namespace AdultEmby.Plugins.Iafd.Test
{
    public class IafdPersonHtmlSearchResultExtractorTest
    {
        [Fact]
        public void ShouldExtractResult()
        {
            IHtmlSearchResultExtractor htmlSearchResultExtractor =
                new IafdPersonHtmlSearchResultExtractor(LogManager());

            List<SearchResult> results = htmlSearchResultExtractor.GetSearchResults(LoadDocument());

            Assert.Equal(134, results.Count);
            Assert.Equal("perfid=jessieandrews/gender=f", results[0].Id);
            Assert.Equal("Jessie Andrews", results[0].Name);
            Assert.Equal("http://www.iafd.com/person.rme/perfid=jessieandrews/gender=f/jessie-andrews.htm", results[0].Url);
            Assert.Equal("http://www.iafd.com/graphics/headshots/thumbs/th_jessieandrews_f_jessieandrews_na.jpg", results[0].ImageUrl);

        }

        private IDocument LoadDocument()
        {
            Stream responseStream = File.OpenRead(@"TestResponses\PersonSearchResponse.html");
            return BrowsingContext.New().OpenAsync(m => m.Content(responseStream).Status(200).Address(IafdConstants.BaseUrl)).Result;
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
