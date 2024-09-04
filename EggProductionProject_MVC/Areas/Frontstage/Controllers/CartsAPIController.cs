using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EggProductionProject_MVC.Models;
using static EggProductionProject_MVC.Areas.Frontstage.Controllers.CartsAPIController;
using EggProductionProject_MVC.Areas.Backstage.Controllers;
using OfficeOpenXml.Export.HtmlExport.StyleCollectors.StyleContracts;
using static EggProductionProject_MVC.Areas.Backstage.Controllers.OrdersController;

namespace EggProductionProject_MVC.Areas.Frontstage.Controllers
{
	[Area("Frontstage")]
	public class CartsAPIController : Controller
	{
		private readonly EggPlatformContext _context;

		public CartsAPIController(EggPlatformContext context)
		{
			_context = context;
		}



		public IActionResult Carts()
		{
			//var carts = _context.Carts.ToList();
			var carts = _context.Products
						//.Include(c => c.ProductS) 
						.ToList();
			return Json(carts);
		}


		public class MemberFilterDto
		{
			public int Msid { get; set; }
			public int[] CartSids { get; set; }

		}


		public class StoreDto
		{
			public int StoreSid { get; set; }
			public string Company { get; set; }
			//public byte[] StoreImg { get; set; }
			public List<ProductDto> Products { get; set; }
		}

		public class ProductDto
		{
			public int CartSid { get; set; }
			public int MemberSid { get; set; }
			public int ProductSid { get; set; }
			public int Qty { get; set; }
			public string ProductNo { get; set; }
			public string ProductName { get; set; }
			public decimal Price { get; set; }
			public int Stock { get; set; }
			public decimal? DiscountPercent { get; set; }

			public int? SubcategoryNo { get; set; }
			public string SubcategoryName { get; set; }
		}

		[HttpPost]
		public IActionResult CartsList([FromBody] MemberFilterDto filter)
		{


			var query = _context.Carts.AsQueryable();


			query = query.Where(c => c.MemberSid == filter.Msid);


			if (filter.CartSids != null && filter.CartSids.Length > 0)
			{
				query = query.Where(c => c.MemberSid == filter.Msid && filter.CartSids.Contains(c.CartSid));
			}



			var cartsWithProducts = query
											.Join(_context.Products,
												  cart => cart.ProductSid,
												  product => product.ProductSid,
												  (cart, product) => new { cart, product })
											.Join(_context.Stores,
												  combined => combined.product.StoreSid,
												  store => store.StoreSid,
												  (combined, store) => new { combined.cart, combined.product, store })
											.Join(_context.ProductSubcategories,
												  combined => combined.product.SubcategoryNo,
												  subcategory => subcategory.SubcategoryNo,
												  (combined, subcategory) => new
												  {
													  combined.store,
													  combined.product,
													  combined.cart,
													  subcategory.SubcategoryName
												  })
											.ToList();


			var groupedByStore = cartsWithProducts
								  .GroupBy(x => new
								  {
									  x.store.StoreSid,
									  x.store.Company,
									  //x.store.StoreImg
								  })
								  .Select(g => new StoreDto
								  {
									  StoreSid = g.Key.StoreSid,
									  Company = g.Key.Company,
									  //StoreImg = g.Key.StoreImg,
									  Products = g.Select(x => new ProductDto
									  {
										  CartSid = x.cart.CartSid,
										  MemberSid = (int)x.cart.MemberSid,
										  ProductSid = x.product.ProductSid,
										  Qty = (int)x.cart.Qty,
										  ProductNo = x.product.ProductNo,
										  ProductName = x.product.ProductName,
										  Price = (decimal)x.product.Price,
										  Stock = (int)x.product.Stock,
										  DiscountPercent = x.product.DiscountPercent,
										  SubcategoryNo = x.product.SubcategoryNo,
										  SubcategoryName = x.SubcategoryName
									  }).ToList()
								  }).ToList();

			return Json(groupedByStore);
		}


