using Assets.DataAccess;
using Assets.Models;
using Microsoft.AspNetCore.Mvc;
namespace Assets.Controllers
{
    public class AssetsController : Controller
    {
        private readonly AssetRepository _assetRepo;
        private readonly AccountRepository _accountRepo;
        private readonly JournalEntryRepository _journalRepo;
        public AssetsController(AssetRepository assetRepo, AccountRepository accountRepo, JournalEntryRepository journalRepo)
        {
            _assetRepo = assetRepo;
            _accountRepo = accountRepo;
            _journalRepo = journalRepo;
        }
        public IActionResult Index()
        {
            var assets = _assetRepo.GetAll();
            return View(assets);
        }
        public IActionResult Create()
        {
            ViewBag.Accounts = _accountRepo.GetAll();

            string lastNumber = _assetRepo.GetLastAssetNumber();

            string newNumber = "AST0001";
            if (!string.IsNullOrEmpty(lastNumber) && lastNumber.StartsWith("AST"))
            {
                int lastNum = int.Parse(lastNumber.Substring(3));
                newNumber = "AST" + (lastNum + 1).ToString("D4");
            }
            var asset = new Asset
            {
                AssetNumber = newNumber,
                AssetDate = DateTime.Today
            };

            return View(asset);
        }
        [HttpPost]
        public IActionResult Create(Asset asset)
        {
            if (ModelState.IsValid)
            {
                var insertedAsset = _assetRepo.Insert(asset);
                return RedirectToAction("Index");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
            ViewBag.Accounts = _accountRepo.GetAll();
            return View(asset);
        }
        public IActionResult Edit(int id)
        {
            var asset = _assetRepo.GetById(id);
            if (asset == null)
                return NotFound();

            ViewBag.Accounts = _accountRepo.GetAll();
            return View(asset);
        }
        [HttpPost]
        public IActionResult Edit(Asset asset)
        {
            if (ModelState.IsValid)
            {
                _assetRepo.Update(asset);
                return RedirectToAction("Index");
            }

            ViewBag.Accounts = _accountRepo.GetAll();
            return View(asset);
        }
        public IActionResult Delete(int id)
        {
            var asset = _assetRepo.GetById(id);
            if (asset == null)
                return NotFound();
            _assetRepo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}