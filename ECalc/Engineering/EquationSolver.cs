﻿using System;
using System.Collections.Generic;
using System.Numerics;

namespace ECalc.Engineering
{
    class EquationSolver
    {
        public object Quadratic(double a, double b, double c)
        {
            if (a == 0)
            {
                if (b == 0)
                {
                    if (c == 0) return 0;
                    else return double.NaN;
                }
                return -c / b;
            }
            else
            {
                double d = (b * b) - 4 * a * c;
                if (d < 0)
                {
                    var dc = new Complex(d, 0);
                    var ret = new Dictionary<string, Complex>();
                    ret.Add("x1", (-b - Complex.Sqrt(dc)) / 2 * a);
                    ret.Add("x2", (-b + Complex.Sqrt(dc)) / 2 * a);
                    return ret;
                }
                else if (d == 0) return -b / 2 * a;
                else
                {
                    var ret = new Dictionary<string, double>();
                    ret.Add("x1", (-b - Math.Sqrt(d)) / 2 * a);
                    ret.Add("x2", (-b + Math.Sqrt(d)) / 2 * a);
                    return ret;
                }
            }
        }
    }
}
