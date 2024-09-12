using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Web;
using System.Security.Cryptography;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class CartsController : Controller
    {
        public IActionResult Index()
        {
			var order = new Dictionary<string, string> { };

			return View(order); 
		}
		public async Task<IActionResult> GetNgrokUrl()
		{
			string ngrokUrl = await GetNgrokUrlAsync();
			return Json(new { url = ngrokUrl });
		}

		private async Task<string> GetNgrokUrlAsync()
		{
			using (var httpClient = new HttpClient())
			{
				var response = await httpClient.GetStringAsync("http://127.0.0.1:4040/api/tunnels");
				dynamic data = JsonConvert.DeserializeObject(response);
				return data.tunnels[0].public_url;
			}
		}

		

        public IActionResult Checkout()
        {
            return View();
        }

		[HttpPost]
		public IActionResult CreateOrder(string itemName, decimal totalAmount)
		{
			// 生成訂單 ID 和其他基本資訊
			var orderId = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 20);
			var website = "https://localhost:7080";
			var order = new Dictionary<string, string>
	{
		{ "MerchantTradeNo", orderId },
		{ "MerchantTradeDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") },
		{ "TotalAmount", totalAmount.ToString() },
		{ "TradeDesc", "無" },
		{ "ItemName", itemName },
		{ "ExpireDate", "3" },
		{ "CustomField1", "" },
		{ "CustomField2", "" },
		{ "CustomField3", "" },
		{ "CustomField4", "" },
		{ "ReturnURL", $"{website}/api/Ecpay/AddPayInfo" },
		{ "OrderResultURL", $"{website}/Frontstage/carts/Checkout" },
		{ "PaymentInfoURL", $"{website}/api/Ecpay/AddAccountInfo" },
		{ "ClientRedirectURL", $"{website}/Frontstage/carts/payment/AccountInfo/{orderId}" },
		{ "MerchantID", "2000132" },
		{ "IgnorePayment", "GooglePay#WebATM#CVS#BARCODE" },
		{ "PaymentType", "aio" },
		{ "ChoosePayment", "ALL" },
		{ "EncryptType", "1" },
		{ "StoreID", "GOODEGG" }
	};


			// 計算檢查碼
			order["CheckMacValue"] = GetCheckMacValue(order);

			// 返回帶有訂單資訊的視圖
			return PartialView("_ECFormPartial", order);
		}

		private string GetCheckMacValue(Dictionary<string, string> order)
		{
			var param = order.Keys.OrderBy(x => x).Select(key => key + "=" + order[key]).ToList();
			var checkValue = string.Join("&", param);
			//測試用的 HashKey
			var hashKey = "5294y06JbISpM5x9";
			//測試用的 HashIV
			var HashIV = "v77hoKGq4kWxNNIS";
			checkValue = $"HashKey={hashKey}" + "&" + checkValue + $"&HashIV={HashIV}";
			checkValue = HttpUtility.UrlEncode(checkValue).ToLower();
			checkValue = GetSHA256(checkValue);
			return checkValue.ToUpper();
		}
		private string GetSHA256(string value)
		{
			var result = new StringBuilder();
			var sha256 = SHA256Managed.Create();
			var bts = Encoding.UTF8.GetBytes(value);
			var hash = sha256.ComputeHash(bts);
			for (int i = 0; i < hash.Length; i++)
			{
				result.Append(hash[i].ToString("X2"));
			}
			return result.ToString();
		}

	}
}
