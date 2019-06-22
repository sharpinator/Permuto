using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Permuto.CxC.Exchange.Entities;
using System.Threading;
using Gremlin.Net.Driver;
using Newtonsoft.Json.Linq;
using Gremlin.Net.Structure;
using System.Dynamic;
using System.Collections;

namespace Permuto.CxC.Exchange.Data
{
    public interface ICommodityDataAdapter
    {
        Task<IEnumerable<Commodity>> GetAll(CancellationToken token = default);
        Task<Commodity> Get(string id, CancellationToken token = default);
        Task Add(Commodity commodity, CancellationToken token = default);
        Task Update(Commodity commodity, CancellationToken token = default);
        Task Delete(string id, CancellationToken token = default);
    }
    
    public class CommodityDataAdapter : GremlinDataAdapter, ICommodityDataAdapter
    {
        public CommodityDataAdapter(IGremlinClientFactory clientFactory) : base(clientFactory) { }
        public Task Add(Commodity commodity, CancellationToken token = default)
        {
            return Execute($"g.addV('Commodity').property('id','{commodity.id}').property('Name','{commodity.Name}')." +
                $"property('Symbol', '{commodity.Symbol}')");
        }

        public Task Delete(string id, CancellationToken token = default)
        {
            return Execute($"g.V().hasId('{id}').drop()");
        }

        public async Task<Commodity> Get(string id, CancellationToken token = default)
        {
            dynamic data = await FetchSingle($"g.V().hasLabel('Commodity').hasId('{id}')");
            return new Commodity() { id = data.id, Name = data.Name, Symbol = data.Symbol };
        }

        public async Task<IEnumerable<Commodity>> GetAll(CancellationToken token = default)
        {
            IEnumerable<dynamic> data = await Fetch("g.V().hasLabel('Commodity').order().by('Symbol')");
            return data.Select(d => new Commodity() { id = d.id, Name = d.Name, Symbol = d.Symbol }).ToArray();
        }

        public Task Update(Commodity commodity, CancellationToken token = default)
        {
            return Execute($"g.V().hasId('{commodity.id}').property('Name','{commodity.Name}').property('Symbol', {commodity.Symbol}')");
        }
    }
}
