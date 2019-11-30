using System;

namespace Shameful_MVC.Models
{

    public class Assignment
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; } = DateTime.UtcNow;

        public byte[] File { get; set; }
    }
}