using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Single neuron of a layer.
public class Neuron {

	public int numInputs;
	public double bias;
	public double output;
	public double errorGradient;
	public List<double> weights = new List<double>();
	public List<double> inputs = new List<double>();

	public Neuron(int numInputs)
	{
		this.numInputs = numInputs;

		// To initialize random values to bias and weights, a number range is needed.
		// Range is wider if there are fewer inputs (neuron number of previous layer).
		float weightRange = (float) 2.4/(float) numInputs;
		bias = UnityEngine.Random.Range(-weightRange,weightRange);

		for(int i = 0; i < numInputs; i++)
			weights.Add(UnityEngine.Random.Range(-weightRange,weightRange));
	}
}
