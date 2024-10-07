using System;
using System.Threading.Tasks;

namespace ws_deadlock
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            using (var client = new SioClient("https://sio-server/"))
            {
                try
                {
                    await client.InitializeConnection();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }

                Console.WriteLine("Post connect");
            }
        }
    }
}