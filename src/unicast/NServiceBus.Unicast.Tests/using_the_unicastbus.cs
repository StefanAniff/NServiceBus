namespace NServiceBus.Unicast.Tests
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using MasterNode;
    using MessageMutator;
    using NUnit.Framework;
    using ObjectBuilder;
    using Queuing;
    using Rhino.Mocks;
    using Serialization;
    using Transport;

    public class using_the_unicastbus
    {
        protected IBus bus;

        protected UnicastBus unicastBus;
        protected ISendMessages messageSender;
        protected Address gatewayAddress;
        MessageHeaderManager headerManager = new MessageHeaderManager();

        [SetUp]
        public void SetUp()
        {
            string localAddress = "endpointA";
            Address masterNodeAddress = localAddress + "@MasterNode";

            try
            {
                Address.InitializeLocalAddress(localAddress);
            }
            catch // intentional
            {
            }

            ExtensionMethods.IsMessageTypeAction = t => typeof(IMessage).IsAssignableFrom(t) && t != typeof(IMessage);
            ExtensionMethods.GetStaticOutgoingHeadersAction = () => MessageHeaderManager.staticHeaders;
            gatewayAddress = masterNodeAddress.SubScope("gateway");

            messageSender = MockRepository.GenerateStub<ISendMessages>();
            var masterNodeManager = MockRepository.GenerateStub<IManageTheMasterNode>();
            var builder = MockRepository.GenerateStub<IBuilder>();

            builder.Stub(x => x.BuildAll<IMutateOutgoingMessages>()).Return(new IMutateOutgoingMessages[] { });

            builder.Stub(x => x.BuildAll<IMutateOutgoingTransportMessages>()).Return(new IMutateOutgoingTransportMessages[] { headerManager });

            masterNodeManager.Stub(x => x.GetMasterNode()).Return(masterNodeAddress);
            unicastBus = new UnicastBus
                      {
                          MessageSerializer = MockRepository.GenerateStub<IMessageSerializer>(),
                          Builder = builder,
                          MasterNodeManager = masterNodeManager,
                          MessageSender = messageSender,
                          Transport = MockRepository.GenerateStub<ITransport>()
                      };
            bus = unicastBus;
            ExtensionMethods.SetHeaderAction = headerManager.SetHeader;

            ((IStartableBus)bus).Start();
        }


    }
}