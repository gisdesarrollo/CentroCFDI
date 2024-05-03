using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aplicacion.Utilidades
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            FieldInfo field = value.GetType().GetField(value.ToString());

            DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute == null ? value.ToString() : attribute.Description;
        }

        public static IEnumerable<KeyValuePair<int, string>> GetAllValuesAndDescriptions(Type t)
        {
            if (!t.IsEnum)
                throw new ArgumentException($"{nameof(t)} must be an enum type");

            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var descriptionAttribute = field.GetCustomAttribute<DescriptionAttribute>();
                int value = (int)field.GetValue(null);
                string description = descriptionAttribute != null ? descriptionAttribute.Description : field.Name;
                yield return new KeyValuePair<int, string>(value, description);
            }
        }
    }
}