		[HttpPost]
		public IActionResult UpdateCartQuantity([FromBody] CartUpdateRequest request)
		{

			var cart = _context.Carts.SingleOrDefault(c => c.CartSid == request.CartSid);

			if (cart != null)
			{
				if (request.Qty < 1)
				{

					_context.Carts.Remove(cart);
					_context.SaveChanges();
				}


				cart.Qty = request.Qty;


				_context.SaveChanges();


				return Ok();
			}
			else
			{

				return NotFound($"Cart with CartSid {request.CartSid} not found.");
			}
		}

		public class CartUpdateRequest
		{
			public int CartSid { get; set; }
			public int Qty { get; set; }
		}


		public class CarrierFilterDto
		{
			public int Msid { get; set; }
			public int storeSid { get; set; }
		}



		[HttpPost]
		public IActionResult AddressList([FromBody] CarrierFilterDto filter)
		{
			var store = _context.CarrierOpens
				.Where(c => c.StoreSid == filter.storeSid)
				.Select(x => new CarrierOpenDto
				{
					StoreSid = (int)x.StoreSid,
					StoreOpen = x.StoreOpen,
					HouseOpen = (int)x.HouseOpen,
				})
				.ToList();

			var query = _context.CarrierAddresses.Where(c => c.MemberSid == filter.Msid);

			if (store.Any())
			{
				var item = store.FirstOrDefault();

				if (item.StoreOpen == 1 && item.HouseOpen == 1)
				{
					query = _context.CarrierAddresses.Where(c => c.MemberSid == filter.Msid);
				}
				else if (item.StoreOpen == 1 && item.HouseOpen == 0)
				{
					query = _context.CarrierAddresses.Where(c => c.MemberSid == filter.Msid && c.CarrierWayNoNavigation.CarrierNo == 1);
				}
				else if (item.StoreOpen == 0 && item.HouseOpen == 1)
				{
					query = _context.CarrierAddresses.Where(c => c.MemberSid == filter.Msid && c.CarrierWayNoNavigation.CarrierNo == 2);
				}
			}

			var cartsWithProducts = query
				.Join(_context.CarrierWays,
					ad => ad.CarrierWayNo,
					way => way.CarrierWayNo,
					(ad, way) => new { ad, way })
				.Join(_context.PublicStatuses,
					combined => combined.ad.PublicStatusNo,
					ps => ps.PublicStatusNo,
					(combined, ps) => new { combined.ad, combined.way, ps })
				.Join(_context.Carriers,
					combined => combined.way.CarrierNo,
					c => c.CarrierNo,
					(combined, c) => new { combined.ad, combined.way, combined.ps, c })
				.Select(x => new AddressDto
				{
					CarrierAddressSid = x.ad.CarrierAddressSid,
					MemberSid = (int)x.ad.MemberSid,
					AddressFirst = (int)x.ad.AddressFirst,
					CarrierWayNo = (int)x.ad.CarrierWayNo,
					CarrierNo = (int)x.c.CarrierNo,
					CarrierCode = x.c.CarrierCode.ToString(),
					CarrierName = x.c.CarrierName.ToString(),
					Way = x.way.Way.ToString(),
					Price = (decimal)x.way.Price,
					RecordName = x.ad.RecordName,
					RecordPhone = x.ad.RecordPhone,
					Adress = x.ad.Adress,
					StoreId = x.ad.StoreId,
					StoreName = x.ad.StoreName,

				}).ToList();


			var groupedByStore = cartsWithProducts
	.GroupBy(x => x.CarrierNo)
	.Select(g => new CarrierDto
	{
		CarrierNo = g.Key,
		CarrierCode = g.First().CarrierCode,

		Addresses = g.Select(x => new AddressfilterDto
		{
			CarrierAddressSid = x.CarrierAddressSid,
			CarrierName = x.CarrierName,
			MemberSid = x.MemberSid,
			AddressFirst = x.AddressFirst,
			CarrierWayNo = x.CarrierWayNo,
			Way = x.Way,
			Price = x.Price,
			RecordName = x.RecordName,
			RecordPhone = x.RecordPhone,
			Adress = x.Adress,
			StoreId = x.StoreId,
			StoreName = x.StoreName,
		}).ToList()
	}).ToList();

			return Json(groupedByStore);
		}


