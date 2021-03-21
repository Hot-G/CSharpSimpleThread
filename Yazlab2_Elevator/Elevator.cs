using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yazlab2_Elevator
{
    public class Elevator
    {
        private const int ELEVATOR_CAPACITY = 10;

        public int GetCustomerNumber()
        {
            return customers.Count;
        }

        public int GetEmptySlot()
        {
            return ELEVATOR_CAPACITY - customers.Count;
        }

        public List<Customer> customers = new List<Customer>();

        public int CurrentFloor { get; set; } = 0;

        public int Direction { get; set; } = 1;
    }
}
