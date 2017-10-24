﻿using LiteNetLib;
using LiteNetLib.Utils;
using OTransport.Network_Channels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OTransport.Implementation
{
    public class UDPServerChannel : INetworkChannel
    {
        private Dictionary<Client, NetPeer> ClientToNetPeerMap = new Dictionary<Client, NetPeer>();
        private IPAddress IPAddress;

        private EventBasedNetListener listener;
        private NetManager server;
        public int Port;


        List<Task> clientTasks = new List<Task>();
        Action<ReceivedMessage> onReceiveCallback = null;
        Action<Client> onConnectCallBack = null;
        Action<Client> onDisconnectCallBack = null;

        public void Stop()
        {
            foreach (var keypair in ClientToNetPeerMap)
            {
                server.DisconnectPeer(keypair.Value);
                server.Stop();
            }
        }

        public UDPServerChannel(string ipAddress, int port,int numberOfConnections)
        {
            listener = new EventBasedNetListener();
            server = new NetManager(listener, numberOfConnections, "ConnectionKey");
            server.UnsyncedEvents = true;
            server.Start(port);
            Port = server.LocalPort;

            listener.PeerDisconnectedEvent += (c,i) =>
            {
                Client client = GetClientRecord(c);
                onDisconnectCallBack?.Invoke(client);
            };

            listener.PeerConnectedEvent += c =>
            {
                Client client = new Client(c.EndPoint.Host, c.EndPoint.Port);
                ClientToNetPeerMap.Add(client, c);
                onConnectCallBack?.Invoke(client);
            };

            listener.NetworkReceiveEvent += (fromPeer, dataReader) =>
            {
                Client client = GetClientRecord(fromPeer);
                var payload = dataReader.GetString();
                ReceivedMessage receivedMessage = new ReceivedMessage(client, payload);
                onReceiveCallback.Invoke(receivedMessage);
            };
        }
        private Client GetClientRecord(NetPeer peer)
        {
            Client client = ClientToNetPeerMap.First(o => o.Value == peer).Key;
            return client;
        }

        public void CheckReceiveClient(Action<Client> callBack)
        {
            onConnectCallBack = callBack;
        }

        public void Receive(Action<ReceivedMessage> callBack)
        {
            onReceiveCallback = callBack;
        }

        public void Send(Client client, string message)
        {
            var netPeer = this.ClientToNetPeerMap[client];

            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            netPeer.Send(writer,SendOptions.Unreliable);
        }

        public void ClientDisconnect(Action<Client> callBack)
        {
            onDisconnectCallBack = callBack;
        }
    }
}