		public class CarrierOpenDto
		{
			public int StoreSid { get; set; }
			public int? StoreOpen { get; set; }
			public int? HouseOpen { get; set; }


		}

		public class CarrierDto
		{
			public int CarrierNo { get; set; }
			public string CarrierCode { get; set; }


			public List<AddressfilterDto> Addresses { get; set; }
		}

		public class AddressDto
		{
			public int CarrierAddressSid { get; set; }

			public int MemberSid { get; set; }
			public int AddressFirst { get; set; }
			public int CarrierWayNo { get; set; }
			public int CarrierNo { get; set; }
			public string CarrierCode { get; set; }
			public string CarrierName { get; set; }
			public string Way { get; set; }
			public decimal Price { get; set; }
			public string RecordName { get; set; }
			public string RecordPhone { get; set; }

			public string Adress { get; set; }

			public string StoreId { get; set; }

			public string StoreName { get; set; }

		}


		public class AddressfilterDto
		{
			public int CarrierAddressSid { get; set; }
			public string CarrierName { get; set; }
			public int MemberSid { get; set; }
			public int AddressFirst { get; set; }
			public int CarrierWayNo { get; set; }

			public string Way { get; set; }
			public decimal Price { get; set; }
			public string RecordName { get; set; }
			public string RecordPhone { get; set; }

			public string Adress { get; set; }

			public string StoreId { get; set; }

			public string StoreName { get; set; }

		}


		public async Task<IActionResult> CoinList([FromQuery] int memberSid)
		{
			var coins = await _context.StoreCoins
				.Where(x => x.MemberSid == memberSid && x.CoinUseAreaNo == 1)
				.Select(x => new CoinDto
				{
					StoreCoinSid = (int)x.StoreCoinSid,
					MemberSid = x.MemberSid,
					CoinUseAreaNo = (int)x.CoinUseAreaNo,
					AreaSid = (int)x.AreaSid,
					IsPositive = (int)x.IsPositive,
					Money = (decimal)x.Money,
					ChangeTime = (DateTime)x.ChangeTime
				})
				.ToListAsync();

			return Json(coins);
		}

		public async Task<IActionResult> GetTotal([FromQuery] int memberSid)
		{

			var coins = await _context.StoreCoins
			.Where(x => x.MemberSid == memberSid && x.CoinUseAreaNo == 1)
			.Select(x => new CoinDto
			{
				StoreCoinSid = (int)x.StoreCoinSid,
				MemberSid = x.MemberSid,
				CoinUseAreaNo = (int)x.CoinUseAreaNo,
				AreaSid = (int)x.AreaSid,
				IsPositive = (int)x.IsPositive,
				Money = (decimal)x.Money,
				ChangeTime = (DateTime)x.ChangeTime
			})
			.ToListAsync();

			var total = coins.Sum(x => x.IsPositive == 1 ? x.Money : -x.Money);

			return Json(total);
		}


		public async Task<IActionResult> GetCoupons([FromQuery] int memberSid, decimal totalmoney)
		{
			var coupons = await _context.Coupons
				.Where(x => x.MemberSid == memberSid && x.CouponStatusNo == 1)
				.Join(_context.CouponTypes,
					c => c.CouponTypeNo,
					ct => ct.CouponTypeNo,
					(c, ct) => new
					{
						CouponSid = c.CouponSid,
						CouponTypeNo = c.CouponTypeNo,
						Name = ct.Name,
						Price = ct.Price,
						Minimum = ct.Minimum,
						CouponStatusNo = c.CouponStatusNo,
						MemberSid = c.MemberSid
					})
				.Select(x => new CouponDto
				{
					CouponSid = x.CouponSid,
					CouponTypeNo = x.CouponTypeNo,
					Name = x.Name,
					Price = x.Price,
					Minimum = x.Minimum,
					CouponStatusNo = x.CouponStatusNo,
					MemberSid = x.MemberSid,
					StatusMessage = totalmoney >= x.Minimum
				? null
				: $"(低消，需滿 NT${x.Minimum:0})"
				})
				.ToListAsync();

			return Json(coupons);
		}

