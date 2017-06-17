using System.Collections.Generic;
using System.Linq;

namespace sudoku
{
    public class Solver
    {
        public class SolverIterator 
        {
	        public IEnumerator<Cell> GetEnumerator()
			{
				for (var row = 0; row < Size; row++)
				{
					for (var col = 0; col < Size; col++)
					{
						yield return new Cell { Row = row, Col = col };
					}
				}
			}
        }

        public class Cell
        {
            public int Row { get; internal set; }
            public int Col { get; internal set; }

			//objと自分自身が等価のときはtrueを返す
			public override bool Equals(object obj)
			{
                if (obj == null || !(obj is Cell))
				{
					return false;
				}
				var c = (Cell)obj;
				return this.Row == c.Row && this.Col == c.Col;
			}

			//Equalsがtrueを返すときに同じ値を返す
			public override int GetHashCode()
			{
				return this.Row * Size + this.Col;
			}
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

		static readonly SolverIterator Iterator = new SolverIterator();

		readonly List<SolvedCell> InitialCell;

        public Solver(int[,] input)
        {
            InitialCell = new List<SolvedCell>();
            foreach (var cell in Iterator)
            {
                var val = input[cell.Row, cell.Col];
                if (val > 0)
                {
                    InitialCell.Add(new SolvedCell
                    {
                        Row = cell.Row,
                        Col = cell.Col,
                        Val = val,
                        Block = SubBlockMask[cell.Row, cell.Col]
                    });
                }
            }
        }


        int[,] AnswerArray(IEnumerable<SolvedCell> results)
        {
            var answer = new int[Size, Size];
			foreach (var currentCell in Iterator)
            {
                var c = results.FirstOrDefault(cell => cell.Equals(currentCell));
                if (c != null)
                {
                    answer[currentCell.Row, currentCell.Col] = c.Val;
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
			var solvedCells = InitialCell.Union(results);
            var unsolvedCells = new List<UnsolvedCell>();
            foreach (var currentCell in Iterator)
            {
                if (!solvedCells.Any(cell => cell.Equals(currentCell)))
                {
                    var unused = UnusedValues(new Cell { Row = currentCell.Row, Col = currentCell.Col }, solvedCells);
                    unsolvedCells.Add(new UnsolvedCell { Row = currentCell.Row, Col = currentCell.Col, Unused = unused });
                }
            }
            if (unsolvedCells.Count == 0)
            {
                return new Result { Solved = true, Array = AnswerArray(solvedCells) };
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
						return answers.First();
                    }
                }
            }
			return new Result { Solved = false };
        }
    }
}