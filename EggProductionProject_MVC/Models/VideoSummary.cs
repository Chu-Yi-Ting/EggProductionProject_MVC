using System;
using System.Collections.Generic;

namespace EggProductionProject_MVC.Models;

public partial class VideoSummary
{
    public int VideoSid { get; set; }

    public int? CreatorSid { get; set; }

    public int? VideoDuration { get; set; }

    public string? VideoTitle { get; set; }

    public int? NatureSid { get; set; }

    public string? InformationColumn { get; set; }

    public DateTime UploadDate { get; set; }

    public int? TimesWatched { get; set; }

    public int? ScreenTextSid { get; set; }

    public string? MoviePath { get; set; }

    public bool AdSource { get; set; }

    public bool Advertised { get; set; }

    public int? PublicStatusNo { get; set; }

    public string? VideoCoverImage { get; set; }

    public virtual ICollection<Advertisment> Advertisments { get; set; } = new List<Advertisment>();

    public virtual Creator? CreatorS { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();

    public virtual Nature? NatureS { get; set; }

    public virtual PublicStatus? PublicStatusNoNavigation { get; set; }

    public virtual ScreenSummary? ScreenTextS { get; set; }
}
