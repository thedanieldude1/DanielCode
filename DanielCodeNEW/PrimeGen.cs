using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.IO.MemoryMappedFiles;
using LargeCollections;
using System.Threading.Tasks;
namespace ConsoleApplication1
{
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
            long n = (long)nn;
            long p;
            if (nn >= 7022)
            {
                p = n * (long)Math.Log(n) + n * ((long)Math.Log((long)Math.Log(n)) - (long)0.9385);
            }
            else if (nn >= 6)
            {
                p = n * (long)Math.Log(n) + n * (long)Math.Log((long)Math.Log(n));
            }
            else if (nn > 0)
            {
                p = new int[] { 2, 3, 5, 7, 11 }[nn - 1];
            }
            else
            {
                p = 0;
            }
            return (long)p;
        }
	public static List<string> findPrimes(long max) {
		//var vallist= new List<List<string>>((int)Math.Ceiling(max/(double)Int32.MaxValue));

		
		//for(int i = 0;i<(int)Math.Ceiling(max/(double)Int32.MaxValue);i++){
		//	int length = i==(int)Math.Ceiling(max/(double)Int32.MaxValue)-1?(int)(max/(Math.Log(max)-//1.08366)):Int32.MaxValue;

			//vallist.Add(new List<string>(length));

		//}
    		var vals = new List<string>((int)(max/(Math.Log(max)-1.08366)));
    		long maxSquareRoot = (long)Math.Sqrt(max);
    		LongBitArray eliminated = new LongBitArray(max + 1);                        
		//long primeAmount=0;
    		//yield return 2;
		//vallist[0].Add(2.ToString());
		vals.Add(2.ToString());
    		for (long i = 3; i <= max; i+=2) {
       			 if (!eliminated[i]) {
            			if (i < maxSquareRoot) {
                			for (long j = i * i; j <= max; j+=2*i)
                    				eliminated[j] = true;
            				}
		
		//Console.WriteLine(i);
            	//yield return i;
		//vallist[(int)Math.Floor(primeAmount/(double)Int32.MaxValue)].Add(i.ToString());
		//primeAmount++;
		vals.Add(i.ToString());
        	}

    		}
	return vals;
}
	public static IEnumerable<long> findPrimesEnum(long max) {
    		//var vals = new List<long>((int)(max/(Math.Log(max)-1.08366)));
    		long maxSquareRoot = (long)Math.Sqrt(max);
    		LongBitArray eliminated = new LongBitArray(max + 1);                        

    		yield return 2;
		//vals.Add(2);
    		for (long i = 3; i <= max; i+=2) {
       			 if (!eliminated[i]) {
            			if (i < maxSquareRoot) {
                			for (long j = i * i; j <= max; j+=2*i)
                    				eliminated[j] = true;
            				}
		
		//Console.WriteLine(i);
            	yield return i;
		//vals.Add(i);
        	}
    		}
	//return vals;
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
            LongBitArray myBA1 = new LongBitArray((long)(k + 1));

            /* SET ALL TO PRIME STATUS */
            //myBA1.SetAll(true);
            /* SEIVE */
            long maxVal = 0;
            long denominator = 0;
            for (long i = 1; i < k; i++)
            {
                if (i % 1 == 0) { Console.WriteLine("Cuz baby now we got baaaaad blood " + i); }
                denominator = (i << 1) + 1;
                maxVal = (k - i) / denominator;

                for (long j = i; j <= maxVal; j++)
                {
                    myBA1[i + j * denominator] = false;
                   // if ((i + j * denominator) > (long)Int32.MaxValue*2) { Console.WriteLine(i + j * denominator); }
                }
            }
            long prime = 0;
            for (long i = 1; i < k; i++)
            {
                if (myBA1[i])
                {
                    //Console.WriteLine(i);
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
			//Console.WriteLine(length);
                       // long poo = 0;
                        DateTime time = DateTime.Now;
			//List<List<string>> primes1 = new List<List<string>>();
			long Length=0;
	
			List<string> primes = findPrimes(length);//new List<string>();
			//foreach(List<string> x in primes1){
			//	Length+=x.Count;
			//}
			Length=primes.Count;
			Console.WriteLine("Finished! Primes Found: " + Length + " Took " + (DateTime.Now - time).TotalSeconds + " Seconds! Writing...");
                        //using (StreamWriter writer = new StreamWriter(@"primes.txt"))
                        //{
                           // foreach (long poop in primes)//findPrimesEnum(length))
                           // {
                                //Console.WriteLine(poop);
                              //  writer.WriteLine(poop);
				File.WriteAllLines(@"primes.txt", primes);
                                //poo++;
                            //}
                        //}
			//Console.WriteLine("Finished! Primes Found: " + poo + " Took " + (DateTime.Now - time).TotalSeconds + " Seconds!");
                        
                        Console.WriteLine("Writing Complete!");
                    }
                    console();
                    break;
                case "getprimes2":
                    File.WriteAllText(@"primes.txt", String.Empty);
                    if (command.Length >= 2)
                    {
                        long length = ApproximateNthPrime(long.Parse(command[1]));
                        Console.WriteLine(length);
                        //long poo = 0;
                        DateTime time = DateTime.Now;
			//List<List<string>> primes1 = findPrimes(length);
			long Length=0;
			List<string> primes = findPrimes(length);//new List<string>();
			//foreach(List<string> x in primes1){
			//	Length+=x.Count;
			//}
			Length=primes.Count;
			Console.WriteLine("Finished! Primes Found: " + Length+ " Took " + (DateTime.Now - time).TotalSeconds + " Seconds! Writing...");
                       // using (StreamWriter writer = new StreamWriter(@"primes.txt"))
                       // {
                          //  foreach (long poop in primes)//findPrimesEnum(length))
                          //  {
                                //Console.WriteLine(poop);
                               // writer.WriteLine(poop);
                                //poo++;
				File.WriteAllLines(@"primes.txt", primes);
                           // }
                        //}
			//Console.WriteLine("Finished! Primes Found: " + poo + " Took " + (DateTime.Now - time).TotalSeconds + " Seconds!");
                        
                        Console.WriteLine("Writing Complete!");
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
}

namespace LargeCollections
{
    public class LongBitArray
    {
	public bool startValue=false;
        BitArray LoadedArray;
        // List<byte> LoadedByteArray;
        int LoadedArrayIndex;
        public long Length { get; private set; }
        MemoryMappedFile mmf;
        public LongBitArray(long length)
        {
            Length = length;
            int Amount = (int)Math.Ceiling((double)(length / Int32.MaxValue));
            int Remainder = (int)(length % Int32.MaxValue);
            LoadedArray = new BitArray(Amount >= 1 ? Int32.MaxValue : Remainder,startValue);
            LoadedArrayIndex = 0;
            mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length/ 8.0));
            //LoadedByteArray = new List<byte>();
            //for(int i=0;i<Amount;i++){
            //	if(i==Amount-1){
            //	Elements.Add(new BitArray(Remainder));	
            //	}
            //	else{
            //	Elements.Add(new BitArray(Int32.MaxValue));
            //	}
            //}
        }
        protected byte ConvertToByte(BitArray bits)
        {
            if (bits.Count != 8)
            {
                throw new ArgumentException("illegal number of bits");
            }

            byte b = 0;
            if (bits.Get(0)) b++;
            if (bits.Get(1)) b += 2;
            if (bits.Get(2)) b += 4;
            if (bits.Get(3)) b += 8;
            if (bits.Get(4)) b += 16;
            if (bits.Get(5)) b += 32;
            if (bits.Get(6)) b += 64;
            if (bits.Get(7)) b += 128;
            return b;
        }
        public byte Read(long T, MemoryMappedViewAccessor stream)
        {

            //byte[] buffer = new byte[1];
            // long Temp = stream.Position;
            // stream.Position = (long)Math.Floor(T / 8.0);
            // BinaryReader reader = new BinaryReader(stream);

            byte lol = default(byte);
            try { stream.Read<byte>(0, out lol); }
            catch { return default(byte); }
            //stream.Position = Temp;
            return lol;


        }
        public void Save(int Index)
        {
            // MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
            // CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType
            // .Allow));
            //using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length / 8.0), MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable))
            // {
            using (var stream = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex), (long)Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex + LoadedArray.Length))
            {
                int Amount = (int)Math.Ceiling((double)(Length / Int32.MaxValue));
                int Remainder = (int)(Length % Int32.MaxValue);
                int length = (int)(Amount == Index ? Math.Ceiling(Remainder / 8.0) : Math.Ceiling(Int32.MaxValue / 8.0));
                byte[] buffer = new byte[(int)Math.Ceiling(LoadedArray.Length / 8.0)];
                LoadedArray.CopyTo(buffer, 0);//BitArrayToByteArray(LoadedArray, 0, LoadedArray.Length);
                                              //stream.Position = (long)Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex;
                                              // BinaryWriter writer = new BinaryWriter(stream);
                stream.WriteArray<byte>(0, buffer, 0, buffer.Length);
                //stream.Position = 0;//(long)Math.Ceiling((Int32.MaxValue/8.0))*Index;
                //BinaryReader reader = new BinaryReader(stream);
                // buffer = new byte[(int)Math.Ceiling((Int32.MaxValue / 8.0))];
                // try { stream.Read(buffer, (int)Math.Ceiling((Int32.MaxValue / 8.0)) * Index, (int)Math.Ceiling((Int32.MaxValue / 8.0))); } catch { LoadedArray = new BitArray((int)(length * 8.0)); }
            }

            //int ElementIndex = (int)Math.Floor((double)(T/Int32.MaxValue));
            //int Index = (int)(T%Int32.MaxValue);
            //Elements[ElementIndex][Index]=value;
            //}
        }
        public void Load(int Index)
        {
            MemoryMappedViewAccessor stream1;
	long length1 = (long)Math.Ceiling((Int32.MaxValue / 8.0)) * (Index + 1) > Length?Length:(long)Math.Ceiling((Int32.MaxValue / 8.0)) * (Index + 1);
            try { stream1 = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * Index), length1); }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            //  using (var stream1)
            // {
            int Amount = (int)Math.Ceiling((double)(Length / Int32.MaxValue));
            int Remainder = (int)(Length % Int32.MaxValue);
            int length = (int)(Amount == Index ? Math.Ceiling(Remainder / 8.0) : Math.Ceiling(Int32.MaxValue / 8.0));
            //byte[] buffer = BitArrayToByteArray(LoadedArray, 0, LoadedArray.Length);
            //stream.Position = (long)Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex;
            // BinaryWriter writer = new BinaryWriter(stream);
            // writer.Write(0, buffer);
            //stream.Position = 0;//(long)Math.Ceiling((Int32.MaxValue/8.0))*Index;
            //BinaryReader reader = new BinaryReader(stream);
            byte[] buffer = new byte[(int)Math.Floor((Int32.MaxValue / 8.0))];
            try { stream1.ReadArray<byte>(0, buffer, 0, buffer.Length); } catch { LoadedArray = new BitArray((int)(length * 8.0) > Int32.MaxValue ? Int32.MaxValue : (int)(length * 8.0),startValue); Console.WriteLine("I hate my life xdxdxdxd"); }
            LoadedArray = new BitArray(buffer);
            //LoadedByteArray = new List<byte>();
            // LoadedByteArray.AddRange(buffer);
            LoadedArrayIndex = Index;
            stream1.Dispose();
            // }
        }
        public bool this[long T]
        {

            get
            {//, MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable)
             //   using (

                //   {
                //    using (var stream = mmf.CreateViewAccessor((long)Math.Floor(T / 8.0), (long)(Math.Floor(T / 8.0)+10)))
                //    {
                //        byte[] buffer = new byte[1];
                //stream.Position = (long)Math.Floor(T / 8.0);
                //       byte lol = 0;
                //BinaryReader reader = new BinaryReader(stream);
                //       try { stream.Read<byte>(0,out lol); }
                //         catch (Exception e)
                //         {
                //             Console.WriteLine(e.Message + " I hate you");
                //             return false;

                //        }
                //        buffer[0] = lol;
                //Console.WriteLine(lol);
                //        BitArray bits = new BitArray(buffer);
                //       foreach (bool a in bits)
                //       {
                //        Console.WriteLine(a);
                //      }
                //Console.WriteLine((T % 8.0));
                //       return bits[(int)(T % 8.0)];
                //   }
                //}
                int ElementIndex = (int)Math.Floor((double)(T / (Int32.MaxValue-7)));
                int Index = (int)(T % (Int32.MaxValue-7));
                if (ElementIndex != LoadedArrayIndex)
                {
                    //Console.WriteLine("Kekxd " + ElementIndex+" "+T);
                    Save(ElementIndex);
                    Load(ElementIndex);

                }
                return LoadedArray[Index];
                // return Elements[ElementIndex][Index];
            }
            set
            {

                // using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length / 8.0)))
                //  {
                // using (var stream = mmf.CreateViewAccessor((long)Math.Floor(T / 8.0), (long)(Math.Floor(T / 8.0) + 10)))
                //  {
                //      byte[] ExistingByte = new byte[1] { Read(T, stream) };
                //
                //     BitArray bits = new BitArray(ExistingByte);
                //     bits[(int)(T % 8.0)] = value;
                //     byte[] buffer = new byte[1];
                //     buffer[0] = ConvertToByte(bits);

                // stream.Position = (long)Math.Floor(T / 8.0);
                //BinaryWriter writer = new BinaryWriter(stream);

                //    stream.Write(0,buffer[0]);
                // }
                //}
                //int ElementIndex = (int)Math.Floor((double)(T/Int32.MaxValue));
                //int Index = (int)(T%Int32.MaxValue);
                //Elements[ElementIndex][Index]=value;
                int ElementIndex = (int)Math.Floor((double)(T / (Int32.MaxValue-7)));
                int Index = (int)(T % (Int32.MaxValue-7));
                if (ElementIndex != LoadedArrayIndex)
                {
                   // Console.WriteLine("Kekxd " + ElementIndex + " " + T);
                    Save(ElementIndex);
                    Load(ElementIndex);

                }
                // byte existingbyte = LoadedByteArray[(int)Math.Floor(Index / 8.0)];
                //BitArray bits = new BitArray(existingbyte);
                //bits[(int)(Index % 8.0)] = value;
                //LoadedByteArray[(int)Math.Floor(Index / 8.0)] = ConvertToByte(bits);

                try { LoadedArray[Index] = value; }
                catch(Exception e)
                {
                    Console.WriteLine("Index: " + Index + " Collection Size: "+LoadedArray.Length+" " + e.Message);
                    Console.ReadLine();
                }
            }
        }
        public const int ByteLength = 8;
        public static byte[] BitArrayToByteArray(BitArray bits, int startIndex, int count)
        {
            // Get the size of bytes needed to store all bytes
            int bytesize = count / ByteLength;

            // Any bit left over another byte is necessary
            if (count % ByteLength > 0)
                bytesize++;

            // For the result
            byte[] bytes = new byte[bytesize];

            // Must init to good value, all zero bit byte has value zero
            // Lowest significant bit has a place value of 1, each position to
            // to the left doubles the value
            byte value = 0;
            byte significance = 1;

            // Remember where in the input/output arrays
            int bytepos = 0;
            int bitpos = startIndex;

            while (bitpos - startIndex < count)
            {
                // If the bit is set add its value to the byte
                if (bits[bitpos])
                    value += significance;

                bitpos++;

                if (bitpos % ByteLength == 0)
                {
                    // A full byte has been processed, store it
                    // increase output buffer index and reset work values
                    bytes[bytepos] = value;
                    bytepos++;
                    value = 0;
                    significance = 1;
                }
                else
                {
                    // Another bit processed, next has doubled value
                    significance *= 2;
                }
            }
            return bytes;
        }


    }
}