using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace RecruitingApp.Models
{
    public class Team
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Level { get; set; }
        public string CoachName { get; set; }
        public string CoachEmail { get; set; }
        public string CoachPhone { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastUpdated { get; set; }
        public string AgeGroupString { get; set; }
        public int AgeGroupID { get; set; }
    

        public Team()
        {

        }
    }
}
