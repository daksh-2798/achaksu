//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Projectnet.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CartMaster
    {
        public int Id { get; set; }
        public string ProdNo { get; set; }
        public int Price { get; set; }
        public int Quantitty { get; set; }
        public decimal total { get; set; }
        public string UserEmail { get; set; }
    }
}
