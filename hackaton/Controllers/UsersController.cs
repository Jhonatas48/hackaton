using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using hackaton.Models;
using hackaton.Models.DAO;
using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models.Caches;
using hackaton.Models.Validations;
using hackaton.Models.Injectors;
using hackaton.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNet.SignalR.Hubs;

namespace hackaton.Controllers
{
    public class UsersController : Controller
    {
        private readonly Context _context;
        private readonly UserCacheService _userCacheService;
        public UsersController(Context context, UserCacheService cache)
        {
            _context = context;
            _userCacheService = cache;
        }

        public IActionResult Search(string searchQuery)
        {
            List<User> ListaUsers;

            if (searchQuery.IsNullOrEmpty())
            {
                ListaUsers = _context.Users.Where(u => u.Active == true).ToList();
            }
            else
            {
                ListaUsers = _context.Users.Where(u => (u.Active == true) && ((u.CPF.Contains(searchQuery)) || (u.Name.Contains(searchQuery)))).OrderBy(u => u.Name).ToList();
            }
            
            return View("~/Views/Admin/Index.cshtml", ListaUsers);
        }

        // private readonly Context context;
        public IActionResult AllowedRegister(string cpf)
        {
            Console.WriteLine(cpf);

            if (_userCacheService.GetUserByCPFAsync(cpf) == null)
            {
                return Json(true);
            }

            return Json("CPF já cadastrado");

        }

       // [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ServiceFilter(typeof(RequireLoginAdminAttributeFactory))]
        // GET: Users
        public async Task<IActionResult> Index()
        {
            return _context.Users != null ?
                        View("~/Views/Admin/Index.cshtml", _context.Users.Where(user => user.Active == true).ToList()) :
                        Problem("Entity set 'Context.Users'  is null.");
            //return View("~/Views/Admin/Index.cshtml");
        }

        // GET: Users/Details/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Users/Create
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]

        [ServiceFilter(typeof(RequireLoginAdminAttributeFactory))]
        public IActionResult Create()
        {
          
            return View();
        }

        // POST: Users/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ServiceFilter(typeof(RequireLoginAdminAttributeFactory))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password,CPF,IsAdmin")] User user)
        {
            if (ModelState.IsValid)
            {
                var cpfExists = await _context.Users.AnyAsync(u => u.CPF == user.CPF);
                if (cpfExists)
                {
                    ModelState.AddModelError("CPF", "O CPF já está cadastrado.");
                    return View(user);
                }

                string password = user.Password;
                user.Password = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());
                _context.Add(user);
                await _context.SaveChangesAsync();
                _userCacheService.AddUserToCache(user);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ServiceFilter(typeof(RequireLoginAdminAttributeFactory))]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Edit.cshtml",user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Edit(int id, User user)
        {
            int userId = id;
            var userRetrieve = _context.Users.Where(u => u.Id == userId).Single();
            user.IsAdmin = userRetrieve.IsAdmin;
            user.Id = userId;
            ModelState.Remove("user.QrCodes");
            ModelState.Remove("user.Agendamentos");
            ModelState.Remove("user.Properties");
            ModelState.Remove("user.CPF");  //suspeito que alguma verificação aqui esteja quebrada, se tiver a validação do "já cadastrado"

            if (ModelState.IsValid)
            {
                try
                {
                    string password = user.Password;
                   
                    user.Password = BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt());

                    _context.ChangeTracker.Clear();

                    _context.Update(user);
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
                return RedirectToAction("Index");
            }
            return View("~/Views/Admin/Edit.cshtml",user);
        }

        // GET: Users/Delete/5
        [ServiceFilter(typeof(RequireLoginAdminAttributeFactory))]
        public async Task<IActionResult> Delete(int? id)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            if(user.Id == userId)
            {
                ModelState.AddModelError("Name", "Voce nao pode excluir a si mesmo");
                return RedirectToAction("Index");
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");

            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.Users'  is null.");
            }

            var user = await _context.Users.FindAsync(id);

            if (user != null)
            {
                return NotFound();
            }

            if (user.Id == userId)
            {
                ModelState.AddModelError("Name", "Voce nao pode excluir a si mesmo");
                return RedirectToAction("Index");
            }

            user.Active = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
