﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ECommereceApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ECommereceApi.Models;

[Table("Product")]
public partial class Product : ISoftDeletable
{
    [Key]
    public int ProductId { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; }

    public double? Discount { get; set; }

    public double OriginalPrice { get; set; }

    public int Amount { get; set; }

    public string Description { get; set; }

    public int CategoryId { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DateDeleted { get ; set ; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductCart> ProductCarts { get; set; } = new List<ProductCart>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [InverseProperty("Product")]
    public virtual ICollection<ProductOffer> ProductOffers { get; set; } = new List<ProductOffer>();

    [InverseProperty("Product")]
    public virtual ICollection<Rate> Rates { get; set; } = new List<Rate>();


	[InverseProperty("Product")]
	public virtual ICollection<WishList> WishLists { get; set; } = new List<WishList>();

	[InverseProperty("Product")]
	public virtual ICollection<ProductOrder> ProductOrders { get; set; } = new List<ProductOrder>();
    
    public ICollection<ProductCategorySubCategoryValues> ProductCategorySubCategoryValues = new List<ProductCategorySubCategoryValues>();
	// Computed properties
	[NotMapped]
	public double? FinalPrice => OriginalPrice - Discount;
}