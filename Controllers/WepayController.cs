﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using LuqinMiniAppBase.Models;
using System.Threading.Tasks;
using System;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Settings;
using SKIT.FlurlHttpClient.Wechat.TenpayV3;
using SKIT.FlurlHttpClient.Wechat.TenpayV3.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;


namespace LuqinMiniAppBase.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WepayController : ControllerBase
    {
        public class Resource
        {
            public string original_type { get; set; }
            public string algorithm { get; set; }
            public string ciphertext { get; set; }
            public string associated_data { get; set; }
            public string nonce { get; set; }
        }

        public class CallBackStruct
        {
            public string id { get; set; }
            public DateTimeOffset create_time { get; set; }
            public string resource_type { get; set; }
            public string event_type { get; set; }
            public string summary { get; set; }
            public Resource resource { get; set; }
        }

        private readonly Db _db;

        private readonly IConfiguration _config;

        private readonly Settings _settings;

        private readonly UserHelperController _userHelper;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public WepayController(Db context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _db = context;
            _config = config;
            _settings = Settings.GetSettings(_config);
            _userHelper = new UserHelperController(_db, _config);
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("{mchId}")]
        public async Task<ActionResult<WepayOrder>> Pay(int mchId, string sessionKey, double amount, string memo)
        {
            string callBack = "https://mini.luqinwenda.com/api/Wepay/PaymentCallback";// + mchId.ToString();
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

        [HttpPost("{mchId}")]
        public async Task<ActionResult<string>> PaymentCallback(int mchId, CallBackStruct postData)
        {

            string apiKey = "";
            WepayKey key = _db.wepayKey.Find(mchId);

            if (key == null)
            {
                return NotFound();
            }

            apiKey = key.api_key.Trim();

            if (apiKey == null || apiKey.Trim().Equals(""))
            {
                return NotFound();
            }

            string postJson = Newtonsoft.Json.JsonConvert.SerializeObject(postData);
            string path = $"{Environment.CurrentDirectory}";
            string paySign = "no sign";
            string nonce = "no nonce";
            string serial = "no serial";
            string timeStamp = "no time";
            try
            {
                paySign = _httpContextAccessor.HttpContext.Request.Headers["Wechatpay-Signature"].ToString();
                nonce = _httpContextAccessor.HttpContext.Request.Headers["Wechatpay-Nonce"].ToString();
                serial = _httpContextAccessor.HttpContext.Request.Headers["Wechatpay-Serial"].ToString();
                timeStamp = _httpContextAccessor.HttpContext.Request.Headers["Wechatpay-Timestamp"].ToString();
            }
            catch
            {

            }
            if (path.StartsWith("/"))
            {
                path = path + "/WepayCertificate/";
            }
            else
            {
                path = path + "\\WepayCertificate\\";
            }
            string dateStr = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0')
                + DateTime.Now.Day.ToString().PadLeft(2, '0');
            //path = path + "callback_" +  + ".txt";
            // 此文本只添加到文件一次。
            using (StreamWriter fw = new StreamWriter(path + "callback_origin_" + dateStr + ".txt", true))
            {
                fw.WriteLine(DateTimeOffset.Now.ToString());
                fw.WriteLine(serial);
                fw.WriteLine(timeStamp);
                fw.WriteLine(nonce);
                fw.WriteLine(paySign);
                fw.WriteLine(postJson);
                fw.WriteLine("");
                fw.WriteLine("--------------------------------------------------------");
                fw.WriteLine("");
                fw.Close();
            }

            try
            {
                string cerStr = "";
                using (StreamReader sr = new StreamReader(path + serial.Trim() + ".pem", true))
                {
                    cerStr = sr.ReadToEnd();
                    sr.Close();
                }



                var certManager = new InMemoryCertificateManager();

                CertificateEntry ce = new CertificateEntry(serial, cerStr, DateTimeOffset.MinValue, DateTimeOffset.MaxValue);

                //certManager.SetCertificate(serial, cerStr);
                certManager.AddEntry(ce);

                var options = new WechatTenpayClientOptions()
                {
                    MerchantV3Secret = apiKey,
                    PlatformCertificateManager = certManager
                };
                var client = new WechatTenpayClient(options);
                bool valid = client.VerifyEventSignature(timeStamp, nonce, postJson, paySign, serial);
                if (valid)
                {
                    var callbackModel = client.DeserializeEvent(postJson);
                    if ("TRANSACTION.SUCCESS".Equals(callbackModel.EventType))
                    {
                        /* 根据事件类型，解密得到支付通知敏感数据 */
                        var callbackResource = client.DecryptEventResource<SKIT.FlurlHttpClient.Wechat.TenpayV3.Events.TransactionResource>(callbackModel);
                        string outTradeNumber = callbackResource.OutTradeNumber;
                        string transactionId = callbackResource.TransactionId;
                        string callbackStr = Newtonsoft.Json.JsonConvert.SerializeObject(callbackResource);
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(path + "callback_decrypt_" + dateStr + ".txt", true))
                            {
                                sw.WriteLine(DateTimeOffset.Now.ToString());
                                sw.WriteLine(callbackStr);
                                sw.WriteLine("");
                                sw.Close();
                            }
                            WepayOrder wepayOrder = await _db.wepayOrder.FindAsync(outTradeNumber);
                            wepayOrder.state = 2;
                            wepayOrder.transaction_id = transactionId.Trim();
                            _db.Entry(wepayOrder).State = EntityState.Modified;
                            await _db.SaveChangesAsync();
                        }
                        catch
                        {

                        }
                        //SetWepayOrderSuccess(outTradeNumber);

                        //Console.WriteLine("订单 {0} 已完成支付，交易单号为 {1}", outTradeNumber, transactionId);
                    }
                }

            }
            catch (Exception err)
            {
                Console.WriteLine(err.ToString());
            }
            return "{ \r\n \"code\": \"SUCCESS\", \r\n \"message\": \"成功\" \r\n}";
        }

        [HttpGet("{outTradeNo}")]
        public async Task<ActionResult<WepayOrderRefund>> Refund(string outTradeNo, double amount, string reason, string sessionKey)
        {
            int realAmount = (int)Math.Round(amount * 100, 0);
            sessionKey = Util.UrlDecode(sessionKey);
            reason = Util.UrlDecode(reason);
            UserHelperController uc = new UserHelperController(_db, _config);
            int userId = uc.CheckToken(sessionKey.Trim());
            if (userId <= 0)
            {
                return NotFound();
            }
            UnicUser user = await _db.unicUser.FindAsync(userId);
            if (user.is_admin == 0)
            {
                return NoContent();
            }
            WepayOrder order = await _db.wepayOrder.FindAsync(outTradeNo);
            WepayOrderRefund refund = new WepayOrderRefund()
            {
                id = 0,
                amount = realAmount,
                state = 0,
                oper_user_id = userId,
                wepay_out_trade_no = outTradeNo.Trim()

            };
            await _db.wepayOrderRefund.AddAsync(refund);
            await _db.SaveChangesAsync();

            WepayKey key = await _db.wepayKey.FindAsync(order.mch_id);
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
            var request = new CreateRefundDomesticRefundRequest()
            {
                TransactionId = order.transaction_id.Trim(),
                OutRefundNumber = refund.id.ToString(),
                Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
                {
                    Total = order.amount,
                    Refund = realAmount
                },
                Reason = reason,
                NotifyUrl = "https://mini.luqinweneda.com/api/Wepay/RefundCallback"
            };
            var response = await client.ExecuteCreateRefundDomesticRefundAsync(request);
            if (!response.IsSuccessful())
            {
                refund.err_msg = response.ErrorMessage.Trim();
            }
            else
            {
                refund.state = 1;
                refund.err_msg = "";
            }
            _db.Entry(refund).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            //var result = JsonResult(response);
            return refund;
            //client.ExecuteCreateRefundDomesticRefundAsync()
            //var request = new Create
            //return NoContent();
        }

    }

    /*


    [HttpGet("{outTradeNo}")]
    public async Task<ActionResult<string>> Refund(string outTradeNo, int amount, string sessionKey)
    {
        sessionKey = Util.UrlDecode(sessionKey);
        UnicUser._context = _context;
        UnicUser user = UnicUser.GetUnicUser(sessionKey);
        if (user == null || !user.isAdmin)
        {
            return NotFound();
        }
        string operName = "管理员申请退款";

        WepayOrder wepayOrder = _context.WepayOrders.Find(outTradeNo);
        if (wepayOrder == null)
        {
            return NotFound();
        }
        int parseResult = 0;
        var wepayOrderRefundArr =
            _context.WePayOrderRefund.Where(r => r.wepay_out_trade_no == outTradeNo).ToList<WepayOrderRefund>();
        int totalRefundAmount = 0;
        for (int i = 0; i < wepayOrderRefundArr.Count; i++)
        {
            WepayOrderRefund currentRefund = wepayOrderRefundArr[i];
            if (int.TryParse(currentRefund.status, out parseResult))
                totalRefundAmount = wepayOrderRefundArr[i].amount + totalRefundAmount;
        }
        if (totalRefundAmount + amount <= wepayOrder.amount)
        {
            WepayOrderRefund refund = new WepayOrderRefund();
            refund.amount = amount;
            refund.oper_open_id = user.miniAppOpenId.Trim();
            refund.status = "";
            refund.wepay_out_trade_no = wepayOrder.out_trade_no.Trim();
            _context.WePayOrderRefund.Add(refund);
            _context.SaveChanges();

            int mchid = wepayOrder.mch_id;
            WepayKey key = _context.WepayKeys.Find(mchid);
            if (key == null)
            {
                return NotFound();
            }
            var certManager = new InMemoryCertificateManager();
            var options = new WechatTenpayClientOptions()
            {
                MerchantId = key.mch_id.Trim(),
                MerchantV3Secret = "",
                MerchantCertSerialNumber = key.key_serial.Trim(),
                MerchantCertPrivateKey = key.private_key.Trim(),
                CertificateManager = certManager
            };
            string refundTransId = refund.wepay_out_trade_no + refund.id.ToString().PadLeft(2, '0');
            var client = new WechatTenpayClient(options);
            var request = new CreateRefundDomesticRefundRequest()
            {
                OutTradeNumber = wepayOrder.out_trade_no.Trim(),
                OutRefundNumber = refundTransId.Trim(),
                Amount = new CreateRefundDomesticRefundRequest.Types.Amount()
                {
                    Total = wepayOrder.amount,
                    Refund = amount
                },
                Reason = operName,
                NotifyUrl = wepayOrder.notify.Replace("PaymentCallback", "RefundCallback")
            };
            var response = await client.ExecuteCreateRefundDomesticRefundAsync(request);
            try
            {
                string refundId = response.RefundId.Trim();
                if (refundId == null || refundId.Trim().Equals(""))
                {
                    return NotFound();
                }
                refund.status = refundId;
                _context.Entry<WepayOrderRefund>(refund).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await Response.WriteAsync("SUCCESS");
            }
            catch
            {
                refund.status = response.ErrorMessage.Trim();
                _context.Entry<WepayOrderRefund>(refund).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NotFound();
            }



        }
        return NotFound();
    }

    */


}
