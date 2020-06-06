using Agents_System.BL;
using Autofac;
using Ships_System.DAL;
using Ships_System.PL;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var resolver = builder.Build();
            return resolver;
        }
    }
}
