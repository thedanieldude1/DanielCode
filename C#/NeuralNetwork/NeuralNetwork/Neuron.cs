using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Xml;
using System.Xml.Serialization;
namespace NeuralNetwork
{
    public delegate void NeuronFire(Neuron sender, float Amount);
    public delegate void NeuronRecieve(Neuron sender, float Amount);
    [Serializable]
    public class ISynaptable
    {
        public virtual float input { get; set; }
        public virtual float output { get; set; }
    }
    [Serializable]
    public class Synapse
    {
        [XmlIgnore]
        public ISynaptable SourceNeuron;
        [XmlIgnore]
        public ISynaptable TargetNeuron;
        private float weight=0;
        public float Weight
        {
            get { return weight; }
            set
            {
                
                if (value > 1 || value < -1)
                {
                    weight= ((float)(1 / (1 + Math.Pow(Math.E, (-value) / 1f)))) * 2 - 1;
                    //Console.WriteLine("It is out of range oh nooooooooooooooooooooooooooooooo");
                }
                else
                {
                    weight = value;
                }
            }
        }
        public float WeightDelta;
        public void Update()
        {
            TargetNeuron.input += SourceNeuron.output * Weight;
        }
        public Synapse(ISynaptable SourceNeuron, ISynaptable TargetNeuron,float Weight)
        {
            this.SourceNeuron = SourceNeuron;
            this.TargetNeuron = TargetNeuron;
            this.Weight = Weight;
        }
        public Synapse Flipped()
        {
            return new Synapse(TargetNeuron, SourceNeuron, Weight);
        }
        public Synapse() { }
    }
    [Serializable]
    public class Neuron : ISynaptable
    {
        public event NeuronFire NeuronFireEvent;
        public event NeuronRecieve NeuronRecieveEvent;
        public List<Neuron> ForwardConnections;
        public List<float> Weights;
        public float threshold=0;
        public float Threshold
        {
            get { return threshold; }
            set
            {
                if (value > 1 || value < -1)
                {
                    threshold = ((float)(1 / (1 + Math.Pow(Math.E, (-value) / 1f)))) * 2 - 1;

                }
                else
                {
                    threshold = value;
                }
            }
        }
        public override float input { get; set; }
        public override float output { get; set; }
        public bool isOutput=true;
        float curActivationAmount;
        float lastOutput;
        float ContextWeight;
        float BiasDelta;
        float Gradient;
        public float RecursiveBuffer = 0;
        public List<Synapse> SourceSynapses=new List<Synapse>();
        public List<Synapse> TargetSynapses;
        public List<Synapse> RecursiveSynapses = new List<Synapse>();
        public void Update()
        {
            foreach(Synapse syn in SourceSynapses)
            {
                syn.Update();
            }
        }
        public void UpdateRecursiveBuffer(float value)
        {
            foreach(Synapse s in RecursiveSynapses)
            {
                ((Neuron)s.TargetNeuron).RecursiveBuffer += value*s.Weight;
            }
        }

        public Neuron(float threshold)
        {
            ForwardConnections = new List<Neuron>();
            Weights = new List<float>();
            TargetSynapses = new List<Synapse>();
            this.Threshold = threshold;
            ContextWeight = new Random().Next(-100, 100);
        }

