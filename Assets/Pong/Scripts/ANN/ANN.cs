using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANN
{

	public int numInputs; // First layer's input values.
	public int numOutputs; // Number of neurons in output layer.
	public int numHidden;  // Number of hidden layers.
	public int numNPerHidden; // Number of neurons in hidden layers.
	public double alpha; // Learning rate.
	List<Layer> layers = new List<Layer>();

	public ANN(int numInputs, int numOutputs, int numHidden, int numNPerHidden, double alpha)
	{
		this.numInputs = numInputs;
		this.numOutputs = numOutputs;
		this.numHidden = numHidden;
		this.numNPerHidden = numNPerHidden;
		this.alpha = alpha;

		if (numHidden > 0) // If there are any hidden layers.
		{
			// Input layer.
			layers.Add(new Layer(numNPerHidden, numInputs));

			// Hidden Layers
			for (int i = 0; i < numHidden - 1; i++)
			{
				layers.Add(new Layer(numNPerHidden, numNPerHidden));
			}

			// Output layers.
			layers.Add(new Layer(numOutputs, numNPerHidden));
		}
		else
		{
			layers.Add(new Layer(numOutputs, numInputs));
		}
	}

	// Train ANN with "Supervised Learning" method (Testcase's have a desired output).
	public List<double> Train(List<double> inputValues, List<double> desiredOutput)
	{
		// Calculate new output values using testcase inputs.
		List<double> outputValues = new List<double>();
		outputValues = CalcOutput(inputValues, desiredOutput);

		// Update weights and biases by new output - desired output differences.
		UpdateWeights(outputValues, desiredOutput);
		return outputValues;
	}

	// Calculating output/s with current weights and biases of neurons.
	public List<double> CalcOutput(List<double> inputValues, List<double> desiredOutput)
	{
		// Theese two lists are temps for;
		// inputs of each neuron at current layer
		// and outputs of each neuron at previous layer.
		List<double> inputs;
		List<double> outputValues = new List<double>();

		int currentInput = 0;

		// Error check.
		if (inputValues.Count != numInputs)
		{
			Debug.Log("ERROR: Number of Inputs must be " + numInputs);
			return outputValues;
		}

		// Insert first inputs to temp list.
		inputs = new List<double>(inputValues);

		// Loop through every layer.
		for (int i = 0; i < numHidden + 1; i++)
		{
			// Insert outputs of previous layer to inputs of current layer.
			if (i > 0)
			{
				inputs = new List<double>(outputValues);
			}
			outputValues.Clear();

			// Loop through every neuron in layer.
			for (int j = 0; j < layers[i].numNeurons; j++)
			{
				// Value that will go in activasion function as x.
				double N = 0;

				// Clear this neuron's input values.
				layers[i].neurons[j].inputs.Clear();

				// Loop through every input of temp inputs list.
				for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					// Insert the inputs to neuron from temp inputs list.
					layers[i].neurons[j].inputs.Add(inputs[currentInput]);

					// Add multiplication of weight and input to N.
					N += layers[i].neurons[j].weights[k] * inputs[currentInput];
					currentInput++;
				}

				// Add(substract) bias to finish the calculation of N.
				N -= layers[i].neurons[j].bias;

				// Insert N into activasion function.
				// In this example, only output layer have different activasion function..
				// But it can be different for all layers if needed.
				if (i == numHidden)
				{
					layers[i].neurons[j].output = ActivationFunctionO(N);
				}
				else
				{
					layers[i].neurons[j].output = ActivationFunction(N);
				}

				outputValues.Add(layers[i].neurons[j].output);
				currentInput = 0;
			}
		}
		return outputValues;
	}


	// Updating weights according to "Delta Rule"
	// Delta Rule is a gradient descent learning rule.
	// It is a backpropagation algorithm for a single-layer neural network with mean-square error loss function.
	// It calculates gradient descent and derivatives very quickly with it's formula.
	// For more information: https://en.wikipedia.org/wiki/Delta_rule .
	void UpdateWeights(List<double> outputs, List<double> desiredOutput)
	{
		double error;

		// Loop through every layer backwards.
		for (int i = numHidden; i >= 0; i--)
		{
			// Loop through every neuron in layer.
			for (int j = 0; j < layers[i].numNeurons; j++)
			{
				// If it is output layer.
				if (i == numHidden)
				{
					error = desiredOutput[j] - outputs[j];
					layers[i].neurons[j].errorGradient = outputs[j] * (1 - outputs[j]) * error;
				}
				else
				{
					layers[i].neurons[j].errorGradient = layers[i].neurons[j].output * (1 - layers[i].neurons[j].output);
					double errorGradSum = 0;

					// Loop through every neuron in latter layer.
					for (int p = 0; p < layers[i + 1].numNeurons; p++)
					{
						errorGradSum += layers[i + 1].neurons[p].errorGradient * layers[i + 1].neurons[p].weights[j];
					}
					layers[i].neurons[j].errorGradient *= errorGradSum;
				}

				// Loop through every input in neuron.
				for (int k = 0; k < layers[i].neurons[j].numInputs; k++)
				{
					// If it is output layer.
					if (i == numHidden)
					{
						error = desiredOutput[j] - outputs[j];
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * error;
					}
					else
					{
						layers[i].neurons[j].weights[k] += alpha * layers[i].neurons[j].inputs[k] * layers[i].neurons[j].errorGradient;
					}
				}
				layers[i].neurons[j].bias += alpha * -1 * layers[i].neurons[j].errorGradient;
			}

		}

	}

	// Print weights and biases to file for saving or transferring purposes.
	public string PrintWeights()
	{
		string weightStr = "";
		foreach (Layer l in layers)
		{
			foreach (Neuron n in l.neurons)
			{
				foreach (double w in n.weights)
				{
					weightStr += w + ",";
				}
			}
		}
		return weightStr;
	}

	// Load weights and biases from file.
	public void LoadWeights(string weightStr)
	{
		if (weightStr == "") return;
		string[] weightValues = weightStr.Split(',');
		int w = 0;
		foreach (Layer l in layers)
		{
			foreach (Neuron n in l.neurons)
			{
				for (int i = 0; i < n.weights.Count; i++)
				{
					n.weights[i] = System.Convert.ToDouble(weightValues[w]);
					w++;
				}
			}
		}
	}


	// Activasion functions part.
	double ActivationFunction(double value)
	{
		return TanH(value);
	}

	double ActivationFunctionO(double value)
	{
		return TanH(value);
	}

	double TanH(double value)
	{
		double k = (double)System.Math.Exp(-2 * value);
		return 2 / (1.0f + k) - 1;
	}

	double Sigmoid(double value)
	{
		double k = (double)System.Math.Exp(value);
		return k / (1.0f + k);
	}
}
