﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Models;

[PrimaryKey("ProductId", "CustomerId")]
public partial class Rate
{
    [Key]
    public int ProductId { get; set; }

    [Key]
    public int CustomerId { get; set; }

    public int? NumOfStars { get; set; }

    public string Comment { get; set; }

    public DateOnly RateDate { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("Rates")]
    public virtual Customer Customer { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Rates")]
    public virtual Product Product { get; set; }
}