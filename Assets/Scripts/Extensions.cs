using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class Extensions
{
    public static bool InRange(this float inp, float min, float max)
    {
        if (inp < min)
            return false;
        if (inp > max)
            return false;
        return true;
    }
}

