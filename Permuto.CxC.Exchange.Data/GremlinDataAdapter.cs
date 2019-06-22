using Gremlin.Net.Driver;
using Gremlin.Net.Driver.Remote;
using Gremlin.Net.Process.Traversal;
using Gremlin.Net.Structure;
using Gremlin.Net.Structure.IO.GraphSON;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Permuto.CxC.Exchange.Data
{
    public interface IGremlinClientFactory
    {
        IGremlinClient Create();
    }
    public class GremlinClientFactory : IGremlinClientFactory
    {
        protected GremlinServer Server { get; private set; }
        public GremlinClientFactory(GremlinServer server)
        {
            Server = server;
        }
        public IGremlinClient Create()
        {
            return new GremlinClient(Server, new GraphSON2Reader(), new GraphSON2Writer(), GremlinClient.GraphSON2MimeType);
        }
    }
    public abstract class GremlinDataAdapter
    {
        protected IGremlinClientFactory ClientFactory { get; private set; }
        public GremlinDataAdapter(IGremlinClientFactory clientFactory)
        {
            ClientFactory = clientFactory;
        }
        protected async Task ExecuteClient(Func<IGremlinClient, Task> action)
        {
            using(var client = ClientFactory.Create())
            {
                await action(client);
            }
        }
        protected Task ExecuteGraph(Func<GraphTraversalSource, Task> g)
        {
            return ExecuteClient(client => g(AnonymousTraversalSource.Traversal().WithRemote(new DriverRemoteConnection(client))));
        }
        protected Task Execute(string query)
        {
            return ExecuteClient(client => client.SubmitAsync(query));
        }
        protected async Task<dynamic> FetchSingle(string query)
        {
            ExpandoObject obj = null;
            await ExecuteClient(async client =>
            {
                var data = await client.SubmitWithSingleResultAsync<IDictionary<string, object>>(query);
                obj = BuildElement(data);
            });
            return obj;
        }
        protected async Task<IEnumerable<dynamic>> Fetch(string query)
        {
            List<dynamic> data = new List<dynamic>();
            await ExecuteClient(async client =>
            {
                var rs = await client.SubmitAsync<IDictionary<string, object>>(query);
                foreach (var r in rs)
                    data.Add(BuildElement(r));
            });
            return data;
        }
        protected ExpandoObject BuildElement(IDictionary<string, object> data)
        {
            dynamic holder = new ExpandoObject();
            holder.id = data["id"];
            var holderDic = (IDictionary<string, object>)holder;
            foreach (var prop in data["properties"] as IDictionary<string, object>)
            {
                var value = prop.Value as IEnumerable<dynamic>;

                holderDic.Add(prop.Key, value.SelectMany(s => (Dictionary<string, object>)s).Single(v => v.Key == "value").Value);
            }
            return holder;
        }
    }
    
}
