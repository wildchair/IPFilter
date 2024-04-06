using System.Net;
using System.Xml.Serialization;

namespace IPFilter.Parameters
{
    public static class ParameterParser
    {
        /// <summary>
        /// Параметры, не введенные в качестве параметров командной строки будут получены из файла конфигурации.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="UnknownParameterException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="FormatException"></exception>
        public static Parameter ParseFromArgs(string[] args)
        {
            var argsList = args.ToList();
            argsList.RemoveAll(a => a == string.Empty || a == " ");

            Parameter parameters = null;
            try
            {
                parameters = ParseFromXml();
            }
            catch (Exception e)
            {
                parameters = null;
            }
            bool _save = false;

            if (parameters == null)
                parameters = new Parameter();

            for (int i = 0; i < argsList.Count; i += 2)
            {
                switch (argsList[i])
                {
                    case "--file-log":
                        parameters.LogPath = argsList[i + 1];
                        break;
                    case "--file-output":
                        parameters.OutputPath = argsList[i + 1];
                        break;
                    case "--address-start":
                        parameters.AddressStart = IPAddress.Parse(argsList[i + 1]);
                        break;
                    case "--address-mask":
                        parameters.AddressMask = IPAddress.Parse(argsList[i + 1]);
                        break;
                    case "--time-start":
                        parameters.StartTime = DateTime.Parse(argsList[i + 1]);
                        break;
                    case "--time-end":
                        parameters.EndTime = DateTime.Parse(argsList[i + 1]);
                        break;
                    case "--save-conf":
                        parameters.NeedSave = bool.Parse(argsList[i + 1]);
                        break;
                    default:
                        throw new UnknownParameterException($"Был введен неизвестный параметр: \"{argsList[i]}\"", argsList[i]);
                }
            }

            return parameters;
        }

        /// <param name="stream"></param>
        /// <returns>Возвращает null в случае неудачной десериализации.</returns>
        public static Parameter ParseFromXml(FileStream stream = null)
        {
            var _needToClose = false;

            if (stream == null)
            {
                stream = new("config.xml", FileMode.Open);
                _needToClose = true;
            }

            var serializer = new XmlSerializer(typeof(Parameter));
            var parameters = serializer.Deserialize(stream) as Parameter;

            if (_needToClose)
            {
                stream.Close();
                stream.Dispose();
            }

            return parameters;
        }

    }
}