using System;
using System.Threading.Tasks;

namespace ws_deadlock
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            using (var client = new SioClient("http://127.0.0.1:8000"))
            {
                try
                {
                    await client.InitializeConnection();
                    
                    Console.WriteLine("Post connect");
                    
                    await client.EmitAsync("foo", new object(){});
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}