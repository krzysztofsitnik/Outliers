using System;
using System.Collections.Generic;
using System.Linq;

namespace OutliersExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var array = new int[] { 2, 3, 4, 5, 6, 7, 8, 9, 25 };
            var stats = new Statistics(array);
            var arrayWithoutOutliers = stats.WithoutOutliers();
            var outliers = stats.AllOutliers;

            Console.WriteLine("Original array: ");
            for (int i = 0; i < array.Length; i++)
            {
                if (i != array.Length - 1)
                    Console.Write(array[i] + ", ");
                else
                    Console.WriteLine(array[i]);
            }

            Console.WriteLine("Array without outliers: ");
            for (int i = 0; i < arrayWithoutOutliers.Length; i++)
            {
                if (i != arrayWithoutOutliers.Length - 1)
                    Console.Write(arrayWithoutOutliers[i] + ", ");
                else
                    Console.WriteLine(arrayWithoutOutliers[i]);
            }

            Console.WriteLine("The outliers: ");
            for (int i = 0; i < outliers.Length; i++)
            {
                if (i != outliers.Length - 1)
                    Console.Write(outliers[i] + ", ");
                else
                    Console.WriteLine(outliers[i]);
            }
            Console.ReadKey();
        }
    }
    public class Statistics
    {
        public IReadOnlyList<double> OriginalValues { get; }
        internal IReadOnlyList<double> SortedValues { get; }
        public int N { get; }
        public double Min { get; }
        public double LowerFence { get; }
        public double Q1 { get; }
        public double Median { get; }
        public double Q3 { get; }
        public double UpperFence { get; }
        public double Max { get; }
        public double InterquartileRange { get; }
        public double[] LowerOutliers { get; }
        public double[] UpperOutliers { get; }
        public double[] AllOutliers { get; }

        public Statistics(params double[] values) :
            this(values.ToList())
        { }

        public Statistics(IEnumerable<int> values) :
            this(values.Select(value => (double)value))
        { }

        public Statistics(IEnumerable<double> values)
        {
            OriginalValues = values.Where(d => !double.IsNaN(d)).ToArray();
            SortedValues = OriginalValues.OrderBy(value => value).ToArray();
            N = SortedValues.Count;
            if (N == 0)
                throw new InvalidOperationException("Sequence of values contains no elements, Statistics can't be calculated");

            if (N == 1)
                Q1 = Median = Q3 = SortedValues[0];
            else
            {
                double GetMedian(IReadOnlyList<double> x) => x.Count % 2 == 0
                    ? (x[x.Count / 2 - 1] + x[x.Count / 2]) / 2
                    : x[x.Count / 2];

                Median = GetMedian(SortedValues);
                Q1 = GetMedian(SortedValues.Take(N / 2).ToList());
                Q3 = GetMedian(SortedValues.Skip((N + 1) / 2).ToList());
            }

            Min = SortedValues.First();
            Max = SortedValues.Last();

            InterquartileRange = Q3 - Q1;
            LowerFence = Q1 - 1.5 * InterquartileRange;
            UpperFence = Q3 + 1.5 * InterquartileRange;

            AllOutliers = SortedValues.Where(IsOutlier).ToArray();
            LowerOutliers = SortedValues.Where(IsLowerOutlier).ToArray();
            UpperOutliers = SortedValues.Where(IsUpperOutlier).ToArray();
        }

        public bool IsLowerOutlier(double value) => value < LowerFence;
        public bool IsUpperOutlier(double value) => value > UpperFence;
        public bool IsOutlier(double value) => value < LowerFence || value > UpperFence;
        public double[] WithoutOutliers() => SortedValues.Where(value => !IsOutlier(value)).ToArray();
    }
}
// https://www.statisticshowto.datasciencecentral.com/upper-and-lower-fences/