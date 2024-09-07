using EggProductionProject_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.CartsAPIController;

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




    }
}
