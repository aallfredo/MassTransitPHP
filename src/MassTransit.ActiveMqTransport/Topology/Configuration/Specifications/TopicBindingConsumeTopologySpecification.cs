// Copyright 2007-2017 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Topology.Specifications
{
    using System.Collections.Generic;
    using Builders;
    using Configurators;
    using Entities;
    using GreenPipes;


    /// <summary>
    /// Used to bind an exchange to the consuming queue's exchange
    /// </summary>
    public class TopicBindingConsumeTopologySpecification :
        TopicBindingConfigurator,
        IActiveMqConsumeTopologySpecification
    {
        public TopicBindingConsumeTopologySpecification(string topicName, bool durable = true, bool autoDelete = false)
            : base(topicName, durable, autoDelete)
        {
        }

        public TopicBindingConsumeTopologySpecification(Topic topic)
            : base(topic)
        {
        }

        public IEnumerable<ValidationResult> Validate()
        {
            yield break;
        }

        public void Apply(IReceiveEndpointBrokerTopologyBuilder builder)
        {
            var exchangeHandle = builder.CreateTopic(EntityName, Durable, AutoDelete);

            var bindingHandle = builder.BindTopic(exchangeHandle, builder.Topic, RoutingKey);
        }
    }
}