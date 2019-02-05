using System;
using System.Collections.Generic;
using DiffMatchPatch;
using Fossil;
using System.IO;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace ConsoleApplication2
{
    class Program
    {
        static void Main(string[] args)
        {

            byte[] left = File.ReadAllBytes("d:\\FFIX_HD_MOD\\FONT_RESOURCES\\resources.ORIG");
            byte[] right = File.ReadAllBytes("d:\\FFIX_HD_MOD\\FONT_RESOURCES\\resources.GARNET_GFX");


            



            byte[] outF = Fossil.Delta.Create(left, right);

            File.WriteAllBytes("d:\\workspace\\Memoria\\Output\\resources_patch.diff", outF);
            byte[] patch = File.ReadAllBytes("d:\\workspace\\Memoria\\Output\\resources_patch.diff");
            var destinationSize = Fossil.Delta.OutputSize(patch);

            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead("d:\\FFIX_HD_MOD\\FONT_RESOURCES\\resources.bak"))
                {
                    var hash = md5.ComputeHash(stream);
                    var hash_str = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    Console.WriteLine(hash_str);
                }

            }

            long length = new System.IO.FileInfo("d:\\FFIX_HD_MOD\\FONT_RESOURCES\\resources.ORIG").Length;
            Console.WriteLine(length);
            Console.WriteLine(destinationSize);

            //byte[] outF = Fossil.Delta.Apply(right, patch);
            //File.WriteAllBytes("d:\\workspace\\Memoria\\Output\\resources.assets", outF);



            /*
              try
              {
                  if (args.Length == 0)
                  {
                      Console.WriteLine("ConsoleApplication2.exe <filePath>");
                      return;
                  }

                  String inputPath = args[0];
                  String outputPath = args[0] + ".encrypted";
                  Byte[] data = File.ReadAllBytes(inputPath);
                  data = Encryption(data);
                  File.WriteAllBytes(outputPath, data);
                  Console.WriteLine("Done");
              }
              catch (Exception ex)
              {
                  Console.WriteLine(ex);
              }
              finally
              {
                  Console.WriteLine("Press enter to exit...");
                  Console.ReadLine();
              }
              */
        }



        public static Byte[] Decryption(Byte[] bytes)
        {
            Int64 num = 1024L;
            Int64 num2 = (Int64)bytes.Length;
            Int64 num3 = num2 - num;
            Byte[] array = new Byte[num3];
            Int64 num4 = 0L;
            Int32 num5 = 0;
            while ((Int64)num5 < num3)
            {
                if (num4 < num)
                {
                    array[num5] = bytes[(Int32)(checked((IntPtr)(unchecked(num2 - num + num4))))];
                    num4 += 1L;
                }
                else if ((Int64)num5 < num2)
                {
                    array[num5] = bytes[num5];
                }
                num5++;
            }
            return array;
        }


        public static Byte[] Encryption(Byte[] bytes)
        {
            Int64 num = 1024L;
            Int64 num2 = (Int64)bytes.Length;
            Int64 num3 = num2 + num;
            Byte[] array = new Byte[num3];
            Int64 num4 = 0L;
            Int64 num5 = 0L;
            Int32 num6 = 0;
            while ((Int64)num6 < num3)
            {
                if (num4 < num)
                {
                    array[num6] = bytes[(Int32)(checked((IntPtr)(unchecked(num4 + num))))];
                    num4 += 1L;
                }
                else if ((Int64)num6 < num2)
                {
                    array[num6] = bytes[num6];
                }
                else
                {
                    array[num6] = bytes[(Int32)(checked((IntPtr)num5))];
                    num5 += 1L;
                }
                num6++;
            }
            return array;
        }

    }
}
