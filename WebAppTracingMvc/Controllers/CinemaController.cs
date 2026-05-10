using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    public class CinemaController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public CinemaController(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var Cinemas =await unitOfWork.cinemaRepositories.GetAll();

            var mappedCinemas = mapper.Map<IEnumerable<Cinema>, IEnumerable<CinemaViewModel>>(Cinemas);

            return View(mappedCinemas);
        }
        public async Task<IActionResult> Search(string term)
        {
            var cinemas =await unitOfWork.cinemaRepositories.GetAll();
            cinemas = await unitOfWork.cinemaRepositories.SearchCinemaByName(term);

            var mapped = mapper.Map<IEnumerable<Cinema>, IEnumerable<CinemaViewModel>>(cinemas);

            return PartialView("PartialView/_CinemaCards", mapped);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CinemaViewModel cinemaVM)
        {
            if (ModelState.IsValid)
            {
                var mappedCinema = mapper.Map<CinemaViewModel, Cinema>(cinemaVM);

                if (cinemaVM.Image != null)
                {
                    mappedCinema.Logo = DocumentSettings.UploadImage(cinemaVM.Image, "Images");
                }

                await unitOfWork.cinemaRepositories.Create(mappedCinema);
                int res =await unitOfWork.Complete();

                if (res > 0)
                {
                    TempData["SuccessAdd"] = $"Cinema {mappedCinema.Name} added successfully 🎉";
                }

                return RedirectToAction(nameof(Index));
            }

            // ❌ متعملش return View
            return RedirectToAction(nameof(Index)); // ✅ الصح
        }

        public async Task<IActionResult> Details(int id)
        {
            var cinema =await unitOfWork.cinemaRepositories.Get(id);
            var MappedCinema = mapper.Map<Cinema, CinemaViewModel>(cinema);

            if (cinema == null) return NotFound();

            return View(MappedCinema);
        }

        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Edit(int id)
        {
            var cinema =await unitOfWork.cinemaRepositories.Get(id);
            var MappedCinema = mapper.Map<Cinema, CinemaViewModel>(cinema);

            if (cinema == null)
                return NotFound();

            return View(MappedCinema);
        }
        [Authorize(Roles = "Admin")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CinemaViewModel cinemaVM)
        {
            if (!ModelState.IsValid)
                return View(cinemaVM);

            var cinema =await unitOfWork.cinemaRepositories.Get(cinemaVM.Id);

            if (cinema == null)
                return NotFound();

            // =========================
            // Update fields
            // =========================
            cinema.Name = cinemaVM.Name;
            cinema.Description = cinemaVM.Description;
            cinema.Address = cinemaVM.Address;

            // =========================
            // Handle Image Update
            // =========================
            if (cinemaVM.Image != null)
            {
                // delete old image
                if (!string.IsNullOrEmpty(cinema.Logo))
                {
                    DocumentSettings.DeleteImage(cinema.Logo, "Images");
                }

                // upload new image
                cinema.Logo = DocumentSettings.UploadImage(cinemaVM.Image, "Images");
            }

            unitOfWork.cinemaRepositories.Update(cinema);
            int res =await unitOfWork.Complete();

            if (res > 0)
            {
                TempData["SuccessUpdate"] = $"Cinema {cinema.Name} updated successfully 🎉";
            }

            return RedirectToAction(nameof(Index));
        }
        //public IActionResult Edit(CinemaViewModel cienamVM)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var MappedCinema = mapper.Map<CinemaViewModel,Cinema>(cienamVM);
        //        unitOfWork.cinemaRepositories.Update(MappedCinema);
        //        int res = unitOfWork.Complete();

        //        if (res > 0)
        //        {
        //            TempData["SuccessUpdate"] = $"Cinema {cienamVM.Name} Update successfully 🎉";
        //        }
        //        return RedirectToAction("Index");
        //    }

        //    return View(cienamVM);
        //}



        [Authorize(Roles = "Admin")]

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var cinema = await unitOfWork.cinemaRepositories.Get(id);

            if (cinema == null)
                return NotFound();

            // =========================
            // Delete image first
            // =========================
            if (!string.IsNullOrEmpty(cinema.Logo))
            {
                DocumentSettings.DeleteImage(cinema.Logo, "Images");
            }

            unitOfWork.cinemaRepositories.Delete(cinema);
            int res = await unitOfWork.Complete();

            if (res > 0)
            {
                TempData["SuccessDelete"] = $"Cinema {cinema.Name} deleted successfully 🎉";
            }

            return RedirectToAction(nameof(Index));
        }
       
    }
}
