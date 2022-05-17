using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LuqinMiniAppBase.Models;
using System.Threading.Tasks;
using System;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using Microsoft.EntityFrameworkCore;

namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WepayController : ControllerBase
    {
        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        public WepayController(Db context, IConfiguration config)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
        }

        [HttpGet("{mchId}")]
        public async Task<ActionResult<WepayOrder>> Pay(int mchId, string sessionKey, double amount, string memo)
        {
            string callBack = "https://mini.luqinwenda.com/api/Wepay/PaymentCallback";
            WepayKey key = await _db.wepayKey.FindAsync(mchId);
            if (key == null)
            {
                return NotFound();
            }
            sessionKey = Util.UrlDecode(sessionKey).Trim();
            int userId = _userHelper.CheckToken(sessionKey);
            string openId = _userHelper.GetOpenId(userId, _settings.originalId.Trim());
            if (openId.Trim().Equals(""))
            {
                return NotFound();
            }
            string timeStamp = Util.GetLongTimeStamp(System.DateTime.Now);
            WepayOrder wepayOrder = await _db.wepayOrder.FindAsync(timeStamp.Trim());

            if (wepayOrder != null)
            {
                return NotFound();
            }
            wepayOrder = new WepayOrder();
            wepayOrder.out_trade_no = timeStamp;
            wepayOrder.user_id = userId;
            wepayOrder.notify = callBack.Trim();
            wepayOrder.order_id = 0;
            wepayOrder.amount = (int)Math.Round((amount * 100), 0);
            wepayOrder.app_id = _settings.appId;
            wepayOrder.description = "";
            wepayOrder.mch_id = mchId;
            wepayOrder.original_id = _settings.originalId.Trim();
            wepayOrder.description = memo.Trim();
            _db.wepayOrder.Add(wepayOrder);
            await _db.SaveChangesAsync();

            var certManager = new InMemoryCertificateManager();
            var options = new WechatTenpayClientOptions()
            {
                MerchantId = key.mch_id.Trim(),
                MerchantV3Secret = "",
                MerchantCertificateSerialNumber = key.key_serial.Trim(),
                MerchantCertificatePrivateKey = key.private_key.Trim(),
                PlatformCertificateManager = certManager
            };
            var client = new WechatTenpayClient(options);
            var request = new CreatePayTransactionJsapiRequest()
            {
                OutTradeNumber = timeStamp,
                AppId = _settings.appId.Trim(),
                Description = wepayOrder.description,
                ExpireTime = DateTimeOffset.Now.AddMinutes(30),
                NotifyUrl = wepayOrder.notify.Trim() + "/" + mchId.ToString(),
                Amount = new CreatePayTransactionJsapiRequest.Types.Amount()
                {
                    Total = wepayOrder.amount
                },
                Payer = new CreatePayTransactionJsapiRequest.Types.Payer()
                {
                    OpenId = openId.Trim()
                }
            };
            var response = await client.ExecuteCreatePayTransactionJsapiAsync(request);

            if (response != null && response.PrepayId != null && !response.PrepayId.Trim().Equals(""))
            {
                wepayOrder.prepay_id = response.PrepayId.Trim();
                wepayOrder.state = 1;
                _db.Entry(wepayOrder).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                var paraMap = client.GenerateParametersForJsapiPayRequest(request.AppId, response.PrepayId);
                wepayOrder.timestamp = paraMap["timeStamp"].Trim();
                wepayOrder.nonce = paraMap["nonceStr"].Trim();
                wepayOrder.sign = paraMap["paySign"].Trim();
                return wepayOrder;
            }



            return NotFound();

           
        }

    }
}
