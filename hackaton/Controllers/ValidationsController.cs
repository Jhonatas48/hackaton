using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace hackaton.Controllers
{
    public class ValidationsController : Controller
    {


        // POST: ValidationsController/validateName
        [HttpPost]
        public ActionResult validateName()
        {
            return View();
        }

        // POST: ValidationsController
        [HttpPost]
        public ActionResult validateCPF()
        {


            return View();
        }



        // POST: ValidationsController
        [HttpPost]
        public ActionResult validatePassword()
        {
            return View();
        }
    }
}
