using System;
using System.Reflection;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace AdultEmby.Plugins.Iafd
{
    public class Iafd : BasePlugin<IafdConfiguration>
    {
        public Iafd(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer) : base(applicationPaths, xmlSerializer)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        public override string Name=> IafdConstants.ProviderName;

        public override string Description => "Gets metadata for adult movies from iafd.com";

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

    public class IafdConfiguration : BasePluginConfiguration
    {
    }

    public class IafdConstants
    {
        public const string BaseUrl = "http://www.iafd.com/";

        public const string ProviderName = "IAFD";

        public const string RootFileCacheName = "iafd";

        public const string PeopleFileCacheName = "people";

        public const string MovieFileCacheName = "movies";

        public const string ContentFileCacheName = "content";

        //public const string CacheItemName = "item.html";
    }
}