        public Neuron(float threshold,List<Neuron> neurons)
        {
            ForwardConnections = neurons;
            Weights= new List<float>(neurons.Count);
            Random random = new Random();
            TargetSynapses = new List<Synapse>(neurons.Count);
            for (int i = 0; i < neurons.Count; i++)
            {
                //= new Random(i*i*i*423);
                Weights.Add((float)(random.Next(-100, 100) / 100.0));
                TargetSynapses.Add(new Synapse(this, neurons[i], Weights[i]));
                neurons[i].SourceSynapses.Add(new Synapse(this, neurons[i], Weights[i]));
            }
            ContextWeight = new Random().Next(-100, 100);
            this.Threshold = threshold;
        }
        public Neuron(float threshold, List<Neuron> neurons,List<float> weights)
        {
            ForwardConnections = neurons;
            Weights = weights;
            ContextWeight = new Random().Next(-100, 100);
            this.Threshold = threshold;
        }
        public void CalculateSourceSynapses(List<Neuron> previousLayer)
        {
            List<Synapse> test = new List<Synapse>();
            foreach(Neuron n in previousLayer)
            {
                List<Synapse> x = n.TargetSynapses.Where(t=>t.TargetNeuron==this).ToList<Synapse>();
                x.ForEach(s => {
                    s = s.Flipped();
                });
                test.AddRange(x);
            }
            SourceSynapses = test;
        }
        public void Activate(Neuron Activator,float ActivationAmount)
        {
            curActivationAmount = input;
           // if (curActivationAmount >= Threshold||isOutput)
           // {
                //Console.WriteLine(curActivationAmount);
                float x = isOutput ? Threshold : Threshold;
                Fire(((float)(1/(1+Math.Pow(Math.E,(-curActivationAmount-x)/1f)))) * 2 - 1);
                //curActivationAmount = 0;
                
           // }
           // else
           // {
                //Fire(0.5f);
           //     Console.WriteLine("Value " + curActivationAmount + " didn't make it against "+Threshold);
           // }
            if (NeuronRecieveEvent != null)
            {
                NeuronRecieveEvent(this, input);
            }
            //NeuronRecieveEvent(this, ActivationAmount);
          //  Console.WriteLine("Neuron Triggered: " + ActivationAmount);
        }
        public Neuron()
        {

        }
        public void Fire(float amount)
        {
            output = amount;
            lastOutput = output;
            foreach (Synapse syn in TargetSynapses)
            {
                syn.Update();
               // Console.WriteLine(curActivationAmount + " Fired xd xd xdd xxd " + syn.TargetNeuron.input);
            }
            //lastOutput = output;
            // for (int i = 0; i < ForwardConnections.Count; i++) {
            //ForwardConnections[i].Activate(this,amount*Weights[i]);
            //Console.WriteLine(Weights[i]);

            // }
            if (NeuronFireEvent != null)
            {
                NeuronFireEvent(this, amount);
                //Console.WriteLine("Fired With " + amount);
            }
            // Console.WriteLine("Neuron Fired: " + amount);
        }
        public static float SigmoidDerivative(float f)
        {
            return f * (1 - f);
        }

        public float CalculateGradient(float target)
        {
            return Gradient = (target-output) * SigmoidDerivative(output);
        }

        public float CalculateGradient()
        {
            return Gradient = TargetSynapses.Sum(a => ((Neuron)a.TargetNeuron).Gradient * a.Weight) * SigmoidDerivative(output);
        }

        public void UpdateWeights(float learnRate, float momentum)
        {
            var prevDelta = BiasDelta;
            BiasDelta = learnRate * Gradient*output; // * 1
            Threshold += BiasDelta + momentum * prevDelta;
            //Console.WriteLine("Gradient: "+ Gradient+" ");
            foreach (var s in SourceSynapses)
            {
                prevDelta = s.WeightDelta;
                Synapse k = ((Neuron)s.SourceNeuron).TargetSynapses.Find(x => x.TargetNeuron == this);
                s.WeightDelta = learnRate * Gradient * s.SourceNeuron.output;
                k.WeightDelta = s.WeightDelta;
                
                
                s.Weight += s.WeightDelta + momentum * prevDelta;
                k.Weight = s.Weight;
                
            }
        }

    }
    public class LSTMGate : ISynaptable
    {
        
        public float input { get; set; }
        public float output { get; set; }
        public LSTMGate()
        {

        }
        public void Process(float input)
        {
            this.input = input;
            output = (float)(1 / (1 + Math.Pow(Math.E, (-input) / 1f)));
        }
    }
    public class LSTMInput : ISynaptable
    {

