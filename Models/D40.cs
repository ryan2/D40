using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace D40.Models
{
    public class D40
    {
        public int ID { get; set; }
        public string Category { get; set; }
        public int Record_ID { get; set; }
        public string Asset_Tag { get; set; }
        public string Asset_status { get; set; }
        public string Serial_Number { get; set; }
        public string BB_Phone { get; set; }
        public DateTime Refresh_Date { get; set; }
        public string Model { get; set; }
        public string Seat_Type { get; set; }
        public string Service_Level { get; set; }
        public string HHS_Billing { get; set; }
        public string OpDiv { get; set; }
        public string StaffDiv { get; set; }
        public string Office { get; set; }
        public string Last_Name { get; set; }
        public string First_Name { get; set; }
        public string Site_Address { get; set; }
        public string Floor { get; set; }
        public string Room { get; set; }
        public Nullable<DateTime> Lumension_Report_Date { get; set; }
        public string Lumension_Computer_Name { get; set; }
        public string Lumension_Login_User { get; set; }
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
                bool IDD = (propertyInfo == typeA.GetProperty("ID"));
                if (IDD){
                    continue;
                }
                object obj1 = propertyInfo.GetValue(this);
                object obj2 = propertyInfo.GetValue(d40);
                bool same = (obj1 == null) ? (obj2== null) : obj1.Equals(obj2);
                if (!same){
                    return false;
                }
            }
            return true;
        }
    }
    public class D40DBContext : DbContext
    {
        public DbSet<D40> D40 { get; set; }
    }
}