using System;
using LSG.GenericCrud.Models;

namespace BikeService.Models.Entities;

public class BikeStats : IEntity
{
    public Guid Id { get; set; }

    public int ChargeLevel { get; set; }

    public int ServiceMinutes { get; set; }
}