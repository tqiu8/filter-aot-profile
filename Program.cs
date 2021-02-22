using System;

namespace filter_aot_profile
{
    class Program
    {
        public static void FilterAOTProfile(string profilePath, string newProfilePath, string[] excluded, bool dump)
        {
            AotProfileFilter filter = new AotProfileFilter(profilePath, newProfilePath, excluded, dump);
            filter.Execute();
        }

        public static void ProfileToJson(string input, string output)
        {
            AotProfileToJson json = new AotProfileToJson(input, output);
            json.Execute();
        }

        public static void JsonToProfile(string input, string output, string methodsPath)
        {
            JsonToAotProfile profile = new JsonToAotProfile(input, output);
            profile.Execute(methodsPath, 20);
        }

        static void Main(string[] args)
        {
            // FilterAOTProfile("aot.profile", "filtered.profile", null, true);
            // ProfileToJson("aot.profile", "data.json");
            JsonToProfile("data.json", "trimmed.profile", "methods.txt");
        }
    }
}
