using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using idat_bank.Models;

namespace idat_bank.Controllers
{
    public class TransferenciasController : Controller
    {
        private readonly IdatBankContext _context;

        public TransferenciasController(IdatBankContext context)
        {
            _context = context;
        }

        // GET: Transferencias
        public async Task<IActionResult> Index()
        {
            var idatBankContext = _context.Transferencias.Include(t => t.CuentaDestino).Include(t => t.CuentaOrigen);
            return View(await idatBankContext.ToListAsync());
        }

        // GET: Transferencias/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencia = await _context.Transferencias
                .Include(t => t.CuentaDestino)
                .Include(t => t.CuentaOrigen)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transferencia == null)
            {
                return NotFound();
            }

            return View(transferencia);
        }

        // GET: Transferencias/Create
        public IActionResult Create()
        {
            ViewData["CuentaDestinoId"] = new SelectList(_context.Cuentas, "Id", "Id");
            ViewData["CuentaOrigenId"] = new SelectList(_context.Cuentas, "Id", "Id");
            return View();
        }

        // POST: Transferencias/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CuentaOrigenId,CuentaDestinoId,Fecha,Monto,Tipo")] Transferencia transferencia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transferencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CuentaDestinoId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaDestinoId);
            ViewData["CuentaOrigenId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaOrigenId);
            return View(transferencia);
        }

        // GET: Transferencias/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencia = await _context.Transferencias.FindAsync(id);
            if (transferencia == null)
            {
                return NotFound();
            }
            ViewData["CuentaDestinoId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaDestinoId);
            ViewData["CuentaOrigenId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaOrigenId);
            return View(transferencia);
        }

        // POST: Transferencias/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,CuentaOrigenId,CuentaDestinoId,Fecha,Monto,Tipo")] Transferencia transferencia)
        {
            if (id != transferencia.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transferencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransferenciaExists(transferencia.Id))
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
            ViewData["CuentaDestinoId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaDestinoId);
            ViewData["CuentaOrigenId"] = new SelectList(_context.Cuentas, "Id", "Id", transferencia.CuentaOrigenId);
            return View(transferencia);
        }

        // GET: Transferencias/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transferencia = await _context.Transferencias
                .Include(t => t.CuentaDestino)
                .Include(t => t.CuentaOrigen)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transferencia == null)
            {
                return NotFound();
            }

            return View(transferencia);
        }

        // POST: Transferencias/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var transferencia = await _context.Transferencias.FindAsync(id);
            if (transferencia != null)
            {
                _context.Transferencias.Remove(transferencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransferenciaExists(long id)
        {
            return _context.Transferencias.Any(e => e.Id == id);
        }

        [HttpGet("api/transferencias/{cuentaId}")]
        public async Task<IActionResult> GetTransferenciasPorCuenta(long cuentaId, int usuarioId)
        {
            var transferencias = await _context.Transferencias
                .Where(t =>
                    (t.CuentaOrigen.UsuarioId == usuarioId || t.CuentaDestino.UsuarioId == usuarioId)
                    && (t.CuentaOrigenId == cuentaId || t.CuentaDestinoId == cuentaId)
                )
                .Select(t => new
                {
                    t.Id,
                    t.Fecha,
                    t.Monto,
                    t.Tipo,
                    CuentaOrigenId = t.CuentaOrigenId,
                    CuentaDestinoId = t.CuentaDestinoId,
                    CuentaOrigen = t.CuentaOrigen.TipoCuenta,
                    CuentaDestino = t.CuentaDestino.TipoCuenta,
                    Descripcion = t.Tipo == "Ingreso" ? "Depósito recibido" : "Transferencia enviada"
                })
                .OrderByDescending(t => t.Fecha)
                .ToListAsync();

            return Ok(transferencias);
        }
    }
}