		public class CouponDto
		{
			public int CouponSid { get; set; }

			public int? CouponTypeNo { get; set; }

			public string? Name { get; set; }

			public decimal? Price { get; set; }

			public decimal? Minimum { get; set; }

			public int? CouponStatusNo { get; set; }


			public int? MemberSid { get; set; }

			public string? StatusMessage { get; set; }


		}







		[HttpPost]
		public async Task<IActionResult> AddCoin([FromBody] CoinDto coinDto)
		{
			if (coinDto == null)
			{
				return BadRequest("Invalid data.");
			}

			// 将 DTO 转换为数据库实体
			var coin = new StoreCoin
			{
				MemberSid = coinDto.MemberSid,
				CoinUseAreaNo = coinDto.CoinUseAreaNo,
				AreaSid = coinDto.AreaSid,
				IsPositive = coinDto.IsPositive,
				Money = coinDto.Money,
				ChangeTime = coinDto.ChangeTime
			};

			// 添加到数据库上下文
			_context.StoreCoins.Add(coin);
			await _context.SaveChangesAsync();

			// 返回成功响应
			return CreatedAtAction(nameof(CoinList), new { memberSid = coinDto.MemberSid }, coinDto);
		}

		public class CoinDto
		{
			public int StoreCoinSid { get; set; }

			public int? MemberSid { get; set; }

			public int? CoinUseAreaNo { get; set; }

			public int? AreaSid { get; set; }

			public int? IsPositive { get; set; }

			public decimal? Money { get; set; }

			public DateTime? ChangeTime { get; set; }
		}

		//----------------------------------------------------------------


		public class OrderRequestDto
		{
			public int CustomerId { get; set; }
			public int? CouponId { get; set; }
			public List<OrderItemDto> Items { get; set; }
		}

		public class OrderItemDto
		{
			public int ProductId { get; set; }
			public int Quantity { get; set; }
		}


		public class OrderResponseDto
		{
			public int OrderId { get; set; }
			public decimal TotalAmount { get; set; }
			public List<OrderItemResponseDto> Items { get; set; }
		}

		public class OrderItemResponseDto
		{
			public int ProductId { get; set; }
			public int Quantity { get; set; }
			public decimal UnitPrice { get; set; }
		}



		//----------------------------------------------------------------



		public class EndStore
		{
			public int StoreSid { get; set; }
			public int AddressSid { get; set; }
			public string CarrierCode { get; set; }

			public decimal? CarrierPrice { get; set; }
			public List<int> Cart { get; set; }
			public decimal ProductsMoney { get; set; }
			public decimal? CouponPrice { get; set; }
			public int Usecoin { get; set; }

			public int Coinback { get; set; }
			public decimal? OrderTotal { get; set; }

		}

		public class RequestModel
		{
			public int MemberSid { get; set; }
			public List<EndStore> Stores { get; set; }
			public decimal Combo { get; set; }
			public int Usecoin { get; set; }
			public int? CouponSid { get; set; }

			public int? PaymentNo { get; set; }
		}


		private async Task<string> GenerateOrderNoAsync()
		{
			var today = DateTime.UtcNow.ToString("yyyyMMdd");
			var sequenceNumber = await GetNextSequenceNumberAsync(today);
			return $"O{today}{sequenceNumber:D4}"; // 用四位数填充序列号
		}

		private async Task<int> GetNextSequenceNumberAsync(string today)
		{
			// 查询当天的最大订单号并提取序列号部分
			var maxOrderNo = await _context.Orders
				.Where(o => o.OrderNo.StartsWith($"O{today}"))
				.Select(o => o.OrderNo)
				.OrderByDescending(o => o)
				.FirstOrDefaultAsync();

			if (maxOrderNo != null)
			{
				// 提取序列号并加1
				var sequenceNumberPart = maxOrderNo.Substring(maxOrderNo.Length - 4); // 获取订单号的最后4位
				if (int.TryParse(sequenceNumberPart, out var sequenceNumber))
				{
					return sequenceNumber + 1;
				}
			}

			// 如果没有找到订单号，返回0001作为初始序列号
			return 1;
		}

