using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MSSQLElasticSearchRiver.RiverService
{
    [XmlRoot("RiverMessage")]
    public class RiverMessage
    {
        [XmlElement("Id")]
        public int Id { get; set; }

        [XmlElement("DatabaseTable")]
        public string DatabaseTable { get; set; }
    }
}
