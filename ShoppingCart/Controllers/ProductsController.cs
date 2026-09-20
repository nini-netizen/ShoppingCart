using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Data;
using ShoppingCart.Models;

namespace ShoppingCart.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;
        public ProductsController(AppDbContext db) => _db = db;

        // 查詢：列表
        public async Task<IActionResult> Index()
        {
            var products = await _db.Products
                .OrderBy(p => p.ProductId)
                .ToListAsync();

            return View(products);
        }

        // 新增：表單
        public IActionResult Create() => View();

        // 新增：送出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product p)
        {
            if (!ModelState.IsValid) return View(p);

            _db.Products.Add(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 修改：表單
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // 修改：送出
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product p)
        {
            if (id != p.ProductId) return BadRequest();
            if (!ModelState.IsValid) return View(p);

            _db.Entry(p).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // 刪除：確認頁
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();
            return View(p);
        }

        // 刪除：送出
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var p = await _db.Products.FindAsync(id);
            if (p == null) return NotFound();

            _db.Products.Remove(p);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
