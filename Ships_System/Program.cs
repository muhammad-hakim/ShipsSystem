using Autofac;
using Ships_System.DAL;
using Ships_System.PL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Ships_System
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var resolver = InitializeDependecies();
            Application.Run(resolver.Resolve<MainScreen>());
        }

        static IContainer InitializeDependecies()
        {
            ContainerBuilder builder = new ContainerBuilder();
            
            builder.RegisterType<SystemContext>().AsSelf();
            builder.RegisterType<UnitOfWork>().AsSelf();
            builder.RegisterType<MainScreen>().AsSelf();
            foreach (var item in Assembly.GetExecutingAssembly().GetExportedTypes().Where(t => t.Namespace == "Ships_System.BL" && t.IsClass))
            {
                builder.RegisterType(item).As(item.GetInterface("I"+item.Name));
            } 

            var resolver = builder.Build();
            return resolver;
        }
    }
}
