#region

using System;
using System.Collections.Generic;
using System.Linq;
using PinnaRent.Core.Common;
using PinnaRent.Core.Enumerations;
using PinnaRent.Core.Extensions;
using PinnaRent.Core.Models;

#endregion

namespace PinnaRent.Core
{
    public static class CommonUtility
    {
        public static string Encrypt(string stringToEncrypt)
        {
            var x = new System.Security.Cryptography.MD5CryptoServiceProvider();
            var data = System.Text.Encoding.ASCII.GetBytes(stringToEncrypt);
            data = x.ComputeHash(data);
            var md5Hash = System.Text.Encoding.ASCII.GetString(data);

            return md5Hash;
        }

        public static IList<RoleDTO> GetRolesList()
        {
            return Enum.GetNames(typeof (RoleTypes))
                .Select(name => (RoleTypes) Enum.Parse(typeof (RoleTypes), name))
                .Select(GetRoleDTO).ToList();
        }

        public static RoleDTO GetRoleDTO(RoleTypes roleType)
        {
            var role = new RoleDTO
            {
                RoleName = roleType.ToString(),
                RoleDescription = EnumUtil.GetEnumDesc(roleType),
                RoleDescriptionShort = roleType.ToString()
            };
            return role;
        }

        public static bool UserHasRole(RoleTypes role) //int userId,
        {
            return Singleton.User.Roles.Any(u => u.Role.RoleName == role.ToString());
        }

        public static IList<ListDataItem> GetList(Type enumType)
        {
            var enumList = new List<ListDataItem>();

            var enumTypes = Enum.GetNames(enumType);
            foreach (var staffType in enumTypes)
            {
                var staffTy = new ListDataItem
                {
                    Display = EnumUtil.GetEnumDesc(Enum.Parse(enumType, staffType)),
                    Value = Convert.ToInt32(Enum.Parse(enumType, staffType))
                };
                enumList.Add(staffTy);
            }

            return enumList;
        }

        public static AddressDTO GetDefaultAddress()
        {
            return new AddressDTO()
            {
                Country = "Ethiopia",
                City = "Addis Abeba"
            };
        }

        public static string GetNumberInWords(string num, bool toEnglish)
        {
            //num = num.Replace(',', ' ');
            
            string mone = "ብር", cen = "ሣንቲም", with = " ከ";
            if (toEnglish)
            {
                mone = "Birr";
                cen = "Cents";
                with = " and ";
            }
            string a, b = "";
            if (num.Contains('.'))
            {
                var mon = num.Split('.')[0];
                var cent = num.Split('.')[1];
                if (mon.Length > 15)
                    return "";
                a = GetNumberDescription(mon, toEnglish) + mone;

                if (cent.Length < 2)
                    cent = cent + "0";
                else if (cent.Length > 2)
                    cent = cent.Substring(0, 2);
                if (string.IsNullOrWhiteSpace(mon) || string.IsNullOrEmpty(mon) || Convert.ToInt32(mon.ToString()) == 0)
                {
                    a = "";
                    with = "";
                }
                if (Convert.ToInt32(cent) > 0)
                    b = with + GetNumberDescription(cent, toEnglish) + cen;
            }
            else a = GetNumberDescription(num, toEnglish) + "ብር";

            var c = a + b;
            return c;
        }

        private static string GetNumberDescription(string num, bool toEnglish)
        {
            string number = num.ToString(), text = "";
            var len = number.Length;

            int i = 0;
            while (i < len)
            {
                var words = toEnglish
                    ? GetWordsInEnglish(number[i], len - i, number)
                    : GetWordsInAmharic(number[i], len - i, number);
                if (!string.IsNullOrWhiteSpace(words))
                    words = words + " ";

                text = text + words; // GetWordsInAmharic(number[i],len-i,number)+" ";
                i++;
            }

            return text;
        }

