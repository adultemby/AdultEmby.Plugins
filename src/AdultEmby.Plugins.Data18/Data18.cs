using System;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Data18
{
    public class Data18 : BasePlugin<Data18Configuration>
    {
        public Data18(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        public override string Name => Data18Constants.ProviderName;

        public override string Description => "Gets metadata for adult movies from data18.com";

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var askedAssembly = new AssemblyName(args.Name);

            var resourceName = string.Format("AdultEmby.Plugins.Assets.Assemblies.{0}.dll",
                askedAssembly.Name);

            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }
                var assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }
    }

    public class Data18Configuration : BasePluginConfiguration
    {
    }

    public class Data18Constants
    {
        public const string BaseUrl = "http://www.data18.com/";

        public const string ProviderName = "Data 18";

        public const string RootFileCacheName = "data18";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
