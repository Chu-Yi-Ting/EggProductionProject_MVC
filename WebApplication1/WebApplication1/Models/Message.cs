﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Message
{
    public int MessageSid { get; set; }

    public int? MemberSid { get; set; }

    public int? VideoSid { get; set; }

    public string MessageContent { get; set; }

    public int? MessageLikes { get; set; }

    public DateTime? MessageDate { get; set; }

    public bool? MessageDelete { get; set; }

    public int? MessageNumber { get; set; }

    public virtual VideoSummary VideoS { get; set; }
}