using System;

namespace filter_aot_profile
{
    class Program
    {
        public static void FilterAOTProfile(string profilePath, string[] excluded, bool dump)
        {
            AotProfileFilter filter = new AotProfileFilter(profilePath, "filtered.aotprofile", excluded, dump);
            filter.Execute();
        }

        public static void ProfileToJson()
        {
            AotProfileToJson json = new AotProfileToJson("data.aotprofile", "data.json");
            json.Execute();
        }
        static void Main(string[] args)
        {
            FilterAOTProfile("aot.profile", null, true);
        }
    }
}
