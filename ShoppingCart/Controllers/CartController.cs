using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _db;
        private const int DemoUserId = 1;

        public CartController(AppDbContext db) => _db = db;

        // 查詢：購物車頁
        public async Task<IActionResult> Index()
        {
            var cart = await GetOrCreateCartAsync();

            var items = await _db.CartItems
                .Where(ci => ci.CartId == cart.CartId)
                .Include(ci => ci.Product)
                .OrderBy(ci => ci.CartItemId)
                .ToListAsync();

            return View(items);
        }

        // 新增：加入購物車（同商品就 +1）
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId)
        {
            var product = await _db.Products.FindAsync(productId);
            if (product == null) return NotFound();

            var cart = await GetOrCreateCartAsync();

            var item = await _db.CartItems
                .FirstOrDefaultAsync(x => x.CartId == cart.CartId && x.ProductId == productId);

            if (item == null)
            {
                item = new CartItem
                {
                    CartId = cart.CartId,
                    ProductId = productId,
                    Quantity = 1,
                    UnitPrice = product.Price
                };
                _db.CartItems.Add(item);
            }
            else
            {
                item.Quantity += 1;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 修改：改數量
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int cartItemId, int quantity)
        {
            if (quantity <= 0) quantity = 1;

            var item = await _db.CartItems
                .Include(x => x.Product)
                .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);

            if (item == null) return NotFound();

            item.Quantity = quantity;
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // 刪除：移除一項
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            var item = await _db.CartItems.FindAsync(cartItemId);
            if (item == null) return NotFound();

            _db.CartItems.Remove(item);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout()
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == DemoUserId && c.Status == "Active");
            if (cart == null) return RedirectToAction(nameof(Index));

            var items = await _db.CartItems
                .Where(ci => ci.CartId == cart.CartId)
                .Include(ci => ci.Product)
                .ToListAsync();

            if (!items.Any()) return RedirectToAction(nameof(Index));

            await using var tx = await _db.Database.BeginTransactionAsync();

            try
            {
                // 1) 庫存檢查
                foreach (var item in items)
                {
                    if (item.Product == null) throw new Exception("Product not found.");
                    if (item.Product.Stock < item.Quantity)
                    {
                        TempData["Error"] = $"庫存不足：{item.Product.Name}（剩 {item.Product.Stock}）";
                        await tx.RollbackAsync();
                        return RedirectToAction(nameof(Index));
                    }
                }

                // 2) 扣庫存
                foreach (var item in items)
                {
                    item.Product!.Stock -= item.Quantity;
                }

                // 3) 清空購物車（或改狀態）
                _db.CartItems.RemoveRange(items);
                cart.Status = "CheckedOut";

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "結帳成功！已扣庫存並清空購物車。";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await tx.RollbackAsync();
                TempData["Error"] = "結帳失敗，交易已回滾。";
                return RedirectToAction(nameof(Index));
            }
        }


        private async Task<Cart> GetOrCreateCartAsync()
        {
            var cart = await _db.Carts.FirstOrDefaultAsync(c => c.UserId == DemoUserId && c.Status == "Active");
            if (cart != null) return cart;

            // 確保 Users 有 UserId=1（如果你沒插入 user，這裡會先建立）
            var userExists = await _db.Users.AnyAsync(u => u.UserId == DemoUserId);
            if (!userExists)
            {
                _db.Users.Add(new User { Email = "demo@example.com", Name = "Demo User" });
                await _db.SaveChangesAsync();
            }

            cart = new Cart { UserId = DemoUserId, Status = "Active" };
            _db.Carts.Add(cart);
            await _db.SaveChangesAsync();
            return cart;
        }
    }
}
