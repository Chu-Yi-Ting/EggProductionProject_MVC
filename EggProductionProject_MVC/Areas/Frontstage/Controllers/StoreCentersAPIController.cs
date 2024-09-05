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

            public string Adress { get; set; }

            public string StoreId { get; set; }

            public string StoreName { get; set; }

            public int? PublicStatusNo { get; set; }
            // 包括所有可能更新的字段
        }


        //Post 用於傳address到資料庫(新增/更改)
        [HttpPost]
        [Route("Frontstage/StoreCentersAPI/CarrierAddress/Save")]
        public IActionResult SaveCarrierAddress([FromBody] CarrierAddress model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
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

            //return Ok("Operation successful.");
            return Ok(new { message = "Operation successful." });
        }





        //public class CarrierAddressFilterDto
        //{
        //    public int CarrierAddressSid { get; set; }

        //    public int lsHome { get; set; }

        //}


        //Get 編輯CarrierAddressSid資料
        public IActionResult AddressOne( int CarrierAddressSid)
        {
            var query = _context.CarrierAddresses.AsQueryable();

            query = query.Where(c => c.CarrierAddressSid == CarrierAddressSid);

            var CarrierAddress = query
                .Select(x => new UpdateCarrierAddressDto
                {
                    CarrierAddressSid = x.CarrierAddressSid,
                    MemberSid = (int)x.MemberSid,
                    AddressFirst = (int)x.AddressFirst,
                    RecordName = x.RecordName,
                    RecordPhone = x.RecordPhone,
                    CarrierWayNo = x.CarrierWayNo,
                    Adress = x.Adress,
                    StoreId = x.StoreId,
                    StoreName = x.StoreName,
                    PublicStatusNo = x.PublicStatusNo

                }).ToList();

            return Json(CarrierAddress);
        }







    }
}
