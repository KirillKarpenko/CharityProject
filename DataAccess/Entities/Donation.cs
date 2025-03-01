﻿namespace DataAccess.Entities;
public class Donation
{
    public required string Id { get; set; }

    public required string DonorId { get; set; }

    public required string OrganizationId { get; set; }

    public required DateTime TimeOfOperation { get; set; }

    public required decimal Amount { get; set; }

    public Donor? Donor { get; set; }

    public Organization? Organization { get; set; }
}
