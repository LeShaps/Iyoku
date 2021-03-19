using System;
using System.Collections.Generic;
using System.Text;

namespace Iyoku.Extensions
{
    public static class StringExtensions
    {
        public static bool Is(this string Compare, params string[] Comparaisons)
        {
            foreach (string comp in Comparaisons)
            {
                if (Compare == comp)
                    return true;
            }

            return false;
        }
    }
}
