using EggProductionProject_MVC.Areas.Backstage.ViewModels;
using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.CouponTypesController;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.CartsAPIController;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.StoreCentersAPIController;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
    [Area("Frontstage")]
    public class StoreCentersAPIController : Controller
    {  

        private readonly EggPlatformContext _context;

        public StoreCentersAPIController(EggPlatformContext context)
        {
            _context = context;
        }



  
        public class UpdateCarrierAddressDto
        {
            public int CarrierAddressSid { get; set; }
            public int? MemberSid { get; set; }
            public int? AddressFirst { get; set; }

            public string RecordName { get; set; }

            public string RecordPhone { get; set; }

            public int? CarrierWayNo { get; set; }

            public string Way { get; set; }
            public decimal? Price { get; set; }

            public string Adress { get; set; }

            public string StoreId { get; set; }

            public string StoreName { get; set; }

            public int? PublicStatusNo { get; set; }

        }



        [HttpPost]
        [Route("Frontstage/StoreCentersAPI/CarrierAddress/Save")]
        public IActionResult SaveCarrierAddress([FromBody] CarrierAddress model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            if (model.AddressFirst == 1 && (model.CarrierWayNo == 1 || model.CarrierWayNo == 2))
            {

                var otherAddresses = _context.CarrierAddresses
                    .Where(ca => ca.MemberSid == model.MemberSid
                              && (ca.CarrierWayNo == 1 || ca.CarrierWayNo == 2)
                              && ca.CarrierAddressSid != model.CarrierAddressSid)
                    .ToList();

                foreach (var addr in otherAddresses)
                {
                    addr.AddressFirst = 0;
                }

                _context.SaveChanges(); 
            }

            if (model.CarrierWayNo == 3)
            {
                var otherAddresses = _context.CarrierAddresses
                    .Where(ca => ca.MemberSid == model.MemberSid && ca.CarrierWayNo == 3 && ca.CarrierAddressSid != model.CarrierAddressSid)
                    .ToList();

                foreach (var addr in otherAddresses)
                {
                    addr.AddressFirst = 0;
                }

                _context.SaveChanges(); 
            }


            if (model.CarrierAddressSid > 0)
            {
   
                var existingAddress = _context.CarrierAddresses
                                              .SingleOrDefault(ca => ca.CarrierAddressSid == model.CarrierAddressSid);

                if (existingAddress == null)
                {
                    return NotFound();
                }

                var properties = typeof(CarrierAddress).GetProperties();
                foreach (var property in properties)
                {
                    var newValue = property.GetValue(model);
                    if (newValue != null && property.Name != "CarrierAddressSid")
                    {
                        var existingProperty = typeof(CarrierAddress).GetProperty(property.Name);
                        if (existingProperty != null && existingProperty.CanWrite)
                        {
                            try
                            {
                                existingProperty.SetValue(existingAddress, newValue);
                            }
                            catch (Exception ex)
                            {
                                return StatusCode(500, $"Error updating property {property.Name}: {ex.Message}");
                            }
                        }
                    }
                }
            }
            else
            {
                // 如果是新建地址
                var newAddress = new CarrierAddress
                {
                    MemberSid = model.MemberSid,
                    AddressFirst = model.AddressFirst,
                    RecordName = model.RecordName,
                    RecordPhone = model.RecordPhone,
                    CarrierWayNo = model.CarrierWayNo,
                    Adress = model.Adress,
                    StoreId = model.StoreId,
                    StoreName = model.StoreName,
                    PublicStatusNo = model.PublicStatusNo
                };

                _context.CarrierAddresses.Add(newAddress);
            }

            _context.SaveChanges();

            return Ok(new { message = "Operation successful." });
        }






        //Get 編輯CarrierAddressSid資料
        public IActionResult AddressOne( int CarrierAddressSid)
        {
            var query = _context.CarrierAddresses.AsQueryable();

            query = query.Where(c => c.CarrierAddressSid == CarrierAddressSid);

            var CarrierAddress = query
                .Join(_context.CarrierWays,
                    ad => ad.CarrierWayNo,
                    way => way.CarrierWayNo,
                    (ad, way) => new { ad, way })
                .Select(x => new UpdateCarrierAddressDto
                {
                    CarrierAddressSid = x.ad.CarrierAddressSid,
                    MemberSid = (int)x.ad.MemberSid,
                    AddressFirst = (int)x.ad.AddressFirst,
                    RecordName = x.ad.RecordName,
                    RecordPhone = x.ad.RecordPhone,
                    CarrierWayNo = x.ad.CarrierWayNo,
                    Way=x.way.Way,
                    Price=x.way.Price,
                    Adress = x.ad.Adress,
                    StoreId = x.ad.StoreId,
                    StoreName = x.ad.StoreName,
                    PublicStatusNo = x.ad.PublicStatusNo

                }).ToList();

            return Json(CarrierAddress);
        }


        public IActionResult CarrierOpenOne(int memberSid)
        {
            var carrierOpenNos = _context.Stores
                .Where(s => s.MemberSid == memberSid)
                .SelectMany(s => s.CarrierOpens)
                .ToList();

            return Json(carrierOpenNos); 
        }


        public class CarrierOpenUpdateDto
        {
            public int CarrierOpenNo { get; set; }

            public int StoreOpen { get; set; }

            public int HouseOpen { get; set; }

        }

        [HttpPost]
        public async Task<IActionResult> UpdateCarrierOpen([FromBody] CarrierOpenUpdateDto model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            // Find the CarrierOpen entity by CarrierOpenNo
            var carrierOpen = await _context.CarrierOpens
                .SingleOrDefaultAsync(c => c.CarrierOpenNo == model.CarrierOpenNo);

            if (carrierOpen == null)
            {
                return NotFound();
            }

            // Update the properties of the found entity
            carrierOpen.StoreOpen = model.StoreOpen;
            carrierOpen.HouseOpen = model.HouseOpen;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok(carrierOpen); // Return the updated entity
        }



        public class trackDto
        {
            public int TrackSid { get; set; }

            public int? OrderSid { get; set; }

            public string TrackingNum { get; set; }

            public int? SendSouceSid { get; set; }

            public int? ReceiveSourceSid { get; set; }

        }

        public class SendReceiveAddressDto
        {
            public int CarrierAddressSid { get; set; }
            public string RecordName { get; set; }

            public string RecordPhone { get; set; }

            public string Adress { get; set; }
            public string StoreName { get; set; }

        }


        [HttpGet]
        public IActionResult GetTrackInfo(string TrackingNum)
        {
  
            var track = _context.Tracks
                .Include(t => t.SendSouceS)    
                .Include(t => t.ReceiveSourceS) 
                .FirstOrDefault(t => t.TrackingNum == TrackingNum); 

            if (track == null)
            {
                return NotFound();
            }

            var receiveSource = _context.CarrierAddresses
                .Include(ca => ca.CarrierWayNoNavigation) 
                .ThenInclude(cw => cw.CarrierNoNavigation) 
                .FirstOrDefault(ca => ca.CarrierAddressSid == track.ReceiveSourceSid);

            string carrierName = null;
            string carrierWay = null;

            if (receiveSource?.CarrierWayNoNavigation != null)
            {
                carrierWay = receiveSource.CarrierWayNoNavigation.Way; 
                carrierName = receiveSource.CarrierWayNoNavigation.CarrierNoNavigation?.CarrierName; 
            }


            var orderDetail = _context.OrderDetails
                .Where(od => od.OrderSid == track.OrderSid)
                .Include(od => od.ProductS) 
                .FirstOrDefault();

            string productImage = null;

            if (orderDetail != null)
            {
                // 通过 ProductSid 查找产品图片
                var productImageObj = _context.ProductImages
                    .FirstOrDefault(pi => pi.ProductSid == orderDetail.ProductSid);

                if (productImageObj != null)
                {
                    productImage = productImageObj.ProductImagePath; 
                }
            }

    
            List<object> trackTimes = new List<object>();
            if (receiveSource.CarrierAddressSid !=0)
            {
                trackTimes = _context.TrackTimes
                    .Where(tt => tt.TrackSid == track.TrackSid)
                    .Include(tt => tt.TrackStatusNoNavigation)
                    .OrderByDescending(tt => tt.CreatedTime)
                    .Select(tt => new
                    {
                        TrackTimeSid = tt.TrackTimeSid,
                        CreatedTime = tt.CreatedTime,
                        Status = tt.TrackStatusNoNavigation.Status 
                    })
                    .ToList<object>();
            }

        
            var result = new
            {
                TrackSid = track.TrackSid,
                OrderSid = track.OrderSid,
                TrackingNum = track.TrackingNum,
                SendSouceSid = track.SendSouceSid,
                ReceiveSourceSid = track.ReceiveSourceSid,
                CarrierName = carrierName,
                CarrierWay = carrierWay,
                productImage = productImage, 
                trackTimes = trackTimes, 
                sendReceiveAddresses = new List<object>
        {
            track.SendSouceS != null ? new
            {
                CarrierAddressSid = track.SendSouceS.CarrierAddressSid,
                RecordName = track.SendSouceS.RecordName,
                RecordPhone = track.SendSouceS.RecordPhone,
                Adress = track.SendSouceS.Adress,
                StoreName = track.SendSouceS.StoreName
            }:"null" ,
            track.ReceiveSourceS != null ? new
            {
                CarrierAddressSid = track.ReceiveSourceS.CarrierAddressSid,
                RecordName = track.ReceiveSourceS.RecordName,
                RecordPhone = track.ReceiveSourceS.RecordPhone,
                Adress = track.ReceiveSourceS.Adress,
                StoreName = track.ReceiveSourceS.StoreName
            }:"null"
        }
            };

            // 返回结果
            return Ok(result);
        }



        public class OrderSec
        {
            public int? memberSid  { get; set; }
            public int? OrderStatusNo { get; set; }
 
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
     .Where(od => od.detail.ProductS.StoreS.MemberSid == sec.memberSid)
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

                StoreSid=p.StoreSid

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



        public class CarrierViewModel
        {
            public int No { get; set; }
            public string ProductNo { get; set; }
            public string ProductName { get; set; }
            public int Qty { get; set; }

        }

        public JsonResult ProductDetailList(int OrderSid)
        {
            var orderDetails = _context.OrderDetails
                .Where(od => od.OrderSid == OrderSid)
                .Join(_context.Products,
                      detail => detail.ProductSid,
                      product => product.ProductSid,
                      (detail, product) => new CarrierViewModel
                      {
                          No = detail.OrderDetailSid,
                          ProductNo = product.ProductNo,
                          ProductName = product.ProductName,
                          Qty = detail.Qty ?? 0 
                      })

                .ToList();

            return Json(orderDetails);
        }


        public class OrderAndTrackUpdateDto
        {
            public int OrderSid { get; set; }
            public int? SendSouceSid { get; set; }
        }

        [HttpPost]
        public IActionResult UpdateOrderAndTrack([FromBody] OrderAndTrackUpdateDto dto)
        {
 
            if (dto == null || dto.OrderSid <= 0 || !dto.SendSouceSid.HasValue)
            {
                return BadRequest(new { error = "OrderSid and SendSouceSid are required and should be valid." });
            }


            var order = _context.Orders.FirstOrDefault(o => o.OrderSid == dto.OrderSid);

            if (order == null)
            {
   
                return NotFound(new { error = "Order not found." });
            }

 
            var track = _context.Tracks.FirstOrDefault(t => t.OrderSid == dto.OrderSid);

            if (track == null)
            {
                return NotFound(new { error = "No track found for the given OrderSid." });
            }

            order.OrderStatusNo = 2;

            track.SendSouceSid = dto.SendSouceSid;

            _context.SaveChanges();

            return Ok(new { message = "Order and Track updated successfully." });
        }







    }
}
