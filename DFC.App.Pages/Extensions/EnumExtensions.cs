using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace DFC.App.Pages.Extensions
{
    [ExcludeFromCodeCoverage]
    public static class EnumExtensions
    {
        public static string GetDescription<T>(this T e)
            where T : IConvertible
        {
            Type genericEnumType = e.GetType();
            MemberInfo[] memberInfo = genericEnumType.GetMember($"{e}");
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
                if (attribs != null && attribs.Length > 0)
                {
                    return ((System.ComponentModel.DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }

            return $"{e}";
        }
    }
}
