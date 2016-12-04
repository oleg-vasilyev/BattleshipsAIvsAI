using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BattleshipsAIvsAI.Models
{
    static class Serializer
    {
        public static void SaveToBinnary<T>(string FileName, T SerializableObject)
        {
            string path = Directory.GetCurrentDirectory() + "\\Data";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using (FileStream fs = File.Create(path + "\\" + FileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, SerializableObject);
            }
        }

        public static T LoadFromBinnary<T>(string FileName)
        {
            using (FileStream fs = File.Open(FileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }
    }
}
