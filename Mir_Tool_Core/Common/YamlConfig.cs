using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Mir_Utilities.Common;

public class YamlConfig
{
    private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
    
    public static T? GetConfigFromFile<T>(string filePath) where T : class
    {
        if (File.Exists(filePath))
        {
            string yamlString = ReadFile(filePath);
            try
            {
                var deserializer = new DeserializerBuilder()
                    .Build();
                return deserializer.Deserialize<T>(yamlString);
            }
            catch (YamlException yamlEx)
            {
                logger.Error($"Failed to deserialize yaml file: {filePath}, error: {yamlEx.Message}");
                return null;
            }
        }
        logger.Info($"File not found: {filePath}");
        return null;
    }

    private static string ReadFile(string path)
    {
        string readContents;
        using (StreamReader streamReader = new StreamReader(path, Encoding.UTF8))
        {
            readContents = streamReader.ReadToEnd();
        }
        return readContents;
    }
}