﻿using Cpnucleo.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Cpnucleo.Infra.Data.Mappings
{
    internal class TarefaMap : IEntityTypeConfiguration<Tarefa>
    {
        public void Configure(EntityTypeBuilder<Tarefa> builder)
        {
            builder.ToTable("CPN_TB_TAREFA");

            builder.Property(c => c.Id)
                .HasColumnName("TAR_ID")
                .HasColumnType("uniqueidentifier")
                .HasDefaultValue(Guid.NewGuid())
                .IsRequired();

            builder.Property(c => c.Nome)
                .HasColumnName("TAR_NOME")
                .HasColumnType("varchar(450)")
                .HasMaxLength(450)
                .IsRequired();

            builder.Property(c => c.DataInicio)
                .HasColumnName("TAR_DATA_INICIO")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.DataTermino)
                .HasColumnName("TAR_DATA_TERMINO")
                .HasColumnType("datetime")
                .IsRequired();

            builder.Property(c => c.QtdHoras)
                .HasColumnName("TAR_QTD_HORAS")
                .HasColumnType("int")
                .IsRequired();

            builder.Property(c => c.Detalhe)
                .HasColumnName("TAR_DETALHE")
                .HasColumnType("varchar(1000)");

            builder.Property(c => c.IdProjeto)
                .HasColumnName("PROJ_ID")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.IdWorkflow)
                .HasColumnName("WOR_ID")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.IdRecurso)
                .HasColumnName("REC_ID")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.IdTipoTarefa)
                .HasColumnName("TIP_ID")
                .HasColumnType("uniqueidentifier")
                .IsRequired();

            builder.Property(c => c.DataInclusao)
                .HasColumnName("TAR_DATA_INCLUSAO")
                .HasColumnType("datetime")
                .HasDefaultValue(DateTime.Now)
                .IsRequired();

            builder.Property(c => c.DataAlteracao)
                .HasColumnName("TAR_DATA_ALTERACAO")
                .HasColumnType("datetime");

            builder.Property(c => c.DataExclusao)
                .HasColumnName("TAR_DATA_EXCLUSAO")
                .HasColumnType("datetime");

            builder.Property(c => c.Ativo)
                .HasColumnName("TAR_ATIVO")
                .HasColumnType("bit")
                .HasDefaultValue(true)
                .IsRequired();

            builder
                .HasOne(p => p.Projeto)
                .WithMany()
                .HasForeignKey(f => f.IdProjeto)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.Workflow)
                .WithMany(b => b.ListaTarefas)
                .HasForeignKey(f => f.IdWorkflow)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.Recurso)
                .WithMany()
                .HasForeignKey(f => f.IdRecurso)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(p => p.TipoTarefa)
                .WithMany()
                .HasForeignKey(f => f.IdTipoTarefa)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.ListaImpedimentos)
                .WithOne(c => c.Tarefa)
                .HasForeignKey(f => f.IdTarefa)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.ListaApontamentos)
                .WithOne(c => c.Tarefa)
                .HasForeignKey(f => f.IdTarefa)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasMany(p => p.ListaRecursoTarefas)
                .WithOne(c => c.Tarefa)
                .HasForeignKey(f => f.IdTarefa)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
