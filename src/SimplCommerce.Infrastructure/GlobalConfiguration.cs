using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace SimplCommerce.Infrastructure
{
    public static class GlobalConfiguration
    {
        static GlobalConfiguration()
        {
            Modules = LoadModules();
        }

        public static IList<ModuleInfo> Modules { get; }

        public static string WebRootPath { get; set; }

        public static string ContentRootPath { get; set; }

        private static IList<ModuleInfo> LoadModules()
        {
            var modules = new List<ModuleInfo>();

            var binFolder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            if (binFolder.Exists)
            {
                var moduleDlls =
                    binFolder.GetFiles("SimplCommerce.Module.*.dll", SearchOption.TopDirectoryOnly);

                foreach (var dll in moduleDlls)
                {
                    Assembly assembly;
                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(dll.FullName);
                    }
                    catch (FileLoadException)
                    {
                        // Get loaded assembly
                        assembly = Assembly.Load(new AssemblyName(Path.GetFileNameWithoutExtension(dll.Name)));
                    }

                    var moduleName = assembly.GetName().Name;

                    if (!modules.Any(i => i.Name == moduleName))
                    {
                        modules.Add(new ModuleInfo
                        {
                            Name = moduleName,
                            Assembly = assembly
                        });
                    }
                }
            }

            return modules;
        }
    }
}
