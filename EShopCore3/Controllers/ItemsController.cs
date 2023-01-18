using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EShopCore3.Data;
using EShopCore3.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EShopCore3.Controllers
{
    public class ItemsController : Controller
    {
        private readonly EShopCore3Context _context;

        public ItemsController(EShopCore3Context context)
        {
            _context = context;
        }

        [HttpPost]
        // GET: Items
        public IActionResult Index(int pg = 1)
        {
            var categoryItemList = new List<string>();

            var categoryItemQry = from item in _context.Item
                                  orderby item.Category
                                  select item.Category;

            categoryItemList.AddRange(categoryItemQry.Distinct());
            ViewBag.categoryItem = new SelectList(categoryItemList);

            List<Item> items = _context.Item.ToList();

            const int pageSize = 3;
            if (pg < 1)
            {
                pg = 1;
            }

            int rescCount = items.Count();
            var pager = new Pager(rescCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = items.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        public IActionResult Index(int pg, string categoryItem, string searchItem)
        {
            var categoryItemList = new List<string>();

            var categoryItemQry = from item in _context.Item
                                  orderby item.Category
                                  select item.Category;

            categoryItemList.AddRange(categoryItemQry.Distinct());
            ViewBag.categoryItem = new SelectList(categoryItemList);

            var itemQry = from item in _context.Item
                          select item;

            if (!string.IsNullOrEmpty(searchItem))
            {
                itemQry = itemQry.Where(model => model.ItemName.StartsWith(searchItem));
            }

            if (!string.IsNullOrEmpty(categoryItem))
            {
                itemQry = itemQry.Where(model => model.Category == categoryItem);
            }

           var items = itemQry;

            //var pg = 1;
            const int pageSize = 3;
            if (pg < 1)
            {
                pg = 1;
            }

            int rescCount = items.Count();
            var pager = new Pager(rescCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = items.Skip(recSkip).Take(pager.PageSize).ToList();

            ViewBag.Pager = pager;

            return View(data);
        }

        // GET: Items/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // GET: Items/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Items/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,Category,ItemName,Description,ItemPrice,URL1,URL2,HOVERURL1,HOVERURL2,Gender,UsedBy,Promotion,PromoPrice")] Item item)
        {
            if (ModelState.IsValid)
            {
                item.ItemId = Guid.NewGuid();
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: Items/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ItemId,Category,ItemName,Description,ItemPrice,URL1,URL2,HOVERURL1,HOVERURL2,Gender,UsedBy,Promotion,PromoPrice")] Item item)
        {
            if (id != item.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemExists(item.ItemId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: Items/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.Item == null)
            {
                return NotFound();
            }

            var item = await _context.Item
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.Item == null)
            {
                return Problem("Entity set 'EShopCore3Context.Item'  is null.");
            }
            var item = await _context.Item.FindAsync(id);
            if (item != null)
            {
                _context.Item.Remove(item);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ItemExists(Guid id)
        {
          return (_context.Item?.Any(e => e.ItemId == id)).GetValueOrDefault();
        }
    }
}
