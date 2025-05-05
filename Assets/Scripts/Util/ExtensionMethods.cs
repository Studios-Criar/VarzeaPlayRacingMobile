using System;
using System.Linq;
using UnityEngine;

namespace Util
{
    public static class ExtensionMethods
    {
        public static bool EqualsAny(this string s, StringComparison comparison, bool trim, params string[] values)
        {
            return values.Any(si => (trim ? s.Trim() : s).Equals(trim ? si.Trim() : si, comparison));
        }

        public static Sprite ToSprite(this Texture2D texture2D)
        {
            var pivot = new Vector2(.5f, .5f);
            var rect = new Rect(0f, 0f, texture2D.width, texture2D.height);
            return Sprite.Create(texture2D, rect, pivot, 100f);
        }
    }
}