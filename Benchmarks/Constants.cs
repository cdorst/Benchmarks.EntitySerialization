using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Benchmarks
{
    public static class Constants
    {
        public static readonly byte[] Bytes = new Entity().ToBytes();
        public static readonly Entity Entity = new Entity();
        public static readonly int EntityId = new Entity().EntityId;
        public static readonly Jil.Options JilOptions = new Jil.Options(excludeNulls: true);
        public static readonly string Json = Jil.JSON.Serialize(Entity, JilOptions);
        public static readonly HttpClient Server = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AspNetStartup>().ConfigureLogging(logging => logging.ClearProviders())).CreateClient();
        public static readonly HttpClient ServerUsingRouter = new TestServer(WebHost.CreateDefaultBuilder().UseStartup<AspNetStartupUseRouter>().ConfigureLogging(logging => logging.ClearProviders())).CreateClient();
    }
}
