using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DAL.Entities;
using Tracing.PLL.Interfaces;
using Tracing.PLL.Repositrios;
using WebAppTracingMvc.Helpers;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.Controllers
{
    [Authorize]

    public class ProducerController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public IMapper Mapper { get; }

        public ProducerController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            Mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var producers =await unitOfWork.producerRepositories.GetAll();

            var mapped = Mapper.Map<IEnumerable<Producer>, IEnumerable<ProducerViewModel>>(producers);

            return View(mapped);
        }

        public async Task<IActionResult> Search(string term)
        {
            var producers =await unitOfWork.producerRepositories.GetAll();

            producers = await unitOfWork.producerRepositories.SearchProducerByName(term);

            var mapped = Mapper.Map<IEnumerable<Producer>, IEnumerable<ProducerViewModel>>(producers);

            return PartialView("PartialView/_ProducerCards", mapped);
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProducerViewModel producerVM)
        {
            if (ModelState.IsValid) 
            {
                var MappedProducer=Mapper.Map<ProducerViewModel,Producer>(producerVM);
                if (producerVM.Image != null)
                {
                    MappedProducer.ProfilePictureURL = DocumentSettings.UploadImage(producerVM.Image, "Images");
                }
                await unitOfWork.producerRepositories.Create(MappedProducer);
                int res =await unitOfWork.Complete();

                if (res > 0)
                {
                    TempData["SuccessAdd"] = $"Producer {MappedProducer.FullName} added successfully 🎉";
                }
                return RedirectToAction(nameof(Index));
            }
            return View(producerVM);
        }


        public async Task<IActionResult> Details(int id)
        {
            var producer = await unitOfWork.producerRepositories.Get(id);
            var MappedProducer = Mapper.Map< Producer, ProducerViewModel>(producer);

            if (producer == null) return NotFound();

            return View(MappedProducer);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var producer =await unitOfWork.producerRepositories.Get(id);
            var MappedProducer = Mapper.Map<Producer, ProducerViewModel>(producer);

            if (producer == null)
                return NotFound();

            return View(MappedProducer);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProducerViewModel producerVM)
        {
            if (!ModelState.IsValid)
                return View(producerVM);
            var producer = await unitOfWork.producerRepositories.Get(producerVM.Id);

            if (producer == null)
                return NotFound();

            // =========================
            // Update fields
            // =========================
            producer.FullName = producerVM.FullName;
            producer.Bio = producerVM.Bio;
            // =========================
            // Handle Image Update
            // =========================
            if (producerVM.Image != null)
            {
                if (!string.IsNullOrEmpty(producer.ProfilePictureURL))
                {
                    DocumentSettings.DeleteImage(producer.ProfilePictureURL, "Images");
                }
                // upload new image
                producer.ProfilePictureURL = DocumentSettings.UploadImage(producerVM.Image, "Images");
            }
            

            unitOfWork.producerRepositories.Update(producer);
            int res = await unitOfWork.Complete();

            if (res > 0)
            {
                    TempData["SuccessUpdate"] = $"Producer {producerVM.FullName} Update successfully 🎉";
            }
            return RedirectToAction("Index");
           

            
        }



        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var producer =await unitOfWork.producerRepositories.Get(id);

            if (producer == null)
                return NotFound();
            // =========================
            // Delete image first
            // =========================
            if (!string.IsNullOrEmpty(producer.ProfilePictureURL))
            {
                DocumentSettings.DeleteImage(producer.ProfilePictureURL, "Images");
            }

            unitOfWork.producerRepositories.Delete(producer);
            int res =await unitOfWork.Complete();

            if (res > 0)
            {
                TempData["SuccessDelete"] = $"Producer {producer.FullName} Deleted successfully 🎉";
            }

            return RedirectToAction("Index");
        }
        
    }
}
