using System.ServiceProcess;

namespace ZeusRunner
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            #if DEBUG
                //While debugging this section is used.
                ZeusRunner myService = new ZeusRunner();
                myService.onDebug();
                System.Threading.Thread.Sleep(System.Threading.Timeout.Infinite);

            #else
                //In Release this section is used. This is the "normal" way.
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[] 
                { 
                    new ZeusRunner() 
                };
                ServiceBase.Run(ServicesToRun);
            #endif
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[]
            //{
            //    new Service1()
            //};
            //ServiceBase.Run(ServicesToRun);
        }
    }
}
