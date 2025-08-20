using System;
using System.Collections.Generic;

namespace idat_bank.Models;

public partial class Cuenta
{
    public long Id { get; set; }

    public int UsuarioId { get; set; }

    public string TipoCuenta { get; set; } = null!;

    public decimal Saldo { get; set; }

    public virtual ICollection<Transferencia> TransferenciaCuentaDestinos { get; set; } = new List<Transferencia>();

    public virtual ICollection<Transferencia> TransferenciaCuentaOrigens { get; set; } = new List<Transferencia>();

    public virtual Usuario Usuario { get; set; } = null!;
}
