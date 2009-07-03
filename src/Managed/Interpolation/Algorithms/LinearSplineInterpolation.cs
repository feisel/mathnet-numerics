﻿// <copyright file="LinearSplineInterpolation.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2009 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.Interpolation.Algorithms
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Linear Spline Interpolation Algorithm.
    /// </summary>
    /// <remarks>
    /// This algorithm supports both differentiation and integration.
    /// </remarks>
    public class LinearSplineInterpolation : IInterpolation
    {
        /// <summary>
        /// Internal Spline Interpolation
        /// </summary>
        private readonly SplineInterpolation spline;

        /// <summary>
        /// Initializes a new instance of the LinearSplineInterpolation class.
        /// </summary>
        public LinearSplineInterpolation()
        {
            this.spline = new SplineInterpolation();
        }

        /// <summary>
        /// Initializes a new instance of the LinearSplineInterpolation class.
        /// </summary>
        /// <param name="samplePoints">Sample Points t</param>
        /// <param name="sampleValues">Sample Values x(samplePoints)</param>
        public LinearSplineInterpolation(
            IList<double> samplePoints,
            IList<double> sampleValues)
        {
            this.spline = new SplineInterpolation();
            this.Initialize(samplePoints, sampleValues);
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports differentiation (interpolated derivative).
        /// </summary>
        /// <seealso cref="Differentiate(double)"/>
        /// <seealso cref="Differentiate(double, out double, out double)"/>
        bool IInterpolation.SupportsDifferentiation
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether the algorithm supports integration (interpolated quadrature).
        /// </summary>
        /// <seealso cref="Integrate"/>
        bool IInterpolation.SupportsIntegration
        {
            get { return true; }
        }

        /// <summary>
        /// Initialize the interpolation method with the given spline coefficients.
        /// </summary>
        /// <param name="samplePoints">Sample Points t</param>
        /// <param name="sampleValues">Sample Values x(samplePoints)</param>
        public void Initialize(
            IList<double> samplePoints,
            IList<double> sampleValues)
        {
            if (null == samplePoints)
            {
                throw new ArgumentNullException("samplePoints");
            }

            if (null == sampleValues)
            {
                throw new ArgumentNullException("sampleValues");
            }

            if (samplePoints.Count < 2)
            {
                throw new ArgumentOutOfRangeException("samplePoints");
            }

            if (samplePoints.Count != sampleValues.Count)
            {
                throw new ArgumentException(Properties.Resources.ArgumentVectorsSameLengths);
            }

            double[] coefficients = new double[4 * (samplePoints.Count - 1)];
            double[] sortedPoints = new double[samplePoints.Count];
            samplePoints.CopyTo(sortedPoints, 0);
            double[] sortedValues = new double[sampleValues.Count];
            sampleValues.CopyTo(sortedValues, 0);

            // TODO: Sorting.Sort(sortedPoints, sortedValues);

            for (int i = 0, j = 0; i < sortedPoints.Length - 1; i++, j += 4)
            {
                coefficients[j] = sortedValues[i];
                coefficients[j + 1] = (sortedValues[i + 1] - sortedValues[i]) / (sortedPoints[i + 1] - sortedPoints[i]);
                coefficients[j + 2] = 0;
                coefficients[j + 3] = 0;
            }

            this.spline.Initialize(sortedPoints, coefficients);
        }

        /// <summary>
        /// Interpolate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated value x(t).</returns>
        public double Interpolate(double t)
        {
            return this.spline.Interpolate(t);
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <returns>Interpolated first derivative at point t.</returns>
        /// <seealso cref="IInterpolation.SupportsDifferentiation"/>
        /// <seealso cref="Differentiate(double, out double, out double)"/>
        public double Differentiate(double t)
        {
            return this.spline.Differentiate(t);
        }

        /// <summary>
        /// Differentiate at point t.
        /// </summary>
        /// <param name="t">Point t to interpolate at.</param>
        /// <param name="interpolatedValue">Interpolated value x(t)</param>
        /// <param name="secondDerivative">Interpolated second derivative at point t.</param>
        /// <returns>Interpolated first derivative at point t.</returns>
        /// <seealso cref="IInterpolation.SupportsDifferentiation"/>
        /// <seealso cref="Differentiate(double)"/>
        public double Differentiate(
            double t,
            out double interpolatedValue,
            out double secondDerivative)
        {
            return this.spline.Differentiate(t, out interpolatedValue, out secondDerivative);
        }

        /// <summary>
        /// Integrate up to point t.
        /// </summary>
        /// <param name="t">Right bound of the integration interval [a,t].</param>
        /// <returns>Interpolated definite integral over the interval [a,t].</returns>
        /// <seealso cref="IInterpolation.SupportsIntegration"/>
        public double Integrate(double t)
        {
            return this.spline.Integrate(t);
        }
    }
}
