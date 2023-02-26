
using System.Collections.Generic;

namespace Tokket.Core
{
    public static class Roles
    {
        public const string Level0 = "level0";

        public const string Level1 = "level1";

        public const string Level2 = "level2";

        public const string Level3 = "level3";

        public const string None = "none";

        public static List<string> ValidRoles = new List<string>() { Level0, Level1, Level2, Level3 };

        public static string GetRoleProperName(string role)
        {
            if (role == Level0)
                return "Super Admin";
            else if (role == Level1)
                return "Lead";
            else if (role == Level2)
                return "Group";
            else if (role == Level3)
                return "Associate";
            return "";
        }

        public static Dictionary<string, string> RolesList { get; set; } = new Dictionary<string, string>()
    {
        { Roles.Level1, Roles.GetRoleProperName(Roles.Level1) },
        { Roles.Level2, Roles.GetRoleProperName(Roles.Level2) },
        { Roles.Level3, Roles.GetRoleProperName(Roles.Level3) },
    };
    }
}