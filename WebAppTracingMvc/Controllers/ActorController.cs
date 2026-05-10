using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DAL.Entities;
using Tracing.PLL.Interfaces;
using WebAppTracingMvc.Helpers;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.Controllers
{
    [Authorize]
    public class ActorController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public ActorController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var Actors=await unitOfWork.actorRepositories.GetAll();
            var mappedActor = mapper.Map<IEnumerable<Actor>, IEnumerable<ActorViewModel>>(Actors);
            return View(mappedActor);
        }
        public async Task<IActionResult> Search(string term)
        {
            var actors =await unitOfWork.actorRepositories.GetAll();

            actors = await unitOfWork.actorRepositories.SearchActorByName(term);

            var mapped = mapper.Map<IEnumerable<Actor>, IEnumerable<ActorViewModel>>(actors);

            return PartialView("PartialView/_ActorCards", mapped);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActorViewModel actorVM)
        {
            if (ModelState.IsValid)
            {
                var mappedActor = mapper.Map<ActorViewModel, Actor>(actorVM);

                if (actorVM.Image != null)
                {
                    mappedActor.ProfilePictureUrl =
                        DocumentSettings.UploadImage(actorVM.Image, "Images");
                }

                await unitOfWork.actorRepositories.Create(mappedActor);
                int res =  await unitOfWork.Complete();

                if (res > 0)
                {
                    TempData["SuccessAdd"] =
                        $"Actor {mappedActor.FullName} added successfully 🎉";
                }

                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index)); // علشان انت شغال popup
        }
        // GET
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id)
        {
            var actor = await unitOfWork.actorRepositories.Get(id);

            if (actor == null)
                return NotFound();

            var mapped = mapper.Map<Actor, ActorViewModel>(actor);

            return View(mapped);
        }
        // POST
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActorViewModel actorVM)
        {
            if (ModelState.IsValid)
            {
                var actor = await unitOfWork.actorRepositories.Get(actorVM.Id);

                if (actor == null)
                    return NotFound();

                // ✅ لو فيه صورة جديدة
                if (actorVM.Image != null)
                {
                    // حذف القديمة
                    if (!string.IsNullOrEmpty(actor.ProfilePictureUrl))
                    {
                        DocumentSettings.DeleteImage(actor.ProfilePictureUrl, "Images");
                    }

                    // رفع الجديدة
                    actor.ProfilePictureUrl =
                        DocumentSettings.UploadImage(actorVM.Image, "Images");
                }

                // تحديث باقي الداتا
                actor.FullName = actorVM.FullName;
                actor.Bio = actorVM.Bio;
                actor.Salary = actorVM.Salary;

                unitOfWork.actorRepositories.Update(actor);
                int res =await unitOfWork.Complete();

                if (res > 0)
                {
                    TempData["SuccessUpdate"] =
                        $"Actor {actor.FullName} updated successfully 🎉";
                }

                return RedirectToAction(nameof(Index));
            }

            return View(actorVM);
        }

        public async Task<IActionResult> Details(int id)
        {
            var actor =await unitOfWork.actorRepositories.Get(id);

            if (actor == null)
                return NotFound();

            var mappedActor = mapper.Map<Actor, ActorViewModel>(actor);

            return View(mappedActor);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var actor = await unitOfWork.actorRepositories.Get(id);

            if (actor == null)
                return NotFound();

            // ✅ حذف الصورة من السيرفر لو موجودة
            if (!string.IsNullOrEmpty(actor.ProfilePictureUrl))
            {
                DocumentSettings.DeleteImage(actor.ProfilePictureUrl, "Images");
            }

            // ✅ حذف من الداتابيز
            unitOfWork.actorRepositories.Delete(actor);
            int res = await unitOfWork.Complete();

            if (res > 0)
            {
                TempData["SuccessDelete"] = $"Actor {actor.FullName} deleted successfully 🎉";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
