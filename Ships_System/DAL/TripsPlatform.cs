//------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    
    public partial class TripsPlatform
    {
        public int Id { get; set; }
        public int TripId { get; set; }
        public int PlatformId { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual Platform Platform { get; set; }
        public virtual Trip Trip { get; set; }
    }
}
