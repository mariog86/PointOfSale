using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace PointOfSale.Test
{
    [TestFixture]
    public class PointOfSaleTest
    {
        private static readonly string ProgramPath = Assembly.GetAssembly(typeof (Program)).Location;
        private Process process;

        [SetUp]
        public void SetUp()
        {
            ProcessStartInfo processStartInfo = new ProcessStartInfo(ProgramPath)
            {
                UseShellExecute = false,
                ErrorDialog = false,
                RedirectStandardInput = true
            };
            this.process = new Process { StartInfo = processStartInfo };
            bool processStarted = this.process.Start();
            if (!processStarted)
            {
                throw new InvalidOperationException("Process did not start");
            }
        }

        [TearDown]
        public void TearDown()
        {
            this.process.WaitForExit(1000);
            if (!this.process.HasExited)
            {
                this.process.Close();
            }
        }

        [TestCase("1234", "CHF 10.-")]
        [TestCase("2345", "CHF 15.-")]
        public void ProductExists_WhenBarcode_ThenCorrectPrice(string barcode, string price)
        {
            this.WriteToStdIn(barcode);

            var actual = ReadFromClient();

            actual.Should().Be(price);
        }

        private void WriteToStdIn(string text)
        {
            using (StreamWriter inputWriter = this.process.StandardInput)
            {
                inputWriter.WriteLine(text);
            }
        }

        private static string ReadFromClient()
        {
            TcpListener listener = null;
            try
            {
                listener = new TcpListener(IPAddress.Any, 6740);
                listener.Start();
                var acceptTcpClientAsync = listener.AcceptTcpClientAsync();
                acceptTcpClientAsync.Wait(5000);
                if (acceptTcpClientAsync.IsCompleted)
                {
                    using (TcpClient client = acceptTcpClientAsync.Result)
                    {
                        using (StreamReader reader = new StreamReader(client.GetStream(), Encoding.UTF8))
                        {
                            string actual;
                            actual = reader.ReadLine();
                            return actual;
                        }

                    }
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                listener?.Stop();
            }
        }
    }
}
