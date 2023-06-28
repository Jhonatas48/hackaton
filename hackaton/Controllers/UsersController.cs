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
using System.Text.Json.Nodes;
using hackaton.Models.Security;
using Newtonsoft.Json;
using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<ActionResult> Index()
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            return Json(_context.Users.Where(u => u.Active == true && u.ApiId==api.ApiId).ToList());
            
        }

        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public IActionResult Search(string searchQuery)
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            List<User> ListaUsers;

            if (searchQuery.IsNullOrEmpty())
            {
                ListaUsers = _context.Users.Where(u => u.Active == true && u.ApiId == api.ApiId).OrderBy(u => u.Name).ToList();
            }
            else
            {
                ListaUsers = _context.Users.Where(u => (u.Active == true) && ((u.CPF.Contains(searchQuery)) || (u.Name.Contains(searchQuery))) && u.ApiId == api.ApiId).OrderBy(u => u.Name).ToList();
            }
            
            return Json(ListaUsers);
           
        }

        
        public IActionResult AllowedRegister(string cpf)
        {
            Console.WriteLine(cpf);

            if (_userCacheService.GetUserByCPFAsync(cpf) == null)
            {
                return Json(true);
            }

            return Json("CPF já cadastrado");

        }

        // GET: Users/Edit/5
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<IActionResult> Edit(int? id, [FromBody]User userAdmin)
        {
            User userAdminRetrieve = _context.Users
             .Where(
             admin => admin.Id == id
             && admin.CPF.Equals(userAdmin.CPF)
             && admin.Name.Equals(userAdmin.Name)
             && admin.IsAdmin == true
             && admin.Active == true
             )
             .FirstOrDefault();

            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Json(user);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<IActionResult> Edit(int id, [FromBody] User user)
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var responses = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var jsons = JsonConvert.SerializeObject(responses);

                return BadRequest(jsons);
            }

            int userId = id;
            var userRetrieve = _context.Users.Where(u => u.Id == userId && u.ApiId == api.ApiId).Single();
            if (userRetrieve != null)
            {
                user.IsAdmin = userRetrieve.IsAdmin;
            }
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
                return Json(user);
            }
            var erros = ModelState.Keys
               .Where(key => ModelState[key].Errors.Any())
               .ToDictionary(key => key, key => ModelState[key].Errors.Select(error => error.ErrorMessage).ToList());

            var response = new
            {
                Message = "Houve erros de validação.",
                Errors = erros,

            };

            var json = JsonConvert.SerializeObject(response);

            return BadRequest(json);
        }

        // GET: Users/Delete/5
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<IActionResult> Delete(int? id, [FromBody]User userAdmin)
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            if (userAdmin == null)
            {
                return BadRequest("User is required");
            }

            User userAdminRetrieve = _context.Users
             .Where(
             admin => admin.Id == id
             && admin.CPF.Equals(userAdmin.CPF)
             && admin.Name.Equals(userAdmin.Name)
             && admin.IsAdmin == true
             && admin.Active == true
             && admin.ApiId == api.ApiId
             )
             .FirstOrDefault();

            if (userAdminRetrieve == null)
            {
                return StatusCode(405, "Voce nao tem permissao para executar esta ação"); ;
            }

         
            if (id == null || _context.Users == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id && m.ApiId == api.ApiId);
            if (user == null)
            {
                return NotFound();
            }

            if(user.Id == userAdminRetrieve.Id)
            {
                ModelState.AddModelError("Name", "Voce nao pode excluir a si mesmo");
                var erros = ModelState.Keys
               .Where(key => ModelState[key].Errors.Any())
               .ToDictionary(key => key, key => ModelState[key].Errors.Select(error => error.ErrorMessage).ToList());

                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = erros,

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ServiceFilter(typeof(BearerAuthorizeAttributeFactory))]
        public async Task<IActionResult> DeleteConfirmed(int id,[FromBody] User userAdmin)
        {
            string apiToken = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            Api api = _context.Apis.Where(a => a.Token.Equals(apiToken)).FirstOrDefault();

            if (string.IsNullOrEmpty(apiToken))
            {
                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = "Api token invalido",

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            if (userAdmin == null)
            {
                return BadRequest("User is required");
            }
            if (_context.Users == null)
            {
                return Problem("Entity set 'Context.Users'  is null.");
            }

            User userAdminRetrieve = _context.Users
                .Where(
                admin => admin.Id == userAdmin.Id 
                && admin.CPF.Equals(userAdmin.CPF) 
                && admin.Name.Equals(userAdmin.Name)
                && admin.IsAdmin == true 
                && admin.Active == true
                && admin.ApiId == api.ApiId
                )
                .FirstOrDefault();
            
            if (userAdminRetrieve == null)
            {
                return StatusCode(405, "Voce nao tem permissao para executar esta ação"); ;
            }

            var user = await _context.Users.FindAsync(id);

            if (user == null || user.ApiId != api.ApiId)
            {
                return NotFound();
            }

            if (user.Id == userAdminRetrieve.Id)
            {
                ModelState.AddModelError("Name", "Voce nao pode excluir a si mesmo");
               
            }

            if (!ModelState.IsValid)
            {
                var erros = ModelState.Keys
                .Where(key => ModelState[key].Errors.Any())
                .ToDictionary(key => key, key => ModelState[key].Errors.Select(error => error.ErrorMessage).ToList());

                var response = new
                {
                    Message = "Houve erros de validação.",
                    Errors = erros,

                };

                var json = JsonConvert.SerializeObject(response);

                return BadRequest(json);
            }

            user.Active = false;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            
            return Ok(user);
        }

        private bool UserExists(int id)
        {
          return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
