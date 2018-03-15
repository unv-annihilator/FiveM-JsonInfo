using CitizenFX.Core.Native;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GHMatti.Data.XML
{
    public class Serializer
    {
        private static string resourceName = API.GetCurrentResourceName();

        public static Task<T> Deserialize<T>(string filename, Core.GHMattiTaskScheduler scheduler) => Task.Factory.StartNew(() =>
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(Path.Combine("resources", resourceName, "xml", filename));
            T res = (T)serializer.Deserialize(reader);
            reader.Close();
            return res;
        }, CancellationToken.None, TaskCreationOptions.None, scheduler);

        public static void SerializeJSON(string filename, object data)
        {
            File.WriteAllText(Path.Combine("resources", resourceName, "xml", "out", filename), JsonConvert.SerializeObject(data, Formatting.Indented), Encoding.UTF8);
        }
    }
}
