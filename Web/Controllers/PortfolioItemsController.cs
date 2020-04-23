using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Core.Entities;
using Infrastructure;
using Web.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Core.Interfaces;

namespace Web.Controllers
{
    public class PortfolioItemsController : Controller
    {
        private readonly IUnitOfWork<PortfolioItem> _portfolio;
        [Obsolete]
        private readonly IHostingEnvironment _hosting;

        [Obsolete]
        public PortfolioItemsController(IUnitOfWork<PortfolioItem> portfolio, IHostingEnvironment hosting)
        {
            _portfolio = portfolio;
            _hosting = hosting;
        }

        // GET: PortfolioItems
        public IActionResult Index()
        {
            return View(_portfolio.Entity.GetAll());
        }

        public IActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }

            return View(portfolioItem);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PortfolioViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.File != null)
                {
                    string uploads = Path.Combine(_hosting.WebRootPath, @"img\portfolio");
                    string fullPath = Path.Combine(uploads, model.File.FileName);
                    model.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                }

                PortfolioItem portfolioItem = new PortfolioItem
                {
                    ProjectName = model.ProjectName,
                    Description = model.Description,
                    ImageUrl = model.File.FileName,
                    UrlGitHub = model.UrlGitHub,
                    UrlView = model.UrlView,
                    DateAdded = DateTime.Now
                };

                _portfolio.Entity.Insert(portfolioItem);
                _portfolio.Save();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        public IActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }

            PortfolioViewModel portfolioViewModel = new PortfolioViewModel
            {
                Id = portfolioItem.Id,
                ProjectName = portfolioItem.ProjectName,
                Description = portfolioItem.Description,
                ImageUrl = portfolioItem.ImageUrl,
                UrlGitHub = portfolioItem.UrlGitHub,
                UrlView = portfolioItem.UrlView,
            };

            return View(portfolioViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, PortfolioViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (model.File != null)
                    {
                        string uploads = Path.Combine(_hosting.WebRootPath, @"img\portfolio");
                        string fullPath = Path.Combine(uploads, model.File.FileName);
                        model.File.CopyTo(new FileStream(fullPath, FileMode.Create));
                    }

                    PortfolioItem portfolioItem = new PortfolioItem
                    {
                        Id = model.Id,
                        ProjectName = model.ProjectName,
                        Description = model.Description,
                        UrlGitHub = model.UrlGitHub,
                        UrlView = model.UrlView,
                        ImageUrl = model.File.FileName,
                        DateAdded = DateTime.Now
                    };

                    _portfolio.Entity.Update(portfolioItem);
                    _portfolio.Save();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PortfolioItemExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }



        public IActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var portfolioItem = _portfolio.Entity.GetById(id);
            if (portfolioItem == null)
            {
                return NotFound();
            }

            return View(portfolioItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _portfolio.Entity.Delete(id);
            _portfolio.Save();
            return RedirectToAction(nameof(Index));
        }



        private bool PortfolioItemExists(Guid id)
        {
            return _portfolio.Entity.GetAll().Any(e => e.Id == id);
        }
    }
}
