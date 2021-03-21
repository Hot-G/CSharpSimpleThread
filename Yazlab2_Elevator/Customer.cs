using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yazlab2_Elevator
{
    public class Customer
    {

        public int Target_Floor { get; set; }

        public Customer(int target)
        {
            Target_Floor = target;
        }
    }
}
