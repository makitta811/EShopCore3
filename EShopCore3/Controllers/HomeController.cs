using EShopCore3.Data;
using EShopCore3.Models;
using EShopCore3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace EShopCore3.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EShopCore3Context _context;
        List<CartViewModel> listOfCartViewModels;
        List<Item> listOfSearchItems;

        public HomeController(ILogger<HomeController> logger, EShopCore3Context context)
        {
            _logger = logger;
            _context = context;
            listOfCartViewModels = new List<CartViewModel>();
            listOfSearchItems = new List<Item>();
        }

        public async Task<IActionResult> Index()
        {
            InitialCategory();
            return _context.Item != null ?
                View(await _context.Item.ToListAsync()) :
                Problem("Entity set 'EShopCore3Context.Item' is null");
        }
        /* APPLICATION       =>     ASP.CORE
         * 
           Session["ImetoNaSessiqta"] = T.Lsit; == HttpContext.Session.Set("ImetoNaSessiqta",T.List)      
         */
        [HttpPost]
        public JsonResult Index(string itemId)
        {
            CartViewModel cartView = new CartViewModel();
            Item item = _context.Item.Single(model => model.ItemId.ToString() == itemId);
            var itemsSession = HttpContext.Session.Get<List<CartViewModel>>("cartItems");
            if (itemsSession != null)
            {
                listOfCartViewModels = itemsSession;
            }
            if (listOfCartViewModels.Any(model => model.ItemId == itemId))
            {
                cartView = listOfCartViewModels.Single(model => model.ItemId == itemId);
                cartView.Quantity = cartView.Quantity + 1;
                if (cartView.PromoPrice != 0)
                {
                    cartView.Total = cartView.Quantity * cartView.PromoPrice;
                }
                else
                {
                    cartView.Total = cartView.Quantity * cartView.UnitPrice;
                }
            }
            else
            {
                cartView.ItemId = itemId;
                cartView.Quantity = 1;
                cartView.ItemName = item.ItemName;
                cartView.Description = item.Description;
                cartView.ImagePath = item.URL1;
                cartView.UnitPrice = item.ItemPrice;
                cartView.PromoPrice = item.PromoPrice;
                if (cartView.PromoPrice != 0)
                {
                    cartView.Total = cartView.PromoPrice;
                }
                else
                {
                    cartView.Total = cartView.UnitPrice;
                }
                listOfCartViewModels.Add(cartView);
            }

            HttpContext.Session.Set("cartItems", listOfCartViewModels);
            return Json(new { Success = true, Message = "Item added to Cart!" });
        }

        //[HttpPost]
        //public IActionResult SearchItems(int pg = 1)
        //{
        //    InitialCategory();
        //    listOfSearchItems = HttpContext.Session.Get<List<Item>>("SearchItem");
        //    List<Item> items = listOfSearchItems;

        //    const int pageSize = 3;
        //    if (pg < 1)
        //    {
        //        pg = 1;
        //    }

        //    int rescCount = items.Count();
        //    var pager = new Pager(rescCount, pg, pageSize);
        //    int recSkip = (pg - 1) * pageSize;
        //    var data = items.Skip(recSkip).Take(pager.PageSize).ToList();

        //    ViewBag.Pager = pager;
        //    HttpContext.Session.Set("SearchItem", data);
        //    return View(data);
        //}

        public IActionResult SearchItems(int pg, string categoryItem, string searchItem)
        {
            InitialCategory();
            HttpContext.Session.Set("categoryItem", categoryItem);
            HttpContext.Session.Set("searchItem", searchItem);

            var itemsQry = from item in _context.Item
                           select item;

            if (!string.IsNullOrEmpty(searchItem))
            {
                itemsQry = itemsQry.Where(model => model.ItemName.StartsWith(searchItem));
                //HttpContext.Session.Set("SearchItem", itemsQry.ToList());
            }

            if (string.IsNullOrEmpty(searchItem))
            {
             //   HttpContext.Session.Set("SearchItem", _context.Item.ToList());
            }

            if (!string.IsNullOrEmpty(categoryItem))
            {
                itemsQry = itemsQry.Where(model => model.Category == categoryItem);
              //  HttpContext.Session.Set("SearchItem", itemsQry.ToList());
            }

            if (categoryItem == "All")
            {
               // HttpContext.Session.Set("SearchItem", _context.Item.ToList());
            }

            //if (Session["SearchItem"] != null)
            //{
            //    listOfSearchItems = Session["SearchItem"] as List<Item>;
            //}

            

            //var pg = 1;
            const int pageSize = 3;
            if (pg < 1)
            {
                pg = 1;
            }

            int rescCount = itemsQry.Count();
            var pager = new Pager(rescCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = itemsQry.Skip(recSkip).Take(pager.PageSize).ToList();
            
            HttpContext.Session.Set("SearchItem", data);

            ViewBag.Pager = pager;
            //var itemsSession = HttpContext.Session.Get<List<Item>>("SearchItem");
            //if (itemsSession != null)
            //{
            //    listOfSearchItems = itemsSession;
            //}

            return View(data);
            //HttpContext.Session.Set("SearchItem", listOfCartViewModels);
            //return View(itemsQry);
        }

        public async Task<IActionResult> CurrentModel(Guid? id)
        {
            InitialCategory();
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

        private void InitialCategory()
        {
            var categoryItemList = new List<string>();

            var categoyrItemQry = from item in _context.Item
                                  orderby item.Category
                                  select item.Category;

            categoryItemList.AddRange(categoyrItemQry.Distinct());
            ViewBag.categoryItem = new SelectList(categoryItemList);
        }

        public IActionResult Cart()
        {
            InitialCategory();
            listOfCartViewModels = HttpContext.Session.Get<List<CartViewModel>>("cartItems");
            if (listOfCartViewModels != null)
            {
                return View(listOfCartViewModels);
            }
            else
            {
                return RedirectToAction("CartEmpty");
            }
        }

        public IActionResult ShowSalesData()
        {
            return View();
        }

        [HttpPost]
        public List<object> GetSalesData()
        {
            listOfCartViewModels = HttpContext.Session.Get<List<CartViewModel>>("cartItems");
            List<object> data = new List<object>();

            List<string> labels = listOfCartViewModels.Select(p => p.ItemName).ToList();
            data.Add(labels);

            List<decimal> salesNumber = listOfCartViewModels.Select(p => p.Quantity).ToList();
            data.Add(salesNumber);

            return data;
        }

        public IActionResult GetCart()
        {
            return PartialView();
            //listOfCartViewModels = HttpContext.Session.Get<List<CartViewModel>>("cartItems");
            //if (listOfCartViewModels != null)
            //{
            //    return PartialView(listOfCartViewModels);
            //}
            //else
            //{
            //    return RedirectToAction("CartEmpty");
            //}
        }

        public IActionResult GridView()
        {
            return PartialView();
        }

        public IActionResult ListView()
        {
            return PartialView();
        }

        public IActionResult CartEmpty()
        {
            InitialCategory();
            return View();
        }

        public IActionResult CheckoutLoggedIn()
        {
            InitialCategory();
            return View();
        }

        public IActionResult Contact()
        {
            InitialCategory();
            return View();
        }

        public IActionResult BlogSingleSidebar()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult NotFoundItem()
        {
            InitialCategory();
            return View();
        }

        public IActionResult Product()
        {
            InitialCategory();
            return View(_context.Item.ToList());
        }

        public IActionResult ProductList()
        {
            InitialCategory();
            return View(_context.Item.ToList());
        }

        public IActionResult Template()
        {
            return View();
        }

        public IActionResult CurrentmodelTemplate()
        {
            return View();
        }

        public IActionResult TmplateChart()
        {
            return View();
        }
    }
}