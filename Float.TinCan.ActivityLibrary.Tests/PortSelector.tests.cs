using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Xunit;

namespace Float.TinCan.ActivityLibrary.Tests
{
    public class PortSelectorTests
    {
        const string defaultAddress = "http://127.0.0.1";

        [Fact]
        public void TestSelectForAddress()
        {
            // this is a little silly
            var result = PortSelector.SelectForAddress(defaultAddress);
            Assert.True(result.SelectedPort >= 61550);
            Assert.True(result.SelectedPort <= 62550);
        }

        [Fact]
        public void TestRejectedPorts()
        {
            ushort occupiedPort = 62222;
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add($"{defaultAddress}:{occupiedPort}/");
            httpListener.Start();

            var result = PortSelector.SelectForAddress(defaultAddress, occupiedPort);
            Assert.Contains(occupiedPort, result.RejectedPorts);
            Assert.True(result.SelectedPort > occupiedPort);

            httpListener.Stop();
        }

        [Fact]
        public void TestEquality()
        {
            var rand = new Random();
            var selected = RandomUshort(rand);
            var rejected = Enumerable.Range(0, 3).Select(_ => RandomUshort(rand)).ToArray();

#if DEBUG
            var psr1 = new PortSelectorResult(selected, rejected);
            var psr2 = new PortSelectorResult(selected, rejected.OrderBy(_ => rand.Next()));
            IPortSelectorResult ipsr1 = psr1;
            IPortSelectorResult ipsr2 = psr2;
            IEquatable<PortSelectorResult> epsr1 = psr1;
            IEquatable<PortSelectorResult> epsr2 = psr2;

            Assert.True(psr1 == psr2);
            Assert.False(psr1 != psr2);

            Assert.Equal(psr1, psr2);
            Assert.Equal(ipsr1, ipsr2);
            Assert.Equal(epsr1, epsr2);

            Assert.Equal(psr1.GetHashCode(), psr2.GetHashCode());
            Assert.Equal(ipsr1.GetHashCode(), ipsr2.GetHashCode());
            Assert.Equal(epsr1.GetHashCode(), epsr2.GetHashCode());

            Assert.Equal(psr1.ToString(), psr2.ToString());
            Assert.Equal(ipsr1.ToString(), ipsr2.ToString());
            Assert.Equal(epsr1.ToString(), epsr2.ToString());

            Assert.True(psr1.Equals(psr2));
            Assert.True(ipsr1.Equals(ipsr2));
            Assert.True(epsr1.Equals(epsr2));

            Assert.True(EqualityComparer<PortSelectorResult>.Default.Equals(psr1, psr2));
            Assert.True(EqualityComparer<IPortSelectorResult>.Default.Equals(ipsr1, ipsr2));
            Assert.True(EqualityComparer<IEquatable<PortSelectorResult>>.Default.Equals(epsr1, epsr2));
#endif
        }

        ushort RandomUshort(Random source)
        {
            return (ushort)source.Next(ushort.MinValue, ushort.MaxValue);
        }
    }
}