        public float input { get; set; }
        public float output { get; set; }
        public LSTMInput()
        {

        }
        public void Process(float input)
        {
            this.input = input;
            output = (float)(1 / (1 + Math.Pow(Math.E, (-input) / 1f)));
        }
    }
    public class LSTMCell:ISynaptable
    {
        public LSTMInput Input;
        public LSTMGate InputGate = new LSTMGate();
        public LSTMGate ForgetGate = new LSTMGate();
        public LSTMGate OutputGate = new LSTMGate();
        public Neuron Output;
        Random random = new Random();
        public List<Synapse> OutputSynapses;
        public float output { get; set; }
        public float input { get; set; }
        public float currentInternalState;
        public LSTMCell(float threshold, List<LSTMCell> cells)
        {
            OutputSynapses = new List<Synapse>(cells.Count * 4);
            foreach (var cell in cells)
            {
                OutputSynapses.Add(new Synapse(this,cell.Input, (float)(random.Next(-100, 100) / 100.0)));
                OutputSynapses.Add(new Synapse(this, cell.InputGate, (float)(random.Next(-100, 100) / 100.0)));
                OutputSynapses.Add(new Synapse(this, cell.ForgetGate, (float)(random.Next(-100, 100) / 100.0)));
                OutputSynapses.Add(new Synapse(this, cell.Output, (float)(random.Next(-100, 100) / 100.0)));
            }
            
        }
    }
    [Serializable]
    public class Network
    {
        public bool isTraining = false;
        public List<Neuron> Inputs;
        public List<Neuron> HiddenLayer1;
        public List<Neuron> HiddenLayer2;
        public List<Neuron> HiddenLayer3;
        public List<Neuron> Outputs;
        public Network() { }
        public Network(int InputLayerAmount, int HiddenLayerAmount, int OutputLayerAmount)
        {
            Outputs = new List<Neuron>(OutputLayerAmount);
            Random random = new Random();
            for (int i = 0;i< OutputLayerAmount; i++)
            {
                float randomval = (float)(random.Next(-100,100)/100.0);
                Outputs.Add(new Neuron(0) { isOutput = true });
               // Console.WriteLine(randomval);
                Outputs[i].NeuronFireEvent += OnOutputFire;
            }


            HiddenLayer3 = new List<Neuron>(HiddenLayerAmount);
            for (int i = 0; i < HiddenLayerAmount; i++)
            {
                //  Random random = new Random(i);
                HiddenLayer3.Add(new Neuron((float)(random.Next(-100, 100) / 100.0), Outputs));
                //HiddenLayer3[i].NeuronFireEvent += OnHiddenFire;
                
            }
            foreach(Neuron n in HiddenLayer3)
            {
                foreach(Neuron x in HiddenLayer3)
                {
                    if (x == n)
                    {
                        continue;
                    }
                    n.RecursiveSynapses.Add(new Synapse(n, x, (float)(random.Next(-100, 100) / 100.0)));
                }
            }
           // foreach (Neuron n in Outputs)
           // {
               // n.CalculateSourceSynapses(HiddenLayer3);
           // }
            HiddenLayer2 = new List<Neuron>(HiddenLayerAmount);
            for (int i = 0; i < HiddenLayerAmount; i++)
            {
                //  Random random = new Random(i);
                HiddenLayer2.Add(new Neuron((float)(random.Next(-100, 100) / 100.0),HiddenLayer3));
               // HiddenLayer2[i].NeuronFireEvent += OnHiddenFire;
            }
            //  foreach (Neuron n in HiddenLayer3)
            //{
            //    n.CalculateSourceSynapses(HiddenLayer2);
            //}
            foreach (Neuron n in HiddenLayer2)
            {
                foreach (Neuron x in HiddenLayer2)
                {
                    if (x == n)
                    {
                        continue;
                    }
                    n.RecursiveSynapses.Add(new Synapse(n, x, (float)(random.Next(-100, 100) / 100.0)));
                }
            }
            HiddenLayer1 = new List<Neuron>(HiddenLayerAmount);
            for (int i = 0; i < HiddenLayerAmount; i++)
            {
                //  Random random = new Random(i);
                HiddenLayer1.Add(new Neuron((float)(random.Next(-100, 100) / 100.0),  HiddenLayer2));
                //HiddenLayer1[i].NeuronFireEvent += OnHiddenFire;
            }
            //foreach (Neuron n in HiddenLayer2)
            //{
            //     n.CalculateSourceSynapses(HiddenLayer1);
            // }
            foreach (Neuron n in HiddenLayer1)
            {
                foreach (Neuron x in HiddenLayer1)
                {
                    if (x == n)
                    {
                        continue;
                    }
                    n.RecursiveSynapses.Add(new Synapse(n, x, (float)(random.Next(-100, 100) / 100.0)));
                }
            }
            Inputs = new List<Neuron>(InputLayerAmount);
            for (int i = 0; i < InputLayerAmount; i++)
            {
               // Random random = new Random(i);
                Inputs.Add(new Neuron((float)(random.Next(-100, 100) / -100.0), HiddenLayer1));
              //  Inputs[i].NeuronFireEvent += OnInputFire;
            }
        }
        public void Fire(int InputNeuron,float input)
        {
            Neuron Input = Inputs[InputNeuron];
            Input.Fire(input);
            foreach(Neuron n in HiddenLayer1)
            {
                Console.WriteLine(n.RecursiveBuffer);
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer1)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            foreach (Neuron n in HiddenLayer2)
            {
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer2)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            foreach (Neuron n in HiddenLayer3)
            {
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer3)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            foreach (Neuron n in Outputs)
            {


                n.Activate(Input, input);

                n.input = 0;
            }
        }
        public void BackPropagate(float[] targets,float LearnRate,float Momentum)
        {
            int i = 0;
            Outputs.ForEach(a => a.CalculateGradient(targets[i++]));
            HiddenLayer3.ForEach(a => a.CalculateGradient());
            HiddenLayer3.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
            HiddenLayer2.ForEach(a => a.CalculateGradient());
            HiddenLayer2.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
            HiddenLayer1.ForEach(a => a.CalculateGradient());
            HiddenLayer1.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
            Outputs.ForEach(a => a.UpdateWeights(LearnRate, Momentum));
        }

