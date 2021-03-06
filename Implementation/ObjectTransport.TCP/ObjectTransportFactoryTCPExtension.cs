﻿using OTransport.Factory;

namespace OTransport.NetworkChannel.TCP
{
    public static class ObjectTransportFactoryTCPExtension
    {
        /// <summary>
        /// Create a TCP server. This network channel only supports reliable communication.
        /// </summary>
        /// <param name="ipAddress">the IP address to start the server on</param>
        /// <param name="port">the port to listen on</param>
        /// <returns></returns>
        public static ObjectTransportAssemblyLine CreateTCPServer(this ObjectTransportFactory o)
        {
            TCPServerChannel server = new TCPServerChannel();
            var assemblyLine = new ObjectTransportAssemblyLine();
            assemblyLine.SetNetworkChannel(server);
            assemblyLine.SetReliableTransport();

            return assemblyLine;
        }

        /// <summary>
        /// Create a TCP client. This network channel only supports reliable communication.
        /// </summary>
        /// <param name="ipAddress">the IP address to start the server on</param>
        /// <param name="port">the port to listen on</param>
        /// <returns></returns>
        public static ObjectTransportAssemblyLine CreateTCPClient(this ObjectTransportFactory o)
        {
            TCPClientChannel client = new TCPClientChannel();

            var assemblyLine = new ObjectTransportAssemblyLine();
            assemblyLine.SetNetworkChannel(client);
            assemblyLine.SetReliableTransport();

            return assemblyLine;
        }

    }
}
