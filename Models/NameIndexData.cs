using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using D40.Models;

namespace D40.ViewModels
{
    public class NameIndexData
    {
        public IEnumerable<Name> Users { get; set; }
        public IEnumerable<D40.Models.D40> Assets { get; set; }
        public IEnumerable<Ticket> Tickets { get; set; }
        public IEnumerable<SoftwareName> Software { get; set; }
    }
}