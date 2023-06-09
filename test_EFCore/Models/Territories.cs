﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace test_EFCore.Models;

public partial class Territories
{
    [Key]
    [Column("TerritoryID")]
    [StringLength(20)]
    public string TerritoryId { get; set; }

    [Required]
    [StringLength(50)]
    public string TerritoryDescription { get; set; }

    [Column("RegionID")]
    public int RegionId { get; set; }

    [ForeignKey("RegionId")]
    [InverseProperty("Territories")]
    public virtual Region Region { get; set; }

    [ForeignKey("TerritoryId")]
    [InverseProperty("Territory")]
    public virtual ICollection<Employees> Employee { get; } = new List<Employees>();
}