using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace JsonBenchmarks
{
    public class AspNetStartupUseRouter
    {
        public void ConfigureServices(IServiceCollection services) => services.AddRouting();

        public void Configure(IApplicationBuilder app)
            => app.UseRouter(router
                => router.MapGet("/resource", async context =>
                {
                    var payload = Constants.Entity.ToBytesReadonlyMemory();
                    var response = context.Response;
                    response.StatusCode = StatusCodes.Status200OK;
                    response.ContentLength = payload.Length;
                    await response.Body.WriteAsync(payload);
                }));
    }
}
