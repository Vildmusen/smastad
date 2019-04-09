using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Småstad.Models
{
    ///<summary> Public class of an employee. </summary> 
    public class Employee
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RoleTitle { get; set; }
        public string DepartmentId { get; set; }
    }
}
