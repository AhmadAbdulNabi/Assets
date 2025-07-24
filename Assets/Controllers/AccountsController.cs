using Assets.DataAccess;
using Assets.Models;
using Microsoft.AspNetCore.Mvc;
namespace Assets.Controllers
{
    public class AccountsController : Controller
    {
        private readonly AccountRepository _Repo;
        public AccountsController(AccountRepository Repo)
        {
            _Repo = Repo;
        }
        public IActionResult Index()
        {
            var Accounts = _Repo.GetAll();
            return View(Accounts);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(Account account)
        {
            if (ModelState.IsValid)
            {
                var insertedAsset = _Repo.Insert(account);


                return RedirectToAction("Index");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            foreach (var err in errors)
            {
                Console.WriteLine(err);
            }
            return View(account);
        }
        public IActionResult Edit(int id)
        {
            var account = _Repo.GetById(id);
            if (account == null)
                return NotFound();

            return View(account);
        }
        [HttpPost]
        public IActionResult Edit(Account account)
        {
            if (ModelState.IsValid)
            {
                _Repo.Update(account);
                return RedirectToAction("Index");
            }

            ViewBag.Accounts = _Repo.GetAll();
            return View(account);
        }
        public IActionResult Delete(int id)
        {
            var account = _Repo.GetById(id);
            if (account == null)
                return NotFound();

            _Repo.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
