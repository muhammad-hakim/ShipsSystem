using Autofac;
using Ships_System.DAL;
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
            InitializeDependecies();
            Application.Run(new PL.MainScreen());
        }

        static void InitializeDependecies()
        {
            ContainerBuilder builder = new ContainerBuilder();

            builder.RegisterType<SystemContext>().AsSelf();
            builder.RegisterType<UnitOfWork>().AsSelf();

            var resolver = builder.Build();
        }
    }
}
