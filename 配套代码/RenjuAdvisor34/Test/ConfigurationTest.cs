using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class ConfigurationTest
    {
        public ConfigurationTest(IConfiguration configuration,int index)
        {
            new Thread(() =>
            {
                while (true)
                {
                    Console.WriteLine((configuration as BaseConfiguration).GameProcessId+"\t"+index);
                    Thread.Sleep(500);
                }

            }).Start();
        }
    }
}
