using Newtonsoft.Json;
using Oar_Audio.Model;

namespace Oar_Audio.Utilities
{
    class JsonUtil
    {
        public static void WriteToSources(List<Source> data)
        {
            // Convert the list of Source objects to JSON
            var sources = new List<Dictionary<string, object>>();
            foreach (var obj in data)
            {
                sources.Add(obj.ToDictionary());
            }

            // Serialize to JSON with indentation
            string json = JsonConvert.SerializeObject(sources, Newtonsoft.Json.Formatting.Indented);

            // Write JSON to the file
            System.IO.File.WriteAllText(@"Resources/conf/sources_conf.json", json);
        }   
   
        public static void WriteToVolumes(List<Volume> data)
        {
            {
                // Convert the list of Source objects to JSON
                var volumes = new List<Dictionary<string, object>>();
                foreach (var obj in data)
                {
                    volumes.Add(obj.ToDictionary());
                }

                // Serialize to JSON with indentation
                string json = JsonConvert.SerializeObject(volumes, Newtonsoft.Json.Formatting.Indented);

                // Write JSON to the file
                System.IO.File.WriteAllText(@"Resources/conf/volumes_conf.json", json);
            }
        }

        public static List<Source> GetSources()
        {
            // Open and read the JSON file
            string jsonData = System.IO.File.ReadAllText(@"Resources/conf/sources_conf.json");


            // Deserialize JSON data into a list of Source objects
            List<Source> sources = JsonConvert.DeserializeObject<List<Source>>(jsonData);
            return sources;   
        }
    
        public static List<Volume> GetVolumes()
        {
            // Open and read the JSON file
            string jsonData = System.IO.File.ReadAllText(@"Resources/conf/volumes_conf.json");

            // Deserialize JSON data into a list of Source objects
            List<Volume> volumes = JsonConvert.DeserializeObject<List<Volume>>(jsonData);

            return volumes;
        }
    }
}
