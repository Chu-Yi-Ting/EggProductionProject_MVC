using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class VaccineTable
{
    public int VaccineSid { get; set; }

    public string VaccineName { get; set; } = null!;

    public decimal Cost { get; set; }

    public virtual ICollection<Vaccinate> Vaccinates { get; set; } = new List<Vaccinate>();
}
