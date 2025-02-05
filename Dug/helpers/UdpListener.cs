﻿// Dug is a DNS lookup tool
// Copyright(C) 2024  Richard Cole
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//   This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.

using dug.dataContainers;
using System.Net;
using System.Net.Sockets;

namespace dug.helpers
{
    public class UdpListener
    {
        private readonly DataContainer _dataContainer;
        private readonly UdpClient _udpClient;
        private readonly CancellationTokenSource _cts;


        public UdpListener(int port, DataContainer dataContainer)
        {
            _udpClient = new UdpClient(port);
            _cts = new CancellationTokenSource();
            _dataContainer = dataContainer;
        }

        public async Task StartListeningAsync()
        {
            while (!_cts.IsCancellationRequested)
            {
                try
                {
                    var udpResult = await _udpClient.ReceiveAsync(_cts.Token);
                    byte[] data = udpResult.Buffer;
                    IPAddress remoteIpAddress= udpResult.RemoteEndPoint.Address;
                    OnPacketReceived(data, remoteIpAddress);
                }
                catch (OperationCanceledException ex)
                {
                    Console.WriteLine("UDP Lisen cancelation token received");
                    break; //exit the loop if cancellation is requested
                }
                catch (SocketException ex)
                {
                    Console.WriteLine($"Socket error ({ex})");
                }
                catch (IOException)
                {
                    Console.WriteLine("IO eror ({ex})");
                }
                catch (ObjectDisposedException ex)
                {
                    Console.WriteLine($"UDP client was disposed of. ({ex})");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error. ({ex})");
                }
            }
        }

        public void StopListening()
        {
            _cts.Cancel();
        }
        public async Task SendDataAsync(byte[] data, string destinationAddress, int destinationPort)
        {
            try
            {
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(destinationAddress), destinationPort);
                await _udpClient.SendAsync(data, data.Length, remoteEndPoint);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Socket error ({ex})");
            }
            catch (IOException)
            {
                Console.WriteLine("IO eror ({ex})");
            }
            catch (ObjectDisposedException ex)
            {
                Console.WriteLine($"UDP client was disposed of. ({ex})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error. ({ex})");
            }
        }

        private async void OnPacketReceived(byte[] data, IPAddress remoteIpAddress)
        {
            ParseDnsPacket read = new(_dataContainer);
            await read.ParseDnsResponse(data, DateTime.UtcNow, remoteIpAddress);
        }
    }
}
