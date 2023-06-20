using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using hackaton.Models.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

// Jhonatas, faz isso aqui verificar se o user tá logado, pf; eu tô perdido sobre onde fica a validação

namespace hackaton.Controllers
{
    public class ScheduleController : Controller
    {
        Context _ctx;
        private readonly UserCacheService _userService;

        public ScheduleController(Context ctx, UserCacheService cache)
        {
            _userService = cache;
            _ctx = ctx;
        }

        //[BearerAuthorize]
        public ActionResult Index(int userId)
        {
           
            List<Schedule> agendamentos = _ctx.Schedules.Where(sch => sch.UserId == userId).ToList();

            return Json(agendamentos);
        }

        // POST: AgendamentoController/Create
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Schedule agendamento)
        {
         
            ModelState.Remove("User");
            if (agendamento.UserId == 0)
            {
                ModelState.AddModelError("UserId", "Campo requerido");

            }

            if (!ModelState.IsValid) {
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
           
            var existingUser = await _ctx.Users.FindAsync(agendamento.UserId);
             if (existingUser != null)
              {
                 agendamento.User = existingUser;
              }
                _ctx. Schedules.Add(agendamento);
                await _ctx.SaveChangesAsync();

            return Json(agendamento);
           
        }


        // POST: AgendamentoController/Delete/5
       
        [HttpPost]
        [BearerAuthorize]
        public ActionResult Delete(int id)
        {
            var del = _ctx.Schedules.Where(a => a.ScheduleId == id).Single();
            _ctx.Schedules.Remove(del);
            _ctx.SaveChanges();

            return Json(del);
        }
    }
}
