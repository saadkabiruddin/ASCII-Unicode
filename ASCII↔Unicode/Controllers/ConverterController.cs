using ASCII_Unicode.Models;
using ASCII_Unicode.Services;
using Microsoft.AspNetCore.Mvc;

namespace ASCII_Unicode.Controllers;

[ApiController]
[Route("")]
public sealed class ConverterController : ControllerBase
{
    [HttpGet("health")]
    public ActionResult<TextResponse> Health()
    {
        return new TextResponse { Text = "ok" };
    }

    [HttpPost("unicode-to-bijoy")]
    public ActionResult<TextResponse> UnicodeToBijoy([FromBody] TextRequest payload)
    {
        var result = new UnicodeConverter().ConvertUnicodeToBijoy(payload.Text);
        return new TextResponse { Text = result };
    }

    [HttpPost("bijoy-to-unicode")]
    public ActionResult<TextResponse> BijoyToUnicode([FromBody] TextRequest payload)
    {
        var result = new BijoyToUnicodeConverter().ConvertBijoyToUnicode(payload.Text);
        return new TextResponse { Text = result };
    }
}
