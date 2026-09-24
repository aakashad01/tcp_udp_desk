using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

public class udp_client
{
    static void Main(string[] args)
    {
        string videoPath ="rainy-forest.1920x1080.mp4";
        string serverIp="100.100.100.10";
        int serverPort=5010;

        const int payloadSize =5000;
        const int headerSize = 8;

        if (!File.Exists(videoPath))
        {
            Console.WriteLine($"Video File not found: {videoPath}");
            return;
        }
        byte[] videoData = File.ReadAllBytes(videoPath);
        Console.WriteLine($"Video size:{videoData.Length}");

        int totalPackets = (int)Math.Ceiling((double)videoData.Length/payloadSize); //calculate number of packets to be created
        Console.WriteLine($"Total size:{totalPackets}");

        using UdpClient client = new UdpClient();
        client.Connect(serverIp,serverPort);

        for(int sequenceNumber=0; sequenceNumber<totalPackets; sequenceNumber++)
        {
            int offset = sequenceNumber * payloadSize;                     //indentifies where the current chunk begins in video array
            int remainingBytes = videoData.Length - offset;                //calculates how many bytes remainning  
            int currentPayloadSize=Math.Min(payloadSize,remainingBytes);   //prevents final packet from trying to read beyond end of video 
            byte[] packet = new byte[headerSize + currentPayloadSize];     //header size(8) + current video chunk 

            //add sequenceNumber into packet in index 0-3
            Array.Copy(
                BitConverter.GetBytes(sequenceNumber),              //source array
                0,                                                  //source index
                packet,                                             //destination array
                0,                                                  //destination index
                4                                                   //number of bytes
            );

            //add totalPackets count into packet in index 4-7
            Array.Copy(
                BitConverter.GetBytes(totalPackets),
                0,
                packet,
                4,
                4
            );

            //copy video data into packet
            Array.Copy(
                videoData,
                offset,
                packet,
                headerSize,
                currentPayloadSize
            );

            client.SendAsync(packet,packet.Length); //send the packet
            Console.WriteLine($"Sent packet {sequenceNumber + 1}/{totalPackets}");
            // Thread.Sleep(2);
        }
        Console.WriteLine("Video Transfer completed.");
    }
}
