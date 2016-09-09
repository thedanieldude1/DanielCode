using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LargeCollections;
using System.Numerics;
namespace PrimeGenerator
{
    class Program
    {
        public static class main
        {
            public static void Main(string[] args)
            {
                LongBitArray test = new LongBitArray((long)Int32.MaxValue+50);
                test[30] = true;
                test[31] = true;
                Console.WriteLine(test[30] + " " + test[31]);
                //test[BigInteger.Parse("9223372036854775810") + 30] = false;
                //test[BigInteger.Parse("9223372036854775810") + 31] = true;
                test[(long)Int32.MaxValue + 30] = false;
                test[(long)Int32.MaxValue + 31] = true;
                Console.WriteLine(test[(long)Int32.MaxValue + 30] + " " + test[(long)Int32.MaxValue + 31]);
                Console.ReadLine();
            }
        }
    }
}
