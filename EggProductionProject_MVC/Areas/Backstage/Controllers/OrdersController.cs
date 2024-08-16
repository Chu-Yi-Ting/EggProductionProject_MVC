using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.CouponTypesController;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.OrdersController;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class OrdersController : Controller
    {   
        private readonly EggPlatformContext _context;
        public OrdersController(EggPlatformContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewData["TimeOptions"] = new SelectList(
                 new List<SelectListItem>
                 {
                        new SelectListItem { Value = "1", Text = "近三月" },
                        new SelectListItem { Value = "2", Text = "本月"},
                        new SelectListItem { Value = "3", Text = "本週"}
                 }, "Value", "Text");

            ViewData["OrderStatusNo"] = new SelectList(_context.OrderStatuses, "OrderStatusNo", "OrderStatus");
            ViewData["PaymentOptions"] = new SelectList(_context.Payments, "PaymentNo", "Payment");
            ViewData["PayOptions"] = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "是" },
                        new SelectListItem { Value = "0", Text = "否" }

                    }, "Value", "Text");
            
            return View();
        }

        public class OrderSec
        {
            public string? TimeSec { get; set; }
            public string? StatusSec { get; set; }
            public string? PaymentSec { get; set; }
            public string? PaySec { get; set; }
        }

        [HttpPost]
        public JsonResult CouponList([FromBody] OrderSec sec)
        {

            var members = _context.Members.Select(c => new memberDto
            {
                MemberSid = c.MemberSid,
                Name = c.Name
            }).ToList();

            var payments = _context.Payments.Select(c => new paymentDto
            {
                PaymentNo = c.PaymentNo,
                Payment = c.Payment
            }).ToList();

            var orderStatuses = _context.OrderStatuses.Select(c => new orderStatusDto
            {
                OrderStatusNo = c.OrderStatusNo,
                OrderStatus = c.OrderStatus
            }).ToList();

            var coupons = _context.Coupons.Select(c => new couponsDto
            {
                CouponSid = c.CouponSid,
                Price = c.CouponTypeNoNavigation.Price
            }).ToList();

            var ori_or = _context.Orders.ToList();

            var two_or = ori_or;
            switch (sec.TimeSec)
            {
                case "1":

                    DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

                    two_or = ori_or
                    .Where(c => 
                    c.OrderCreatedTime >= threeMonthsAgo)
                    .ToList();
                    break;

                case "2":
                    DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);
                    two_or = ori_or
                      .Where(c => 
                    c.OrderCreatedTime >= oneMonthsAgo)
                    .ToList();
                    break;

                case "3":
                    DateTime now = DateTime.Now;
                    DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endOfWeek = startOfWeek.AddDays(6);

                    two_or = ori_or
                    .Where(c =>
                    c.OrderCreatedTime >= startOfWeek &&
                    c.OrderCreatedTime <= endOfWeek)
                    .ToList();
                    break;

                default:
                    two_or = ori_or
                     .ToList();
                    break;
            }

            var three_or = two_or;

            if (sec.StatusSec != "all" && sec.StatusSec != null && sec.StatusSec != "" && sec.StatusSec != "0")
            {
                if (!int.TryParse(sec.StatusSec, out int statusSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                three_or = two_or
                .Where(c => c.OrderStatusNo == statusSecInt)
                .ToList();
            }
            else
            {
                three_or = two_or
                .ToList();
            }

            var four_or = three_or;

            if (sec.PaymentSec != "all" && sec.PaymentSec != null && sec.PaymentSec != "" && sec.PaymentSec != "0")
            {
                if (!int.TryParse(sec.PaymentSec, out int paymentSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                four_or = three_or
                .Where(c => c.PaymentNo == paymentSecInt)
                .ToList();
            }
            else
            {
                four_or = three_or
                .ToList();
            }

            var end_or = four_or.Where(C=>C.AlreadyPaid==0);

            switch (sec.PaySec)
            {
                case "0":
                    end_or = four_or.Where(C => C.AlreadyPaid == 0).ToList();
                    break;

                case "1":
                    end_or = four_or.Where(C => C.AlreadyPaid == 1).ToList();
                    break;

                default:
                    end_or = four_or;
                    break;
            }



            var CouponViewModels = end_or.Select(p => new OrderViewModel
            {
                OrderSid = p.OrderSid,
                OrderNo = p.OrderNo,
                OrderCreatedTime = p.OrderCreatedTime,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name,
                PaymentNo = p.PaymentNo,
                Payment = payments.FirstOrDefault(s => s.PaymentNo == p.PaymentNo)?.Payment,
                AlreadyPaid = p.AlreadyPaid,
                OrderStatusNo = p.OrderStatusNo,
                OrderStatus = orderStatuses.FirstOrDefault(s => s.OrderStatusNo == p.OrderStatusNo)?.OrderStatus,

                TrackingNum = orderStatuses.FirstOrDefault(s => s.OrderStatusNo == p.OrderStatusNo)?.OrderStatus,
                Price = coupons.FirstOrDefault(s => s.CouponSid == p.CouponSid)?.Price,
                TotalPrice = p.TotalPrice
            });
            return Json(CouponViewModels);
        }

        public JsonResult CouponList1(CouponSec sec)
        {
            var ori_or = _context.Orders.ToList();
            var members = _context.Members.ToList();
            var payments = _context.Payments.ToList();
            var OrderStatuses = _context.OrderStatuses.ToList();
            var Tracks = _context.Tracks.ToList();
            var CouponTypes = _context.CouponTypes.ToList();
            var Coupons = _context.Coupons.ToList();

            var CouponViewModels = ori_or.Select(p => new OrderViewModel
            {
                OrderSid =p.OrderSid,
                OrderNo = p.OrderNo,
                OrderCreatedTime = p.OrderCreatedTime,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name,
                PaymentNo = p.PaymentNo,
                Payment = payments.FirstOrDefault(s => s.PaymentNo == p.PaymentNo)?.Payment,
                AlreadyPaid = p.AlreadyPaid,
                OrderStatusNo = p.OrderStatusNo,                
                OrderStatus = OrderStatuses.FirstOrDefault(s => s.OrderStatusNo == p.OrderStatusNo)?.OrderStatus,
                
                TrackingNum = OrderStatuses.FirstOrDefault(s => s.OrderStatusNo == p.OrderStatusNo)?.OrderStatus,
                Price= Coupons.FirstOrDefault(s => s.CouponSid == p.CouponSid)?.CouponTypeNoNavigation.Price,
                TotalPrice=p.TotalPrice
            });
            return Json(CouponViewModels);
        }

        internal class memberDto
        {
            public int MemberSid { get; set; }
            public string? Name { get; set; }
        }

        internal class orderStatusDto
        {
            public int OrderStatusNo { get; set; }
            public string? OrderStatus { get; set; }
        }

        internal class paymentDto
        {
            public int PaymentNo { get; set; }
            public string? Payment { get; set; }
        }

        internal class couponsDto
        {
            public int CouponSid { get; set; }
            public decimal? Price { get; set; }
        }
    }

  

    
}
