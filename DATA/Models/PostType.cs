﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace DATA.Models;

public partial class PostType
{
    public int TypeId { get; set; }

    public string TypeName { get; set; }

    public string Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}