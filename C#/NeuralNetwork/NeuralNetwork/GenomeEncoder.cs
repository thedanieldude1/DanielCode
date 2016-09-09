using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
namespace NeuralNetwork
{
    public static class GenomeEncoder
    {
        public static int inputNeuronAmount = 6;
        public static int HiddenLayerNeuronAmount = 4;
        public static int HiddenLayerAmount = 3;
        public static int outputNeuronAmount = 2;
        public static string Encode(Network input)
        {
            StringBuilder output = new StringBuilder();
            foreach(Neuron n in input.Inputs)
            {
                foreach(Synapse t in n.TargetSynapses)
                {
                    BitArray x = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach(bool i in x)
                    {
                        output.Append(i?"1":"0");
                    }
                }
                
            }
            foreach (Neuron n in input.HiddenLayer1)
            {
                foreach (Synapse t in n.TargetSynapses)
                {
                    BitArray u = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in u)
                    {
                        output.Append(i ? "1" : "0");
                    }
                }
                foreach (Synapse t in n.RecursiveSynapses)
                {
                    BitArray u = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in u)
                    {
                        output.Append(i ? "1" : "0");
                    }
                }
                BitArray x = new BitArray(BitConverter.GetBytes(n.Threshold));
                foreach (bool i in x)
                {
                    output.Append(i ? "1" : "0");
                }
            }
            foreach (Neuron n in input.HiddenLayer2)
            {
                foreach (Synapse t in n.TargetSynapses)
                {
                    BitArray g = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in g)
                    {
                        output.Append(i ? "1" : "0");
                    }
                    
                }
                foreach (Synapse t in n.RecursiveSynapses)
                {
                    BitArray x = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in x)
                    {
                        output.Append(i ? "1" : "0");
                    }
                }
                BitArray u = new BitArray(BitConverter.GetBytes(n.Threshold));
                foreach (bool i in u)
                {
                    output.Append(i ? "1" : "0");
                }
            }
            foreach (Neuron n in input.HiddenLayer3)
            {
                foreach (Synapse t in n.TargetSynapses)
                {
                    BitArray u= new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in u)
                    {
                        output.Append(i ? "1" : "0");
                        
                    }
                    //Console.WriteLine(t.Weight);
                }
                foreach (Synapse t in n.RecursiveSynapses)
                {
                    BitArray u = new BitArray(BitConverter.GetBytes(t.Weight));
                    foreach (bool i in u)
                    {
                        output.Append(i ? "1" : "0");
                    }
                }
                
                BitArray x = new BitArray(BitConverter.GetBytes(n.Threshold));
                foreach (bool i in x)
                {
                    output.Append(i ? "1" : "0");
                }
                //Console.WriteLine(n.Threshold);
            }
            foreach (Neuron n in input.Outputs)
            {
                BitArray u = new BitArray(BitConverter.GetBytes(n.Threshold));
                foreach (bool i in u)
                {
                    output.Append(i ? "1" : "0");
                }
                //Console.WriteLine(n.Threshold);
            }
            
            return output.ToString();
        }
        public static byte[] ConvertToByte(BitArray bits)
        {
            if (bits.Count % 8!=0)
            {
                throw new ArgumentException("illegal number of bits");
            }
            byte[] output = new byte[bits.Count/8];
            for (int i = 0; i < bits.Count; i += 8)
            {
                byte b = 0;
                if (bits.Get(i+0)) b++;
                if (bits.Get(i+1)) b += 2;
                if (bits.Get(i+2)) b += 4;
                if (bits.Get(i+3)) b += 8;
                if (bits.Get(i+4)) b += 16;
                if (bits.Get(i+5)) b += 32;
                if (bits.Get(i+6)) b += 64;
                if (bits.Get(i+7)) b += 128;
                output[i / 8] = b;
            }
            return output;
        }
        public static Network Decode(string input)
        {
            List<Neuron> outputs = new List<Neuron>(outputNeuronAmount);
            int outputoffset = (int)(inputNeuronAmount * HiddenLayerNeuronAmount+ (HiddenLayerNeuronAmount*2)*HiddenLayerNeuronAmount*(HiddenLayerAmount-1)+(HiddenLayerNeuronAmount+outputNeuronAmount)*HiddenLayerNeuronAmount) * 4 ;
            char[] c = input.ToCharArray();
            BitArray bits = new BitArray(input.Length);
            for(int i = 0; i < input.Length; i++)
            {
                bits[i] = c[i] == '1' ? true : false;
            }
            byte[] bytes = ConvertToByte(bits);
            
            for (int i = 0; i < outputNeuronAmount; i++)
            {
                //Console.WriteLine(bytes.Length+" " + (outputoffset));
                float threshold = BitConverter.ToSingle(new byte[] { bytes[outputoffset + i * 4], bytes[outputoffset + i * 4 + 1], bytes[outputoffset + i * 4 + 2], bytes[outputoffset + i * 4 + 3] },0) + (new Random().Next(-10, 10) / 100.0f);
                outputs.Add(new Neuron(threshold));
                //Console.WriteLine(threshold);
            }
            int hiddenlayer3output = (int)(inputNeuronAmount * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount * 2) * HiddenLayerNeuronAmount * (HiddenLayerAmount-1)) * 4;
            List<Neuron> HiddenLayer3 = new List<Neuron>(HiddenLayerNeuronAmount);
            List<List<float>> recursiveList = new List<List<float>>(HiddenLayerNeuronAmount);
            for (int i = 0; i < HiddenLayerNeuronAmount; i++)
            {
                float threshold = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount-4], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount - 3], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount - 2], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount - 1] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                //Console.WriteLine(threshold>1||threshold<-1);
                float[] NextLayerWeights = new float[outputNeuronAmount];
                for (int x = 0; x < outputNeuronAmount; x++)
                {
                    NextLayerWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + x * 4], bytes[hiddenlayer3output +i* (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount+ x * 4 + 1], bytes[hiddenlayer3output +i* (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount+ x * 4 + 2], bytes[hiddenlayer3output +i* (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount+ x * 4 + 3] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                    //Console.WriteLine(NextLayerWeights[x]);

                }
                int hiddenlayerrecursiveoffset = hiddenlayer3output + outputNeuronAmount * 4;
                float[] RecursiveWeights = new float[HiddenLayerNeuronAmount - 1];
                for (int x = 0; x < HiddenLayerNeuronAmount - 1; x++)
                {
                    RecursiveWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + x * 4 + outputNeuronAmount*4], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + x * 4 + outputNeuronAmount * 4+1], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + x * 4 + outputNeuronAmount * 4+2], bytes[hiddenlayer3output + i * (HiddenLayerNeuronAmount + outputNeuronAmount) * HiddenLayerNeuronAmount + x * 4 + (outputNeuronAmount) * 4+3] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                }
                Neuron n = new Neuron(threshold, outputs);
                for (int x = 0; x < n.TargetSynapses.Count; x++)
                {
                    n.TargetSynapses[x].Weight = NextLayerWeights[x];

                }
                recursiveList.Add(RecursiveWeights.ToList());
                HiddenLayer3.Add(n);
            }
            int q = 0;
            int z = 0;
            foreach (Neuron n in HiddenLayer3)
            {
                n.RecursiveSynapses = new List<Synapse>();
                foreach (Neuron r in HiddenLayer3)
                {
                    if (n == r) continue;
                    //Console.WriteLine(HiddenLayer3.Count);
                    n.RecursiveSynapses.Add(new Synapse(n, r, recursiveList[q][z]));
                    z++;
                }
                z = 0;
                q++;
            }
            hiddenlayer3output = (int)(inputNeuronAmount * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount * 2) * HiddenLayerNeuronAmount * (HiddenLayerAmount - 2)) * 4;
            List<Neuron> HiddenLayer2 = new List<Neuron>(HiddenLayerNeuronAmount);
            recursiveList = new List<List<float>>(HiddenLayerNeuronAmount);
            for (int i = 0; i < HiddenLayerNeuronAmount; i++)
            {
                float threshold = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + 28], bytes[hiddenlayer3output + i * 32 + 29], bytes[hiddenlayer3output + i * 32 + 30], bytes[hiddenlayer3output + i * 32 + 31] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                //Console.WriteLine(threshold);
                //Console.WriteLine(threshold > 1 || threshold < -1);
                float[] NextLayerWeights = new float[HiddenLayerNeuronAmount];
                for (int x = 0; x < HiddenLayerNeuronAmount; x++)
                {
                    NextLayerWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount*2*HiddenLayerNeuronAmount + x * 4], bytes[hiddenlayer3output + x * 4 + 1], bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4 + 2], bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4 + 3] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                   // Console.WriteLine(NextLayerWeights[x]);
                }
                int hiddenlayerrecursiveoffset = hiddenlayer3output + HiddenLayerNeuronAmount * 4;
                float[] RecursiveWeights = new float[HiddenLayerNeuronAmount - 1];
                for (int x = 0; x < HiddenLayerNeuronAmount - 1; x++)
                {
                    RecursiveWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + x * 4 + 16], bytes[hiddenlayer3output + i * 32 + x * 4 + 17], bytes[hiddenlayer3output + i * 32 + x * 4 + 18], bytes[hiddenlayer3output + i * 32 + x * 4 + 19] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                }
                Neuron n = new Neuron(threshold, HiddenLayer3);
                for (int x = 0; x < n.TargetSynapses.Count; x++)
                {
                    n.TargetSynapses[x].Weight = NextLayerWeights[x];

                }
                recursiveList.Add(RecursiveWeights.ToList());
                HiddenLayer2.Add(n);
            }
            q = 0;
            z = 0;
            foreach (Neuron n in HiddenLayer2)
            {
                n.RecursiveSynapses = new List<Synapse>();
                foreach (Neuron r in HiddenLayer2)
                {
                    if (n == r) continue;
                    //Console.WriteLine(HiddenLayer3.Count);
                    n.RecursiveSynapses.Add(new Synapse(n, r, recursiveList[q][z]));
                    z++;
                }
                z = 0;
                q++;
            }
            hiddenlayer3output = (int)(inputNeuronAmount * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount * 2) * HiddenLayerNeuronAmount * (HiddenLayerAmount - 3)) * 4;
            List<Neuron> HiddenLayer1 = new List<Neuron>(HiddenLayerNeuronAmount);
            recursiveList = new List<List<float>>(HiddenLayerNeuronAmount);
            for (int i = 0; i < HiddenLayerNeuronAmount; i++)
            {
                float threshold = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + 28], bytes[hiddenlayer3output + i * 32 + 29], bytes[hiddenlayer3output + i * 32 + 30], bytes[hiddenlayer3output + i * 32 + 31] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                //Console.WriteLine(threshold);
                //Console.WriteLine(threshold > 1 || threshold < -1);
                float[] NextLayerWeights = new float[HiddenLayerNeuronAmount];
                for (int x = 0; x < HiddenLayerNeuronAmount; x++)
                {
                    NextLayerWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output  +i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4], bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4 + 1], bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4 + 2], bytes[hiddenlayer3output + i * HiddenLayerNeuronAmount * 2 * HiddenLayerNeuronAmount + x * 4 + 3] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                    //Console.WriteLine(NextLayerWeights[x]);
                }
                int hiddenlayerrecursiveoffset = hiddenlayer3output + HiddenLayerNeuronAmount * 4;
                float[] RecursiveWeights = new float[HiddenLayerNeuronAmount - 1];
                for (int x = 0; x < HiddenLayerNeuronAmount - 1; x++)
                {
                    RecursiveWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + x * 4 + 16], bytes[hiddenlayer3output + i * 32 + x * 4 + 17], bytes[hiddenlayer3output + i * 32 + x * 4 + 18], bytes[hiddenlayer3output + i * 32 + x * 4 + 19] }, 0) + (new Random().Next(-10, 10) / 100.0f);
                }
                Neuron n = new Neuron(threshold, HiddenLayer2);
                for (int x = 0; x < n.TargetSynapses.Count; x++)
                {
                    n.TargetSynapses[x].Weight = NextLayerWeights[x];

                }
                recursiveList.Add(RecursiveWeights.ToList());
                HiddenLayer1.Add(n);
            }
            q = 0;
            z = 0;
            foreach (Neuron n in HiddenLayer1)
            {
                n.RecursiveSynapses = new List<Synapse>();
                foreach (Neuron r in HiddenLayer1)
                {
                    if (n == r) continue;
                    //Console.WriteLine(HiddenLayer3.Count);
                    n.RecursiveSynapses.Add(new Synapse(n, r, recursiveList[q][z]));
                    z++;
                }
                z = 0;
                q++;
            }
            
            hiddenlayer3output = 0;//(int)(inputNeuronAmount * HiddenLayerNeuronAmount + (HiddenLayerNeuronAmount * 2) * HiddenLayerNeuronAmount * (HiddenLayerAmount - 2)) * 4;
            List<Neuron> inputs = new List<Neuron>(inputNeuronAmount);
            //recursiveList = new List<List<float>>(HiddenLayerNeuronAmount);
            for (int i = 0; i < inputNeuronAmount; i++)
            {
                float threshold = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + 28], bytes[hiddenlayer3output + i * 32 + 29], bytes[hiddenlayer3output + i * 32 + 30], bytes[hiddenlayer3output + i * 32 + 31] }, 0) + (new Random().Next(-10, 10) / 100.0f) + (new Random().Next(-10, 10) / 100.0f);
                //Console.WriteLine(threshold);
                //Console.WriteLine(threshold > 1 || threshold < -1);
                float[] NextLayerWeights = new float[HiddenLayerNeuronAmount];
                for (int x = 0; x < HiddenLayerNeuronAmount; x++)
                {
                    NextLayerWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * inputNeuronAmount * HiddenLayerNeuronAmount + x * 4], bytes[hiddenlayer3output + i * inputNeuronAmount * HiddenLayerNeuronAmount + x * 4 + 1], bytes[hiddenlayer3output + i * inputNeuronAmount * HiddenLayerNeuronAmount + x * 4 + 2], bytes[hiddenlayer3output + i * inputNeuronAmount * HiddenLayerNeuronAmount + x * 4 + 3] }, 0)+ (new Random().Next(-10, 10) / 100.0f) + (new Random().Next(-10, 10) / 100.0f);
                    //Console.WriteLine(NextLayerWeights[x]);
                }
                //int hiddenlayerrecursiveoffset = hiddenlayer3output + outputNeuronAmount * 4;
                //float[] RecursiveWeights = new float[outputNeuronAmount - 1];
                //for (int x = 0; x < outputNeuronAmount - 1; x++)
                //{
                 //   RecursiveWeights[x] = BitConverter.ToSingle(new byte[] { bytes[hiddenlayer3output + i * 32 + x * 4 + 16], bytes[hiddenlayer3output + i * 32 + x * 4 + 17], bytes[hiddenlayer3output + i * 32 + x * 4 + 18], bytes[hiddenlayer3output + i * 32 + x * 4 + 19] }, 0);
                //}
                Neuron n = new Neuron(threshold, HiddenLayer1);
                for (int x = 0; x < n.TargetSynapses.Count; x++)
                {
                    n.TargetSynapses[x].Weight = NextLayerWeights[x];

                }
                //recursiveList.Add(RecursiveWeights.ToList());
                inputs.Add(n);
            }
            return new Network(inputNeuronAmount, HiddenLayerNeuronAmount, outputNeuronAmount) {
                Inputs = inputs,
                Outputs = outputs,
                HiddenLayer1 = HiddenLayer1,
                HiddenLayer2 = HiddenLayer2,
                HiddenLayer3 =HiddenLayer3
            
            };
        }

        public static string Combine(string a,string b)
        {
            int x = 0;
            StringBuilder output = new StringBuilder();
            int random = (new Random()).Next(0, a.Length/32-1);
            for(int i = 0; i < a.Length; i+=32)
            {
                if ((i)/32.0 < random)
                {
                    for (int c = i; c < i + 32; c++)
                    {
                        output.Append(a[c]);
                    }
                }
                else
                {
                    for (int c = i; c < i + 32; c++)
                    {
                        output.Append(b[c]);
                    }
                
                    
                    
                }
            }
            return output.ToString();
        }
    }
}
