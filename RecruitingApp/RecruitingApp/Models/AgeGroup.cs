using SQLite;
using System;

namespace RecruitingApp.Models
{
    public class AgeGroup
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }


        public string Name { get { return Year.Year.ToString(); } }

        public DateTime Year { get; set; }
            
        public DateTime Created { get; set; }

        public DateTime LastUpdated { get; set; }
       
        public AgeGroup()
        {

        }
    }
}
