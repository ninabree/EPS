using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExpenseProcessingSystem.Data;
using ExpenseProcessingSystem.Models.Gbase;

namespace ExpenseProcessingSystem.Controllers
{
    public class tblRequest_DetailsModelController : Controller
    {
        private readonly GoWriteContext _context;

        public tblRequest_DetailsModelController(GoWriteContext context)
        {
            _context = context;
        }

        // GET: tblRequest_DetailsModel
        public async Task<IActionResult> Index()
        {
            return View(await _context.tblRequest_Details.ToListAsync());
        }

        // GET: tblRequest_DetailsModel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_DetailsModel = await _context.tblRequest_Details
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (tblRequest_DetailsModel == null)
            {
                return NotFound();
            }

            return View(tblRequest_DetailsModel);
        }

        // GET: tblRequest_DetailsModel/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: tblRequest_DetailsModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestID,RacfID,RacfPassword,RequestCreated,ReturnMessage,Status,StatusDate,SystemAbbr,Priority")] tblRequest_DetailsModel tblRequest_DetailsModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblRequest_DetailsModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblRequest_DetailsModel);
        }

        // GET: tblRequest_DetailsModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_DetailsModel = await _context.tblRequest_Details.FindAsync(id);
            if (tblRequest_DetailsModel == null)
            {
                return NotFound();
            }
            return View(tblRequest_DetailsModel);
        }

        // POST: tblRequest_DetailsModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RequestID,RacfID,RacfPassword,RequestCreated,ReturnMessage,Status,StatusDate,SystemAbbr,Priority")] tblRequest_DetailsModel tblRequest_DetailsModel)
        {
            if (id != tblRequest_DetailsModel.RequestID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblRequest_DetailsModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tblRequest_DetailsModelExists(tblRequest_DetailsModel.RequestID))
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
            return View(tblRequest_DetailsModel);
        }

        // GET: tblRequest_DetailsModel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_DetailsModel = await _context.tblRequest_Details
                .FirstOrDefaultAsync(m => m.RequestID == id);
            if (tblRequest_DetailsModel == null)
            {
                return NotFound();
            }

            return View(tblRequest_DetailsModel);
        }

        // POST: tblRequest_DetailsModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblRequest_DetailsModel = await _context.tblRequest_Details.FindAsync(id);
            _context.tblRequest_Details.Remove(tblRequest_DetailsModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tblRequest_DetailsModelExists(int id)
        {
            return _context.tblRequest_Details.Any(e => e.RequestID == id);
        }
    }
}
