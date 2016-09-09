using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
//using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
namespace PrimenumGen

{
   
    }
    public static class ClientThread
    {
        public static int ApproximateNthPrime(int nn)
        {
            double n = (double)nn;
            double p;
            if (nn >= 7022)
            {
                p = n * Math.Log(n) + n * (Math.Log(Math.Log(n)) - 0.9385);
            }
            else if (nn >= 6)
            {
                p = n * Math.Log(n) + n * Math.Log(Math.Log(n));
            }
            else if (nn > 0)
            {
                p = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                p = 0;
            }
            return (int)p;
        }
        public static long ApproximateNthPrime(long nn)
        {
            double n = (double)nn;
            double p;
            if (nn >= 7022)
            {
                p = n * Math.Log(n) + n * (Math.Log(Math.Log(n)) - 0.9385);
            }
            else if (nn >= 6)
            {
                p = n * Math.Log(n) + n * Math.Log(Math.Log(n));
            }
            else if (nn > 0)
            {
                p = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                p = 0;
            }
            return (int)p;
        }

        public static IEnumerable<int> test(int end)
        {
            List<int> fart = new List<int>();

            fart.Add(3);
            yield return 2;
            yield return 3;
            for (int i = 3; i < end; i += 2)
            {
                double topshelf = Math.Sqrt(i);

                for (int x = 0; x < fart.Count; x++)
                {

                    if (i % fart[x] == 0)
                    {
                        break;

                    }
                    //else if(x==fart.Count-1){
                    //	fart.Add(i);
                    //	yield return i;
                    //}
                    else if (fart[x] >= topshelf)
                    {
                        fart.Add(i);
                        yield return i;
                        break;
                    }
                }

            }
        }
        public static IEnumerable<long> Sund(long n)
        {
            yield return 2;
            long totalCount = 0;
            long k = n / 2;
            BitArray myBA1 = new BitArray((int)(k+1));

            /* SET ALL TO PRIME STATUS */
            
            /* SEIVE */
            long maxVal = 0;
            long denominator = 0;
            for (long i = 1; i < k; i++)
            {
                denominator = (i << 1) + 1;
                maxVal = (k - i) / denominator;
                for (long j = i; j <= maxVal; j++)
                {
                    myBA1[(int)(i + j * denominator)]=false;
                }
            }
            long prime = 0;
            for (long i = 1; i < k; i++)
            {
                if (myBA1[(int)(i)])
                {
                    totalCount++;
                    prime = (i << 1) + 1;
                    yield return prime;
                }
            }
        }
        public static IEnumerable<int> Sund(int n)
        {
            yield return 2;
            int totalCount = 0;
            int k = n / 2;
            BitArray myBA1 = new BitArray((k + 1));

            /* SET ALL TO PRIME STATUS */
            myBA1.SetAll(true);
            /* SEIVE */
            int maxVal = 0;
            int denominator = 0;
            for (int i = 1; i < k; i++)
            {
                denominator = (i << 1) + 1;
                maxVal = (k - i) / denominator;
                for (int j = i; j <= maxVal; j++)
                {
                    myBA1[i + j * denominator] = false;
                }
            }
            int prime = 0;
            for (int i = 1; i < k; i++)
            {
                if (myBA1[i])
                {
                    totalCount++;
                    prime = (i << 1) + 1;
                    yield return prime;
                }
            }
        }
        public static void console()
        {
            string str = Console.ReadLine();
            string[] command = str.Split(" ".ToCharArray());
            switch (command[0].ToLower())
            {
                case "getprimes":
                    File.WriteAllText(@"primes.txt", String.Empty);
                    if (command.Length >= 2)
                    {
                        long length = long.Parse(command[1]);
                        int poo = 0;
                        DateTime time = DateTime.Now;
                        using (StreamWriter writer = new StreamWriter(@"primes.txt"))
                        {
                            foreach (int poop in Sund(length))
                            {
                                //Console.WriteLine(poop);
                                writer.WriteLine(poop);
                                poo++;
                            }
                        }

                        Console.WriteLine("Finished! Primes Found: " + poo + " Took " + (DateTime.Now - time).TotalSeconds + " Seconds!");
                    }
                    console();
                    break;
                case "getprimes2":
                    File.WriteAllText(@"primes.txt", String.Empty);
                    if (command.Length >= 2)
                    {
                        int length = ApproximateNthPrime(Int32.Parse(command[1]));
                        int poo = 0;
                        DateTime time = DateTime.Now;
                        using (StreamWriter writer = new StreamWriter(@"primes.txt"))
                        {
                            foreach (int poop in Sund(length))
                            {
                                //Console.WriteLine(poop);
                                writer.WriteLine(poop);
                                poo++;
                            }
                        }

                        Console.WriteLine("Finished! Primes Found: " + poo + " Took " + (DateTime.Now - time).TotalSeconds + " Seconds!");
                    }
                    console();
                    break;
            }
        }

        public static void Main(string[] args)
        {

            console();
        }
    }
    public class HugeArray<T> : IEnumerable<T>
    where T : struct
    {
        public static int arysize = (Int32.MaxValue >> 4) / Marshal.SizeOf(typeof(T));

        public readonly long Capacity;
        private readonly T[][] content;

        public T this[long index]
        {
            get
            {
                if (index < 0 || index >= Capacity)
                    throw new IndexOutOfRangeException();
                int chunk = (int)(index / arysize);
                int offset = (int)(index % arysize);
                return content[chunk][offset];
            }
            set
            {
                if (index < 0 || index >= Capacity)
                    throw new IndexOutOfRangeException();
                int chunk = (int)(index / arysize);
                int offset = (int)(index % arysize);
                content[chunk][offset] = value;
            }
        }

        public HugeArray(long capacity)
        {
            Capacity = capacity;
            int nChunks = (int)(capacity / arysize);
            int nRemainder = (int)(capacity % arysize);

            if (nRemainder == 0)
                content = new T[nChunks][];
            else
                content = new T[nChunks+1][];

            for (int i = 0; i < nChunks; i++)
                content[i] = new T[arysize];
            if (nRemainder > 0)
                content[content.Length - 1] = new T[nRemainder];
        }

        public IEnumerator<T> GetEnumerator()
        {
            return content.SelectMany(c => c).GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

