﻿using AlguienDijoChamba.Api.Professionals.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AlguienDijoChamba.Api.Shared.Infrastructure.Persistence.EFC.Configuration;

public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
{
    public void Configure(EntityTypeBuilder<Professional> builder)
    {
        builder.ToTable("Professionals");
        builder.HasKey(p => p.Id);
        builder.HasIndex(p => p.Dni).IsUnique();
        builder.Property(p => p.Nombres).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Apellidos).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Celular).IsRequired().HasMaxLength(9);
        builder.Property(p => p.Email).IsRequired().HasMaxLength(255);
        builder.Property(p => p.ProfilePhotoUrl).IsRequired(false); // Opcional
        builder.Property(p => p.YearsOfExperience).IsRequired();
        builder.Property(p => p.HourlyRate).IsRequired(false); // Opcional
        builder.Property(p => p.ProfessionalBio).IsRequired().HasMaxLength(1200);
        builder.Property(p => p.Ocupacion).IsRequired().HasMaxLength(100);
        builder.Property(p => p.FechaNacimiento).IsRequired(false); // Opcional
        builder.Property(p => p.Genero).IsRequired(false).HasMaxLength(50); // Opcional

        // Relación uno a uno con User
        builder.HasOne<IAM.Domain.User>()
            .WithOne()
            .HasForeignKey<Professional>(p => p.UserId);
    }
}