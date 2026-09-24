using System.Net;
using System.Net.Sockets;
using System.Text;

public class tcp_client{
    public static void Main(string[] args){

        TcpClient client= new TcpClient();

        String server_ip="100.100.100.10";
        int port=5000;

        client.Connect(server_ip, port);
        Console.WriteLine($"Connected to server....via:{server_ip}:{port}");
        
        NetworkStream stream =client.GetStream();

        // String msg="Hello server";
        while(true){
            Console.Write("Enter message: ");
            string msg=Console.ReadLine();
            if (msg==""){break;}
            byte[] data=Encoding.UTF8.GetBytes(msg); //converting to bytes so that TCP can tranfer
            stream.Write(data,0,data.Length);

            // Console.WriteLine("Message sent to server...");
            // Console.WriteLine("Press enter to close the client...");
            // Console.ReadLine();

            byte[] buffer=new byte[24];
            int bytesRead = stream.Read(buffer,0,buffer.Length);
            string recv_msg= Encoding.UTF8.GetString(buffer,0,bytesRead);
            Console.WriteLine($"Response from server: {recv_msg}");
        }
    }
}