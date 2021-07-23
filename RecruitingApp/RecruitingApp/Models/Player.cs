using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RecruitingApp.Models
{
    public class Player
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Number { get; set; } // sometimes they have letters on the jersey instead of numbers
        public string Position { get; set; }
        public string Rating { get; set; }
        public string Email { get; set; }
        public long Cell { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZIP { get; set; } // sometimes ZIP may contain letters or other characters
        public string Notes { get; set; }
        public bool ActivelyRecruiting { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public int TeamID { get; set; }
        public Player()
        {

        }
    }
}
