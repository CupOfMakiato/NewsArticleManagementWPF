using Microsoft.Extensions.DependencyInjection;
using Repository;
using Service;
using System.Configuration;
using System.Data;
using System.Windows;

namespace NguyenLMFall2024WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        ServiceProvider serviceProvider;

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            // Register your services
            services.AddSingleton<ISystemAccountRepository, SystemAccountRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<INewsArticleRepository, NewsArticleRepository>();
            services.AddSingleton<ITagRepository, TagRepository>();

            services.AddSingleton<ISystemAccountService, SystemAccountService>();
            services.AddSingleton<ICategoryService, CategoryService>();
            services.AddSingleton<INewsArticleService, NewsArticleService>();
            services.AddSingleton<ITagService, TagService>();

            services.AddSingleton<Login>();  // Register Login with dependencies

            services.AddSingleton<Admin>();
            services.AddSingleton<AccountDetail>();
            services.AddSingleton<ReportStatistic>();

            services.AddSingleton<ProfileManagement>();

            services.AddSingleton<NewsArticleDetail>();
            services.AddSingleton<NewsArticleManagement>();
            services.AddSingleton<TagManagement>();

            services.AddSingleton<EditProfile>();
            services.AddSingleton<CategoryManagement>();
            services.AddSingleton<CategoryDetail>();
            services.AddSingleton<NewsHistory>();
            
            

            serviceProvider = serviceProvider ?? services.BuildServiceProvider();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Resolve and show the Login window with its dependencies
            var loginWindow = serviceProvider.GetService<Login>();
            loginWindow?.ShowDialog();
        }
    }
}
