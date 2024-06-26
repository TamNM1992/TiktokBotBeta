using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartBot;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using static System.Windows.Forms.DataFormats;
using Microsoft.Extensions.Logging;
using SmartBot.Service.Api.Users;
using Microsoft.Extensions.Configuration;
using SmartBot.Common.Configuration;

namespace SmartBot
{
    static class Program
    {
       
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            var time = new DateTime(2024,1,10);
            var month = time.AddMonths(-1).Month;
			var year = time.AddMonths(-1).Year;
            var start = new DateTime(year, month, 1);
			var end = new DateTime(year, month, DateTime.DaysInMonth(year, month));

			var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
            AppConfigs.LoadAll(config);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            services.AddScoped<fMain>();
            ConfigureServices(services);
            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var form1 = serviceProvider.GetRequiredService<fMain>();
            Application.Run(form1);
        }

        private static void ConfigureServices(ServiceCollection services)
        {
            services.AddLogging(configure => configure.AddConsole())
                    .AddScoped<IUsersApiServices, UsersApiServices>();
        }

    }

}
