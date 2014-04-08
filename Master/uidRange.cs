using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master
{
    class UIDRange
    {

        private int left_uid;
        private int right_uid;

        public UIDRange(int left_uid, int right_uid)
        {
            this.left_uid = left_uid;
            this.right_uid = right_uid;
        }

        public bool UIDInRange(int uid)
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
        public UIDRange Split()
        {

            int range_size = (int)(right_uid - left_uid) + 1;
            int new_left_uid = (int)left_uid;
            int new_right_uid = left_uid + (int)(range_size / 2) - 1;

            //Update referencia
            left_uid = left_uid + range_size / 2;
            //right_uid = right_uid;

            return new UIDRange(new_left_uid, new_right_uid);

        }

        public string ToString()
        {
            return "[ " + left_uid + " , " + right_uid + " ]";
        }

    }
}