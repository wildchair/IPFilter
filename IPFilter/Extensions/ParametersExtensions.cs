using IPFilter.Parameters;
using System.Xml.Serialization;

namespace IPFilter.Extensions
{
    public static class ParametersExtensions
    {
        public static void SaveConfig(this Parameter parameters, FileStream stream = null)
        {
            var _needToClose = false;

            if (stream == null)
            {
                stream = new("config.xml", FileMode.OpenOrCreate);
                _needToClose = true;
            }

            var serializer = new XmlSerializer(typeof(Parameter));
            serializer.Serialize(stream, parameters);
            if (_needToClose)
            {
                stream.Close();
                stream.Dispose();
            }
        }
    }
}