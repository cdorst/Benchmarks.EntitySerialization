using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks
{
    public class AspNetStartup
    {
        public void ConfigureServices(IServiceCollection services) => services.AddMvcCore().AddJsonFormatters();

        public void Configure(IApplicationBuilder app) => app.UseMvc();
    }
}
