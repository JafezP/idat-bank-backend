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
    public class CuentasController : Controller
    {
        private readonly IdatBankContext _context;

        public CuentasController(IdatBankContext context)
        {
            _context = context;
        }

        // GET: Cuentas
        public async Task<IActionResult> Index()
        {
            var idatBankContext = _context.Cuentas.Include(c => c.Usuario);
            return View(await idatBankContext.ToListAsync());
        }

        // GET: Cuentas/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuentas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // GET: Cuentas/Create
        public IActionResult Create()
        {
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id");
            return View();
        }

        // POST: Cuentas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UsuarioId,TipoCuenta,Saldo")] Cuenta cuenta)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cuenta);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", cuenta.UsuarioId);
            return View(cuenta);
        }

        // GET: Cuentas/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", cuenta.UsuarioId);
            return View(cuenta);
        }

        // POST: Cuentas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,UsuarioId,TipoCuenta,Saldo")] Cuenta cuenta)
        {
            if (id != cuenta.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cuenta);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CuentaExists(cuenta.Id))
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
            ViewData["UsuarioId"] = new SelectList(_context.Usuarios, "Id", "Id", cuenta.UsuarioId);
            return View(cuenta);
        }

        // GET: Cuentas/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cuenta = await _context.Cuentas
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cuenta == null)
            {
                return NotFound();
            }

            return View(cuenta);
        }

        // POST: Cuentas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta != null)
            {
                _context.Cuentas.Remove(cuenta);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Route("api/transferir")]
        public async Task<IActionResult> Transferir([FromBody] TransferRequest transferRequest)
        {
            if (ModelState.IsValid)
            {
                var cuentaOrigen = await _context.Cuentas.FindAsync(transferRequest.CuentaOrigenId);
                var cuentaDestino = await _context.Cuentas.FindAsync(transferRequest.CuentaDestinoId);

                if (cuentaOrigen == null || cuentaDestino == null)
                {
                    return NotFound("Una o ambas cuentas no existen");
                }

                if (cuentaOrigen.Saldo < transferRequest.Monto)
                {
                    return BadRequest("Saldo insuficiente en la cuenta de origen");
                }

                // Actualizar saldos
                cuentaOrigen.Saldo -= transferRequest.Monto;
                cuentaDestino.Saldo += transferRequest.Monto;

                _context.Update(cuentaOrigen);
                _context.Update(cuentaDestino);

                // Crear registro de transferencia
                var transferencia = new Transferencia
                {
                    CuentaOrigenId = cuentaOrigen.Id,
                    CuentaDestinoId = cuentaDestino.Id,
                    Monto = transferRequest.Monto,
                    Tipo = "Egreso", // o lo que quieras según tu lógica
                    Fecha = DateTime.Now
                };
                _context.Transferencias.Add(transferencia);

                await _context.SaveChangesAsync();

                return Ok("Transferencia exitosa");
            }

            return BadRequest("Datos inválidos");
        }

        public class TransferRequest
        {
            public long CuentaOrigenId { get; set; }
            public long CuentaDestinoId { get; set; }
            public decimal Monto { get; set; }
            public string Moneda { get; set; } // Soles o dólares
        }

        private bool CuentaExists(long id)
        {
            return _context.Cuentas.Any(e => e.Id == id);
        }

        [HttpGet("api/cuentas")]
        public async Task<IActionResult> GetCuentas()
        {
            var cuentas = await _context.Cuentas.Include(c => c.Usuario).ToListAsync();

            // Si deseas solo una lista de cuentas (sin incluir usuarios u otra información):
            var cuentasData = cuentas.Select(c => new
            {
                c.Id,
                c.TipoCuenta,
                c.Saldo,
                Usuario = c.Usuario.Nombre // Asumiendo que "Usuario" tiene un nombre
            });

            return Ok(cuentasData);
        }

        [HttpGet("api/cuentas/destino/{cuentaOrigenId}")]
        public async Task<IActionResult> GetCuentasDestino(long cuentaOrigenId)
        {
            var cuentaOrigen = await _context.Cuentas.FindAsync(cuentaOrigenId);
            if (cuentaOrigen == null)
                return NotFound("Cuenta de origen no encontrada");

            var targetType = cuentaOrigen.TipoCuenta == "Ahorros" ? "Sueldo" : "Ahorros";

            var cuentasDestino = await _context.Cuentas
                .Where(c => c.TipoCuenta == targetType && c.Id != cuentaOrigenId && c.UsuarioId == cuentaOrigen.UsuarioId)
                .Include(c => c.Usuario)
                .Select(c => new {
                    c.Id,
                    c.TipoCuenta,
                    c.Saldo,
                    Usuario = c.Usuario.Nombre
                })
                .ToListAsync();

            return Ok(cuentasDestino);
        }
    }
}
