using System;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LuqinMiniAppBase.Models;


namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        public MediaController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Meida>> GetMedia(int id)
        {
            Meida media = await _db.meida.FindAsync(id);
            Meida ret = new Meida()
            {
                id = media.id,
                name = media.name,
                type = media.type,
                thumb = media.thumb
            };
            return ret;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> PlayMedia(int id, string sessionKey)
        {
            sessionKey = Util.UrlDecode(sessionKey);
            Meida media = await _db.meida.FindAsync(id);

            if (media == null)
            {
                return NotFound();
            }

            if (media.free == 0)
            {
                //checkright
                int userId = _userHelper.CheckToken(sessionKey);
                if (userId > 0)
                {
                    var list = _db.userMediaAsset.Where(m => (m.user_id == userId && m.media_id == id)).ToList();
                    if (list == null || list.Count <= 0)
                    {
                        return NoContent();
                    }
                }
                else
                {
                    return NotFound();
                }
      
            }

            //token = Util.UrlEncode(token);
            string realTemplatePath = Util.workingPath +  _settings.mediaRoot + media.path;
            
            if (!System.IO.File.Exists(realTemplatePath))
            {
                return NotFound();
            }
            
            string contentType = "audio/mp3";

            switch (media.type.Trim())
            {
                case "video":
                    contentType = "vieo/mp4";
                    break;
                default:
                    break;
            }

            
            Response.ContentType = contentType;
            PipeWriter pw = Response.BodyWriter;
            Stream s = pw.AsStream();
            System.IO.FileInfo mediaFileInfo = new System.IO.FileInfo(realTemplatePath);
            byte[] buffer = new byte[mediaFileInfo.Length];
            FileStream fs = System.IO.File.OpenRead(realTemplatePath);
            int seg = 1024 * 1024;
            for (int i = 0; (long)(i * seg) < mediaFileInfo.Length; i++)
            {
                int count = seg;
                if ((long)((i + 1) * seg) > mediaFileInfo.Length)
                {
                    count = (int)(mediaFileInfo.Length - i * seg);
                }
                fs.Read(buffer, i * seg, count);
            }
            fs.Close();
            fs.Dispose();
            //s.Write(buffer);
            await s.WriteAsync(buffer);
            s.Close();
            s.Dispose();
            return NoContent();
        }


        [HttpGet]
        public async Task<IActionResult> GetMediaData()
        {
            string realTemplatePath = Util.workingPath + "/medias/test.mp3";
            Response.ContentType = "audio/mp3";
            PipeWriter pw = Response.BodyWriter;
            Stream s = pw.AsStream();
            System.IO.FileInfo mediaFileInfo = new System.IO.FileInfo(realTemplatePath);
            byte[] buffer = new byte[mediaFileInfo.Length];
            FileStream fs = System.IO.File.OpenRead(realTemplatePath);
            int seg = 1024 * 1024;
            for (int i = 0; (long)(i * seg) < mediaFileInfo.Length; i++)
            {
                int count = seg;
                if ((long)((i + 1) * seg) > mediaFileInfo.Length)
                {
                    count = (int)(mediaFileInfo.Length - i * seg);
                }
                fs.Read(buffer, i * seg, count);
            }
            fs.Close();
            fs.Dispose();
            //s.Write(buffer);
            await s.WriteAsync(buffer);
            s.Close();
            s.Dispose();

            return NoContent();


        }
    }
}
