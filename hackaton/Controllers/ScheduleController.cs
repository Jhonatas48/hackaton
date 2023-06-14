using hackaton.Models;
using hackaton.Models.Caches;
using hackaton.Models.DAO;
using hackaton.Models.Injectors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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


        // Eu ACHO que é isso aqui que faz o usuário PRECISAR estar logado
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public ActionResult Index()
        {
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            List<Schedule> agendamentos = _ctx.Schedules.Where(sch => sch.UserId == userId).ToList();

            return View("~/Views/Schedule/Index.cshtml", agendamentos);
        }

        public IActionResult LoadPartialListAgendamentos()
        {
            List<Schedule> agendamentos;

            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);

            if (user != null && user.Id == userId && user.IsAdmin)
            {
                agendamentos = _ctx. Schedules.Where(a => a.User != null).ToList();
            }
            else
            {
                agendamentos = _ctx. Schedules.Where(a => a.User.Id == user.Id).ToList();
            }

            return PartialView("~/Views/Modules/partial_list_agendamentos", agendamentos);
        }

        public IActionResult LoadPartialCardAgendar()
        {
            return PartialView("~/Views/Modules/partial_card_agendar");
        }

        // POST: AgendamentoController/Create
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Schedule agendamento)
        {
            string cpf = HttpContext.Session.GetString("CPF");
            int userId = (int)HttpContext.Session.GetInt32("UserId");
            User user = _userService.GetUserByCPFAsync(cpf);
           // agendamento.User = user;

            //ModelState.ClearValidationState("User");
            //TryValidateModel(agendamento);

            if (user != null) //ent, isso -> ModelState.IsValid tava dando problema, então aqui é só um bandaid sobre uma hemorragia :D
            {
                agendamento.UserId = userId;
                _ctx. Schedules.Add(agendamento);
                await _ctx.SaveChangesAsync();
            }
            else
            {
                ModelState.AddModelError("Description", "Algo deu errado. Contate a administração do sistema.");
            }

            return RedirectToAction("Index");
        }

        // GET: AgendamentoController/Edit/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AgendamentoController/Edit/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
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

        //// GET: AgendamentoController/Delete/5
        //[ServiceFilter(typeof(RequireLoginAttributeFactory))]
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        // POST: AgendamentoController/Delete/5
        [ServiceFilter(typeof(RequireLoginAttributeFactory))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var del = _ctx.Schedules.Where(a => a.ScheduleId == id).Single();
            _ctx.Schedules.Remove(del);
            _ctx.SaveChanges();

            return RedirectToAction(nameof(Index));
        }
    }
}
