namespace ConsoleGOPS
{
    public class NeuralNetwork
    {
        NeuralNetMatrix[] _weights;
        NeuralNetMatrix[] _biases;
        float _learningRate;

        public NeuralNetwork()
        {
            _weights = new NeuralNetMatrix[0];
            _biases = new NeuralNetMatrix[0];
            _learningRate = 0.0f;
        }

        public NeuralNetwork(string path)
        {
            _weights = new NeuralNetMatrix[0];
            _biases = new NeuralNetMatrix[0];
            _learningRate = 0.0f;
            Load(path);
        }

        public NeuralNetwork(int[] layers, float lr)
        {
            int length = layers.Length - 1;
            _weights = new NeuralNetMatrix[length];
            _biases = new NeuralNetMatrix[length];
            _learningRate = lr;

            for (int i = 0; i < length; i++)
            {
                _weights[i] = new NeuralNetMatrix(layers[i + 1], layers[i]);
                _weights[i].Randomize();
                _biases[i] = new NeuralNetMatrix(layers[i + 1], 1);
                _biases[i].Randomize();
            }
        }

        public float[] Predict(float[] inputsArray)
        {
            var result = new NeuralNetMatrix(inputsArray);

            for (int i = 0; i < _weights.Length; i++)
            {
                result = NeuralNetMatrix.DotProduct(_weights[i], result);
                result.Add(_biases[i]);
                result = NeuralNetMatrix.Sigmoid(result);
            }

            return result.Flatten();
        }

        public void Train(float[][] trainInputsArray, float[][] trainTargetsArray, int batchSize, out float[][] errors)
        {
            int batchCount = 0;

            var shuffle = Shuffle(trainInputsArray.Length);

            errors = new float[trainInputsArray.Length][];

            var gradients = new NeuralNetMatrix[_weights.Length + 1];
            gradients[0] = new NeuralNetMatrix(trainInputsArray[0].Length, 1);

            for (int i = 0; i < _weights.Length; i++)
                gradients[i + 1] = NeuralNetMatrix.DotProduct(_weights[i], gradients[i]);

            gradients[_weights.Length] = new NeuralNetMatrix(_biases[_biases.Length - 1].RowLength, _biases[_biases.Length - 1].ColLength);

            for (int j = 0; j < trainInputsArray.Length; j++)
            {
                int k = shuffle[j];
                var inputsArray = trainInputsArray[k];
                var targetsArray = trainTargetsArray[k];

                // Feed Forward and save each layers calculation
                var layerOutputs = new NeuralNetMatrix[_weights.Length + 1];
                layerOutputs[0] = new NeuralNetMatrix(inputsArray);

                for (int i = 0; i < _weights.Length; i++)
                {
                    layerOutputs[i + 1] = NeuralNetMatrix.DotProduct(_weights[i], layerOutputs[i]);
                    layerOutputs[i + 1].Add(_biases[i]);
                    layerOutputs[i + 1] = NeuralNetMatrix.Sigmoid(layerOutputs[i + 1]);
                }

                //Calculate the output errors
                var outputErrors = new NeuralNetMatrix(targetsArray);
                outputErrors.Subtract(layerOutputs[layerOutputs.Length - 1]);
                var absCopy = outputErrors.Copy();
                absCopy.ScalarAbs();
                outputErrors.Multiply(absCopy);

                errors[k] = outputErrors.Flatten();
                //Calculate the output gradients
                var outputGradients = NeuralNetMatrix.SigmoidFakeDerivative(layerOutputs[layerOutputs.Length - 1]);
                outputGradients.Multiply(outputErrors);
                outputGradients.ScalarMultiply(_learningRate);

                //Save Gradients
                gradients[gradients.Length - 1].Add(outputGradients);

                //Apply Gradients
                if ((j + 1) % batchSize == 0 || (j + 1) == trainInputsArray.Length)
                {
                    if ((j + 1) == trainInputsArray.Length)
                    {
                        int lengthFinalBatch = trainInputsArray.Length - (batchSize * batchCount);
                        //Average the gradients
                        gradients[gradients.Length - 1].ScalarDivide(lengthFinalBatch);
                    }
                    else
                    {
                        batchCount++;
                        //Average the gradients
                        gradients[gradients.Length - 1].ScalarDivide(batchSize);
                    }

                    //Calculate the output deltas
                    var lastHiddenLayerTransposition = NeuralNetMatrix.Transpose(layerOutputs[layerOutputs.Length - 2]);
                    var outputDeltas = NeuralNetMatrix.DotProduct(gradients[gradients.Length - 1], lastHiddenLayerTransposition);

                    //Update the Weights and Biases
                    _weights[_weights.Length - 1].Add(outputDeltas);
                    _biases[_biases.Length - 1].Add(gradients[gradients.Length - 1]);

                    gradients[gradients.Length - 1] = new NeuralNetMatrix(layerOutputs[layerOutputs.Length - 1].RowLength, layerOutputs[layerOutputs.Length - 1].ColLength);
                }

                //Calculate Hidden Errors
                var previousErrors = outputErrors;
                for (int i = layerOutputs.Length - 2; i > 0; i--)
                {
                    // Calculate Error
                    var weightsTransposed = NeuralNetMatrix.Transpose(_weights[i]);
                    var currentErrors = NeuralNetMatrix.DotProduct(weightsTransposed, previousErrors);

                    absCopy = currentErrors.Copy();
                    absCopy.ScalarAbs();
                    currentErrors.Multiply(absCopy);

                    //Calculate the gradients
                    var hiddenGradients = NeuralNetMatrix.SigmoidFakeDerivative(layerOutputs[i]);
                    hiddenGradients.Multiply(currentErrors);
                    hiddenGradients.ScalarMultiply(_learningRate);

                    //Save Gradients
                    gradients[i].Add(hiddenGradients);

                    //Apply Gradients
                    if ((j + 1) % batchSize == 0 || (j + 1) == trainInputsArray.Length)
                    {
                        if ((j + 1) == trainInputsArray.Length)
                        {
                            int lengthFinalBatch = trainInputsArray.Length - (batchSize * batchCount);
                            //Average the gradients
                            gradients[i].ScalarDivide(lengthFinalBatch);
                        }
                        else
                        {
                            //Average the gradients
                            gradients[i].ScalarDivide(batchSize);
                        }

                        //Calculate the output deltas
                        var lastHiddenLayerTransposition = NeuralNetMatrix.Transpose(layerOutputs[i - 1]);
                        var hiddenDeltas = NeuralNetMatrix.DotProduct(gradients[i], lastHiddenLayerTransposition);

                        //Update the Weights and Biases
                        _weights[i - 1].Add(hiddenDeltas);
                        _biases[i - 1].Add(gradients[i]);

                        gradients[i] = new NeuralNetMatrix(layerOutputs[i].RowLength, layerOutputs[i].ColLength);
                    }

                    //Update Previous Errors
                    previousErrors = currentErrors;
                }
            }
        }

