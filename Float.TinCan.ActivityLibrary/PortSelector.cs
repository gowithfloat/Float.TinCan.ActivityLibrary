using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;

namespace Float.TinCan.ActivityLibrary
{
    /// <summary>
    /// Selects an available port on this device.
    /// </summary>
    public static class PortSelector
    {
        internal const ushort DefaultStartPort = 61550;

        /// <summary>
        /// Select an available port for serving local content using the given address.
        /// </summary>
        /// <param name="address">The address which requires a port.</param>
        /// <param name="startPort">The starting port; default is 61550.
        ///                         IANA suggests the range 49152 to 65535 for dynamic or private ports.
        ///                         So the port must land between these values.
        /// </param>
        /// <param name="portRange">The range of ports to consider; default is 1000.</param>
        /// <param name="retryCount">How many times to retry any port; default is 10.</param>
        /// <returns>The selected port and all tested unavailable ports.</returns>
        public static IPortSelectorResult SelectForAddress(string address, ushort startPort = DefaultStartPort, ushort portRange = 1000, ushort retryCount = 10)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(address));
            Contract.Requires(startPort >= 49152);
            Contract.Requires(startPort + portRange <= 65535);

            var rejectedPorts = new List<ushort>();

            for (var i = 0; i <= retryCount; i++)
            {
                for (ushort j = startPort; j < startPort + portRange; j++)
                {
                    try
                    {
                        var httpListener = new HttpListener();
                        httpListener.Prefixes.Add($"{address}:{j}/");
                        httpListener.Start();
                        httpListener.Close();
                        return new PortSelectorResult(j, rejectedPorts);
                    }

                    // A Windows function call failed. Check the exception's ErrorCode
                    // property to determine the cause of the exception. This exception
                    // is thrown if another HttpListener has already added the prefix uriPrefix.
                    catch (HttpListenerException)
                    {
                        rejectedPorts.Add(j);
                    }

                    // Address already in use
                    catch (SocketException)
                    {
                        rejectedPorts.Add(j);
                    }
                }
            }

            return new PortSelectorResult(startPort, rejectedPorts);
        }
    }
}
