using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitDistributor
{
    public class BinPacker
    {
        /// <summary>
        /// 정해진 bin들의 남는 공간이 최소가 되게 stuff를 배치하고, [bin][stuff]의 초기 입력순 리스트로 반환합니다.  
        /// </summary>
        public static List<List<double>> PackToBins(List<double> stuffs, List<double> bins, out List<List<int>> stuffIndex)
        {
            //scale
            List<double> scaledStuff = new List<double>();

            double binTotal = bins.Sum();
            double stuffTotal = stuffs.Sum();

            if (binTotal < stuffTotal)
                scaledStuff = ScaleToNewSum(binTotal, stuffs);
            else
                scaledStuff = stuffs;

            //set
            List<Stuff> stuffList = new List<Stuff>();
            for (int i = 0; i < scaledStuff.Count; i++)
            {
                Stuff eachStuff = new Stuff(scaledStuff[i], i);
                stuffList.Add(eachStuff);
            }

            List<Bin> binList = new List<Bin>();
            for (int i = 0; i < bins.Count; i++)
            {
                Bin eachBin = new Bin(bins[i], i);
                binList.Add(eachBin);
            }


            //initial sorting
            stuffList.Sort((a, b) => -a.Volume.CompareTo(b.Volume));

            foreach (Stuff i in stuffList)
            {
                binList.Sort((a, b) => PackCostComparer(a, b, i));
                binList.First().StuffIncluded.Add(i);
            }

            //balancing
            BalanceBins(binList);
            binList.Sort((a, b) => (a.IndexInitial.CompareTo(b.IndexInitial)));

            //output
            List<List<double>> packedStuff = new List<List<double>>();
            List<List<int>> packedIndex = new List<List<int>>();

            foreach (Bin i in binList)
            {
                List<double> currentPack = new List<double>();
                List<int> currentIndexPack = new List<int>();

                i.StuffIncluded.Sort((a, b) => a.Index.CompareTo(b.Index));
                foreach (Stuff j in i.StuffIncluded)
                {
                    currentPack.Add(j.Volume);
                    currentIndexPack.Add(j.Index);
                }

                packedStuff.Add(currentPack);
                packedIndex.Add(currentIndexPack);
            }

            stuffIndex = packedIndex;
            return packedStuff;
        }

        private static void BalanceBins(List<Bin> unbalancedBins)
        {
            List<List<Stuff>> stuffByTotalDescend = new List<List<Stuff>>();
            unbalancedBins.Sort((a, b) => -a.GetTotal().CompareTo(b.GetTotal()));

            foreach (Bin i in unbalancedBins)
            {
                List<Stuff> tempStuff = new List<Stuff>();

                foreach (Stuff j in i.StuffIncluded)
                    tempStuff.Add(new Stuff(j));

                stuffByTotalDescend.Add(tempStuff);
            }

            unbalancedBins.Sort((a, b) => -a.Volume.CompareTo(b.Volume));
            for (int i = 0; i < unbalancedBins.Count; i++)
                unbalancedBins[i].StuffIncluded = stuffByTotalDescend[i];

            return;
        }

        private static int PackCostComparer(Bin a, Bin b, Stuff s)
        {
            //tolerance
            double nearDecidingRate = 1.2;

            //binPackSet
            double aVolume = a.Volume;
            double bVolume = b.Volume;

            double aRemain = a.GetRemain();
            double bRemain = b.GetRemain();

            double aCost = Math.Abs(a.GetRemain() - s.Volume);
            double bCost = Math.Abs(b.GetRemain() - s.Volume);

            double sVolume = s.Volume;

            //decider
            bool IsAMoreProper = aCost < bCost;


            //compare
            //seive: subzero bin
            if (aVolume <= 0)
            {
                if (bVolume <= 0)
                {
                    if (IsAMoreProper)
                        return -1;
                    return 1;
                }

                if (bRemain >= sVolume)
                    return 1;
                else
                {
                    if (IsAMoreProper)
                        return -1;
                    return 1;
                }
            }

            if (bVolume <= 0)
            {
                if (aRemain >= sVolume)
                    return -1;
                else
                {
                    if (IsAMoreProper)
                        return -1;
                    return 1;
                }
            }

            //actual comparer
            if (aRemain <= sVolume)
            {
                if (bRemain <= sVolume)
                {
                    if (IsAMoreProper)
                        return -1;
                    return 1;
                }

                if (aRemain * nearDecidingRate >= sVolume)
                    return -1;
                return 1;
            }

            if (bRemain <= sVolume)
            {
                if (bRemain * nearDecidingRate >= sVolume)
                    return 1;
                return -1;
            }

            if (IsAMoreProper)
                return -1;
            return 1;

        }

        private class Stuff
        {
            public Stuff(double volume, int indexInitial)
            {
                this.Volume = volume;
                this.Index = indexInitial;
            }

            public Stuff(Stuff otherStuff)
            {
                this.Volume = otherStuff.Volume;
                this.Index = otherStuff.Index;
            }

            public double Volume { get; set; }
            public int Index { get; set; }
        }

        private class Bin
        {
            private List<Stuff> stuffIncluded = new List<Stuff>();
            public Bin(double volume, int indexInitial)
            {
                this.Volume = volume;
                this.IndexInitial = indexInitial;
            }

            //
            public double GetRemain()
            {
                double stuffTotal = GetTotal();
                return Volume - stuffTotal;
            }

            public double GetTotal()
            {
                double stuffTotal = 0;
                if (StuffIncluded.Count != 0)
                {
                    foreach (Stuff i in StuffIncluded)
                        stuffTotal += i.Volume;
                }

                return stuffTotal;
            }

            public double Volume { get; private set; }
            public int IndexInitial { get; private set; }
            public List<Stuff> StuffIncluded { get { return stuffIncluded; } set { stuffIncluded = value as List<Stuff>; } }
        }

        public static List<double> ScaleToNewSum(double newSum, List<double> oldPortions)
        {
            List<double> newPortions = new List<double>();

            double oldSum = oldPortions.Sum();
            foreach (double i in oldPortions)
                newPortions.Add(newSum * (i / oldSum));

            return newPortions;
        }
    }

}
