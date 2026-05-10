using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DAL.Entities;
using Tracing.PLL.Interfaces;
using WebAppTracingMvc.Helpers;
using WebAppTracingMvc.ViewModels;

namespace WebAppTracingMvc.Controllers
{
    //[Authorize]

    public class MovieController : Controller
    {
        
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MovieController(IUnitOfWork unitOfWork,IMapper mapper)
        {
            
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Producers = new SelectList(
               await unitOfWork.producerRepositories.GetAll(),
                "Id",
                "FullName"
            );
            ViewBag.Cinemas = new SelectList(
                await unitOfWork.cinemaRepositories.GetAll(),
                "Id",
                "Name"
            );

            var vm = new MovieViewModel
            {
                AllActors = mapper.Map<List<ActorViewModel>>(
               await unitOfWork.actorRepositories.GetAll()
                 )
            };
            ViewBag.MovieVM = vm;


            var movies = await unitOfWork.movieRepositories.GetAll();
            var mapped = mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

            return View(mapped);
        }

        public async Task<IActionResult> Search(string term)
        {
            var movies =await unitOfWork.movieRepositories.GetAll();

            movies =await unitOfWork.movieRepositories.SearchMovieByName( term );

            var mapped = mapper.Map<IEnumerable<Movie>, IEnumerable<MovieViewModel>>(movies);

            return PartialView("PartialView/_MovieCard", mapped);
        }

        //[HttpGet]
        //public IActionResult Create() 
        //{
        //    ViewBag.Producers = new SelectList(
        //         producerRepositories.GetAll(),
        //         "Id",
        //         "FullName"

        //     );
        //    return View();
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MovieViewModel movieVM)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Producers = new SelectList(
                    await unitOfWork.producerRepositories.GetAll(),
                    "Id",
                    "FullName",
                    movieVM.ProducerId
                );

                ViewBag.Cinemas = new SelectList(
                   await unitOfWork.cinemaRepositories.GetAll(),
                    "Id",
                    "Name",
                    movieVM.CinemaId
                );

                movieVM.AllActors = mapper.Map<List<ActorViewModel>>(
                    await unitOfWork.actorRepositories.GetAll()
                );

