using TablesQuickStartMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ABC_Retail.Controllers
{
    public class ProductController : Controller
    {
        private readonly Services.TableService _tableService;

        public ProductController(Services.TableService tableService)
        {
            _tableService = tableService;
        }

        // GET: ProductController
        public async Task<ActionResult> Index()
        {
            List<Product> products = await _tableService.ListAllAsync<Product>();
            return View(products);
        }

        // GET: ProductController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product model, IFormFile productImage)
        {   
            await _tableService.AddEntityAsync(model);

            return RedirectToAction(nameof(Index));
        }
    }
}
