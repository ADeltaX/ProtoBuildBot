using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace ProtoBuildBot
{
    //for asp.net
    public class StartupAsp
    {
        public void ConfigureServices(IServiceCollection services)
        {
            //If we want to use this globally
            //options => options.Filters.Add(typeof(SubdomainFilterAttribute))

            // cache in memory
            services.AddMemoryCache();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
}
