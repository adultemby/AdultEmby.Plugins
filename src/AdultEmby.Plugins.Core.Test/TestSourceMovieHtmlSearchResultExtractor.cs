using System;
using System.Collections.Generic;
using AdultEmby.Plugins.Core;
using AngleSharp.Dom;
using MediaBrowser.Model.Logging;

namespace AdultEmby.Plugins.Core.Test
{
    public class TestSourceMovieHtmlSearchResultExtractor : IHtmlSearchResultExtractor
    {
        public TestSourceMovieHtmlSearchResultExtractor(ILogManager logManager)
        {
        }
        public List<SearchResult> GetSearchResults(IDocument htmlDocument)
        {
            return new List<SearchResult>()
            {
                new SearchResult
                    {
                        Id = "MOVIE1_ID",
                        Name ="MOVIE1_NAME",
                        Overview = "MOVIE1_OVERVIEW",
                        Year = 2011,
                        PremiereDate = new DateTime(2011, 1, 1),
                        Url = "MOVIE1_URL",
                        ImageUrl = "MOVIE1_IMAGE_URL",
                        Relevance = 0.1
                    },
                new SearchResult
                    {
                        Id = "MOVIE2_ID",
                        Name = "MOVIE2_NAME",
                        Overview = "MOVIE2_OVERVIEW",
                        Year = 2012,
                        PremiereDate = new DateTime(2012, 2, 2),
                        Url = "MOVIE2_URL",
                        ImageUrl = "MOVIE2_IMAGE_URL",
                        Relevance = 0.2
                    }
            };
        }
    }
}