        public float Test(float[][] testInputsArray, float[][] testTargetsArray)
        {
            float accuracy = 0;

            for (int j = 0; j < testInputsArray.Length; j++)
            {
                var inputsArray = testInputsArray[j];
                var targetsArray = testTargetsArray[j];

                // Feed Forward and save each layers calculation
                var layerOutputs = new NeuralNetMatrix[_weights.Length + 1];
                layerOutputs[0] = new NeuralNetMatrix(inputsArray);

                for (int i = 0; i < _weights.Length; i++)
                {
                    layerOutputs[i + 1] = NeuralNetMatrix.DotProduct(_weights[i], layerOutputs[i]);
                    layerOutputs[i + 1].Add(_biases[i]);
                    layerOutputs[i + 1] = NeuralNetMatrix.Sigmoid(layerOutputs[i + 1]);
                }

                var outputs = layerOutputs[layerOutputs.Length - 1].Flatten();
                bool correct = true;

                for(int i = 0; i < outputs.Length; i++)
                {
                    if (Math.Round(outputs[i]) != targetsArray[i])
                    {
                        correct = false;
                    }
                }

                if (correct)
                    accuracy++;
            }

            accuracy /= testTargetsArray.Length;

            return accuracy;
        }

        public override string ToString()
        {
            string result = $"Neural Net Info,{_learningRate},{_weights.Length}\n";

            for(int i = 0; i < _weights.Length; i++)
            {
                result += $"Layer Info,{i},{_weights[i].RowLength},{_weights[i].ColLength}\n";
                result += $"Weights\n";
                result += _weights[i].ToString();
                result += $"Biases\n";
                result += _biases[i].ToString();
            }

            return result;
        }

        public void Save(string path)
        {
            File.WriteAllText(path, ToString());
        }

        private void Load(string path)
        {
            var lines = File.ReadAllLines(path);
            int currentLayer = 0;
            int rowLength = 0;
            int colLength = 0;
            var matrix = new NeuralNetMatrix();
            int currRow = 0;
            bool isWeights = true;
            foreach (string line in lines)
            {
                var tokens = line.Split(',');

                if (tokens[0] == "Neural Net Info")
                {
                    _learningRate = float.Parse(tokens[1]);
                    int size = int.Parse(tokens[2]);
                    _weights = new NeuralNetMatrix[size];
                    _biases = new NeuralNetMatrix[size];
                }
                else if (tokens[0] == "Layer Info")
                {
                    currentLayer = int.Parse(tokens[1]);
                    rowLength = int.Parse(tokens[2]);
                    colLength = int.Parse(tokens[3]);
                }
                else if (tokens[0] == "Weights")
                {
                    isWeights = true;
                    currRow = 0;
                    matrix = new NeuralNetMatrix(rowLength, colLength);
                }
                else if (tokens[0] == "Biases")
                {
                    isWeights = false;
                    currRow = 0;
                    matrix = new NeuralNetMatrix(rowLength, 1);
                }
                else
                {
                    if(isWeights)
                    {
                        for (int i = 0; i < colLength; i++)
                        {
                            matrix[currRow, i] = float.Parse(tokens[i]);
                        }
                        currRow++;
                        if (currRow >= rowLength)
                            _weights[currentLayer] = matrix;
                    }
                    else
                    {
                        matrix[currRow, 0] = float.Parse(tokens[0]);
                        currRow++;
                        if (currRow >= rowLength)
                            _biases[currentLayer] = matrix;
                    }
                }
            }
        }

        public void Mutate(double mutationRate)
        {
            foreach(var w in _weights)
            {
                w.Mutate(mutationRate);
            }

            foreach(var b in _biases)
            {
                b.Mutate(mutationRate);
            }
        }

        int[] Shuffle(int max)
        {
            int[] result = new int[max];

            for(int i = 0; i < max; i++)
            {
                result[i] = i;
            }

            int n = max;
            var rng = new Random();
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = result[k];
                result[k] = result[n];
                result[n] = value;
            }

            return result;
        }
    }
}
