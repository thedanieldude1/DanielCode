using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.IO.MemoryMappedFiles;
using System.Numerics;
namespace LargeCollections
{
    #region Longbit
    public class LongBitArray
{
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
        LoadedArray = new BitArray(Amount >= 1 ? Int32.MaxValue : Remainder);
        LoadedArrayIndex = 0;
        mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Length);/// 8.0);
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
        using (var stream = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex), (long)Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex + (long)Math.Ceiling((Int32.MaxValue / 8.0))))
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
        try { stream1 = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * Index), (long)Math.Ceiling((Int32.MaxValue / 8.0)) * (Index + 1)); }
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
        try { stream1.ReadArray<byte>(0, buffer, 0, buffer.Length); } catch { LoadedArray = new BitArray((int)(length * 8.0) > Int32.MaxValue ? Int32.MaxValue : (int)(length * 8.0)); }
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
            int ElementIndex = (int)Math.Floor((double)(T / Int32.MaxValue));
            int Index = (int)(T % Int32.MaxValue);
            if (ElementIndex != LoadedArrayIndex)
            {
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
            int ElementIndex = (int)Math.Floor((double)(T / Int32.MaxValue));
            int Index = (int)(T % Int32.MaxValue);
            if (ElementIndex != LoadedArrayIndex)
            {
                Save(ElementIndex);
                Load(ElementIndex);
            }
           // byte existingbyte = LoadedByteArray[(int)Math.Floor(Index / 8.0)];
            //BitArray bits = new BitArray(existingbyte);
            //bits[(int)(Index % 8.0)] = value;
            //LoadedByteArray[(int)Math.Floor(Index / 8.0)] = ConvertToByte(bits);
            LoadedArray[Index] = value;
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
    #endregion
    public class BigIntBitArray
    {
        BitArray LoadedArray;
        // List<byte> LoadedByteArray;
        int LoadedArrayIndex;
        public BigInteger Length { get; private set; }
      //  MemoryMappedFile mmf;
        public BigIntBitArray(BigInteger length)
        {
            Length = length;
            int Amount = (int)Math.Ceiling((double)(length / Int32.MaxValue));
            int Remainder = (int)(length % Int64.MaxValue % Int32.MaxValue);
            LoadedArray = new BitArray(Amount > 1 ? Int32.MaxValue : Remainder, true);
            LoadedArrayIndex = 0;
          //  mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Length);/// 8.0);
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
        public void Save(int Index,int ElementIndex)
        {
            // MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
            // CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType
            // .Allow));
            using (MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length / 8.0), MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable))
            {
            using (var stream = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex), (long)Math.Ceiling((Int32.MaxValue / 8.0)) * LoadedArrayIndex + (long)Math.Ceiling((Int32.MaxValue / 8.0))))
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
            try { stream1 = mmf.CreateViewAccessor((long)(Math.Ceiling((Int32.MaxValue / 8.0)) * Index), (long)Math.Ceiling((Int32.MaxValue / 8.0)) * (Index + 1)); }
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
            try { stream1.ReadArray<byte>(0, buffer, 0, buffer.Length); } catch { LoadedArray = new BitArray((int)(length * 8.0) > Int32.MaxValue ? Int32.MaxValue : (int)(length * 8.0), true); }
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
                int ElementIndex = (int)Math.Floor((double)(T / Int64.MaxValue));
                int BitIndex = (int)Math.Floor((double)(T % Int64.MaxValue / Int32.MaxValue));
                int Index = (int)(T % Int64.MaxValue % Int32.MaxValue);
                if (ElementIndex != LoadedArrayIndex)
                {
                    Save(BitIndex,ElementIndex);
                    Load(BitIndex,ElementIndex);
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
                int ElementIndex = (int)Math.Floor((double)(T / Int32.MaxValue));
                int Index = (int)(T % Int32.MaxValue);
                if (ElementIndex != LoadedArrayIndex)
                {
                    Save(ElementIndex);
                    Load(ElementIndex);
                }
                // byte existingbyte = LoadedByteArray[(int)Math.Floor(Index / 8.0)];
                //BitArray bits = new BitArray(existingbyte);
                //bits[(int)(Index % 8.0)] = value;
                //LoadedByteArray[(int)Math.Floor(Index / 8.0)] = ConvertToByte(bits);
                LoadedArray[Index] = value;
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
public static class main
{
    public static void Main(string[] args)
    {
        LongBitArray test = new LongBitArray(5000000000);
        test[30] = true;
        test[31] = true;
        Console.WriteLine(test[30] + " " + test[31]);
        test[(long)Int32.MaxValue + 30] = true;
        test[(long)Int32.MaxValue + 31] = true;

        Console.WriteLine(test[(long)Int32.MaxValue + 30] + " " + test[(long)Int32.MaxValue + 31]);
        Console.ReadLine();
    }
}