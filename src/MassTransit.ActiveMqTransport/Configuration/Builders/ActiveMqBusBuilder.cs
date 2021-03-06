// Copyright 2007-2018 Chris Patterson, Dru Sellers, Travis Smith, et. al.
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
namespace MassTransit.ActiveMqTransport.Builders
{
    using System;
    using EndpointSpecifications;
    using MassTransit.Builders;
    using Topology.Settings;
    using Transport;
    using Transports;


    public class ActiveMqBusBuilder :
        BusBuilder
    {
        readonly ActiveMqReceiveEndpointSpecification _busEndpointSpecification;
        readonly BusHostCollection<ActiveMqHost> _hosts;

        public ActiveMqBusBuilder(BusHostCollection<ActiveMqHost> hosts, QueueReceiveSettings busSettings, IActiveMqEndpointConfiguration configuration, bool deployTopologyOnly)
            : base(hosts, configuration, deployTopologyOnly)
        {
            _hosts = hosts;

            var endpointConfiguration = configuration.CreateNewConfiguration(ConsumePipe);

            _busEndpointSpecification = new ActiveMqReceiveEndpointSpecification(_hosts[0], endpointConfiguration, busSettings);

            foreach (var host in hosts.Hosts)
            {
                var factory = new ActiveMqReceiveEndpointFactory(this, host, configuration);

                host.ReceiveEndpointFactory = factory;
            }
        }

        public BusHostCollection<ActiveMqHost> Hosts => _hosts;

        public override IPublishEndpointProvider PublishEndpointProvider => _busEndpointSpecification.PublishEndpointProvider;

        public override ISendEndpointProvider SendEndpointProvider => _busEndpointSpecification.SendEndpointProvider;

        protected override void PreBuild()
        {
            _busEndpointSpecification.Apply(this);
        }

        protected override Uri GetInputAddress()
        {
            return _busEndpointSpecification.InputAddress;
        }
    }
}