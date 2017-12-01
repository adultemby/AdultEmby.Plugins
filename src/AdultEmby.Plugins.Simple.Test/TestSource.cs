using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Simple.Test
{
    public class TestSource : BasePlugin<TestSourceConfiguration>
    {
        public TestSource(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
        }

        public override string Name => TestSourceConstants.ProviderName;

        public override string Description => "Used for unit testing Base classes";
    }

    public class TestSourceConfiguration : BasePluginConfiguration
    {
    }

    public class TestSourceConstants
    {
        public const string BaseUrl = "http://www.example.com/";

        public const string ProviderName = "TestSource";

        public const string RootFileCacheName = "testsource";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";
    }
}
