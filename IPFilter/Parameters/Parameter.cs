using System.Net;
using System.Xml.Serialization;

namespace IPFilter.Parameters
{
    [Serializable]
    public class Parameter
    {
        public string LogPath { get; set; }
        public string OutputPath { get; set; }
        [XmlIgnore]
        public IPAddress AddressStart { get; set; } = new IPAddress(0);
        [XmlIgnore]
        public IPAddress AddressMask { get; set; } = new IPAddress(4294967295);
        public DateTime? StartTime { get; set; } = null;
        public DateTime? EndTime { get; set; } = null;
        [XmlIgnore]
        public bool NeedSave { get; set; } = false;

        public bool IsFull()
        {
            return LogPath != null && OutputPath != null && AddressStart != null &&
                   AddressMask != null && StartTime != null && EndTime != null;
        }
    }
}
