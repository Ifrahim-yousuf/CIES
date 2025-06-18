using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SDAProject.Models
{
    [Table("Employess")]
    public class RegistrationModel
    {
        public Guid DeviceID { get; set; }

        public string EmpiName { get; set; }

        public string Address { get; set; }

        public string ContactNo { get; set; }

        public string DepartName { get; set; }


    }
}
