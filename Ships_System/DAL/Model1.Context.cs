﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Ships_System.DAL
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class SystemContext : DbContext
    {
        public SystemContext()
            : base("name=SystemContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Agent> Agents { get; set; }
        public virtual DbSet<Platform> Platforms { get; set; }
        public virtual DbSet<Port> Ports { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Ship> Ships { get; set; }
        public virtual DbSet<Trip> Trips { get; set; }
        public virtual DbSet<TripsLoad> TripsLoads { get; set; }
        public virtual DbSet<TripsPlatform> TripsPlatforms { get; set; }
        public virtual DbSet<TripsStatu> TripsStatus { get; set; }
    }
}
