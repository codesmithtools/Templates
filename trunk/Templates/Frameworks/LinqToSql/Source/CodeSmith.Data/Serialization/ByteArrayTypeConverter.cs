using System;
using System.ComponentModel;
using System.Data.Linq;

namespace CodeSmith.Data.Serialization
{
    /// <summary>
    /// Implements a byte array <see cref="TypeConverter"/>.
    /// </summary>
    /// <remarks>
    /// The converter supports an array of <see cref="T:System.Byte"/> and <see cref="T:System.Data.Linq.Binary"/>.
    /// </remarks>
    public class ByteArrayTypeConverter : TypeConverter
    {
        /// <summary>
        /// Returns whether this converter can convert an object of the given type to the type of this converter, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you want to convert from.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
                return true;
            if (sourceType == typeof(long))
                return true;

            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Returns whether this converter can convert the object to the specified type, using the specified context.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="destinationType">A <see cref="T:System.Type"/> that represents the type you want to convert to.</param>
        /// <returns>
        /// true if this converter can perform the conversion; otherwise, false.
        /// </returns>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
                return true;
            if (destinationType == typeof(long))
                return true;

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        /// Converts the given object to the type of this converter, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">The <see cref="T:System.Globalization.CultureInfo"/> to use as the current culture.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            byte[] bytes;

            if (value is string)
                bytes = Convert.FromBase64String((string)value);
            else if (value is long)
                bytes = BitConverter.GetBytes((long)value);
            else
                return base.ConvertFrom(context, culture, value);

            return new Binary(bytes);
        }

        /// <summary>
        /// Converts the given value object to the specified type, using the specified context and culture information.
        /// </summary>
        /// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
        /// <param name="culture">A <see cref="T:System.Globalization.CultureInfo"/>. If null is passed, the current culture is assumed.</param>
        /// <param name="value">The <see cref="T:System.Object"/> to convert.</param>
        /// <param name="destinationType">The <see cref="T:System.Type"/> to convert the <paramref name="value"/> parameter to.</param>
        /// <returns>
        /// An <see cref="T:System.Object"/> that represents the converted value.
        /// </returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="destinationType"/> parameter is null. </exception>
        /// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            Binary binary = value as Binary;
            if (binary == null)
                return null;

            byte[] bytes = binary.ToArray();

            if (destinationType == typeof(string))
                return Convert.ToBase64String(bytes, 0, bytes.Length);
            if (destinationType == typeof(long))
                return BitConverter.ToInt64(bytes, 0);

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
