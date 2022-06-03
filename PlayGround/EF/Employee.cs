namespace PlayGround.EF
{
    public class Employee
    {
        public long ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? Birthdate { get; set; }

        public long? DepartmentId { get; set; }

        public virtual Department? Department { get; set; }
    }
}
