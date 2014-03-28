using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class UIDRange
    {

        private long left_uid;
        private long right_uid;

        public UIDRange(long left_uid, long right_uid)
        {
            this.left_uid = left_uid;
            this.right_uid = right_uid;
        }

        public bool UIDInRange(long uid)
        {

            if (uid >= left_uid && uid <= right_uid)
            {
                return true;
            }
            else 
            {
                return false;
            }
        
        }

        public void PrintRange() 
        {
            Console.WriteLine("[ " + left_uid + " - " + right_uid + " ]");
        }


    }
}
