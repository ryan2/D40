using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;

namespace D40.Models
{
    public class Name
    {
        public int ID { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public bool Active { get; set; }
        public string Office { get; set; }
        public virtual ICollection<D40> Assets { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public virtual ICollection<SoftwareName> Software { get; set; }

    }
    public class D40
    {
        public int ID { get; set; }
        [Required()]
        public string Category { get; set; }
        public Nullable<int> Record_ID { get; set; }
        [Required()]
        public string Asset_Tag { get; set; }
        public string Asset_status { get; set; }
        public string Serial_Number { get; set; }
        public string BB_Phone { get; set; }
        public Nullable<DateTime> Refresh_Date { get; set; }
        public string Model { get; set; }
        public string Seat_Type { get; set; }
        public string Service_Level { get; set; }
        public string HHS_Billing { get; set; }
        public string OpDiv { get; set; }
        public string StaffDiv { get; set; }
        public string Office { get; set; }
        [Required()]
        public string Last_Name { get; set; }
        [Required()]
        public string First_Name { get; set; }
        public string Site_Address { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public Nullable<DateTime> Lumension_Report_Date { get; set; }
        public string Lumension_Computer_Name { get; set; }
        public string Lumension_Login_User { get; set; }
        public DateTime Received_Date { get; set; }
        [DataType(DataType.Date)]
         [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Returned_Date { get; set; }
        public int NameID { get; set; }
        public virtual Name User { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }
        public override bool Equals(object obj)
        {
            return this.Equals(obj as D40);
        }
        public bool Equals(D40 d40)
        {
            Type typeA = this.GetType();
            System.Reflection.PropertyInfo[] properties = typeA.GetProperties();
            foreach (System.Reflection.PropertyInfo propertyInfo in properties)
            {
                bool IDD = (propertyInfo == typeA.GetProperty("Category")) || (propertyInfo == typeA.GetProperty("Asset_Tag"))|| (propertyInfo == typeA.GetProperty("Asset_status"))|| (propertyInfo == typeA.GetProperty("Serial_Number"))|| (propertyInfo == typeA.GetProperty("BB_Phone"))|| (propertyInfo == typeA.GetProperty("Refresh_Date"))|| (propertyInfo == typeA.GetProperty("Model"))|| (propertyInfo == typeA.GetProperty("Service_Level"))|| (propertyInfo == typeA.GetProperty("Last_Name"))|| (propertyInfo == typeA.GetProperty("First_Name")) || (propertyInfo == typeA.GetProperty("Returned_Date"));
                if (!IDD){
                    continue;
                }
                object obj1 = propertyInfo.GetValue(this);
                object obj2 = propertyInfo.GetValue(d40);
                bool hi = (obj2 is string);
                if (hi)
                {
                    string str = (string)obj2;
                    string replacement = System.Text.RegularExpressions.Regex.Replace(str, @"\t|\n|\r", "");
                    obj2 = replacement;
                }
                bool same = (obj1 == null) ? (obj2== null) : obj1.Equals(obj2);
                if (!same){
                    return false;
                }
            }
            return true;
        }
    }
    public class Ticket
    {
        public int ID { get; set; }
        [Required()]
        public string Ticket_Num { get; set; }
        public DateTime Open_Date { get; set; }
        public Nullable<DateTime> Closed_Date { get; set; }
        [Required()]
        public string Description { get; set; }
        public int NameID { get; set; }
        public int? D40ID { get; set; }
        public virtual Name User { get; set; }
        public virtual D40 Asset { get; set; }
    }
    public class Software
    {
        public int ID { get; set; }
        [Required()]
        public string title { get; set; }
        [Required()]
        public string license { get; set; }
        public int num { get; set; }
        public virtual ICollection<SoftwareName> Users { get; set; }
    }
    public class SoftwareName
    {
        public int ID { get; set; }
        public int NameID { get; set; }
        public int SoftwareID { get; set; }
        public virtual Software Software { get; set; }
        public virtual Name User { get; set; }

    }
    public class Prices
    {
        public int ID { get; set; }
        public decimal C_G { get; set; }
        public decimal C_S { get; set; }
        public decimal C_P { get; set; }
        public decimal Ph_G { get; set; }
        public decimal Ph_S { get; set; }
        public decimal Ph_P { get; set; }
        public decimal Pr_S { get; set; }
        public decimal Pr_P { get; set; }
        public decimal Pr_G { get; set; }
        public decimal Ps_S { get; set; }
        public decimal Ps_P { get; set; }
        public decimal Ps_G { get; set; }
        public DateTime FY { get; set; }

    }

    public class D40DBContext : DbContext
    {
        public DbSet<D40> D40 { get; set; }
        public DbSet<Name> Names { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Software> Software { get; set; }
        public DbSet<SoftwareName> SoftwareNames { get; set; }
        public DbSet<Prices> Prices { get; set; }
    }
}