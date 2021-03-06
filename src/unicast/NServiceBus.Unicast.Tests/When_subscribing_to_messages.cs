namespace NServiceBus.Unicast.Tests
{
    using NUnit.Framework;
    using Rhino.Mocks;
    using SomeUserNamespace;
    using Transport;

    [TestFixture]
    public class When_subscribing_to_messages : using_the_unicastbus
    {
        readonly Address addressToOwnerOfTestMessage = new Address("TestMessageOwner","localhost");
        [SetUp]
        public void SetUp()
        {
            unicastBus.RegisterMessageType(typeof(TestMessage),addressToOwnerOfTestMessage,false);
        }
        [Test]
        public void Only_the_major_version_should_be_used_as_the_subscription_key_in_order_to_make_versioning_easier()
        {
            bus.Subscribe<TestMessage>();

            var version = typeof(TestMessage).Assembly.GetName().Version.Major + ".0.0.0";

            messageSender.AssertWasCalled(x => 
                x.Send(Arg<TransportMessage>.Matches(
                    m => m.Headers.ContainsKey(UnicastBus.SubscriptionMessageType) && m.Headers[UnicastBus.SubscriptionMessageType].Contains("Version=" + version)),
                    Arg<Address>.Is.Equal(addressToOwnerOfTestMessage)));
     
        }
    }
}