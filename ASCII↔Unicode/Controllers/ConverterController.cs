using ASCII_Unicode.Models;
using ASCII_Unicode.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASCII_Unicode.Controllers;

[ApiController]
[Route("")]
public sealed class ConverterController : ControllerBase
{
    private readonly BijoyToUnicodeConverter _bijoyConverter;
    private readonly ConversionMappingsLoader _mappingsLoader;
    private readonly ILoggerFactory _loggerFactory;

    public ConverterController(
        BijoyToUnicodeConverter bijoyConverter,
        ConversionMappingsLoader mappingsLoader,
        ILoggerFactory loggerFactory)
    {
        _bijoyConverter = bijoyConverter;
        _mappingsLoader = mappingsLoader;
        _loggerFactory = loggerFactory;
    }

    [HttpGet("health")]
    public ActionResult<TextResponse> Health()
    {
        return new TextResponse { Text = "ok" };
    }

    [HttpPost("unicode-to-bijoy")]
    public ActionResult<TextResponse> UnicodeToBijoy([FromBody] TextRequest payload)
    {
        var logger = _loggerFactory.CreateLogger<UnicodeConverter>();
        var converter = new UnicodeConverter(_mappingsLoader, logger);
        var result = converter.ConvertUnicodeToBijoy(payload.Text);
        return new TextResponse { Text = result };
    }

    [HttpPost("bijoy-to-unicode")]
    public ActionResult<TextResponse> BijoyToUnicode([FromBody] TextRequest payload)
    {
        var result = _bijoyConverter.ConvertBijoyToUnicode(payload.Text);
        return new TextResponse { Text = result };
    }
}
