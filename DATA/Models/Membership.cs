﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DATA.Models;

public partial class Membership
{
    public int MembershipId { get; set; }

    public int? UserId { get; set; }

    public string Status { get; set; }

    public virtual ICollection<MembershipPlanAssignment> MembershipPlanAssignments { get; set; } = new List<MembershipPlanAssignment>();

    public virtual User User { get; set; }
}