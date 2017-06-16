using System;
using System.Collections.Generic;
namespace sudoku
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var array = new int[,]{
	            {2, 0, 0, 0, 0, 9, 0, 8, 0,},
	            {0, 4, 6, 0, 0, 0, 0, 0, 0,},
	            {7, 0, 0, 4, 0, 2, 1, 0, 0,},
	            {6, 0, 0, 0, 0, 1, 4, 0, 8,},
	            {0, 0, 0, 6, 3, 0, 0, 2, 0,},
	            {9, 2, 7, 0, 0, 0, 0, 0, 3,},
	            {1, 0, 0, 9, 0, 0, 8, 4, 0,},
	            {0, 9, 0, 0, 0, 0, 0, 0, 0,},
	            {4, 0, 0, 0, 0, 0, 0, 6, 0,},
            };

			var solver = new Solver(array);
			var answer = solver.Solve(new List<Solver.SolvedCell>());
			if (answer.Solved) {
				Console.WriteLine("solved!");
				for (var row = 0; row < Solver.Size; row++)
				{
					Console.WriteLine();
					for (var col = 0; col < Solver.Size; col++)
					{
						Console.Write(answer.Array[row, col]);
					}
				}
			}
            else
            {
				Console.WriteLine("unsolved!");
			}
		}
    }
}
