using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BWBinding.Interfaces
{
    interface IListResultHandler
    {
        void Result(string result);
        void finish();
    }
}
