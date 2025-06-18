namespace SDAProject.Models
{
    public class EmployessDetailModel
    {
        public Guid DeviceID { get; set; }
        public string EmpiName { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
        public string DepartmentName { get; set; }
        public string ManagerName { get; set; }
        public string DesktopName { get; set; }
        public string OSVersion { get; set; }
        public string RamSize { get; set; }
        public string MacAddress { get; set; }
        public int ViolationCount { get; set; }
        public int Score { get; set; }
        public string KeylogData { get; set; }
    }
}
