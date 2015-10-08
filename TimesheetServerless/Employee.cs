using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimesheetServerless
{
    public class Employee : IComparable<Employee>
    {
        //Properties
        public int EmployeeID {get; set;}
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string StartingDate { get; set; }

        public Employee() { }

        public int CompareTo(Employee otherEmployee)
        {
            //Reverse CompareTo order (low to high)
            return this.EmployeeID.CompareTo(otherEmployee.EmployeeID);
        }


        /*
         * Equals and GetHashCode         
         */
        public override bool Equals(Object obj)
        {
            if(obj == null || (Employee)obj == null)
                return false;
            
            Employee temp = (Employee)obj;
            return this.EmployeeID == temp.EmployeeID
                && this.FirstName == temp.FirstName
                && this.LastName == temp.LastName
                && this.Department == temp.Department
                && this.Phone == temp.Phone
                && this.Email == temp.Email
                && this.StartingDate == temp.StartingDate;            
        }


        //Hash code: employee unique number
        public override int GetHashCode()
        {
            return EmployeeID.GetHashCode();
        }


        //Operators overload
        //==
        public static bool operator==(Employee a1, Employee a2)
        {
			if ((Object)a1 == (Object)a2) return true;
			else if ((Object)a1 == null) return false;
			else if ((Object)a2 == null) return false;
			else return a1.Equals(a2);
        }

        //!=
        public static bool operator !=(Employee a1, Employee a2)
        {
			if ((Object)a1 != (Object)a2) return true;
			else if ((Object)a1 != null) return false;
			else if ((Object)a2 != null) return false;
			else return a1.Equals(a2);
        }
    }
}
