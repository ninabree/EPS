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
    public class TblCm10Controller : Controller
    {
        private readonly GOExpressContext _context;

        public TblCm10Controller(GOExpressContext context)
        {
            _context = context;
        }

        // GET: TblCm10
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblCm10.ToListAsync());
        }

        // GET: TblCm10/Details/5
        public async Task<IActionResult> Details(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCm10 = await _context.TblCm10
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblCm10 == null)
            {
                return NotFound();
            }

            return View(tblCm10);
        }

        // GET: TblCm10/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblCm10/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SystemName,Groupcode,Branchno,OpeKind,AutoApproved,WarningOverride,CcyFormat,OpeBranch,ValueDate,ReferenceType,ReferenceNo,Comment,Section,Remarks,Memo,SchemeNo,Entry11Type,Entry11IbfCode,Entry11Ccy,Entry11Amt,Entry11Cust,Entry11Actcde,Entry11ActType,Entry11ActNo,Entry11ExchRate,Entry11ExchCcy,Entry11Fund,Entry11CheckNo,Entry11Available,Entry11AdvcPrnt,Entry11Details,Entry11Entity,Entry11Division,Entry11InterAmt,Entry11InterRate,Entry12Type,Entry12IbfCode,Entry12Ccy,Entry12Amt,Entry12Cust,Entry12Actcde,Entry12ActType,Entry12ActNo,Entry12ExchRate,Entry12ExchCcy,Entry12Fund,Entry12CheckNo,Entry12Available,Entry12AdvcPrnt,Entry12Details,Entry12Entity,Entry12Division,Entry12InterAmt,Entry12InterRate,Entry21Type,Entry21IbfCode,Entry21Ccy,Entry21Amt,Entry21Cust,Entry21Actcde,Entry21ActType,Entry21ActNo,Entry21ExchRate,Entry21ExchCcy,Entry21Fund,Entry21CheckNo,Entry21Available,Entry21AdvcPrnt,Entry21Details,Entry21Entity,Entry21Division,Entry21InterAmt,Entry21InterRate,Entry22Type,Entry22IbfCode,Entry22Ccy,Entry22Amt,Entry22Cust,Entry22Actcde,Entry22ActType,Entry22ActNo,Entry22ExchRate,Entry22ExchCcy,Entry22Fund,Entry22CheckNo,Entry22Available,Entry22AdvcPrnt,Entry22Details,Entry22Entity,Entry22Division,Entry22InterAmt,Entry22InterRate,Entry31Type,Entry31IbfCode,Entry31Ccy,Entry31Amt,Entry31Cust,Entry31Actcde,Entry31ActType,Entry31ActNo,Entry31ExchRate,Entry31ExchCcy,Entry31Fund,Entry31CheckNo,Entry31Available,Entry31AdvcPrnt,Entry31Details,Entry31Entity,Entry31Division,Entry31InterAmt,Entry31InterRate,Entry32Type,Entry32IbfCode,Entry32Ccy,Entry32Amt,Entry32Cust,Entry32Actcde,Entry32ActType,Entry32ActNo,Entry32ExchRate,Entry32ExchCcy,Entry32Fund,Entry32CheckNo,Entry32Available,Entry32AdvcPrnt,Entry32Details,Entry32Entity,Entry32Division,Entry32InterAmt,Entry32InterRate,Entry41Type,Entry41IbfCode,Entry41Ccy,Entry41Amt,Entry41Cust,Entry41Actcde,Entry41ActType,Entry41ActNo,Entry41ExchRate,Entry41ExchCcy,Entry41Fund,Entry41CheckNo,Entry41Available,Entry41AdvcPrnt,Entry41Details,Entry41Entity,Entry41Division,Entry41InterAmt,Entry41InterRate,Entry42Type,Entry42IbfCode,Entry42Ccy,Entry42Amt,Entry42Cust,Entry42Actcde,Entry42ActType,Entry42ActNo,Entry42ExchRate,Entry42ExchCcy,Entry42Fund,Entry42CheckNo,Entry42Available,Entry42AdvcPrnt,Entry42Details,Entry42Entity,Entry42Division,Entry42InterAmt,Entry42InterRate,MakerEmpno,Empno,Datestamp,Transno,Xmlmsg,Recstatus,Timesent,Timerespond")] TblCm10 tblCm10)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblCm10);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblCm10);
        }

        // GET: TblCm10/Edit/5
        public async Task<IActionResult> Edit(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCm10 = await _context.TblCm10.FindAsync(id);
            if (tblCm10 == null)
            {
                return NotFound();
            }
            return View(tblCm10);
        }

        // POST: TblCm10/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(decimal id, [Bind("Id,SystemName,Groupcode,Branchno,OpeKind,AutoApproved,WarningOverride,CcyFormat,OpeBranch,ValueDate,ReferenceType,ReferenceNo,Comment,Section,Remarks,Memo,SchemeNo,Entry11Type,Entry11IbfCode,Entry11Ccy,Entry11Amt,Entry11Cust,Entry11Actcde,Entry11ActType,Entry11ActNo,Entry11ExchRate,Entry11ExchCcy,Entry11Fund,Entry11CheckNo,Entry11Available,Entry11AdvcPrnt,Entry11Details,Entry11Entity,Entry11Division,Entry11InterAmt,Entry11InterRate,Entry12Type,Entry12IbfCode,Entry12Ccy,Entry12Amt,Entry12Cust,Entry12Actcde,Entry12ActType,Entry12ActNo,Entry12ExchRate,Entry12ExchCcy,Entry12Fund,Entry12CheckNo,Entry12Available,Entry12AdvcPrnt,Entry12Details,Entry12Entity,Entry12Division,Entry12InterAmt,Entry12InterRate,Entry21Type,Entry21IbfCode,Entry21Ccy,Entry21Amt,Entry21Cust,Entry21Actcde,Entry21ActType,Entry21ActNo,Entry21ExchRate,Entry21ExchCcy,Entry21Fund,Entry21CheckNo,Entry21Available,Entry21AdvcPrnt,Entry21Details,Entry21Entity,Entry21Division,Entry21InterAmt,Entry21InterRate,Entry22Type,Entry22IbfCode,Entry22Ccy,Entry22Amt,Entry22Cust,Entry22Actcde,Entry22ActType,Entry22ActNo,Entry22ExchRate,Entry22ExchCcy,Entry22Fund,Entry22CheckNo,Entry22Available,Entry22AdvcPrnt,Entry22Details,Entry22Entity,Entry22Division,Entry22InterAmt,Entry22InterRate,Entry31Type,Entry31IbfCode,Entry31Ccy,Entry31Amt,Entry31Cust,Entry31Actcde,Entry31ActType,Entry31ActNo,Entry31ExchRate,Entry31ExchCcy,Entry31Fund,Entry31CheckNo,Entry31Available,Entry31AdvcPrnt,Entry31Details,Entry31Entity,Entry31Division,Entry31InterAmt,Entry31InterRate,Entry32Type,Entry32IbfCode,Entry32Ccy,Entry32Amt,Entry32Cust,Entry32Actcde,Entry32ActType,Entry32ActNo,Entry32ExchRate,Entry32ExchCcy,Entry32Fund,Entry32CheckNo,Entry32Available,Entry32AdvcPrnt,Entry32Details,Entry32Entity,Entry32Division,Entry32InterAmt,Entry32InterRate,Entry41Type,Entry41IbfCode,Entry41Ccy,Entry41Amt,Entry41Cust,Entry41Actcde,Entry41ActType,Entry41ActNo,Entry41ExchRate,Entry41ExchCcy,Entry41Fund,Entry41CheckNo,Entry41Available,Entry41AdvcPrnt,Entry41Details,Entry41Entity,Entry41Division,Entry41InterAmt,Entry41InterRate,Entry42Type,Entry42IbfCode,Entry42Ccy,Entry42Amt,Entry42Cust,Entry42Actcde,Entry42ActType,Entry42ActNo,Entry42ExchRate,Entry42ExchCcy,Entry42Fund,Entry42CheckNo,Entry42Available,Entry42AdvcPrnt,Entry42Details,Entry42Entity,Entry42Division,Entry42InterAmt,Entry42InterRate,MakerEmpno,Empno,Datestamp,Transno,Xmlmsg,Recstatus,Timesent,Timerespond")] TblCm10 tblCm10)
        {
            if (id != tblCm10.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblCm10);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblCm10Exists(tblCm10.Id))
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
            return View(tblCm10);
        }

        // GET: TblCm10/Delete/5
        public async Task<IActionResult> Delete(decimal? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCm10 = await _context.TblCm10
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblCm10 == null)
            {
                return NotFound();
            }

            return View(tblCm10);
        }

        // POST: TblCm10/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(decimal id)
        {
            var tblCm10 = await _context.TblCm10.FindAsync(id);
            _context.TblCm10.Remove(tblCm10);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblCm10Exists(decimal id)
        {
            return _context.TblCm10.Any(e => e.Id == id);
        }
    }
}
