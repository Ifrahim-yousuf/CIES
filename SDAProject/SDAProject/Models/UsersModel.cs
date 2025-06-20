﻿using System.ComponentModel.DataAnnotations;

namespace SDAProject.Models
{
    public class UsersModel
    {
        [Key]
        public int UserID { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int DepartmentID { get; set; }
    }
}
