﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TechnoShieldApp.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class TechnoShieldBaseEntities : DbContext
    {
        public TechnoShieldBaseEntities()
            : base("name=TechnoShieldBaseEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<Organization> Organization { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Service> Service { get; set; }
        public virtual DbSet<ServiceOfProductInOrder> ServiceOfProductInOrder { get; set; }
        public virtual DbSet<StatusOfOrder> StatusOfOrder { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<TypeOfProduct> TypeOfProduct { get; set; }
        public virtual DbSet<TypeOfService> TypeOfService { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }
    }
}
