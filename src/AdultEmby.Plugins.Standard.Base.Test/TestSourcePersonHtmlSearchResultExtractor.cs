using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Base;
using AngleSharp.Dom;
using MediaBrowser.Model.Logging;

namespace AdultEmby.Plugins.Base.Test
{
    public class TestSourcePersonHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        private ILogger _logger;

        public TestSourcePersonHtmlSearchResultExtractor(ILogManager logManager)
        {
            _logger = _logger = logManager.GetLogger(GetType().FullName);
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            return new List<SearchResult>()
            {
                new SearchResult
                    {
                        Id = "PERSON1_ID",
                        Name ="PERSON1_NAME",
                        Overview = "PERSON1_OVERVIEW",
                        Year = 2011,
                        PremiereDate = new DateTime(2011, 1, 1),
                        Url = "PERSON1_URL",
                        ImageUrl = "PERSON1_IMAGE_URL",
                        Relevance = 0.1
                    },
                new SearchResult
                    {
                        Id = "PERSON2_ID",
                        Name = "PERSON2_NAME",
                        Overview = "PERSON2_OVERVIEW",
                        Year = 2012,
                        PremiereDate = new DateTime(2012, 2, 2),
                        Url = "PERSON2_URL",
                        ImageUrl = "PERSON2_IMAGE_URL",
                        Relevance = 0.2
                    }
            };
        }
    }
}
