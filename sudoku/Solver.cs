using System.Collections.Generic;
using System.Linq;

namespace sudoku
{
    public class Solver
    {
        public class Cell
        {
            public int Row { get; internal set; }
            public int Col { get; internal set; }
        }

        public class SolvedCell : Cell
        {
            public int Block { get; internal set; }
            public int Val { get; internal set; }
        }
        public class UnsolvedCell : Cell
        {
            public IEnumerable<int> Unused { get; internal set; }
        }

        public class Result
        {
            public bool Solved { get; internal set; }
            public int[,] Array { get; internal set; }
        }

        public static readonly int Size = 9;

        static readonly IEnumerable<int> AllValues = Enumerable.Range(1, Size);

        static readonly int[,] SubBlockMask = {
                   {0, 0, 0, 1, 1, 1, 2, 2, 2},
                   {0, 0, 0, 1, 1, 1, 2, 2, 2},
                   {0, 0, 0, 1, 1, 1, 2, 2, 2},
                   {3, 3, 3, 4, 4, 4, 5, 5, 5},
                   {3, 3, 3, 4, 4, 4, 5, 5, 5},
                   {3, 3, 3, 4, 4, 4, 5, 5, 5},
                   {6, 6, 6, 7, 7, 7, 8, 8, 8},
                   {6, 6, 6, 7, 7, 7, 8, 8, 8},
                   {6, 6, 6, 7, 7, 7, 8, 8, 8},
               };

        readonly List<SolvedCell> InitialCell;

        public Solver(int[,] input)
        {
            InitialCell = new List<SolvedCell>();
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    var val = input[row, col];
                    if (val > 0)
                    {
                        InitialCell.Add(new SolvedCell
                        {
                            Row = row,
                            Col = col,
                            Val = val,
                            Block = SubBlockMask[row, col]
                        });
                    }
                }
            }
        }


        int[,] answerArray(IEnumerable<SolvedCell> results)
        {
            var answer = new int[Size, Size];
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    var c = results.FirstOrDefault(cell => cell.Row == row && cell.Col == col);
                    if (c != null)
                    {
                        answer[row, col] = c.Val;
                    }
                }
            }
            return answer;
        }

        IEnumerable<int> UnusedValues(Cell currentCell, IEnumerable<SolvedCell> solvedCells)
        {
            var block = SubBlockMask[currentCell.Row, currentCell.Col];
            var usedValues = solvedCells
                .Where(cell => cell.Row == currentCell.Row || cell.Col == currentCell.Col || cell.Block == block)
                .Select(cell => cell.Val);
            return AllValues.Where(val => !usedValues.Contains(val));
        }

        public Result Solve(IEnumerable<SolvedCell> results)
        {
            if (results.Count() > 1)
                System.Console.WriteLine("row: {0}, col: {1}, val: {2}", results.Last().Row, results.Last().Col, results.Last().Val);

			var solvedCells = InitialCell.Union(results);
            var unsolvedCells = new List<UnsolvedCell>();
            for (var row = 0; row < Size; row++)
            {
                for (var col = 0; col < Size; col++)
                {
                    if (!solvedCells.Any(cell => cell.Row == row && cell.Col == col))
                    {
                        var unused = UnusedValues(new Cell { Row = row, Col = col }, solvedCells);
                        unsolvedCells.Add(new UnsolvedCell { Row = row, Col = col, Unused = unused });
                    }
                }
            }
            if (unsolvedCells.Count == 0)
            {
                return new Result { Solved = true, Array = answerArray(solvedCells) };
            }
            else
            {
                if (unsolvedCells.All(cell => cell.Unused.Count() > 0))
                {
					//  unused の数が最も小さいCellを取得。
					var head = unsolvedCells.Aggregate((cell1, cell2) => cell1.Unused.Count() < cell2.Unused.Count() ? cell1 : cell2);
					var answers = head.Unused
						.Select(val => Solve(results.Union(new[] { new SolvedCell { Row = head.Row, Col = head.Col, Block = SubBlockMask[head.Row, head.Col], Val = val } })))
						.Where(p => p.Solved)
						.ToList();
                    if (answers.Count() == 1)
                    {
                        System.Console.WriteLine("solved!");
						return answers.First();
                    }
                }
            }
			System.Console.WriteLine("back!");
			return new Result { Solved = false };
        }
    }
}