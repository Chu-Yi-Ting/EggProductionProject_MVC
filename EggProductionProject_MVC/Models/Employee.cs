﻿using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Employee
{
    public int EmployeeSid { get; set; }

    public string? EmpName { get; set; }

    public int? EmpDepartmentSid { get; set; }

    public string? EmpPhone { get; set; }

    public int? ReportTo { get; set; }

    public string? Password { get; set; }

    public string? Account { get; set; }

    public virtual EmpDepartment? EmpDepartmentS { get; set; }
}
