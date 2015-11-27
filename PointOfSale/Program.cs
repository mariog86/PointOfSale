using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace PointOfSale
{
    public class Program
    {
        static void Main(string[] args)
        {
            string barcode = Console.ReadLine();
            string price;
            switch (barcode)
            {
                case "1234":
                    price = "CHF 10.-";
                    break;
                case "2345":
                    price = "CHF 15.-";
                    break;
                default:
                    throw new InvalidOperationException("Could not find barcode");
            }
            using (TcpClient client = new TcpClient())
            {
                client.Connect("localhost", 6740);
                using (StreamWriter writer = new StreamWriter(client.GetStream(), Encoding.UTF8))
                {
                    writer.WriteLine(price);
                }
            }
            
        }
    }
}
