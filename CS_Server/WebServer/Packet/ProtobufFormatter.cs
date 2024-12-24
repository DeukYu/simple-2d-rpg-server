using Google.Protobuf;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;
using ServerCore;

namespace WebServer.Packet;
public class ProtobufInputFormatter : InputFormatter
{
    public ProtobufInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-protobuf"));
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
    {
        var httpContext = context.HttpContext;
        var type = context.ModelType;

        try
        {
            using var stream = new MemoryStream();
            await httpContext.Request.Body.CopyToAsync(stream);
            stream.Seek(0, SeekOrigin.Begin);

            if(Activator.CreateInstance(type) is IMessage message)
            {
                message.MergeFrom(stream);
                return await InputFormatterResult.SuccessAsync(message);
            }

            return await InputFormatterResult.FailureAsync();
        }
        catch (Exception e)
        {
            Log.Error($"Failed to read request body. Error: {e.Message}");
            return await InputFormatterResult.FailureAsync();
        }
    }

    protected override bool CanReadType(Type type)
    {
        return type != null && typeof(IMessage).IsAssignableFrom(type);
    }
}

public class ProtobufOutputFormatter : OutputFormatter
{
    public ProtobufOutputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/x-protobuf"));
    }

    public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
    {
        var httpContext = context.HttpContext;

        if (context.Object is IMessage message)
        {
            using var stream = new MemoryStream();
            message.WriteTo(stream);
            stream.Seek(0, SeekOrigin.Begin);

            await stream.CopyToAsync(httpContext.Response.Body);
        }
    }

    protected override bool CanWriteType(Type type)
    {
        return type != null && typeof(IMessage).IsAssignableFrom(type);
    }
}