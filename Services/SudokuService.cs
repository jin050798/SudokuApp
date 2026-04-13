using SudokuApp.Models;

namespace SudokuApp.Services
{
    public class SudokuService
    {
        // 완성된 정답 보드
        private int[,] _solution = new int[9, 9];

        // 퍼즐 생성
        public SudokuBoard GeneratePuzzle(string difficulty = "중급")
        {
            _solution = new int[9, 9];
            FillBoard(_solution);

            var puzzle = CopyBoard(_solution);
            RemoveCells(puzzle, difficulty);

            var board = new SudokuBoard { Difficulty = difficulty };
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    board.Cells[r, c] = new SudokuCell
                    {
                        Value = puzzle[r, c],
                        IsGiven = puzzle[r, c] != 0
                    };

            return board;
        }

        // 정답 검증
        public bool ValidateBoard(SudokuBoard board)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    var cell = board.Cells[r, c];
                    if (!cell.IsGiven && cell.Value != 0)
                        cell.IsError = cell.Value != _solution[r, c];
                }
            return true;
        }

        // 완성 여부 확인
        public bool IsComplete(SudokuBoard board)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    if (board.Cells[r, c].Value != _solution[r, c])
                        return false;
            return true;
        }

        // 보드 URL 인코딩 (공유용)
        public string SerializeBoard(SudokuBoard board)
        {
            var values = new System.Text.StringBuilder();
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    values.Append(board.Cells[r, c].Value);
            return Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(values.ToString()));
        }

        // URL 디코딩 (공유 링크 복원)
        public SudokuBoard DeserializeBoard(string encoded, SudokuBoard original)
        {
            var decoded = System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(encoded));
            int idx = 0;
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    int val = int.Parse(decoded[idx++].ToString());
                    if (!original.Cells[r, c].IsGiven)
                        original.Cells[r, c].Value = val;
                }
            return original;
        }

        // ── 내부 로직 ──────────────────────────────

        private bool FillBoard(int[,] board)
        {
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                {
                    if (board[r, c] != 0) continue;
                    var nums = Enumerable.Range(1, 9)
                                        .OrderBy(_ => Guid.NewGuid()).ToList();
                    foreach (var num in nums)
                    {
                        if (!IsValid(board, r, c, num)) continue;
                        board[r, c] = num;
                        if (FillBoard(board)) return true;
                        board[r, c] = 0;
                    }
                    return false;
                }
            return true;
        }

        private void RemoveCells(int[,] board, string difficulty)
        {
            int remove = difficulty switch
            {
                "초급" => 30,
                "중급" => 45,
                "고급" => 55,
                _ => 45
            };

            var rng = new Random();
            int removed = 0;
            while (removed < remove)
            {
                int r = rng.Next(9);
                int c = rng.Next(9);
                if (board[r, c] == 0) continue;
                board[r, c] = 0;
                removed++;
            }
        }

        private bool IsValid(int[,] board, int row, int col, int num)
        {
            // 행 체크
            for (int c = 0; c < 9; c++)
                if (board[row, c] == num) return false;

            // 열 체크
            for (int r = 0; r < 9; r++)
                if (board[r, col] == num) return false;

            // 3x3 박스 체크
            int br = (row / 3) * 3, bc = (col / 3) * 3;
            for (int r = br; r < br + 3; r++)
                for (int c = bc; c < bc + 3; c++)
                    if (board[r, c] == num) return false;

            return true;
        }

        private int[,] CopyBoard(int[,] source)
        {
            var copy = new int[9, 9];
            for (int r = 0; r < 9; r++)
                for (int c = 0; c < 9; c++)
                    copy[r, c] = source[r, c];
            return copy;
        }
    }
}