using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework.Internal;

namespace PointOfSale.Test
{
    [TestClass]
    public class PointOfSaleTest
    {
        private static readonly string ProgramPath = Assembly.GetAssembly(typeof (Program)).Location;

        [TestMethod]
        public void ProductExists_When1234_ThenChf10()
        {

            var actual = ReadFromClient();

            actual.Should().Be("CHF 10.-");
        }

        private static string ReadFromClient()
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, 6740);
                using (TcpClient client = listener.AcceptTcpClient())
                {
                    using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                    {
                        string actual;
                        actual = reader.ReadLine();
                        return actual;
                    }

                }
            }
            finally
            {
                listener?.Stop();
            }
        }
    }
}
