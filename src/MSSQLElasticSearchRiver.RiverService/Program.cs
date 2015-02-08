using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using Nest;
using Elasticsearch.Net;
using System.Xml;
using System.Xml.Serialization;

namespace MSSQLElasticSearchRiver.RiverService
{
    class Program
    {
        static void Main(string[] args)
        {

            RiverConfiguration _riverConfiguration = (RiverConfiguration)ConfigurationManager.GetSection("riverConfiguration");

            ConnectionSettings searchSettings = new ConnectionSettings(new Uri(_riverConfiguration.ElasticSearchConnectionString));
            var searchClient =  new ElasticsearchClient(searchSettings);

            while (true)
            {
                //Connect to DB and listen for a message. 
                using (SqlConnection sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["queueDatabase"].ConnectionString))
                {
                    sqlConnection.Open();

                    SqlCommand receiveCommand = new SqlCommand("WAITFOR( RECEIVE TOP(1) CONVERT(XML, message_body) AS Message FROM RiverUpdateReceiveQueue )", sqlConnection);
                    SqlCommand retrieveDataCommand;

                    RiverMessage riverMessage;
                    using (SqlDataReader receiveCommandReader = receiveCommand.ExecuteReader())
                    {
                        //Deserialize message. 
                        receiveCommandReader.Read();
                        XmlSerializer serializer = new XmlSerializer(typeof(RiverMessage));
                        riverMessage = (RiverMessage)serializer.Deserialize(receiveCommandReader.GetXmlReader(0));

                        //Get the entire record out of the DB. 
                        retrieveDataCommand = new SqlCommand(string.Format("SELECT TOP(1) * FROM {0} WHERE Id = {1}", riverMessage.DatabaseTable, riverMessage.Id), sqlConnection);
                    }

                    using (SqlDataReader retrieveDataCommandReader = retrieveDataCommand.ExecuteReader())
                    {
                        JObject item = null;
                        //Read it from the DB and store in a simple JSON object. 

                        if (retrieveDataCommandReader.Read())
                        {
                            item = new JObject();
                            for (int i = 0; i < retrieveDataCommandReader.FieldCount; i++)
                            {
                                item[retrieveDataCommandReader.GetName(i)] = new JValue(retrieveDataCommandReader.GetValue(i));
                            }
                        }

                        //Foreach river that wants this type of record. Send it. 
                        foreach (var riverRequired in _riverConfiguration.Rivers.Cast<RiverElement>().Where(x => x.DatabaseTable == riverMessage.DatabaseTable))
                        {
                            if (item != null)
                                searchClient.Index(riverRequired.ElasticIndex, riverRequired.ElasticType, riverMessage.Id.ToString(), item.ToString());
                            else
                                searchClient.Delete(riverRequired.ElasticIndex, riverRequired.ElasticType, riverMessage.Id.ToString());
                        }
                    }
                }
            }
        }
    }
}