        public float[] Fire(float[] InputNeuron)
        {
            float[] outputs = new float[Outputs.Count];
            Neuron Input = Inputs[0];
            float input = InputNeuron[0];
            for (int i = 0; i < InputNeuron.Length; i++)
            {
                Inputs[i].Fire(InputNeuron[i]);
            }
            foreach (Neuron n in HiddenLayer1)
            {
               // Console.WriteLine(n.RecursiveBuffer);
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer1)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            foreach (Neuron n in HiddenLayer2)
            {
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer2)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            foreach (Neuron n in HiddenLayer3)
            {
                n.input += n.RecursiveBuffer;
                n.RecursiveBuffer = 0;
                n.Activate(Input, input);

                n.input = 0;
            }
            foreach (Neuron n in HiddenLayer3)
            {
                n.UpdateRecursiveBuffer(n.output);
            }
            int x = 0;
            foreach (Neuron n in Outputs)
            {

                n.Activate(Input, input);
                n.input = 0;
                outputs[x] = n.output;
                x++;
            }
            return outputs;
        }
        public void OnOutputFire(Neuron Activator,float amount)
        {
            if (!isTraining)
            {
                //Console.WriteLine(Outputs.IndexOf(Activator) + " of output layer was activated with " + amount + " " + Activator.Threshold);
            }
        }
        public void OnHiddenFire(Neuron Activator, float amount)
        {
            Console.WriteLine( " of hidden layer was activated with " + amount + " " + Activator.Threshold);
        }
        public void OnInputFire(Neuron Activator, float amount)
        {
            Console.WriteLine(Inputs.IndexOf(Activator) + " of input layer was activated with " + amount + " " + Activator.Threshold);
        }
    }
}
