using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    //This will be a neural network trained to play tetris
    public class Network
    {
        public int iteration = 0;

        public List<Layer> layers;

        public Network(int type, int inputSize, int outputSize)
        {
            layers = new List<Layer>() { new Layer(0, inputSize) };

            if (type == 0)
            {
                layers.Add(new Layer(1, inputSize, layers[0]));
                layers.Add(new Layer(1, inputSize, layers[1]));

                int newSize = inputSize / 2;

                while (newSize > outputSize)
                {
                    layers.Add(new Layer(1, newSize, layers.Last()));
                    newSize /= 2;
                }
            }
        }
    }

    public class Layer
    {
        public Neuron[] neurons;

        //This is the constructor of the layer
        public Layer(int activationFunction, int neuronCount, Layer previousLayer = null)
        {
            neurons = new Neuron[neuronCount];
            for (int i = 0; i < neuronCount; i++)
            {
                neurons[i] = new Neuron(activationFunction, previousLayer);
            }
        }
    }

    public class Neuron
    {
        public int activationFunction;
        public double output;

        //These are the current weights and bias of the neuron
        public double bias;
        public Weight[] weights;

        //These are the values that will be used to calculate the gradient of the neuron
        public List<double> biasGradients;

        //This is the constructor of the neuron
        public Neuron(int activationFunction, Layer previousLayer)
        {
            this.activationFunction = activationFunction;
            bias = 0.01;

            biasGradients = new List<double>();

            InitWeights(previousLayer);
        }

        //This function will initialize the weights using Xavier initialization
        void InitWeights(Layer previousLayer)
        {
            weights = new Weight[previousLayer.neurons.Length];

            double variance = 1.0 / weights.Length;

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = new Weight(previousLayer.neurons[i], variance);
            }
        }

        //This function will calculate the output of the neuron
        public void CalculateOutput()
        {
            double weightedSum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                weightedSum += weights[i].weight * weights[i].connectedNeuron.output;
            }

            //0 is the linear activation function
            if (activationFunction == 0)
            {
                output = weightedSum + bias;
            }
            //1 is the ReLU activation function
            else if (activationFunction == 1)
            {
                output = Math.Max(0, weightedSum + bias);
            }
            //2 is the leaky ReLU activation function
            else if (activationFunction == 2)
            {
                output = Math.Max(0.01 * (weightedSum + bias), weightedSum + bias);
            }
            //3 is the sigmoid activation function
            else if (activationFunction == 3)
            {
                output = 1 / (1 + Math.Exp(-(weightedSum + bias)));
            }
            //4 is the tanh activation function
            else if (activationFunction == 4)
            {
                output = Math.Tanh(weightedSum + bias);
            }
            //5 is the softmax activation function
            else if (activationFunction == 5)
            {
                double sum = 0;
                for (int i = 0; i < weights.Length; i++)
                {
                    sum += Math.Exp(weights[i].weight * weights[i].connectedNeuron.output);
                }

                output = Math.Exp(weightedSum + bias) / sum;
            }
        }
    }

    //This class is used to represent a weight connection between two neurons
    public class Weight
    {
        public double weight;
        public Neuron connectedNeuron;

        public List<double> gradients;

        //This is the constructor of the weight
        public Weight(Neuron connectedNeuron, double variance)
        {
            this.connectedNeuron = connectedNeuron;
            double tempWeight = -1;
            while (tempWeight < 0 | tempWeight > 2 * variance) { tempWeight = Program.rng.NextDouble(); }
            weight = tempWeight;

            gradients = new List<double>();
        }

        //This function will update the weight using the gradients collected from the training batch
        public void UpdateWeight(double learningRate)
        {
            double sum = 0;
            for (int i = 0; i < gradients.Count; i++)
            {
                sum += gradients[i];
            }

            weight -= learningRate * sum / gradients.Count();
            gradients.Clear();
        }
    }
}
