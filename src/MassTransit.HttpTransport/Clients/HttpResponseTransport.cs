﻿// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.HttpTransport.Clients
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using GreenPipes;
    using MassTransit.Pipeline.Observables;
    using Microsoft.Owin;
    using Transports;


    public class HttpResponseTransport : 
        ISendTransport
    {
        readonly SendObservable _observers;
        readonly IOwinContext _owinContext;

        public HttpResponseTransport(IOwinContext context)
        {
            _owinContext = context;
            _observers = new SendObservable();
        }

        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
            return _observers.Connect(observer);
        }

        public async Task Send<T>(T message, IPipe<SendContext<T>> pipe, CancellationToken cancelSend)
            where T : class
        {
            var context = new HttpSendContextImpl<T>(message, cancelSend);
            await pipe.Send(context).ConfigureAwait(false);

            _owinContext.Response.Headers["Content-Type"] = context.ContentType.MediaType;
            foreach (KeyValuePair<string, object> header in context.Headers.GetAll().Where(h => h.Value != null && (h.Value is string || h.Value.GetType().IsValueType)))
                _owinContext.Response.Headers[header.Key] = header.Value.ToString();

            if (context.MessageId.HasValue)
                _owinContext.Response.Headers[HttpHeaders.MessageId] = context.MessageId.Value.ToString();

            if (context.CorrelationId.HasValue)
                _owinContext.Response.Headers[HttpHeaders.CorrelationId] = context.CorrelationId.Value.ToString();

            if (context.InitiatorId.HasValue)
                _owinContext.Response.Headers[HttpHeaders.InitiatorId] = context.InitiatorId.Value.ToString();

            if (context.ConversationId.HasValue)
                _owinContext.Response.Headers[HttpHeaders.ConversationId] = context.ConversationId.Value.ToString();

            if (context.RequestId.HasValue)
                _owinContext.Response.Headers[HttpHeaders.RequestId] = context.RequestId.Value.ToString();

            _owinContext.Response.StatusCode = (int)HttpStatusCode.OK;
            _owinContext.Response.Write(context.Body);

            // ??
        }
    }
}