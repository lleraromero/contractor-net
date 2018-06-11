using System;

namespace DC.Slicer
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            System.Threading.Tasks.Task<DC.Slicer.SlicerConfig> task;
            if(args.Length!=0){
                task = SlicerConfig.FromXml(@"..\..\..\SlicerConf.xml", args[2], args[0], args[1],args[3]);
            }else{
                task = SlicerConfig.FromXml(@"..\..\..\SlicerConf.xml");
            }       

             task.Wait();
            SlicerConfig slicerConfig = task.Result;

            var orchestrator = new Orchestrator (slicerConfig) { UserInteraction = true };
            orchestrator.Orchestrate();
            //Console.ReadLine();
            ///////////sarasa
        }

    }
}