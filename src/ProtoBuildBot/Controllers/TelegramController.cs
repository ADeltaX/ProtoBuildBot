using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuildBot;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace ProtoBuildBotWebApi.Controllers
{
    [Route("api/internal/telegram/" + TelegramBotSettings.ApiUrlKey + "/data")]
    public class TelegramController : Controller
    {
        [HttpPost]
        public IActionResult Post([FromBody]Update value)
        {
            _ = Task.Run(() => TelegramBotSettings.TelegramUpdatesCallback(value)).ConfigureAwait(false);
            return Ok();
        }
    }
}
