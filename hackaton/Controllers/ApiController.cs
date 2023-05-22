using Microsoft.AspNetCore.Mvc;
using hackaton.Models.Security;
using hackaton.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace hackaton.Controllers
{
    public class ApiController : Controller
    {

        private readonly HomeController _homeController;

       public ApiController(HomeController homeController)
       {
         _homeController = homeController;

        }
    // GET: ApiController/QRCode
    [BearerAuthorize]
 
        public ActionResult QRCode([FromBody] QrCode qrCode)
        {
            return Json(qrCode);
        }

        // GET: ApiController/Details/5
        public ActionResult validadeUser([FromBody] User user)
        {
            if(user == null)
            {
                return new BadRequestObjectResult(new { message = "User is required" });
            }

            if (!ModelState.IsValid)
            {
                ModelState.Remove("Name");
                ModelState.Remove("Properties");

            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_homeController.validateLogin(user))
            {
                return Ok(user);
            }

            return new UnauthorizedObjectResult(new { message = "Invalid Credentials" });
        }

        // GET: ApiController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ApiController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ApiController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ApiController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ApiController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
