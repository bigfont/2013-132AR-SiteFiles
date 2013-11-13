using DropNet;
using DropNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyDropNet
{
    class Program
    {
        static void Main(string[] args)
        {
            DropNetClient _client = new DropNetClient("sp2p8ei6wvnn62t", "jyayox3moai0h9v");   
            
            // Sync
            _client.GetToken();            

            var accessToken = _client.GetAccessToken();

            Console.WriteLine(accessToken);
                        
            if(accessToken != null)
            { 
            
            }            

            Console.ReadLine();
        }
    }
}
