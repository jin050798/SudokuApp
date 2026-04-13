namespace SudokuApp.Models
{
    public class SudokuBoard
    {
        public SudokuCell[,] Cells { get; set; } = new SudokuCell[9, 9];
        public string Difficulty { get; set; } = "중급";
    }
}