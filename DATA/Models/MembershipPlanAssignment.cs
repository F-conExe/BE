﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DATA.Models;

public partial class MembershipPlanAssignment
{
    public int AssignmentId { get; set; }

    public int? MembershipId { get; set; }

    public int? PlanId { get; set; }

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string Status { get; set; }

    public virtual Membership Membership { get; set; }

    public virtual MembershipPlan Plan { get; set; }
}