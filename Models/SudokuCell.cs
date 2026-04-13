namespace SudokuApp.Models
{
    public class SudokuCell
    {
        public int Value { get; set; }
        public bool IsGiven { get; set; }   // 처음부터 주어진 숫자
        public bool IsError { get; set; }   // 오답 여부
        public bool IsSelected { get; set; } // 선택 여부
    }
}