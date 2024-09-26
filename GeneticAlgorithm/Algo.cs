﻿using System.Linq;
using System.Runtime.CompilerServices;

namespace GenAlgo
{
    public class Algorithm
    {
        private List<Arrangement> population = [];
        private readonly int randMin = 0;
        private readonly int randMax = 10;
        private readonly Random rnd = new Random();
        private readonly int[] sidesLen = [1, 2, 3];
        private Arrangement? _bestArrangement;
        public Arrangement BestArrangement { get {
            if (_bestArrangement == null)
                throw new Exception("solution has not been calculated yet");
            return _bestArrangement;
        } }
        private List<Square> initSquaresArray(int[] amounts) {
            var ret = new List<Square>();
            if (amounts.Length != sidesLen.Length)
                throw new Exception($"input array parameter must have {sidesLen.Length} length");
            for (var i = 0; i < amounts.Length; i++) {
                ret.AddRange(Enumerable.Range(0, amounts[i])
                .Select(_ => new Square(getRandom(), getRandom(), sidesLen[i]))
                .ToList());
            }
            return ret;
        }
        private int getRandom() {
            return rnd.Next(randMin, randMax);
        }

        public void CreateRandomPopulation(int size) {
            for (int i = 0; i < size;) {
                var squaresList = new List<Square> {
                    new(getRandom(), getRandom(), 1),
                    new(getRandom(), getRandom(), 1),
                    new(getRandom(), getRandom(), 2),
                    new(getRandom(), getRandom(), 2),
                    new(getRandom(), getRandom(), 3)
                };
                var arrangement = new Arrangement(squaresList);
                if (!arrangement.HaveCollisions()) {
                    i++;
                    population.Add(arrangement);
                }
            }
        }
        private Arrangement Crossover(Arrangement ar1, Arrangement ar2) {
            var crossovered = new List<Square>();
            if (ar1.Lst.Count != ar2.Lst.Count)
                throw new Exception("sizes of arrangement differs");
            var delimIndex = rnd.Next(1, ar1.Lst.Count-1);
            return new Arrangement(Enumerable.Concat(
            Enumerable.Range(0, delimIndex).Select(i=>ar1.Lst[i].Clone()),
            Enumerable.Range(delimIndex, ar2.Lst.Count - delimIndex).Select(i=>ar2.Lst[i].Clone())
            ).ToList());
        }
        private void Mutate(Arrangement arr) {
            foreach (var sq in arr.Lst) {
                if (rnd.NextDouble() < 0.1) {
                    sq.X += rnd.Next(-1, 2);
                    sq.Y += rnd.Next(-1, 2);
                }
            }
        }

        private void EvolvePopulation() {
            var bestArrangements = population.OrderBy(s => s.CalcCoverageArea()).Take(population.Count() / 2).ToList();
            var newPopulation = new List<Arrangement>();
            while (newPopulation.Count < population.Count) {
                var child = Crossover(bestArrangements[rnd.Next(bestArrangements.Count)], bestArrangements[rnd.Next(bestArrangements.Count)]);
                Mutate(child);
                if (!child.HaveCollisions())
                    newPopulation.Add(child);
            }
            population = newPopulation;
        }

        public void StartEvolution(int maxGenerations) {
            var generation = 0;
            while (generation < maxGenerations) {
                var bestArrangement = population.OrderBy(arr => arr.CalcCoverageArea()).First();
                Console.WriteLine($"#{generation} generation, best area is {bestArrangement.CalcCoverageArea()}");
                EvolvePopulation();
                if (Console.KeyAvailable) {
                    Console.ReadKey();
                    _bestArrangement = bestArrangement;
                    return;
                }
                generation++;
            }
            _bestArrangement = population.OrderBy(arr => arr.CalcCoverageArea()).First();
        }
    }
}