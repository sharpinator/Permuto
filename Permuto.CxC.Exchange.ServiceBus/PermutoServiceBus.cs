using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using System.Threading;
using Permuto.CxC.Exchange.Entities;
using Newtonsoft.Json;

namespace Permuto.CxC.Exchange.ServiceBus
{
    public interface IPermutoServiceBus
    {
        void Connect(bool registerMessageHandlers = true);
        Task Disconnect();
        Task PostAsk(Ask ask);
        Task PostUpdate(AskBase leg);
        event Func<Ask, CancellationToken, Task<bool>> NewAsk;
        event Func<AskBase, CancellationToken, Task<bool>> UpdatedLeg;
    }
    public class PermutoServiceBus : IPermutoServiceBus
    {
        protected ServiceBusConnectionDetails IncomingAsks { get; private set; }
        protected ServiceBusConnectionDetails IncomingUpdates { get; set; }
        public PermutoServiceBus(ServiceBusConnectionDetails incomingAsks, ServiceBusConnectionDetails incomingUpdates)
        {
            IncomingAsks = incomingAsks;
            IncomingUpdates = incomingUpdates;
        }
        public event Func<Ask, CancellationToken, Task<bool>> NewAsk;
        public event Func<AskBase, CancellationToken, Task<bool>> UpdatedLeg;
        protected QueueClient AskQueue { get; private set; }
        protected QueueClient UpdateQueue { get; private set; }
        public void Connect(bool registerMessageHandlers = true)
        {
            AskQueue = new QueueClient(IncomingAsks.ConnectionString, IncomingAsks.QueueName, ReceiveMode.PeekLock);
            UpdateQueue = new QueueClient(IncomingUpdates.ConnectionString, IncomingUpdates.QueueName, ReceiveMode.PeekLock);
            if (registerMessageHandlers)
            {
                AskQueue.RegisterMessageHandler(async (msg, cancelToken) =>
                {
                    bool doDelete = await (NewAsk?.Invoke(JsonConvert.DeserializeObject<Ask>(Encoding.UTF8.GetString(msg.Body)), cancelToken) ?? Task.FromResult(false));
                    if (doDelete)
                        await AskQueue.CompleteAsync(msg.SystemProperties.LockToken);
                },
                    ex => Task.FromException(ex.Exception)); //TODO: Logging
                UpdateQueue.RegisterMessageHandler(async (msg, cancelToken) =>
                {
                    bool doDeleted = await (UpdatedLeg?.Invoke(JsonConvert.DeserializeObject<AskLeg>(Encoding.UTF8.GetString(msg.Body)), cancelToken) ?? Task.FromResult(false));
                    if (doDeleted)
                        await UpdateQueue.CompleteAsync(msg.SystemProperties.LockToken);
                },
                    ex => Task.FromException(ex.Exception));
            }
        }

        public async Task Disconnect()
        {
            var askClose = (AskQueue?.CloseAsync() ?? Task.CompletedTask);
            var updateClose = (UpdateQueue?.CloseAsync() ?? Task.CompletedTask);
            await Task.WhenAll(askClose, updateClose);
            AskQueue = null;
            UpdateQueue = null;
        }

        public Task PostAsk(Ask ask)
        {
            return AskQueue.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(ask))));
        }

        public Task PostUpdate(AskBase leg)
        {
            return UpdateQueue.SendAsync(new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(leg))));
        }
    }
    public class ServiceBusConnectionDetails
    {
        public string ConnectionString { get; set; }
        public string QueueName { get; set; }
    }
}
