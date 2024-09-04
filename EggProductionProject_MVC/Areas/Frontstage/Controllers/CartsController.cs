using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class CartsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public class CheckoutViewModel
        {
            public string Combo { get; set; }
            public string Usecoin { get; set; }
            public string CouponSid { get; set; }
            public string PaymentNo { get; set; }
            public List<string> OrderNumbers { get; set; }
        }

        public IActionResult Checkout()
        {

            var combo = Request.Form["combo"];
            var usecoin = Request.Form["usecoin"];
            var couponSid = Request.Form["couponSid"];
            var paymentNo = Request.Form["paymentNo"];
            var orderNumbersJson = Request.Form["orderNumbers"];


            List<string> orderNumbersList = null;
            if (!string.IsNullOrEmpty(orderNumbersJson))
            {
                try
                {
                    orderNumbersList = JsonConvert.DeserializeObject<List<string>>(orderNumbersJson);
                }
                catch (JsonException ex)
                {
                    ModelState.AddModelError("orderNumbers", "Invalid order numbers format.");
                }
            }


            var model = new CheckoutViewModel
            {
                Combo = combo,
                Usecoin = usecoin,
                CouponSid = couponSid,
                PaymentNo = paymentNo,
                OrderNumbers = orderNumbersList
            };
            return View(model);
        }
    }
}
