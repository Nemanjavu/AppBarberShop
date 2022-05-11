using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarberShopClient
{
    public class Barber
    {
        public int BarberId { get; set; }
        public string BarberName { get; set; }

        public override string ToString()
        {
            return "Id: " + BarberId + "\tName: " + BarberName;
        }

    }
}
