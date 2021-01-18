using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimpleCQRS.Web.Models;
using System;
using System.Diagnostics;

namespace SimpleCQRS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly FakeBus _bus;
        private readonly IReadModelFacade _readmodel;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _bus = ServiceLocator.Bus;
            _readmodel = new ReadModelFacade();
        }

        public ActionResult Index()
        {
            ViewData.Model = _readmodel.GetInventoryItems();
            return View();
        }

        public ActionResult Details(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            var command = new CreateInventoryItem(Guid.NewGuid(), name);
            _bus.Send(command);
            return RedirectToAction("Index");
        }

        public ActionResult ChangeName(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult ChangeName(Guid id, string name, int version)
        {
            var command = new RenameInventoryItem(id, name, version);
            _bus.Send(command);
            return RedirectToAction("Index");
        }

        public ActionResult Deactivate(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult Deactivate(Guid id, int version)
        {
            var command = new DeactivateInventoryItem(id, version);
            _bus.Send(command);
            return RedirectToAction("Index");
        }

        public ActionResult CheckIn(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult CheckIn(Guid id, int number, int version)
        {
            var command = new CheckInItemsToInventory(id, number, version);
            _bus.Send(command);
            return RedirectToAction("Index");
        }

        public ActionResult Remove(Guid id)
        {
            ViewData.Model = _readmodel.GetInventoryItemDetails(id);
            return View();
        }

        [HttpPost]
        public ActionResult Remove(Guid id, int number, int version)
        {
            var command = new RemoveItemsFromInventory(id, number, version);
            _bus.Send(command);
            return RedirectToAction("Index");
        }

        [Route("/Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
