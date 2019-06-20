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
    public class tblRequest_ItemModelController : Controller
    {
        private readonly GoWriteContext _context;

        public tblRequest_ItemModelController(GoWriteContext context)
        {
            _context = context;
        }

        // GET: tblRequest_ItemModel
        public async Task<IActionResult> Index()
        {
            var goWriteContext = _context.tblRequest_Item.Include(t => t.Request_Details);
            return View(await goWriteContext.ToListAsync());
        }

        // GET: tblRequest_ItemModel/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_ItemModel = await _context.tblRequest_Item
                .Include(t => t.Request_Details)
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (tblRequest_ItemModel == null)
            {
                return NotFound();
            }

            return View(tblRequest_ItemModel);
        }

        // GET: tblRequest_ItemModel/Create
        public IActionResult Create()
        {
            ViewData["RequestID"] = new SelectList(_context.tblRequest_Details, "RequestID", "RequestID");
            return View();
        }

        // POST: tblRequest_ItemModel/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemID,SequenceNo,ReturnFlag,Command,ScreenCapture,RequestID")] tblRequest_ItemModel tblRequest_ItemModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblRequest_ItemModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RequestID"] = new SelectList(_context.tblRequest_Details, "RequestID", "RequestID", tblRequest_ItemModel.RequestID);
            return View(tblRequest_ItemModel);
        }

        // GET: tblRequest_ItemModel/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_ItemModel = await _context.tblRequest_Item.FindAsync(id);
            if (tblRequest_ItemModel == null)
            {
                return NotFound();
            }
            ViewData["RequestID"] = new SelectList(_context.tblRequest_Details, "RequestID", "RequestID", tblRequest_ItemModel.RequestID);
            return View(tblRequest_ItemModel);
        }

        // POST: tblRequest_ItemModel/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemID,SequenceNo,ReturnFlag,Command,ScreenCapture,RequestID")] tblRequest_ItemModel tblRequest_ItemModel)
        {
            if (id != tblRequest_ItemModel.ItemID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblRequest_ItemModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!tblRequest_ItemModelExists(tblRequest_ItemModel.ItemID))
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
            ViewData["RequestID"] = new SelectList(_context.tblRequest_Details, "RequestID", "RequestID", tblRequest_ItemModel.RequestID);
            return View(tblRequest_ItemModel);
        }

        // GET: tblRequest_ItemModel/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblRequest_ItemModel = await _context.tblRequest_Item
                .Include(t => t.Request_Details)
                .FirstOrDefaultAsync(m => m.ItemID == id);
            if (tblRequest_ItemModel == null)
            {
                return NotFound();
            }

            return View(tblRequest_ItemModel);
        }

        // POST: tblRequest_ItemModel/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblRequest_ItemModel = await _context.tblRequest_Item.FindAsync(id);
            _context.tblRequest_Item.Remove(tblRequest_ItemModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool tblRequest_ItemModelExists(int id)
        {
            return _context.tblRequest_Item.Any(e => e.ItemID == id);
        }
    }
}
