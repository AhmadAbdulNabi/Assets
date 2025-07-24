using Assets.DataAccess;
using Microsoft.AspNetCore.Mvc;
namespace Assets.Controllers
{
    public class JournalEntriesController : Controller
    {
        private readonly JournalEntryRepository _repo;
        public JournalEntriesController(JournalEntryRepository repo)
        {
            _repo = repo;
        }
        public IActionResult Index()
        {
            var entries = _repo.GetAll();
            return View(entries);
        }
    }
}
