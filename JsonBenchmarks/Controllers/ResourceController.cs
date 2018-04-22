using Jil;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using System;
using System.Text;
using System.Threading.Tasks;

namespace JsonBenchmarks.Controllers
{
    [Route("resource")]
    public class ResourceController : ControllerBase
    {
        [HttpGet("json-default")]
        [Produces("application/json")]
        public Entity JsonDefault() => Constants.Entity;

        [HttpGet("json-formatter")]
        [JilFormatter]
        [Produces("application/json")]
        public Entity JsonJilFormatter() => Constants.Entity;

        [HttpGet("json-actionresult")]
        public IActionResult JsonJilActionResult() => new JsonActionResult(Constants.Entity);

        [HttpGet("csv")]
        public IActionResult Csv() => new CsvActionResult(Constants.Entity.ToStringCsv());

        [HttpGet("bytes")]
        public byte[] Bytes() => Constants.Entity.ToBytes();

        [HttpGet("bytes-actionresult")]
        public IActionResult BytesAction() => new BytesActionResult(Constants.Entity.ToBytes());

        private class BytesActionResult : IActionResult
        {
            private readonly byte[] _payload;
            private readonly int _payloadLength;

            public BytesActionResult(byte[] bytes)
            {
                _payload = bytes;
                _payloadLength = _payload.Length;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status200OK;
                response.ContentLength = _payloadLength;
                return response.Body.WriteAsync(_payload, 0, _payloadLength);
            }
        }

        private class CsvActionResult : IActionResult
        {
            private readonly byte[] _payload;
            private readonly int _payloadLength;

            public CsvActionResult(string csv)
            {
                _payload = Encoding.UTF8.GetBytes(csv);
                _payloadLength = _payload.Length;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status200OK;
                response.ContentType = "text/csv";
                response.ContentLength = _payloadLength;
                return response.Body.WriteAsync(_payload, 0, _payloadLength);
            }
        }

        private class JsonActionResult : IActionResult
        {
            private readonly byte[] _payload;
            private readonly int _payloadLength;

            public JsonActionResult(object @object)
            {
                _payload = Encoding.UTF8.GetBytes(JSON.Serialize(@object));
                _payloadLength = _payload.Length;
            }

            public Task ExecuteResultAsync(ActionContext context)
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status200OK;
                response.ContentType = "application/json";
                response.ContentLength = _payloadLength;
                return response.Body.WriteAsync(_payload, 0, _payloadLength);
            }
        }

    }

    internal class JilOutputFormatter : TextOutputFormatter
    {
        public JilOutputFormatter()
        {
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.TextJson);
            SupportedMediaTypes.Add(MediaTypeHeaderValues.ApplicationAnyJsonSyntax);
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding encoding)
        {
            using (var writer = context.WriterFactory(context.HttpContext.Response.Body, encoding))
            {
                JSON.Serialize(context.Object, writer);

                await writer.FlushAsync();
            }
        }
    }

    internal sealed class JilFormatterAttribute : Attribute, IResultFilter
    {
        private readonly IOutputFormatter _jilFormatter = new JilOutputFormatter();

        public void OnResultExecuted(ResultExecutedContext context)
        {

        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                objectResult.Formatters.Add(_jilFormatter);
            }
        }
    }

    internal class MediaTypeHeaderValues
    {
        public static readonly MediaTypeHeaderValue ApplicationJson
            = MediaTypeHeaderValue.Parse("application/json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue TextJson
            = MediaTypeHeaderValue.Parse("text/json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationJsonPatch
            = MediaTypeHeaderValue.Parse("application/json-patch+json").CopyAsReadOnly();

        public static readonly MediaTypeHeaderValue ApplicationAnyJsonSyntax
            = MediaTypeHeaderValue.Parse("application/*+json").CopyAsReadOnly();
    }
}