		private async Task<string> GenerateTrackingNumAsync(string carrierCode)
		{
			var today = DateTime.UtcNow.ToString("yyyyMMdd");
			var sequenceNumber = await GetNextTrackingSequenceNumberAsync(carrierCode, today);
			return $"{carrierCode}{today}{sequenceNumber:D4}"; // 用四位数填充序列号
		}

		private async Task<int> GetNextTrackingSequenceNumberAsync(string carrierCode, string today)
		{
			// 查询当天的最大追踪号并提取序列号部分
			var maxTrackingNum = await _context.Tracks
				.Where(t => t.TrackingNum.StartsWith($"{carrierCode}{today}"))
				.Select(t => t.TrackingNum)
				.OrderByDescending(t => t)
				.FirstOrDefaultAsync();

			if (maxTrackingNum != null)
			{
				// 提取序列号并加1
				var sequenceNumberPart = maxTrackingNum.Substring(maxTrackingNum.Length - 4); // 获取追踪号的最后4位
				if (int.TryParse(sequenceNumberPart, out var sequenceNumber))
				{
					return sequenceNumber + 1;
				}
			}

			// 如果没有找到追踪号，返回0001作为初始序列号
			return 1;
		}



		[HttpPost]
		public async Task<IActionResult> UpdatePrices([FromBody] RequestModel request)
		{
			if (request?.Stores == null || !request.Stores.Any())
			{
				return BadRequest("Invalid request data.");
			}


			decimal? couponPrice = null;
			if (request.CouponSid.HasValue)
			{
				var coupon = await _context.Coupons
					.Include(c => c.CouponTypeNoNavigation)
					.FirstOrDefaultAsync(c => c.CouponSid == request.CouponSid.Value);

				if (coupon != null)
				{
					couponPrice = coupon.CouponTypeNoNavigation?.Price;
				}
			}


			var numberOfStores = request.Stores.Count;
			var couponPricePerStore = couponPrice.HasValue ? Math.Round(couponPrice.Value / numberOfStores, 2) : 0;
			var remainingCouponPrice = couponPrice.HasValue ? couponPrice.Value - (couponPricePerStore * numberOfStores) : 0;

			var usecoinTotal = request.Usecoin;
			var usecoinPerStore = usecoinTotal / numberOfStores;
			var remainingUsecoin = usecoinTotal - (usecoinPerStore * numberOfStores);


			decimal? totalOrderAmount = 0;


			decimal distributedUsecoin = 0;

			foreach (var store in request.Stores)
			{

				if (store.AddressSid > 0)
				{
					var carrierAddress = await _context.CarrierAddresses
						.Include(ca => ca.CarrierWayNoNavigation)
						.ThenInclude(cw => cw.CarrierNoNavigation)
						.FirstOrDefaultAsync(ca => ca.CarrierAddressSid == store.AddressSid);

					if (carrierAddress != null && carrierAddress.CarrierWayNoNavigation != null)
					{
						store.CarrierPrice = carrierAddress.CarrierWayNoNavigation.Price;
						store.CarrierCode = carrierAddress.CarrierWayNoNavigation.CarrierNoNavigation?.CarrierCode ?? string.Empty;
					}
				}


				if (store.Cart != null && store.Cart.Any())
				{
					var carts = await _context.Carts
						.Where(c => store.Cart.Contains(c.CartSid))
						.Include(c => c.ProductS)
						.ToListAsync();

					store.ProductsMoney = carts.Sum(c =>
					{
						if (c.ProductS != null)
						{
							var price = c.ProductS.Price ?? 0;
							var discountPercent = c.ProductS.DiscountPercent ?? 1;
							var discountedPrice = Math.Round(price * discountPercent, 2);

							return discountedPrice * (c.Qty ?? 1);
						}
						return 0;
					});
				}


				store.Usecoin = usecoinPerStore;
				distributedUsecoin += store.Usecoin;

				store.CouponPrice = couponPricePerStore;


				store.OrderTotal = Math.Round(
	(store.ProductsMoney) * (request.Combo)
	- (store.Usecoin)
	- (store.CouponPrice ?? 0)
	+ (store.CarrierPrice ?? 0)
);


				totalOrderAmount += store.OrderTotal;
			}


			var remainingUsecoinToDistribute = remainingUsecoin;
			if (remainingUsecoinToDistribute > 0 && request.Stores.Any())
			{
				request.Stores.First().Usecoin += remainingUsecoinToDistribute;
				request.Stores.First().OrderTotal -= remainingUsecoinToDistribute;
			}

			decimal totalOrderTotal = request.Stores.Sum(store => store.OrderTotal) ?? 0m;


			int coinbackTotal = (int)Math.Round(totalOrderTotal * 0.01m, MidpointRounding.AwayFromZero);

			int distributedCoinback = 0;

			foreach (var store in request.Stores)
			{

				decimal proportion = (decimal)(totalOrderTotal == 0 ? 0 : store.OrderTotal / totalOrderTotal);
				int storeCoinback = (int)Math.Round(coinbackTotal * proportion, MidpointRounding.AwayFromZero);

				store.Coinback = storeCoinback;
				distributedCoinback += storeCoinback;
			}


			int difference = coinbackTotal - distributedCoinback;
			if (difference != 0 && request.Stores.Any())
			{
				request.Stores.First().Coinback += difference;
			}


			var orderNumbers = await SaveOrders(request);

			return Ok(new
			{
				memberSid = request.MemberSid,
				stores = request.Stores,
				combo = request.Combo,
				usecoin = request.Usecoin,
				couponSid = request.CouponSid,
				paymentNo = request.PaymentNo,
				orderNumbers = orderNumbers
			});
		}


