using System;
using Mono.Options;
using System.IO;

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
            string profilePath = null;
            string methodList = "methods.txt";
            string jsonPath = null;
            string trimmedPath = null;

            var optionsParser = new OptionSet() {
                { "d|dump", "dump all methdods", v => { dump = v !=null; }},
                { "b|batch=", "batch size of fuzzed profiles", v => { batch = Int32.Parse(v); }},
                { "f|fuzz", "fuzz input profile", v => { fuzz = v != null; }},
                { "p|profilepath=", "path to profile", v => { profilePath = v; }},
                { "m|methodlist=", "list of methods dumped from profile", v => { methodList = v; }},
                { "j|jsonpath=", "path to profile json file", v => { jsonPath = v; }},
                { "t|trimmedpath=", "path of directory for trimmed profiles", v => { trimmedPath = v; }}
            };

            if (args.Length > 0) {
                optionsParser.Parse (args);
            }

            if (profilePath == null) {
                Console.WriteLine("Please enter a profile path");
                return 1;
            }
            if (dump) {
                Console.WriteLine($"Dumping methods from {profilePath} to {methodList}");
                FilterAOTProfile(profilePath, null, null, true, methodList);
            }
            if (fuzz) {
                if (jsonPath != null) {
                    ProfileToJson(profilePath, jsonPath);
                    if (trimmedPath == null) {
                        trimmedPath = "trimmed-profiles";
                    }
                    JsonToProfile(jsonPath, trimmedPath, methodList, batch);
                }
                else {
                    Console.WriteLine("Please enter the path to the profile json file");
                }
            }
            return 0;
        }
    }
}
