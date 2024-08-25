﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class EmpDepartment
{
    public int EmpDepartmentSid { get; set; }

    public string? EmpDepartment1 { get; set; }

    public string? EmpDepartmentLocation { get; set; }

    public int? EmpDepartmentHeadSid { get; set; }

    public string? EmpDepartmentPhone { get; set; }

    public DateOnly? EmpDepartmentDate { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
