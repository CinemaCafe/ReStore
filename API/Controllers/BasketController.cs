using API.Data;
using API.DTOs;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class BasketController : BaseApiController
    {
        private readonly StoreContext _context;

        public BasketController(StoreContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<BasketDto>> GetBasket()
        {
            var basket = await RetrieveBasket();

            if (basket == null) return NotFound();

            return new BasketDto
            {
                Id = basket.Id,
                BuyerId = basket.BuyerId,
                Items = basket.Items.Select(item => new BasketItemDto
                {
                    ProductId = item.ProductId,
                    Name = item.Product.Name,
                    Price = item.Product.Price,
                    PictureUrl = item.Product.PictureUrl,
                    Type = item.Product.Type,
                    Brand = item.Product.Brand,
                    Quantity = item.Quantity
                }).ToList()
            };
        }


        [HttpPost] // api/basket?productId&quantity=2
        public async Task<ActionResult> AddItemToBasket(int productId, int quantity)
        {
            // get basket || create basket
            var basket = await RetrieveBasket();
            if (basket == null) basket = CreateBasket();

            // get product
            var product = await _context.Products.FindAsync(productId);
            if (product == null) return NotFound();

            // add item
            basket.AddItem(product, quantity);
            // save changes 
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return StatusCode(201);

            return BadRequest(new ProblemDetails { Title = "Problem saving item to basket" });
        }

        [HttpDelete]
        public async Task<ActionResult> RemoveBasketItem(int productId, int quantity)
        {
            // get basket
            var basket = await RetrieveBasket();
            // remove item or reduce quantity
            if (basket == null) return NotFound();
            basket.RemoveItem(productId, quantity);
            // save changes
            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Problem removing item from the basket" });
        }

        /// <summary>
        /// get basket method, do not use Basket return, it will cause the object cycle, casue the Basket contain the BasketItem and then the BasketItem it also contain Basket
        /// </summary>
        /// <returns></returns>
        private async Task<Basket> RetrieveBasket()
        {
            // Entities structure : Basket { BasketItem { Product } }
            // the buyer id from cookie [Request.Cookies]
            var basket = await _context.Baskets
                // to inculde basketitems, this will mean the entity framework now will include the related items with the baskets.
                // Basket { BasketItem }
                .Include(i => i.Items)
                // because our items have a related property for the product.
                // So if we want the product information, we also need to include that
                // Basket { BasketItem { Product } }
                .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(x => x.BuyerId == Request.Cookies["buyerId"]);
            return basket;
        }


        private Basket CreateBasket()
        {
            var buyerId = Guid.NewGuid().ToString();
            var cookieOptions = new CookieOptions { IsEssential = true, Expires = DateTime.Now.AddDays(30) };
            Response.Cookies.Append("buyerId", buyerId, cookieOptions);
            var basket = new Basket { BuyerId = buyerId };
            _context.Baskets.Add(basket);
            return basket;
        }


    }
}