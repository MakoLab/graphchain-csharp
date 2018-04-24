using System;
using System.Collections.Generic;
using System.Linq;
using elephant.core.model.chain;
using elephant.core.model.p2p.message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace elephant.core.util
{
    public class PeerToPeerMessageConverter : JsonConverter
    {
        //private ILogger _logger;

        //public PeerToPeerMessageConverter(ILogger logger)
        //{
        //    _logger = logger;
        //}

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PeerToPeerMessage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var message = new PeerToPeerMessage();
            while (reader.Read())
            {
                if ("type".Equals(reader.Value))
                {
                    message.MessageType = (MessageType)reader.ReadAsInt32();
                }
                else if ("data".Equals(reader.Value))
                {
                    message.Data = HandleData(reader);
                }
                else if ("graph".Equals(reader.Value))
                {
                    message.Graph = HandleGraph(reader);
                }
                else
                {
                    //_logger.LogDebug("Unknown element of parsing JSON: '{0}'.", reader.Value);
                }
            }
            //var jObject = JObject.Load(reader);
            //if (jObject["type"] != null)
            //{
            //    message.MessageType = (MessageType)jObject["type"].Value<int>();
            //}
            //if (jObject["data"] != null && jObject["data"].Type == JTokenType.Array)
            //{
            //    message.Data = new List<BlockHeader>();
            //    foreach (var elem in jObject["data"].ToArray())
            //    {
            //        message.Data.Add(GetData(elem.Value<JObject>()));
            //    }
            //}
            //if (jObject["graph"] != null)
            //{
            //    message.Graph = GetGraph(jObject["graph"].Value<JObject>());
            //}
            return message;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var message = (PeerToPeerMessage)value;
            writer.WriteStartObject();
                writer.WritePropertyName("type");
                writer.WriteValue(message.MessageType);
                writer.WritePropertyName("data");
                writer.WriteStartArray();
                foreach (var header in message.Data)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("dataGraphIri");
                    writer.WriteValue(header.DataGraphIri);
                    writer.WritePropertyName("dataHash");
                    writer.WriteValue(header.DataHash);
                    writer.WritePropertyName("hash");
                    writer.WriteValue(header.Hash);
                    writer.WritePropertyName("index");
                    writer.WriteValue(header.Index);
                    writer.WritePropertyName("previousBlock");
                    writer.WriteValue(header.PreviousBlock);
                    writer.WritePropertyName("previousHash");
                    writer.WriteValue(header.PreviousHash);
                    writer.WritePropertyName("timestamp");
                    writer.WriteValue(header.Timestamp);
                    writer.WriteEndObject();
                }
                writer.WriteEndArray();
                writer.WritePropertyName("graph");
                writer.WriteStartObject();
                    foreach (var graph in message.Graph)
                    {
                        writer.WritePropertyName(graph.graphIri);
                        writer.WriteValue(graph.graphContent);
                    }
                writer.WriteEndObject();
            writer.WriteEndObject();
        }

        private BlockHeader GetData(JObject jObject)
        {
            string dataGraphIri = null;
            string dataHash = null;
            string hash = null;
            string index = null;
            string previousBlock = null;
            string previousHash = null;
            string timestamp = null;

            if (jObject["dataGraphIri"] != null)
            {
                dataGraphIri = jObject["dataGraphIri"].Value<string>();
            }
            if (jObject["dataHash"] != null)
            {
                dataHash = jObject["dataHash"].Value<string>();
            }
            if (jObject["hash"] != null)
            {
                hash = jObject["hash"].Value<string>();
            }
            if (jObject["index"] != null)
            {
                index = jObject["index"].Value<string>();
            }
            if (jObject["previousBlock"] != null)
            {
                previousBlock = jObject["previousBlock"].Value<string>();
            }
            if (jObject["previousHash"] != null)
            {
                previousHash = jObject["previousHash"].Value<string>();
            }
            if (jObject["timestamp"] != null)
            {
                timestamp = jObject["timestamp"].Value<string>();
            }
            return new BlockHeader(dataGraphIri, dataHash, hash, index, previousBlock, previousHash, timestamp);
        }

        private List<GraphData> GetGraph(JObject jObject)
        {
            var props = jObject.Properties();
            return jObject.Properties().Select(p => new GraphData(p.Name, (string)p.Value)).ToList();
        }

        private List<BlockHeader> HandleData(JsonReader r)
        {
            var result = new List<BlockHeader>();

            r.Read(); //Array start
            r.Read(); //Object start
            while (r.TokenType != JsonToken.EndArray)
            {
                string dataGraphIri = null;
                string dataHash = null;
                string hash = null;
                string index = null;
                string previousBlock = null;
                string previousHash = null;
                string timestamp = null;

                r.Read();
                while (r.TokenType != JsonToken.EndObject)
                {
                    switch ((string)r.Value)
                    {
                        case "dataGraphIri":
                            {
                                dataGraphIri = r.ReadAsString();
                                break;
                            }
                        case "dataHash":
                            {
                                dataHash = r.ReadAsString();
                                break;
                            }
                        case "hash":
                            {
                                hash = r.ReadAsString();
                                break;
                            }
                        case "index":
                            {
                                index = r.ReadAsString();
                                break;
                            }
                        case "previousBlock":
                            {
                                previousBlock = r.ReadAsString();
                                break;
                            }
                        case "previousHash":
                            {
                                previousHash = r.ReadAsString();
                                break;
                            }
                        case "timestamp":
                            {
                                timestamp = r.ReadAsString();
                                break;
                            }
                    }
                    r.Read();
                }
                result.Add(new BlockHeader(dataGraphIri, dataHash, hash, index, previousBlock, previousHash, timestamp));
                r.Read();
            }
            return result;
        }

        private List<GraphData> HandleGraph(JsonReader r)
        {
            var result = new List<GraphData>();

            r.Read(); //Object start
            r.Read();
            while (r.TokenType != JsonToken.EndObject)
            {
                var gd = new GraphData(r.Value as string, r.ReadAsString());
                result.Add(gd);
                r.Read();
            }
            return result;
        }
    }
}
