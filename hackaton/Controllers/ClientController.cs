using DevOne.Security.Cryptography.BCrypt;
using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using hackaton.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

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

        // POST: Users/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [BearerAuthorize]
        public async Task<IActionResult> Edit([FromBody] User user)
        {

            ModelState.Remove("user.QrCodes");
            ModelState.Remove("user.Agendamentos");
            ModelState.Remove("user.Properties");
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
            };
                User userRetrieve = _userService.GetUserByCPFAsync(user.CPF);
          
            if (userRetrieve == null)
            {
                return NotFound();
            }

                try
                {
                    string password = user.Password;
                     userRetrieve.Name = user.Name;
                    userRetrieve.Password = (!password.IsNullOrEmpty()) ? BCryptHelper.HashPassword(password, BCryptHelper.GenerateSalt()) : userRetrieve.Password;
                    _context.ChangeTracker.Clear();
                    _context.Update(userRetrieve);
                    await _context.SaveChangesAsync();
                    return Json(user);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                       return Problem("Erro no banco de dados");
                    }
                }
             
        }

        //// GET: Users/Delete/5
         [BearerAuthorize]
        //   [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public async Task<IActionResult> Delete()
       {

           string cpf = HttpContext.Session.GetString("CPF");
           int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);

            if (user == null || !user.CPF.Equals(cpf) || user.Id != userId)
            {
                return NotFound();
            }

            return Json(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [BearerAuthorize]
        //  [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        // [ValidateAntiForgeryToken]
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
                return Json(user);
            }

            else {
                return StatusCode(405, "Voce nao tem permissao para executar esta ação");
            }

        }

        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }

}

