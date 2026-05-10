using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tracing.DAL.Contexts;
using Tracing.DAL.Entities;
using Tracing.PLL.Interfaces;
using Tracing.PLL.Repositrios;
using WebAppTracingMvc.MapProfiles;

namespace WebAppTracingMvc
{
    public class Program
    {
        public static void Main(string[] args)
        {
           var builder = WebApplication.CreateBuilder(args);
            // Add services to the container DI.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<TracingDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            //services.AddScoped<IMovieRepositories,MovieRepositories>();
            //services.AddScoped<IProducerRepositories,ProducerRepositories>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            /// Auto Mapper service
            builder.Services.AddAutoMapper(m => m.AddProfile(new ProducerProfile()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new CinemaProfile()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new MovieProfile()));
            builder.Services.AddAutoMapper(m => m.AddProfile(new ActorProfile()));

            // configuration security module

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 6;
            }).AddEntityFrameworkStores<TracingDbContext>().AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
            });


            //-------------------End of service configuration------------------


            var app = builder.Build();
            var env = app.Environment;
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Movie}/{action=Index}/{id?}");
            });
            app.Run();
        }

    }
}
