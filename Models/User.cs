using System;
using System.ComponentModel.DataAnnotations;

namespace PersonalSite.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAuthorized { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreationDate { get; set; }
    }
}
