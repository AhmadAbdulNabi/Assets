using Assets.DataAccess;
using Assets.Models;
using Microsoft.AspNetCore.Mvc;
namespace Assets.Controllers
{
    public class AssetAdditionsController : Controller
    {
        private readonly AssetAdditionRepository _repo;
        private readonly AssetRepository _assetRepo;
        public AssetAdditionsController(AssetAdditionRepository repo, AssetRepository assetRepo)
        {
            _repo = repo;
            _assetRepo = assetRepo;
        }
        public IActionResult Index()
        {
            var additions = _repo.GetAll();
            return View(additions);
        }
        public IActionResult Create()
        {
            ViewBag.Assets = _assetRepo.GetAll();
            return View(new AssetAddition
            {
                AdditionDate = DateTime.Today
            });
        }
        [HttpPost]
        public IActionResult Create(AssetAddition model)
        {
            if (model.AdditionDate == default)
                model.AdditionDate = DateTime.Today;

            if (ModelState.IsValid)
            {
                try
                {
                    _repo.Insert(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"خطأ أثناء الحفظ: {ex.Message}");
                }
            }

            ViewBag.Assets = _assetRepo.GetAll();
            return View(model);
        }
        public IActionResult Edit(int id)
        {
            var model = _repo.GetById(id);
            if (model == null)
                return NotFound();

            ViewBag.Assets = _assetRepo.GetAll();
            return View(model);
        }
        [HttpPost]
        public IActionResult Edit(AssetAddition model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _repo.Update(model);
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"خطأ أثناء التعديل: {ex.Message}");
                }
            }

            ViewBag.Assets = _assetRepo.GetAll();
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            var asset = _repo.GetById(id);
            if (asset == null)
                return NotFound();

            _repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
