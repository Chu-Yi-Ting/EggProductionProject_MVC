using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Vaccinate
{
    public int VaccinateSid { get; set; }

    public int ChickSid { get; set; }

    public string ChickLotNo { get; set; } = null!;

    public int VaccineSid { get; set; }

    public DateOnly VaccineDate { get; set; }

    public DateOnly VaccinateDate { get; set; }

    public virtual ChickLotNo ChickS { get; set; } = null!;

    public virtual VaccineTable VaccineS { get; set; } = null!;
}
