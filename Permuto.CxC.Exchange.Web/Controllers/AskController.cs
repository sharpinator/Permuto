using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Threading;
using Permuto.CxC.Exchange.Entities;
using Permuto.CxC.Exchange.ServiceBus;

namespace Permuto.CxC.Exchange.Web.Controllers
{
    public class AskController : ApiController
    {
        protected IPermutoServiceBus ServiceBus { get; private set; }
        public AskController(IPermutoServiceBus serviceBus)
        {
            ServiceBus = serviceBus;
        }
        public async Task<IHttpActionResult> EnqueueAsk(Ask ask, CancellationToken token = default)
        {
            ServiceBus.Connect(false);
            try
            {
                await ServiceBus.PostAsk(ask);
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
            finally { await ServiceBus.Disconnect(); }
            return Ok();
        }
    }
}