		public async Task<List<string>> SaveOrders(RequestModel request)
		{
			if (request?.Stores == null || !request.Stores.Any())
			{
				return new List<string>();
			}

			var cartSidsToDelete = new List<int>();
			var orderNumbers = new List<string>();

			foreach (var store in request.Stores)
			{
				var orderNo = await GenerateOrderNoAsync();

				var order = new Order
				{
					OrderNo = orderNo,
					MemberSid = request.MemberSid,
					PaymentNo = request.PaymentNo,
					AlreadyPaid = request.PaymentNo == 1 ? 1 : 0,
					OrderStatusNo = 1,
					CouponSid = request.CouponSid,
					TotalPrice = store.OrderTotal
				};

				_context.Orders.Add(order);
				await _context.SaveChangesAsync();

				orderNumbers.Add(orderNo);


				foreach (var cartSid in store.Cart)
				{
					var cart = await _context.Carts.Include(c => c.ProductS)
												   .FirstOrDefaultAsync(c => c.CartSid == cartSid);

					if (cart != null && cart.ProductS != null)
					{
						var orderDetail = new OrderDetail
						{
							OrderSid = order.OrderSid,
							ProductSid = cart.ProductSid,
							BuyPrice = (cart.ProductS?.Price ?? 0) * (cart.ProductS?.DiscountPercent ?? 1),
							Qty = cart.Qty
						};

						_context.OrderDetails.Add(orderDetail);


						cartSidsToDelete.Add(cart.CartSid);
					}
				}


				var track = new Track
				{
					OrderSid = order.OrderSid,
					TrackingNum = await GenerateTrackingNumAsync(store.CarrierCode),
					ReceiveSourceSid = store.AddressSid
				};

				_context.Tracks.Add(track);


				if (store.Usecoin > 0)
				{
					var storeCoinUse = new StoreCoin
					{
						MemberSid = request.MemberSid,
						CoinUseAreaNo = 1,
						AreaSid = order.OrderSid,
						IsPositive = 0,
						Money = store.Usecoin
					};

					_context.StoreCoins.Add(storeCoinUse);
				}


				if (store.Coinback > 0)
				{
					var storeCoinBack = new StoreCoin
					{
						MemberSid = request.MemberSid,
						CoinUseAreaNo = 1,
						AreaSid = order.OrderSid,
						IsPositive = 1,
						Money = store.Coinback
					};

					_context.StoreCoins.Add(storeCoinBack);
				}

				await _context.SaveChangesAsync();
			}

			// Uncomment to delete carts and update coupon status if needed
			// if (cartSidsToDelete.Any())
			// {
			//     var cartsToDelete = await _context.Carts
			//                                        .Where(c => cartSidsToDelete.Contains(c.CartSid))
			//                                        .ToListAsync();
			//     _context.Carts.RemoveRange(cartsToDelete);
			//     await _context.SaveChangesAsync();
			// }

			// if (request.CouponSid.HasValue)
			// {
			//     var coupon = await _context.Coupons
			//                                .FirstOrDefaultAsync(c => c.CouponSid == request.CouponSid.Value);
			//     if (coupon != null)
			//     {
			//         coupon.CouponStatusNo = 2; // Update status to 2
			//         _context.Coupons.Update(coupon);
			//         await _context.SaveChangesAsync();
			//     }
			// }

			return orderNumbers;
		}



