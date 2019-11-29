using System;

namespace Shameful_MVC.Models
{

    public class Assignment
    {
        public string Name { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public byte[] File { get; set; }
    }
}