﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PlcTag.Test
{
    [Serializable]
    public class Test12
    {
        public int AA1 { get; set; }
        public int AA2 { get; set; }
        public int AA3 { get; set; }
        public int AA4 { get; set; }
        public int AA5 { get; set; }
        public int AA6 { get; set; }
        public int AA7 { get; set; }
        public int AA8 { get; set; }
    }

    public class Program
    {
        private static void PrintChange(string @event, OperationResult result)
        {
            Console.Out.WriteLine($"{@event} {result.Timestamp} Changed: {result.Tag.Name} {result.StatusCode}");
        }

        private static void TagChanged(OperationResult result)
        {
            PrintChange("TagChanged", result);
        }

        private static void GroupChanged(IEnumerable<OperationResult> results)
        {
            foreach (var result in results) PrintChange("GroupTagChanged", result);
        }

        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .AddFilter("LoggingConsoleApp.Program", LogLevel.Debug)
                    .AddConsole();
            });
            ILogger<PlcController> logger = loggerFactory.CreateLogger<PlcController>();

            logger.LogInformation("Example log message");

            using (var controller = new PlcController("192.168.18.200", "1,0", CPUType.LGX))
            {
                controller.Timeout = 1000;
                //controller.DebugLevel = 3;

                var heightTag = controller.CreateTag<int>("PrintBar_1_Height");
                var ackTag = controller.CreateTag<bool>("PrintBar_1_HeightAck");

                controller.Connect();

                var currentHeight = heightTag.Read();
                Console.WriteLine("Height: ", currentHeight);
                heightTag.Write(4);

                var acked = false;
                while (!acked)
                {
                    Thread.Sleep(250);
                    Console.WriteLine("Checking for Ack");
                    acked = ackTag.Read();
                }
                Console.WriteLine("Acked");
                ackTag.Write(false);


                currentHeight = heightTag.Read();
                Console.WriteLine("Enabled: ", currentHeight);

                //var tag12 = controller.CreateTag<Int32>("TKP_PLC_D_P1[10]");

                //var tagBPLC1 = controller.CreateTag<Int32>("TKP_PLC_B_P1");
                //tagBPLC1.Read();

                //var tagOvenEnabled = controller.CreateTag<bool>("TKP_PLC_B_OVEN");
                //var oven = tagOvenEnabled.Read();
                //Console.Out.WriteLine($"oven value: {oven}");

                //System.Threading.Thread.Sleep(800);

                //Console.Out.WriteLine("pippo");

                //oven = tagOvenEnabled.Read();
                //Console.Out.WriteLine($"oven value: {oven}");

                // var tagBPC1 = grp.CreateTagInt32("TKP_PC_B_P1"); var tagBarcode = grp.CreateTagString("TKP_PLC_S_P1");

                // var tag3 = grp.CreateTagArray<float[]>("OvenTemp", 36);

                // //var tag_1 = grp.CreateTagArray<string[]>("Track", 300);

                //or
                //var tag = controller.CreateTag<string>("Track");
                //tag.Changed += TagChanged;
                //var aa = tag.Read();

                //Console.Out.WriteLine(aa);

                //var tag1 = controller.CreateTag<Test12>("Test");
                //tag.Changed += TagChanged;

                //var tag2 = controller.CreateTag<float>("Fl32");

                //grp.Changed += GroupChanged;
                //grp.Read();
            }
        }
    }
}
