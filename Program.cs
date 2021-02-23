using System;
using Mono.Options;

namespace filter_aot_profile
{
    class Program
    {
        public static void FilterAOTProfile(string profilePath, string newProfilePath, string[] excluded, bool dump, string methodPath)
        {
            AotProfileFilter filter = new AotProfileFilter(profilePath, newProfilePath, excluded, dump);
            filter.Execute(methodPath);
        }

        public static void ProfileToJson(string input, string output)
        {
            AotProfileToJson json = new AotProfileToJson(input, output);
            json.Execute();
        }

        public static void JsonToProfile(string input, string output, string methodsPath, int batch)
        {
            JsonToAotProfile profile = new JsonToAotProfile(input, output);
            profile.Execute(methodsPath, batch);
        }

        static int Main(string[] args)
        {
            // JsonToProfile("data.json", "trimmed.profile", "methods.txt");

            bool dump = false;
            bool fuzz = false;
            int batch = 10;
            string profilePath = "";
            string methodList = "methods.txt";
            string jsonPath = "data.json";

            var optionsParser = new OptionSet() {
                { "d|dump", "dump all methdods", v => { dump = v !=null; }},
                { "b|batch", "batch size of fuzzed profiles", v => { batch = Int32.Parse(v); }},
                { "f|fuzz", "fuzz input profile", v => { fuzz = v != null; }},
                { "p|profilePath", "path to profile", v => { profilePath = v; }},
                { "m|methodList", "list of methods dumped from profile", v => { methodList = v; }},
                { "j|jsonPath", "path to profile json file", v => { jsonPath = v; }}
            };

            if (args.Length > 0)
                optionsParser.Parse (args);

            if (profilePath != "") {
                Console.WriteLine("Please enter a profile path");
                return 1;
            }
            if (dump)
                FilterAOTProfile(profilePath, null, null, true, methodList);

            if (fuzz) {
                if (jsonPath != "") {
                    ProfileToJson(profilePath, jsonPath);
                    JsonToProfile(jsonPath, "trimmed-profiles/trimmed.profile", methodList, batch);
                }
                else {
                    Console.WriteLine("Please enter the path to the profile json file");
                }
            }
            return 0;
        }
    }
}
