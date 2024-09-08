using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleGOPS
{
    struct NeuralNetMatrix
    {
        float[,] _values;
        public readonly int RowLength { get; }
        public readonly int ColLength { get; }

        public float this[int r, int c]
        {
            get
            {
                return _values[r, c];
            }

            set
            {
                _values[r, c] = value;
            }
        }

        public NeuralNetMatrix()
        {
            _values = new float[0,0];
            RowLength = 0;
            ColLength = 0;
        }

        public NeuralNetMatrix(int rowLength, int colLength)
        {
            _values = new float[rowLength, colLength];
            RowLength = rowLength;
            ColLength = colLength;
        }

        public NeuralNetMatrix(float[] values)
        {
            _values = new float[values.Length, 1];
            RowLength = values.Length;
            ColLength = 1;

            for (int i = 0; i < RowLength; i++)
            {
                _values[i, 0] = values[i];
            }
        }

        public void ScalarMultiply(float x)
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] *= x; 
                }
            }
        }

        public void ScalarDivide(int x)
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] /= x;
                }
            }
        }

        public void ScalarAbs()
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] = Math.Abs(_values[i, j]);
                }
            }
        }

        public void Add(NeuralNetMatrix m)
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] += m[i, j]; 
                }
            }
        }

        public void Subtract(NeuralNetMatrix m)
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] -= m[i, j]; 
                }
            }
        }

        public void Multiply(NeuralNetMatrix m)
        {
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] *= m[i, j];
                }
            }
        }

        public static NeuralNetMatrix DotProduct(NeuralNetMatrix a, NeuralNetMatrix b)
        {
            if (a.ColLength != b.RowLength)
                throw new Exception("Columns and Rows must equal");

            NeuralNetMatrix result = new NeuralNetMatrix(a.RowLength, b.ColLength);

            for (int i = 0; i < result.RowLength; i++)
            {
                for (int j = 0; j < result.ColLength; j++)
                {
                    float sum = 0;
                    for (int k = 0; k < a.ColLength; k++)
                    {
                        sum += a[i, k] * b[k, j];
                    }
                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static NeuralNetMatrix Transpose(NeuralNetMatrix a)
        {
            NeuralNetMatrix result = new NeuralNetMatrix(a.ColLength, a.RowLength);

            for (int i = 0; i < a.RowLength; i++)
            {
                for (int j = 0; j < a.ColLength; j++)
                {
                    result[j, i] = a[i,j];
                }
            }

            return result;
        }

        public static NeuralNetMatrix Sigmoid(NeuralNetMatrix m)
        {
            NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
            for (int i = 0; i < m.RowLength; i++)
            {
                for (int j = 0; j < m.ColLength; j++)
                {
                    result[i, j] = (float)(1.0 / (1.0 + Math.Exp(-m[i,j])));
                }
            }
            return result;
        }

        public static NeuralNetMatrix SigmoidFakeDerivative(NeuralNetMatrix m)
        {
            NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
            for (int i = 0; i < m.RowLength; i++)
            {
                for (int j = 0; j < m.ColLength; j++)
                {
                    result[i, j] = m[i, j] * (1 - m[i, j]);
                }
            }
            return result;
        }

        public float[] Flatten()
        {
            float[] result = new float[RowLength * ColLength];
            int counter = 0;

            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    result[counter] = _values[i, j];
                    counter++;
                }
            }

            return result;
        }

        public void Randomize()
        {
            Random rng = new();
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] = (float)(rng.NextDouble() * 2) - 1;
                }
            }
        }

        public void Mutate(double mutationRate)
        {
            Random rng = new();
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    _values[i, j] = rng.NextDouble() < mutationRate ? (float)GaussianDistribution(0, .1) : _values[i, j];
                }
            }
        }

        public override string ToString()
        {
            string result = "";

            for(int i = 0; i < RowLength; i++)
            {
                for(int j = 0; j < ColLength - 1; j++)
                {
                    result += $"{_values[i, j]},";
                }
                result += $"{_values[i, ColLength - 1]}\n";
            }

            return result;
        }

        public NeuralNetMatrix Copy()
        {
            var copy = new NeuralNetMatrix(RowLength, ColLength);
            for (int i = 0; i < RowLength; i++)
            {
                for (int j = 0; j < ColLength; j++)
                {
                    copy[i, j] = _values[i, j];
                }
            }
            return copy;
        }

        private double GaussianDistribution(double mean, double stddev)
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stddev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }

        //public static NeuralNetMatrix TanH(NeuralNetMatrix m)
        //{
        //    NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
        //    for (int i = 0; i < m.RowLength; i++)
        //    {
        //        for (int j = 0; j < m.ColLength; j++)
        //        {
        //            float e2 = (float)Math.Exp((float)(2 * m._values[i, j]));
        //            result[i, j] = (e2 - 1) / (e2 + 1);
        //        }
        //    }
        //    return result;
        //}

        //// Assume the values of M have already been activated
        //public static NeuralNetMatrix TanHFakeDerivative(NeuralNetMatrix m)
        //{
        //    NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
        //    for (int i = 0; i < m.RowLength; i++)
        //    {
        //        for (int j = 0; j < m.ColLength; j++)
        //        {
        //            result[i, j] = 1 - (m[i, j] * m[i, j]);
        //        }
        //    }
        //    return result;
        //}

        //public static NeuralNetMatrix LeakyRelu(NeuralNetMatrix m)
        //{
        //    NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
        //    for (int i = 0; i < m.RowLength; i++)
        //    {
        //        for (int j = 0; j < m.ColLength; j++)
        //        {
        //            result[i, j] = Math.Max(.01f * m[i, j], m[i, j]);
        //        }
        //    }
        //    return result;
        //}

        //public static NeuralNetMatrix LeakyReluDerivative(NeuralNetMatrix m)
        //{
        //    NeuralNetMatrix result = new NeuralNetMatrix(m.RowLength, m.ColLength);
        //    for (int i = 0; i < m.RowLength; i++)
        //    {
        //        for (int j = 0; j < m.ColLength; j++)
        //        {
        //            result[i, j] = m[i, j] >= 0 ? 1 : .01f;
        //        }
        //    }
        //    return result;
        //}
    }
}
