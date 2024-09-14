using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.CouponTypesController;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class MemberCenterAPIController : Controller
	{
		

		private readonly EggPlatformContext _context;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public MemberCenterAPIController(EggPlatformContext context, IWebHostEnvironment webHostEnvironment)
		{
			_context = context;
			_webHostEnvironment = webHostEnvironment;
		}
		//---------------------order-------------------------

		public class OrderSec
		{
			public int? memberSid { get; set; }
			public int? OrderStatusNo { get; set; }

		}

		public class OrderViewModel
		{
			public int OrderSid { get; set; }

			public string? OrderNo { get; set; }

			public decimal? TotalPrice { get; set; }

			public int? AlreadyPaid { get; set; }

			public string? Payment { get; set; }

			public int? CarrierWayNo { get; set; }
			public string? CarrierNameWay { get; set; }

			public string? TrackingNum { get; set; }

			public int? OrderStatusNo { get; set; }

			public string? TrackStatus { get; set; }

			public int? StoreSid { get; set; }

		}


		[HttpPost]
		public JsonResult OrderList([FromBody] OrderSec sec)
		{

			if (!sec.memberSid.HasValue)
			{
				return Json(new { error = "memberSid is required." });
			}


			var orders = _context.Orders
	 .Join(_context.OrderDetails,
		   order => order.OrderSid,
		   detail => detail.OrderSid,
		   (order, detail) => new { order, detail })
	 .Where(od => od.order.MemberSid == sec.memberSid)
	 .Where(od => !sec.OrderStatusNo.HasValue || od.order.OrderStatusNo == sec.OrderStatusNo)
	 .Select(od => new
	 {
		 od.order,
		 StoreSid = od.detail.ProductS.StoreSid
	 })
	 .Distinct()
	 .OrderByDescending(o => o.order.OrderCreatedTime)
	 .ToList();

			var payments = _context.Payments.Select(c => new
			{
				c.PaymentNo,
				c.Pay
			}).ToList();

			var orderStatuses = _context.OrderStatuses.Select(s => new
			{
				s.OrderStatusNo,
				s.Status
			}).ToList();

			var tracks = _context.Tracks
				.Include(t => t.TrackTimes)
				.ThenInclude(tt => tt.TrackStatusNoNavigation)
				.Select(t => new
				{
					t.OrderSid,
					CarrierName = t.ReceiveSourceS.CarrierWayNoNavigation.CarrierNoNavigation.CarrierName,
					WayNo = t.ReceiveSourceS.CarrierWayNoNavigation.CarrierWayNo,
					Way = t.ReceiveSourceS.CarrierWayNoNavigation.Way,
					t.TrackingNum,
					LatestTrackStatus = t.TrackTimes.OrderByDescending(tt => tt.TrackTimeSid)
												.Select(tt => tt.TrackStatusNoNavigation.Status)
												.FirstOrDefault()
				}).ToList();

			// Map the filtered orders to DTOs
			var orderViewModels = orders.Select(p => new OrderViewModel
			{
				OrderSid = p.order.OrderSid,
				OrderNo = p.order.OrderNo,
				TotalPrice = p.order.TotalPrice,
				AlreadyPaid = p.order.AlreadyPaid,

				// Map Payment data using PaymentNo
				Payment = payments.FirstOrDefault(s => s.PaymentNo == p.order.PaymentNo)?.Pay,

				CarrierWayNo = tracks.FirstOrDefault(s => s.OrderSid == p.order.OrderSid)?.WayNo,
				// Map CarrierNameWay with custom logic
				CarrierNameWay = GetCarrierNameWay(
					tracks.FirstOrDefault(s => s.OrderSid == p.order.OrderSid)?.CarrierName,
					tracks.FirstOrDefault(s => s.OrderSid == p.order.OrderSid)?.Way),

				TrackingNum = tracks.FirstOrDefault(s => s.OrderSid == p.order.OrderSid)?.TrackingNum,

				// Map the latest TrackStatus
				TrackStatus = tracks.FirstOrDefault(s => s.OrderSid == p.order.OrderSid)?.LatestTrackStatus,

				// Map Order Status
				OrderStatusNo = p.order.OrderStatusNo,

				StoreSid = p.StoreSid

			}).ToList();

			// Return the filtered DTO data as JSON
			return Json(orderViewModels);
		}

		// Helper method to handle the CarrierName logic
		private static string GetCarrierNameWay(string carrierName, string way)
		{
			if (carrierName == "黑貓宅急便")
			{
				return "黑貓" + way;
			}
			else if (carrierName == "7-ELEVEN")
			{
				return "7-11" + way;
			}
			else
			{
				return carrierName + way;
			}
		}

	

		//---------------------------coupon-------------------------------

		public class CouponRequest
		{
			public int memberSid { get; set; }
			public int CouponStatusNo { get; set; }
		}
		public class CouponDto
		{
			public string KindTime { get; set; }  // 活動券或常駐券資訊
			public string CouponName { get; set; } // 券的名稱
			public decimal? Price { get; set; }    // 券的價格
			public decimal? Minimum { get; set; }  // 最低消費金額
		}

		[HttpPost]
		public JsonResult GetCouponsForMember([FromBody] CouponRequest request)
		{
			var coupons = _context.Coupons
				.Where(c => c.MemberSid == request.memberSid && c.CouponStatusNo == request.CouponStatusNo)
				.Select(c => new CouponDto
				{
					KindTime = c.CouponTypeNoNavigation.StartTime.HasValue
						? $"活動券 {c.CouponTypeNoNavigation.StartTime.Value:yyyy/MM/dd}~{c.CouponTypeNoNavigation.EndTime.Value:yyyy/MM/dd}"
		                : "常駐券",

            CouponName = c.CouponTypeNoNavigation.Name,


            Price = c.CouponTypeNoNavigation.Price,


            Minimum = c.CouponTypeNoNavigation.Minimum

		})
        .ToList();

    // 回傳結果
    return Json(coupons);
	}

		//---------------------------coin-------------------------------

		public class StoreCoinRequest
		{
			public int memberSid { get; set; }
			public int IsPositive { get; set; }
		}
		public class StoreCoinDto
		{
			public string OrderNo { get; set; }
			public decimal? Money { get; set; }
			public DateTime? ChangeTime { get; set; }
		}

		public IActionResult GetStoreCoinData([FromBody] StoreCoinRequest request)
		{
			if (request == null || request.memberSid <= 0 || request.IsPositive < 0)
			{
				return BadRequest("Invalid request.");
			}

			var query = from storeCoin in _context.StoreCoins
						join order in _context.Orders
						on storeCoin.AreaSid equals order.OrderSid
						where storeCoin.MemberSid == request.memberSid
							  && storeCoin.IsPositive == request.IsPositive
						select new StoreCoinDto
						{
							OrderNo = order.OrderNo,
							Money = request.IsPositive == 1 ? +storeCoin.Money : -storeCoin.Money,
							ChangeTime = storeCoin.ChangeTime
						};

			var result = query.OrderByDescending(s => s.ChangeTime).ToList();

			return Ok(result);
		}



	}
}
