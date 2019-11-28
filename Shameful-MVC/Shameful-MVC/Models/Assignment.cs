using System;

namespace Shameful_MVC.Models
{
    public partial class Assignment
    {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
    }
}
