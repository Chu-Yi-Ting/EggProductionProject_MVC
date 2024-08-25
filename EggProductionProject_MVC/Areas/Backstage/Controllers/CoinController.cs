using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EggProductionProject_MVC.Areas.Backstage.Controllers
{
    [Area("Backstage")]
    public class CoinController : Controller
    {
        private readonly EggPlatformContext _context;
        public CoinController(EggPlatformContext context)
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

            ViewData["CoinUseAreaNo"] = new SelectList(_context.CoinUseAreas, "CoinUseAreaNo", "UseArea");
            ViewData["MoveOptions"] = new SelectList(
                    new List<SelectListItem>
                    {
                        new SelectListItem { Value = "1", Text = "增加" },
                        new SelectListItem { Value = "0", Text = "減少" }

                    }, "Value", "Text");

            return View();
        }

 


        public class OrderSec
        {
            public string? TimeSec { get; set; }
            public string? AreaSec { get; set; }
            public string? ActionSec { get; set; }
        }

        [HttpPost]
        public JsonResult CouponList([FromBody] OrderSec sec)
        {

            var members = _context.Members.Select(c => new memberDto
            {
                MemberSid = c.MemberSid,
                Name = c.Name
            }).ToList();

            var coinArea = _context.CoinUseAreas.Select(c => new coinAreaDto
            {
                CoinUseAreaNo = c.CoinUseAreaNo,
                CoinUseArea = c.UseArea
            }).ToList();


            //var ori_tr = _context.Tracks.ToList();
            
            var ori_ca = coinArea.ToList();


            if (sec.AreaSec != "all" && sec.AreaSec != null && sec.AreaSec != "" && sec.AreaSec != "0")
            {
                if (!int.TryParse(sec.AreaSec, out int AreaSecInt))
                {
                    return new JsonResult(new { Success = false, Message = "Invalid CouponTypeNo" });
                }
                ori_ca = coinArea.Where(C => C.CoinUseAreaNo == AreaSecInt).ToList();
            }
            else
            {
                ori_ca = coinArea.ToList();
            }

            var filteredNo = ori_ca.Select(c => c.CoinUseAreaNo).ToList();
            //-------------------------------------------------------------------------
            //var ori_os = OrderStatus.ToList();
            var ori_sc = _context.StoreCoins.ToList();
         

            switch (sec.ActionSec)
            {
                case "0":
                    ori_sc = _context.StoreCoins
                     .Where(c => c.IsPositive==0)
                     .ToList();
                    break;

                case "1":
                    ori_sc = _context.StoreCoins
                     .Where(c => c.IsPositive == 1)
                     .ToList();
                    break;

                default:
                    ori_sc = _context.StoreCoins
                     .ToList();
                    break;
            }

            //---------------------------------------------------------------------------------

            //var ori_tt = _context.TrackTimes.ToList();
            var end_sc = ori_sc;

            switch (sec.TimeSec)
            {
                case "1":

                    DateTime threeMonthsAgo = DateTime.Now.AddMonths(-3);

                    end_sc = ori_sc
                    .Where(c => filteredNo.Contains((int)c.CoinUseAreaNo) &&
                    c.ChangeTime >= threeMonthsAgo)
                    .ToList();
                    break;

                case "2":
                    DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);
                    end_sc = ori_sc
                      .Where(c => filteredNo.Contains((int)c.CoinUseAreaNo) &&
                    c.ChangeTime >= oneMonthsAgo)
                    .ToList();
                    break;

                case "3":
                    DateTime now = DateTime.Now;
                    DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek + (int)DayOfWeek.Monday);
                    DateTime endOfWeek = startOfWeek.AddDays(6);

                    end_sc = ori_sc
                    .Where(c => filteredNo.Contains((int)c.CoinUseAreaNo) &&
                    c.ChangeTime >= startOfWeek &&
                    c.ChangeTime <= endOfWeek)
                    .ToList();
                    break;

                default:
                    end_sc = ori_sc
                     .Where(c => filteredNo.Contains((int)c.CoinUseAreaNo))
                     .ToList();
                    break;
            }

            var CouponViewModels = end_sc.Select(p => new CoinViewModel
            {
                StoreCoinSid = p.StoreCoinSid,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name,
                CoinUseAreaNo = p.MemberSid,
                CoinUseArea = coinArea.FirstOrDefault(s => s.CoinUseAreaNo == p.CoinUseAreaNo)?.CoinUseArea,
                IsPositive = p.IsPositive,
                Money = p.Money,
                ChangeTime = p.ChangeTime

            });
            return Json(CouponViewModels);

        }

        public JsonResult CouponList1()
        {



            //var filteredNo  = CarrierAddresses
            //    .Where(C=>C.CarrierWayNo==2).Select(c => c.CarrierAddressSid).ToList();



            //        DateTime oneMonthsAgo = DateTime.Now.AddMonths(-1);

            //        var recentTrackTimes = _context.TrackTimes
            //.Where(tt => tt.D >= oneMonthsAgo) // 篩選 CreatedTime 在三個月內
            //.Select(tt => tt.TrackSid) // 選擇 TrackTimeSid 作為過濾條件
            //.ToList();


            //        var filteredTracks = _context.Tracks
            //        .Where(c => filteredNo.Contains((int)c.ReceiveSourceSid) || filteredNo.Contains((int)c.ReceiveSourceSid))
            //        .ToList();

            //        var endTracks = _context.Tracks
            //    .Where(t => recentTrackTimes.Contains(t.TrackSid)) // 篩選 TrackTimeSid
            //    .ToList();

            var ori_tr = _context.StoreCoins.ToList();

    
            var members = _context.Members.Select(c => new memberDto
            {
                MemberSid = c.MemberSid,
                Name = c.Name
            }).ToList();

            var coinArea = _context.CoinUseAreas.Select(c => new coinAreaDto
            {
                CoinUseAreaNo = c.CoinUseAreaNo,
                CoinUseArea = c.UseArea
            }).ToList();



            


            var CouponViewModels = ori_tr.Select(p => new CoinViewModel
            {
                StoreCoinSid = p.StoreCoinSid,
                MemberName = members.FirstOrDefault(s => s.MemberSid == p.MemberSid)?.Name,
                CoinUseAreaNo = p.MemberSid,
                CoinUseArea = coinArea.FirstOrDefault(s => s.CoinUseAreaNo == p.CoinUseAreaNo)?.CoinUseArea,
                IsPositive = p.IsPositive,
                Money = p.Money,
                ChangeTime = p.ChangeTime

            });



            return Json(CouponViewModels);
        }

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

    internal class coinAreaDto
    {
        public int CoinUseAreaNo { get; set; }
        public string CoinUseArea { get; set; }
    }
}
