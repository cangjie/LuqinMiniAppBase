using System;
using System.Xml;
using Microsoft.Extensions.Configuration;
using System.Linq;
using LuqinMiniAppBase.Models;
namespace LuqinMiniAppBase.Controllers
{
    public class OfficailAccountReply
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly OAReceive _message;

        public OfficailAccountReply(Db context,
            IConfiguration config, OAReceive message)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _message = message;
        }

        public string Reply()
        {
            string retStr = "success";
            XmlDocument xmlD = new XmlDocument();
            switch (_message.MsgType.Trim().ToLower())
            {
                case "event":
                    switch (_message.Event.Trim().ToLower())
                    {
                        
                        default:
                            break;
                    }

                    break;
                case "text":
                default:
                    switch (_message.Content.Trim().ToLower())
                    {
                        
                        default:
                            retStr = "success";
                            break;
                    }
                    break;
            }

            try
            {
                OASent sent = new OASent()
                {
                    id = 0,
                    FromUserName = xmlD.SelectSingleNode("//xml/FromUserName").InnerText.Trim(),
                    ToUserName = xmlD.SelectSingleNode("//xml/ToUserName").InnerText.Trim(),
                    is_service = 0,
                    origin_message_id = _message.id,
                    MsgType = xmlD.SelectSingleNode("//xml/MsgType").InnerText.Trim(),
                    Content = xmlD.SelectSingleNode("//xml/Content").InnerXml.Trim(),
                    err_code = "",
                    err_msg = ""

                };
                _db.oASent.Add(sent);
                _db.SaveChanges();
            }
            catch
            {

            }
            return retStr.Trim();
        }

       


        

    }
}
