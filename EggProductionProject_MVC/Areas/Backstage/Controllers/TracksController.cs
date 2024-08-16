using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.CouponTypesController;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.OrdersController;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.TracksController;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class TracksController : Controller
    {
        private readonly EggPlatformContext _context;
        public TracksController(EggPlatformContext context)
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
            ViewData["CarrierOptions"] = new SelectList(_context.Carriers, "CarrierNo", "CarrierName");

            var carrierWays = _context.CarrierWays
        .Select(c => new
        {
            CarrierWayNo = c.CarrierWayNo,
            DisplayText = $"{c.CarrierWay} ({c.CarrierNoNavigation.CarrierCode})" 
        })
        .ToList();

            ViewData["WayOptions"] = new SelectList(carrierWays, "CarrierWayNo", "DisplayText");

      
            return View();
        }


        public class OrderSec
        {
            public string? TimeSec { get; set; }
            public string? StatusSec { get; set; }
            public string? CarrierSec { get; set; }
            public string? WaySec { get; set; }
        }

        [HttpPost]
        public JsonResult CouponList([FromBody] OrderSec sec)
        {


            var trackTimes = _context.TrackTimes.Select(c => new trackTimesDto
            {
                TrackSid = (int)c.TrackSid,
                CreatedTime = c.CreatedTime
            }).ToList();

            var OrderStatus = _context.Orders.Select(c => new OrderStatusDto
            {
                OrderSid = (int)c.OrderSid,
                OrderStatusNo = (int)c.OrderStatusNo,
                OrderStatus = c.OrderStatusNoNavigation.OrderStatus
            }).ToList();

            var CarrierAddresses = _context.CarrierAddresses.Select(c => new CarrierAddressesDto
            {
                CarrierAddressSid = (int)c.CarrierAddressSid,
                CarrierNo = c.CarrierWayNoNavigation.CarrierNoNavigation.CarrierNo,
                CarrierName = c.CarrierWayNoNavigation.CarrierNoNavigation.CarrierName,
                CarrierWay = c.CarrierWayNoNavigation.CarrierWay,
                CarrierWayNo = c.CarrierWayNoNavigation.CarrierWayNo
            }).ToList();


            var ori_tr = _context.Tracks.ToList();

            var two_ca = CarrierAddresses.ToList();
            var three_ca = two_ca;


            if (sec.WaySec != "all" && sec.WaySec != null && sec.WaySec != "" && sec.WaySec != "0")
            {
                if (!int.TryParse(sec.WaySec, out int WaySecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                two_ca = CarrierAddresses.Where(C => C.CarrierWayNo == WaySecInt).ToList();
            }
            else
            {
                two_ca = CarrierAddresses
                .ToList();
            }

            if (sec.CarrierSec != "all" && sec.CarrierSec != null && sec.CarrierSec != "" && sec.CarrierSec != "0")
            {
                if (!int.TryParse(sec.CarrierSec, out int CarrierSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                three_ca = two_ca.Where(C => C.CarrierNo == CarrierSecInt).ToList();
            }
            else
            {
                three_ca = two_ca
               .ToList();
            }
            var filteredNo = three_ca.Select(c => c.CarrierAddressSid).ToList();
            //-------------------------------------------------------------------------
            var ori_os = OrderStatus.ToList();

            if (sec.StatusSec != "all" && sec.StatusSec != null && sec.StatusSec != "" && sec.StatusSec != "0")
            {
                if (!int.TryParse(sec.StatusSec, out int StatusSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                ori_os = OrderStatus
                .Where(C => C.OrderStatusNo == StatusSecInt)
                .ToList();
            }
            else
            {
                ori_os = OrderStatus
               .ToList();
            }

            var filteredNo2 = ori_os.Select(c => c.OrderSid).ToList();
            //---------------------------------------------------------------------------------

            var ori_tt = _context.TrackTimes.ToList();

            switch (sec.TimeSec)
            {
                case "1":

                    DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

                    ori_tt = _context.TrackTimes
                    .Where(c =>  c.CreatedTime >= threeMonthsAgo)
                    .ToList();
                    break;

                case "2":
                    DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);
                  
                    ori_tt = _context.TrackTimes
                    .Where(c => c.CreatedTime >= oneMonthsAgo)
                    .ToList();
                    break;

                case "3":
                    DateTime now = DateTime.Now;
                    DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endOfWeek = startOfWeek.AddDays(6);

                    ori_tt = _context.TrackTimes
                    .Where(c => c.CreatedTime >= startOfWeek &&
                    c.CreatedTime <= endOfWeek)
                    .ToList();

                    break;

                default:
                    ori_tt = _context.TrackTimes
                   .ToList();
                    break;
            }

            var filteredNo3 = ori_tt.Select(c => c.TrackSid).ToList();


            var filteredTracks = _context.Tracks
            .Where(c => filteredNo.Contains((int)c.ReceiveSourceSid) || filteredNo.Contains((int)c.ReceiveSourceSid))
            .ToList();

            var filteredTracks2 = filteredTracks
            .Where(c => filteredNo2.Contains((int)c.OrderSid))
            .ToList();

            var filteredTracks3 = filteredTracks2
            .Where(c => filteredNo3.Contains((int)c.TrackSid))
            .ToList();



            var CouponViewModels = filteredTracks3.Select(p => new TrackViewModel
            {
                TrackSid = p.TrackSid,
                OrderSid = p.OrderSid,
                TrackingNum = ori_tr.FirstOrDefault(s => s.OrderSid == p.OrderSid)?.TrackingNum,
                OrderStatus = OrderStatus.FirstOrDefault(s => s.OrderSid == p.OrderSid)?.OrderStatus,
                CreatedTime = trackTimes.FirstOrDefault(s => s.TrackSid == p.TrackSid)?.CreatedTime,
                CarrierWay = CarrierAddresses.FirstOrDefault(s => s.CarrierAddressSid == p.ReceiveSourceSid)?.CarrierWay,
                CarrierName = CarrierAddresses.FirstOrDefault(s => s.CarrierAddressSid == p.ReceiveSourceSid)?.CarrierName,
            });
            return Json(CouponViewModels);
        }

        //public JsonResult CouponList1(CouponSec sec)
        //{
        //    var ori_tr = _context.Tracks.ToList();


        //    var trackTimes = _context.TrackTimes.Select(c => new trackTimesDto
        //    {
        //        TrackSid = (int)c.TrackSid,
        //        CreatedTime = c.CreatedTime
        //    }).ToList();

        //    var OrderStatus = _context.Orders.Select(c => new OrderStatusDto
        //    {
        //        OrderSid = (int)c.OrderSid,
        //        OrderStatusNo =(int)c.OrderStatusNo,
        //        OrderStatus = c.OrderStatusNoNavigation.OrderStatus
        //    }).ToList();

        //    var CarrierAddresses = _context.CarrierAddresses.Select(c => new CarrierAddressesDto
        //    {
        //        CarrierAddressSid = (int)c.CarrierAddressSid,
        //        CarrierNo = c.CarrierWayNoNavigation.CarrierNoNavigation.CarrierNo,
        //        CarrierName = c.CarrierWayNoNavigation.CarrierNoNavigation.CarrierName,
        //        CarrierWay = c.CarrierWayNoNavigation.CarrierWay,
        //        CarrierWayNo = c.CarrierWayNoNavigation.CarrierWayNo
        //    }).ToList();


        //    var nn= OrderStatus
        //        .Where(C => C.OrderStatusNo == 2)
        //        .ToList();


        //    //var filteredNo  = CarrierAddresses
        //    //    .Where(C=>C.CarrierWayNo==2).Select(c => c.CarrierAddressSid).ToList();



        //    //        DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);

        //    //        var recentTrackTimes = _context.TrackTimes
        //    //.Where(tt => tt.D >= oneMonthsAgo) // 篩選 CreatedTime 在三個月內
        //    //.Select(tt => tt.TrackSid) // 選擇 TrackTimeSid 作為過濾條件
        //    //.ToList();


        //    //        var filteredTracks = _context.Tracks
        //    //        .Where(c => filteredNo.Contains((int)c.ReceiveSourceSid) || filteredNo.Contains((int)c.ReceiveSourceSid))
        //    //        .ToList();

        //    //        var endTracks = _context.Tracks
        //    //    .Where(t => recentTrackTimes.Contains(t.TrackSid)) // 篩選 TrackTimeSid
        //    //    .ToList();

        //    //var CouponViewModels = endTracks.Select(p => new TrackViewModel
        //    //{
        //    //    TrackSid = p.TrackSid,
        //    //    OrderSid = p.OrderSid,
        //    //    TrackingNum = ori_tr.FirstOrDefault(s => s.OrderSid == p.OrderSid)?.TrackingNum,
        //    //    OrderStatus = OrderStatus.FirstOrDefault(s => s.OrderSid == p.OrderSid)?.OrderStatus,
        //    //    CreatedTime = trackTimes.FirstOrDefault(s => s.TrackSid == p.TrackSid)?.CreatedTime,
        //    //    CarrierWay = CarrierAddresses.FirstOrDefault(s => s.CarrierAddressSid == p.ReceiveSourceSid)?.CarrierWay,
        //    //    CarrierName = CarrierAddresses.FirstOrDefault(s => s.CarrierAddressSid == p.ReceiveSourceSid)?.CarrierName,
        //    //});



        //    return Json(nn);
        //}

        private class memberDto
        {
            public int MemberSid { get; set; }
            public string? Name { get; set; }
        }

        private class orderStatusDto
        {
            public int OrderStatusNo { get; set; }
            public string? OrderStatus { get; set; }
        }

        private class paymentDto
        {
            public int PaymentNo { get; set; }
            public string? Payment { get; set; }
        }

        private class couponsDto
        {
            public int CouponSid { get; set; }
            public decimal? Price { get; set; }
        }


        private class OrderStatusDto
        {
            public int OrderSid { get; set; }

            public int OrderStatusNo { get; set; }  
            public string? OrderStatus { get; set; }
        }

        private class CarrierAddressesDto
        {
            public int CarrierAddressSid { get; set; }

            public int CarrierNo { get; set; }
            public string? CarrierName { get; set; }

            public int CarrierWayNo { get; set; }
            public string? CarrierWay { get; set; }
            
        }

        private class trackTimesDto
        {
            public int TrackSid { get; set; }
            public DateTime? CreatedTime { get; set; }
        }
    }

 
}
