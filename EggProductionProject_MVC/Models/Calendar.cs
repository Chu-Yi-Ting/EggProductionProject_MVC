using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class Calendar
{
    public int CalendarSid { get; set; }

    public int? MemberSid { get; set; }

    public string? TodoList { get; set; }

    public DateOnly? Date { get; set; }

    public DateOnly? InsertDate { get; set; }

    public int? Finished { get; set; }

    public virtual Member? MemberS { get; set; }
}
