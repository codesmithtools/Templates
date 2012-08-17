using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace CodeSmith.Data.Linq
{
    public static class XElementExtensions
    {
        /// <summary>
        /// Converts a string to an XElement safely.  If the string is null or empty, null is returned.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static XElement AsXElement(this string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            try
            {
                XElement element = XElement.Parse(value);
                return element;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Converts an XElement to a string safely. If element is null or invaild, null is returned.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        public static string AsString(this XElement element)
        {
            if (element == null)
                return null;

            try
            {
                return element.ToString();
            }
            catch
            {
                return null;
            }
        }
    }
}
