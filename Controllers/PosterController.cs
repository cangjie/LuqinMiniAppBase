using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace LuqinMiniAppBase.Controllers
{
    [Route("pages/[controller]/[action]")]
    public class PosterController : Controller
    {
        [HttpGet]
        public IActionResult Index(string posterScene = "")
        {
            ViewData["title"] = "海报下载";
            return View("/Views/Poster.cshtml");
        }
    }
}