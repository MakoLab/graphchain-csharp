using System.Collections.Generic;
using System.Text;
using elephant.core.model.chain;
using elephant.core.util;
using Newtonsoft.Json;

namespace elephant.core.model.p2p.message
{
    [JsonConverter(typeof(PeerToPeerMessageConverter))]
    public class PeerToPeerMessage : Message
    {
        public override MessageType MessageType { get; set; }
        public List<BlockHeader> Data { get; set; }
        public List<GraphData> Graph { get; set; }

        public PeerToPeerMessage() { }

        public PeerToPeerMessage(MessageType messageType)
        {
            MessageType = messageType;
            Data = new List<BlockHeader>();
            Graph = new List<GraphData>();
        }

        public PeerToPeerMessage(MessageType messageType, List<BlockHeader> data, List<GraphData> graph) : this(messageType)
        {
            Data = data;
            Graph = graph;
        }

        public override bool IsMessageValid()
        {
            if (Data == null)
            {
                return false;
            }
            return true;
        }

        public override string SerializeAsJson()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public override string ToString()
        {
            return "PeerToPeerMessage{" + "messageType=" + MessageType + ", data=" + Data + ", graph=" + Graph + "}";
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            PeerToPeerMessage message = (PeerToPeerMessage)obj;
            return MessageType == message.MessageType && Equals(Data, message.Data) && Equals(Graph, message.Graph);
        }

        public override int GetHashCode()
        {
            return MessageType.GetHashCode() + Data.GetHashCode() + Graph.GetHashCode();
        }
    }

    public class GraphData
    {
        public string graphIri;
        public string graphContent;

        public GraphData(string graphIri, string graphContent)
        {
            this.graphIri = graphIri;
            this.graphContent = graphContent;
        }

    public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("GraphData{")
                .Append("graphIri='").Append(graphIri).Append("'");
            if (graphContent != null && graphContent.Length > 100)
            {
                sb.Append(", graphContent(first 100 chars)='").Append(graphContent.Substring(0, 100)).Append("'");
            }
            sb.Append("}");
            return sb.ToString();
        }

    public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            GraphData graphData = (GraphData)o;
            return Equals(graphIri, graphData.graphIri) && Equals(graphContent, graphData.graphContent);
        }

        public override int GetHashCode()
        {
            return graphIri.GetHashCode() + graphContent.GetHashCode();
        }
    }
}
