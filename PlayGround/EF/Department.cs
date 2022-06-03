using System.ComponentModel.DataAnnotations.Schema;

namespace PlayGround.EF
{
    public class Department
    {
        public long ID { get; set; }
        public string Name { get; set; }

        [InverseProperty("Department")]
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