		//-----------------------------------------------------------------------------------------


		public class OrderDto
		{
			public int OrderSid { get; set; }
			public string OrderNo { get; set; }
			public DateTime? OrderCreatedTime { get; set; }
			public int? MemberSid { get; set; }
			public int? PaymentNo { get; set; }
			public int? AlreadyPaid { get; set; }
			public int? OrderStatusNo { get; set; }
			public int? CouponSid { get; set; }
			public decimal? TotalPrice { get; set; }
			public List<OrderDetailDto> OrderDetails { get; set; }
		}

		public class OrderDetailDto
		{
			public int OrderDetailSid { get; set; }
			public int? OrderSid { get; set; }
			public int? ProductSid { get; set; }
			public decimal? BuyPrice { get; set; }
			public int? Qty { get; set; }
			public ProductDto Product { get; set; }
		}

		public class ProductOrderDto
		{
			public int ProductSid { get; set; }
			public int? StoreSid { get; set; }
			public StoreDto Store { get; set; }
		}

		public class StoreOrderDto
		{
			public int StoreSid { get; set; }
			public string Company { get; set; }
		}


		//        [HttpPost]
		//public IActionResult GetOrdersByNumbers([FromBody] List<string> orderNumbers)
		//{
		//            var query = _context.Orders
		//        .Where(o => orderNumbers.Contains(o.OrderNo))
		//        .AsQueryable();

		//            // 查询数据并连接相关表
		//         var ordersWithDetails = query
		//    .Join(_context.OrderDetails,
		//          o => o.OrderSid,
		//          od => od.OrderSid,
		//          (o, od) => new { o, od })
		//    .Join(_context.Products,
		//          o_od => o_od.od.ProductSid,
		//          p => p.ProductSid,
		//          (o_od, p) => new { o_od.o, o_od.od, p })
		//    .Join(_context.Stores,
		//          o_od_p => o_od_p.p.StoreSid,
		//          s => s.StoreSid,
		//          (o_od_p, s) => new
		//          {
		//              Order = o_od_p.o,
		//              OrderDetail = o_od_p.od,
		//              Product = o_od_p.p,
		//              Store = s
		//          })
		//    .GroupBy(result => new
		//    {
		//        result.Order.OrderSid,
		//        result.Order.OrderNo,
		//        result.Order.OrderCreatedTime,
		//        result.Order.PaymentNo,
		//        result.Order.AlreadyPaid,
		//        result.Order.TotalPrice,
		//        result.Store.StoreSid,
		//        result.Store.Company,
		//        result.Order.CouponSid // Including CouponSid in the grouping
		//    })
		//    .Select(g => new
		//    {
		//        OrderSid = g.Key.OrderSid,
		//        OrderNo = g.Key.OrderNo,
		//        OrderCreatedTime = g.Key.OrderCreatedTime,
		//        PaymentNo = g.Key.PaymentNo,
		//        AlreadyPaid = g.Key.AlreadyPaid,
		//        TotalPrice = g.Key.TotalPrice,
		//        StoreSid = g.Key.StoreSid,
		//        Company = g.Key.Company,
		//        CouponSid = g.Key.CouponSid,
		//        OrderDetails = g.Select(x => new
		//        {
		//            OrderDetailSid = x.OrderDetail.OrderDetailSid,
		//            PName = x.Product.ProductName,
		//            PSName = x.Product.SubcategoryNoNavigation.SubcategoryName,
		//            BuyPrice = x.OrderDetail.BuyPrice,
		//            Qty = x.OrderDetail.Qty,
		//        }).ToList()
		//    })
		//    .ToList();

