using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Certification
{
    public int CertificationSid { get; set; }

    public int? MemberSid { get; set; }

    public string? CertificationNo { get; set; }

    public virtual Member? MemberS { get; set; }
}
