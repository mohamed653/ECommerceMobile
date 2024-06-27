using ECommereceApi.Validations;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;

namespace ECommereceApi.DTOs;

public class ReviewDTO 
{
    [ValidProductId]
    public int ProductId { get; set; }

    [ValidCustomerId]
    public int CustomerId { get; set; }

    [Range(0,5)]
    public int? NumOfStars { get; set; }

    [StringLength(200,MinimumLength = 2)]
    public string Comment { get; set; }

    [ValidReviewDate]
    public DateOnly RateDate { get; set; }


   
}