		//// Calculating total order price
		//decimal? allPrice = ordersWithDetails.Sum(o => o.TotalPrice);

		//// Retrieving total coupon price
		//decimal? couponprice = ordersWithDetails.Sum(o => 
		//    _context.Coupons
		//        .Where(c => c.CouponSid == o.CouponSid)
		//        .Select(c => c.CouponTypeNoNavigation.Price)
		//        .FirstOrDefault() ?? 0
		//);

		//// Returning result with allPrice and couponprice
		//var result = new
		//{
		//    Orders = ordersWithDetails,
		//    AllPrice = allPrice,
		//    CouponPrice = couponprice
		//};


		//            return Ok(result);




		//        }

		[HttpPost]
		public IActionResult GetOrdersByNumbers([FromBody] List<string> orderNumbers)
		{

			var orderSids = _context.Orders
				.Where(o => orderNumbers.Contains(o.OrderNo))
				.Select(o => o.OrderSid)
				.ToList();


			var ordersWithDetails = _context.Orders
				.Where(o => orderNumbers.Contains(o.OrderNo))
				.Join(_context.OrderDetails,
					  o => o.OrderSid,
					  od => od.OrderSid,
					  (o, od) => new { o, od })
				.Join(_context.Products,
					  o_od => o_od.od.ProductSid,
					  p => p.ProductSid,
					  (o_od, p) => new { o_od.o, o_od.od, p })
				.Join(_context.Stores,
					  o_od_p => o_od_p.p.StoreSid,
					  s => s.StoreSid,
					  (o_od_p, s) => new
					  {
						  Order = o_od_p.o,
						  OrderDetail = o_od_p.od,
						  Product = o_od_p.p,
						  Store = s
					  })
				.GroupBy(result => new
				{
					result.Order.OrderSid,
					result.Order.OrderNo,
					result.Order.OrderCreatedTime,
					result.Order.PaymentNo,
					result.Order.AlreadyPaid,
					result.Order.TotalPrice,
					result.Store.StoreSid,
					result.Store.Company,
					result.Order.CouponSid
				})
				.Select(g => new
				{
					OrderSid = g.Key.OrderSid,
					OrderNo = g.Key.OrderNo,
					OrderCreatedTime = g.Key.OrderCreatedTime,
					PaymentNo = g.Key.PaymentNo,
					AlreadyPaid = g.Key.AlreadyPaid,
					TotalPrice = g.Key.TotalPrice,
					StoreSid = g.Key.StoreSid,
					Company = g.Key.Company,
					CouponSid = g.Key.CouponSid,
					OrderDetails = g.Select(x => new
					{
						OrderDetailSid = x.OrderDetail.OrderDetailSid,
						PName = x.Product.ProductName,
						PSName = x.Product.SubcategoryNoNavigation.SubcategoryName,
						BuyPrice = x.OrderDetail.BuyPrice,
						Qty = x.OrderDetail.Qty,
					}).ToList()
				})
				.ToList();


			decimal? allPrice = ordersWithDetails.Sum(o => o.TotalPrice);

			decimal? couponprice = ordersWithDetails
		.Select(o => _context.Coupons
			.Where(c => c.CouponSid == o.CouponSid)
			.Select(c => c.CouponTypeNoNavigation.Price)
			.FirstOrDefault() ?? 0
		)
		.FirstOrDefault();


			decimal? totalStoreCoin = _context.StoreCoins
				.Where(sc => orderSids.Contains(sc.AreaSid ?? 0) && sc.IsPositive == 1)
				.Sum(sc => sc.Money) ?? 0;


			var result = new
			{
				Orders = ordersWithDetails,
				AllPrice = allPrice,
				CouponPrice = couponprice,
				TotalStoreCoin = totalStoreCoin
			};

			return Ok(result);
		}






	}
}

