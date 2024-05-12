﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Models;

public enum RoleType
{
	Admin,
	Customer
}

[Table("User")]
public partial class User
{
    [Key]
    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string FName { get; set; }

    [StringLength(50)]
    public string LName { get; set; }

    [Required]
    [StringLength(50)]
    public string Email { get; set; }

    [Required]
    [StringLength(50)]
    public string Phone { get; set; }

    [StringLength(50)]
    public string Governorate { get; set; }

    [StringLength(50)]
    public string City { get; set; }

    [StringLength(50)]
    public string Street { get; set; }

    [StringLength(50)]
    public string PostalCode { get; set; }

    [Required]
    [StringLength(50)]
    public string Password { get; set; }


    [Required]
    [StringLength(8)]
    public string VertificationCode { get; set; }


    public DateTime? VerifiedAt{ get; set; }

    public bool IsVerified { get; set; }


    public bool IsDeleted { get; set; }

    public RoleType Role { get; set; }

    [InverseProperty("User")]
    public virtual Admin Admin { get; set; }

    [InverseProperty("User")]
    public virtual Customer Customer { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<NotificationMessage> NotificationMessages { get; set; } = new List<NotificationMessage>();

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();


	[InverseProperty("User")]
	public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();


}