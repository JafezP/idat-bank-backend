using System;
using System.Collections.Generic;

namespace idat_bank.Models;

public partial class Transferencia
{
    public long Id { get; set; }

    public long CuentaOrigenId { get; set; }

    public long CuentaDestinoId { get; set; }

    public DateTime? Fecha { get; set; }

    public decimal Monto { get; set; }

    public string Tipo { get; set; } = null!;

    public virtual Cuenta CuentaDestino { get; set; } = null!;

    public virtual Cuenta CuentaOrigen { get; set; } = null!;
}
