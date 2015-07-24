using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSSQLElasticSearchRiver.RiverService
{
    public class RiverConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("elasticSearchConnectionString")]
        public string ElasticSearchConnectionString { get { return (string)base["elasticSearchConnectionString"]; } }

        [ConfigurationProperty("rivers")]
        public RiverElementCollection Rivers { get { return (RiverElementCollection)base["rivers"]; } }
    }

    public class RiverElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
 	        return new RiverElement();
        }

        public RiverElement this[int index]
        {
            get { return (RiverElement)base.BaseGet(index); }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                base.BaseAdd(index, value);
            }
        }

        //The key is the Table + Index combination. Never used, but inheritance requires it. 
        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RiverElement).DatabaseTable + "-" + (element as RiverElement).ElasticIndex;
        }
    }

    public class RiverElement : ConfigurationElement
    {
        [ConfigurationProperty("databaseTable", IsRequired=true)]
        public string DatabaseTable { get { return (string)base["databaseTable"]; } }

        [ConfigurationProperty("tableId", IsRequired=true)]
        public string tableId { get { return (string)base["tableId"]; } }

        [ConfigurationProperty("elasticIndex", IsRequired=true)]
        public string ElasticIndex { get { return (string)base["elasticIndex"]; } }

        [ConfigurationProperty("elasticType", IsRequired = true)]
        public string ElasticType { get { return (string)base["elasticType"]; } }
    }
}