                return View(movieVM);
            }

            // 1- Mapping
            var mappedMovie = mapper.Map<Movie>(movieVM);

            // 2- Image Upload
            if (movieVM.Image != null)
            {
                mappedMovie.ImageURL = DocumentSettings.UploadImage(movieVM.Image, "Images");
            }

            // 3- Save Movie first
            await unitOfWork.movieRepositories.Create(mappedMovie);
            await unitOfWork.Complete();

            // 4- Save Many-to-Many (Actors)
            if (movieVM.SelectedActorsIds != null && movieVM.SelectedActorsIds.Any())
            {
                mappedMovie.ActorMovies = movieVM.SelectedActorsIds.Select(actorId => new ActorMovie
                {
                    MovieId = mappedMovie.Id,
                    ActorId = actorId
                }).ToList();

                await unitOfWork.Complete();
            }

            // 5- Success message
            TempData["SuccessAdd"] = $"Movie {mappedMovie.Name} added successfully 🎉";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Details(int id)
        {
            var movie =await unitOfWork.movieRepositories.Get(id);
            if (movie == null) return NotFound();
            var mappedMovie = mapper.Map<Movie,MovieViewModel>(movie);


            return View(mappedMovie);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var movie =await unitOfWork.movieRepositories.Get(id);

            if (movie == null)
                return NotFound();

            var mapped = mapper.Map<MovieViewModel>(movie);

            // كل الأكتـرز
            mapped.AllActors = mapper.Map<List<ActorViewModel>>(
                await unitOfWork.actorRepositories.GetAll()
            );

            // الأكتـرز المختارين
            mapped.SelectedActorsIds = movie.ActorMovies?
                .Select(a => a.ActorId)
                .ToList() ?? new List<int>();

            ViewBag.Producers = new SelectList(
                 await unitOfWork.producerRepositories.GetAll(),
                "Id",
                "FullName",
                mapped.ProducerId
            );

            ViewBag.Cinemas = new SelectList(
                await unitOfWork.cinemaRepositories.GetAll(),
                "Id",
                "Name",
                mapped.CinemaId
            );

            return View(mapped);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MovieViewModel movieVM)
        {
            if (!ModelState.IsValid)
            {
                return View(movieVM);
            }

            var movie = await unitOfWork.movieRepositories.Get(movieVM.Id);

            if (movie == null)
                return NotFound();

            // update basic data
            movie.Name = movieVM.Name;
            movie.Description = movieVM.Description;
            movie.Price = movieVM.Price;
            movie.StartDate = movieVM.StartDate;
            movie.EndDate = movieVM.EndDate;
            movie.Catigory = movieVM.Catigory;
            movie.ProducerId = movieVM.ProducerId;
            movie.CinemaId = movieVM.CinemaId;

            if (movieVM.Image != null)
            {
                if (!string.IsNullOrEmpty(movie.ImageURL))
                {
                    DocumentSettings.DeleteImage(movie.ImageURL, "Images");
                }
                // upload new image
                movie.ImageURL = DocumentSettings.UploadImage(movieVM.Image, "Images");
            }

                 //🔥 update actors(many-to - many)
                movie.ActorMovies.Clear();

            if (movieVM.SelectedActorsIds != null && movieVM.SelectedActorsIds.Any())
            {
                movie.ActorMovies = movieVM.SelectedActorsIds.Select(actorId => new ActorMovie
                {
                    MovieId = movie.Id,
                    ActorId = actorId
                }).ToList();
            }

            unitOfWork.movieRepositories.Update(movie);
            await unitOfWork.Complete();

            TempData["SuccessUpdate"] = $"Movie {movie.Name} updated successfully 🎉";

            return RedirectToAction(nameof(Index));
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken] // بياخد الداتا من البروزر بتاعى بس
        //public IActionResult Edit([FromRoute]int id,MovieViewModel movieVM)
        //{
        //    if (id != movieVM.Id)
        //    {
        //        return BadRequest();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return View(movieVM);
        //    }
        //    var movie = unitOfWork.movieRepositories.Get(movieVM.Id);

        //    if (movie == null)
        //        return NotFound();
        //    // =========================
        //    // Update fields
        //    // =========================
        //    movie.Name = movieVM.Name;
        //    movie.Description = movieVM.Description;
        //    movie.Catigory= movieVM.Catigory;
        //    movie.StartDate = movieVM.StartDate;
        //    movie.EndDate = movieVM.EndDate;
        //    movie.ProducerId = movieVM.ProducerId;
        //    movie.CinemaId = movieVM.CinemaId;
        //    movie.Price = movieVM.Price;
        //    // =========================
        //    // Handle Image Update
        //    // =========================
        //    if (movieVM.Image != null)
        //    {
        //        if (!string.IsNullOrEmpty(movie.ImageURL))
        //        {
        //            DocumentSettings.DeleteImage(movie.ImageURL, "Images");
        //        }
        //        // upload new image
        //        movie.ImageURL = DocumentSettings.UploadImage(movieVM.Image, "Images");
        //    }

        //    unitOfWork.movieRepositories.Update(movie);
        //    int res = unitOfWork.Complete();
        //    if (res > 0)
        //    {
        //        TempData["SuccessUpdate"] = $"Movie {movieVM.Name} Update successfully 🎉";
        //    }
          
        //    ViewBag.Producers = new SelectList(
        //          unitOfWork.producerRepositories.GetAll(),
        //          "Id",
        //          "FullName",
        //          movieVM.ProducerId
        //      );
        //    ViewBag.Cinemas = new SelectList(
        //          unitOfWork.cinemaRepositories.GetAll(),
        //          "Id",
        //          "Name",
        //          movieVM.CinemaId
        //      );
        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var movie =await unitOfWork.movieRepositories.Get(id);

            if (movie == null)
                return NotFound();
            // =========================
            // Delete image first
            // =========================
            if (!string.IsNullOrEmpty(movie.ImageURL))
            {
                DocumentSettings.DeleteImage(movie.ImageURL, "Images");
            }

            unitOfWork.movieRepositories.Delete(movie);
            int res =await unitOfWork.Complete();
            if (res > 0)
            {
                TempData["SuccessDelete"] = $"Movie {movie.Name} Delete successfully 🎉";
            }

            return RedirectToAction("Index");
        }
    }
}
