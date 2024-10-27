using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// One layer of neural network.
public class Layer {

	public int numNeurons;
	public List<Neuron> neurons = new List<Neuron>();

	public Layer(int numNeurons, int numNeuronInputs)
	{
		this.numNeurons = numNeurons;

		// number of neuron inputs means, number of neurons on the previous layer.
		for(int i = 0; i < numNeurons; i++)
		{
			neurons.Add(new Neuron(numNeuronInputs));
		}
	}
}
