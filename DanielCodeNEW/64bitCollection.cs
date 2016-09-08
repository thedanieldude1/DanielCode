using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.IO.MemoryMappedFiles;
public class LongBitArray{
        BitArray LoadedArray;
	int LoadedArrayIndex;
	public long Length{get;private set;}
	
	public LongBitArray(long length){
		Length=length;
		int Amount = (int)Math.Ceiling((double)(length/Int32.MaxValue));
		int Remainder = (int)(length%Int32.MaxValue);
		LoadedArray=new BitArray(Amount>1?Int32.MaxValue:Remainder);
		LoadedArrayIndex=0;
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
    		if (bits.Get(7)) b++;
    		if (bits.Get(6)) b += 2;
    		if (bits.Get(5)) b += 4;
    		if (bits.Get(4)) b += 8;
    		if (bits.Get(3)) b += 16;
    		if (bits.Get(2)) b += 32;
    		if (bits.Get(1)) b += 64;
    		if (bits.Get(0)) b += 128;
    		return b;
	}
	public byte Read(long T,MemoryMappedViewStream stream){

				byte[] buffer = new byte[1];
                		BinaryReader reader = new BinaryReader(stream);
     				try{reader.Read(buffer,(int)Math.Floor(T/8.0),1);}
				catch{return default(byte);}
				
				return buffer[0];
           		
			
	}
	public void Load(int Index){
			MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType
.Allow));
			using(MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length/8.0), MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable)){
			using (MemoryMappedViewStream stream = mmf.CreateViewStream())
           		{
				int Amount = (int)Math.Ceiling((double)(Length/Int32.MaxValue));
				int Remainder = (int)(Length%Int32.MaxValue);
				int length = (int)(Amount==Index?Math.Ceiling(Remainder/8.0):Math.Ceiling(Int32.MaxValue/8.0));
				byte[] buffer = BitArrayToByteArray(LoadedArray,0,LoadedArray.Length);
				stream.Position=(long)Math.Ceiling((Int32.MaxValue/8.0))*LoadedArrayIndex;
                		BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(buffer);
				stream.Position=0;//(long)Math.Ceiling((Int32.MaxValue/8.0))*Index;
				BinaryReader reader = new BinaryReader(stream);
				buffer = new byte[(int)Math.Ceiling((Int32.MaxValue/8.0))];
				try{reader.Read(buffer,(int)Math.Ceiling((Int32.MaxValue/8.0))*Index,(int)Math.Ceiling((Int32.MaxValue/8.0)));} catch{LoadedArray = new BitArray((int)(length*8.0));}
           		}
			}
			//int ElementIndex = (int)Math.Floor((double)(T/Int32.MaxValue));
			//int Index = (int)(T%Int32.MaxValue);
			//Elements[ElementIndex][Index]=value;
		//}
	}
	public bool this[long T]{
		get{
			MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType
.Allow));
			using(MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length/8.0), MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable)){
			using (MemoryMappedViewStream stream = mmf.CreateViewStream())
           		{
				byte[] buffer = new byte[1];
                		BinaryReader reader = new BinaryReader(stream);
     				try{reader.Read(buffer,(int)Math.Floor(T/8.0),1);}
				catch{return false;}
				
				BitArray bits = new BitArray(buffer);
				return bits[(int)(T%8.0)];
           		}
			}
			//int ElementIndex = (int)Math.Floor((double)(T/Int32.MaxValue));
			//int Index = (int)(T%Int32.MaxValue);
			//return Elements[ElementIndex][Index];
		}
		set{
			MemoryMappedFileSecurity CustomSecurity = new MemoryMappedFileSecurity();
CustomSecurity.AddAccessRule(new System.Security.AccessControl.AccessRule<MemoryMappedFileRights>("everyone", MemoryMappedFileRights.FullControl, System.Security.AccessControl.AccessControlType
.Allow));
			using(MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("bits", (long)Math.Ceiling(Length/8.0), MemoryMappedFileAccess.ReadWriteExecute, MemoryMappedFileOptions.None, CustomSecurity, System.IO.HandleInheritability.Inheritable)){
			using (MemoryMappedViewStream stream = mmf.CreateViewStream(0,(long)Math.Ceiling(Length/8.0),MemoryMappedFileAccess.ReadWrite))
           		{
				byte[] ExistingByte=new byte[1]{Read(T,stream)};
				BitArray bits = new BitArray(ExistingByte);
				bits[(int)(T%8.0)]=value;
				byte[] buffer = new byte[1]{ConvertToByte(bits)};
				stream.Position=(long)Math.Floor(T/8.0);
                		BinaryWriter writer = new BinaryWriter(stream);
				writer.Write(buffer);
           		}
			}
			//int ElementIndex = (int)Math.Floor((double)(T/Int32.MaxValue));
			//int Index = (int)(T%Int32.MaxValue);
			//Elements[ElementIndex][Index]=value;
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
public static class main{
	public static void Main(string[] args){
	LongBitArray test = new LongBitArray(5000000000);
test[2000000000]=true;
	Console.WriteLine(test[2000000000]);

	Console.ReadLine();
}
}