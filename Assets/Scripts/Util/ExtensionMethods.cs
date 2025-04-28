using System;
using System.Linq;

namespace Util
{
    public static class ExtensionMethods
    {
        public static bool EqualsAny(this string s, StringComparison comparison, bool trim, params string[] values)
        {
            return values.Any(si => (trim ? s.Trim() : s).Equals(trim ? si.Trim() : si, comparison));
        }
    }
}