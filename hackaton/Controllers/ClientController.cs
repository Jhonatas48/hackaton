using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hackaton.Controllers
{
    public class ClientController : Controller
    {
        private readonly UserCacheService _userService;
        private readonly Context _context;
        public ClientController(UserCacheService cache, Context context) { 
            _userService = cache;
            _context = context;
        }
        // GET: ClientController
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public ActionResult Index()
        {
            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);
            Console.WriteLine(user.IsAdmin);
            if (user != null && user.Id == userId && user.IsAdmin) {
                return RedirectToAction("Index", "Users");
            }
            
            return View(user);
        }

        // GET: ClientController/Logout
        public ActionResult Logout()
        {
            //limpa a session do client
            HttpContext.Session.Clear();
            //Deleta o cookie que tem os dados de sessão
            Response.Cookies.Delete("MyAuthCookie");

            //Direciona o usuario a pagina /Home/Index
            return RedirectToAction("Index","Home");
        }

        // GET: Users/Edit/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task<IActionResult> Edit()
        {
            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);
           
            if (user == null || !user.CPF.Equals(cpf) || user.Id != userId)
            {
                return NotFound();
            }
           
            return View(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password,CPF,IsAdmin")] User user)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            string cpf = HttpContext.Session.GetString("CPF");
            User userRetrieve = _userService.GetUserByCPFAsync(cpf);

            if (userRetrieve == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    string password = user.Password;

                    userRetrieve.Password = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());
                    _context.Update(userRetrieve);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // GET: Users/Delete/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task<IActionResult> Delete()
        {

            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);

            if (user == null || !user.CPF.Equals(cpf) || user.Id != userId)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.Users'  is null.");
            }
            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);
            if(user == null)
            {
                return NotFound();
            }

            if (user.IsAdmin == false)
            {
                user.Active = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
            }
            else {
                RedirectToAction("Index", "Users");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }

}

