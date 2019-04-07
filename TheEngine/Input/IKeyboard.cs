using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheEngine.Input
{
    public interface IKeyboard
    {
        bool IsDown(System.Windows.Forms.Keys keys);
    }
}
