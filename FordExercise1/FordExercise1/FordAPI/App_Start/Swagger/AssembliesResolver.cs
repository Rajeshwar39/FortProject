using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Web.Http.Dispatcher;

namespace FordAPI.App_Start.Swagger
{
    public class AssembliesResolver: DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
            var controllersAssembly = Assembly.LoadFrom(path);
            baseAssemblies.Add(controllersAssembly);
            return assemblies;
        }

        public string path { get; set; }
        public AssembliesResolver(string path)
        {
            this.path = path;
        }
    }
}