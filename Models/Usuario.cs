using System;
using System.Collections.Generic;

namespace idat_bank.Models;

public partial class Usuario
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public virtual ICollection<Cuenta> Cuenta { get; set; } = new List<Cuenta>();
}