        private static string GetWordsInAmharic(char num, int loc, string allnumber)
        {
            int num2;
            int len = allnumber.Length;
            int locindex = len - loc;
            int.TryParse(num.ToString(), out num2);
            string[] amhmonths = {"", "አንድ", "ሁለት", "ሶስት", "አራት", "አምስት", "ስድስት", "ሰባት", "ስምንት", "ዘጠኝ"};
            string[] amhmonths2 = {"", "አስር", "ሀያ", "ሰላሳ", "አርባ", "አምሳ", "ስድሳ", "ሰባ", "ሰማኒያ", "ዘጠና"};
            string[] amhmonths3 = {"መቶ", "ሺህ", "ሚሊዮን", "ቢሊዮን", "ትርሊዮን"};

            if (loc%3 == 0)
            {
                if (Convert.ToInt32(allnumber.Substring(locindex, 1)) > 0)
                    return amhmonths[num2] + " መቶ";
            }
            if (loc%3 == 2)
            {
                if (num2 == 1)
                    if (Convert.ToInt32(allnumber.Substring(locindex + 1, 1)) > 0)
                        return "አስራ";
                return amhmonths2[num2];
            }
            if (loc%3 == 1)
            {
                if (loc < 3)
                    return amhmonths[num2];
                else if ((locindex - 2 >= 0) && Convert.ToInt32(allnumber.Substring(locindex - 2, 3)) == 0)
                    return "";
                return amhmonths[num2] + " " + amhmonths3[loc/3];
            }
            return amhmonths[num2];
        }

        private static string GetWordsInEnglish(char num, int loc, string allnumber)
        {
            int num2;
            int len = allnumber.Length;
            int locindex = len - loc;
            int.TryParse(num.ToString(), out num2);
            string[] amhmonths = {"", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"};
            string[] amhmonths2 =
            {
                "", "Ten", "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty",
                "Ninty"
            };
            string[] amhmonths3 = {"Hundred", "Thousand", "Million", "Billion", "Trilion"};
            string[] amhmonths4 =
            {
                "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen",
                "Eighteen", "Nineteen"
            };

            if (loc%3 == 0)
            {
                if (Convert.ToInt32(allnumber.Substring(locindex, 1)) > 0)
                    return amhmonths[num2] + " Hundred";
            }
            if (loc%3 == 2)
            {
                if (num2 == 1)
                    return "";

                return amhmonths2[num2];
            }
            if (loc%3 == 1)
            {
                if ((locindex - 1 >= 0) && Convert.ToInt32(allnumber.Substring(locindex - 1, 1)) == 1)
                    if (loc > 3)
                        return amhmonths4[num2] + " " + amhmonths3[loc/3];
                    else return amhmonths4[num2];

                if (loc < 3)
                    return amhmonths[num2];
                else if ((locindex - 2 >= 0) && Convert.ToInt32(allnumber.Substring(locindex - 2, 3)) == 0)
                    return "";

                return amhmonths[num2] + " " + amhmonths3[loc/3];
            }
            return amhmonths[num2];
        }

        public static int GetMonthsFromDays(DateTime beginDate,DateTime endDate)
        {
            if (beginDate.Year < 1990 || endDate.Year < 1990)
                return 0;
            var fromDay = new DateTime(beginDate.Year, beginDate.Month, beginDate.Day, 23, 59, 59);
            var toDay = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);

            var overDueDays = toDay.Subtract(fromDay).Days;

            if (overDueDays < 0)
                return 0;

            var overdueMonths = overDueDays / 30;
            overdueMonths += 1;

            return overdueMonths;
        }

        public static string GetNumberInWordsWithNumber(string num, bool toEnglish,bool isMoney)
        {
            var words = GetNumberInWords(num, toEnglish);
            if (!isMoney)
                words = words.Replace(" ብር", "").Replace(" ሣንቲም", "").Replace("ከ", "ነጥብ");
            return num + "/" + words + "/";
        }

        public static string GetNumberInWordsWithNumber(string num, bool toEnglish)
        {
            int numb = Convert.ToInt32(num);
            if ( numb< 12)
                return GetNumberInWordsWithNumber(num, toEnglish, false) + " ወር";
            else
            {
                return "1/አንድ/" + " አመት";
            }
        }
    }
}