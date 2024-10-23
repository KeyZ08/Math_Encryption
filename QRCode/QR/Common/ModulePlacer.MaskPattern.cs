namespace QRCode.QR.Common;

public static partial class ModulePlacer
{
    private static class MaskPattern
    {
        /// <summary>
        ///     Список масок
        /// </summary>
        public static readonly List<Func<int, int, bool>> Patterns =
        [
            Pattern1, Pattern2, Pattern3, Pattern4,
            Pattern5, Pattern6, Pattern7, Pattern8
        ];

        private static bool Pattern1(int x, int y)
            => (x + y) % 2 == 0;

        private static bool Pattern2(int x, int y)
            => y % 2 == 0;

        private static bool Pattern3(int x, int y)
            => x % 3 == 0;

        private static bool Pattern4(int x, int y)
            => (x + y) % 3 == 0;

        private static bool Pattern5(int x, int y)
            => (int)(Math.Floor(y / 2d) + Math.Floor(x / 3d)) % 2 == 0;

        private static bool Pattern6(int x, int y)
            => x * y % 2 + x * y % 3 == 0;

        private static bool Pattern7(int x, int y)
            => (x * y % 2 + x * y % 3) % 2 == 0;

        private static bool Pattern8(int x, int y)
            => ((x + y) % 2 + x * y % 3) % 2 == 0;

        /// <summary>
        ///     Вычисляет штрафной балл за QR-код
        /// </summary>
        public static int Score(QRCodeData qrCode)
        {
            int score1 = 0, // Штраф за группы из пяти или более модулей одного цвета в строке (или столбце)
                score2 = 0, // Штраф за блоки 2x2 одного цвета
                score3 = 0, // Штраф за определенные шаблоны, обнаруженные в QR-коде
                score4 = 0; // Штраф за наличие более 50% черных модулей или более 50% белых модулей
            var size = qrCode.ModuleMatrix.Count;

            //Штраф 1: Проверка наличия последовательных модулей одного цвета в строках и столбцах
            for (var y = 0; y < size; y++)
            {
                var modInRow = 0;
                var modInColumn = 0;
                var lastValRow = qrCode.ModuleMatrix[y][0];
                var lastValColumn = qrCode.ModuleMatrix[0][y];
                for (var x = 0; x < size; x++)
                {
                    // строки
                    if (qrCode.ModuleMatrix[y][x] == lastValRow)
                        modInRow++;
                    else
                        modInRow = 1;
                    if (modInRow == 5)
                        score1 += 3;
                    else if (modInRow > 5)
                        score1++;
                    lastValRow = qrCode.ModuleMatrix[y][x];

                    // столбцы
                    if (qrCode.ModuleMatrix[x][y] == lastValColumn)
                        modInColumn++;
                    else
                        modInColumn = 1;
                    if (modInColumn == 5)
                        score1 += 3;
                    else if (modInColumn > 5)
                        score1++;
                    lastValColumn = qrCode.ModuleMatrix[x][y];
                }
            }

            //Штраф 2: Проверка наличия блоков 2x2 одного цвета.
            for (var y = 0; y < size - 1; y++)
            for (var x = 0; x < size - 1; x++)
                if (qrCode.ModuleMatrix[y][x] == qrCode.ModuleMatrix[y][x + 1] &&
                    qrCode.ModuleMatrix[y][x] == qrCode.ModuleMatrix[y + 1][x] &&
                    qrCode.ModuleMatrix[y][x] == qrCode.ModuleMatrix[y + 1][x + 1])
                    score2 += 3;

            //Штраф 3: Проверка наличия определенных узоров в QR-коде (ЧБЧЧЧБЧББББ или наоборот).
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size - 10; x++)
            {
                // по горизонтали
                if ((qrCode.ModuleMatrix[y][x] &&
                     !qrCode.ModuleMatrix[y][x + 1] &&
                     qrCode.ModuleMatrix[y][x + 2] &&
                     qrCode.ModuleMatrix[y][x + 3] &&
                     qrCode.ModuleMatrix[y][x + 4] &&
                     !qrCode.ModuleMatrix[y][x + 5] &&
                     qrCode.ModuleMatrix[y][x + 6] &&
                     !qrCode.ModuleMatrix[y][x + 7] &&
                     !qrCode.ModuleMatrix[y][x + 8] &&
                     !qrCode.ModuleMatrix[y][x + 9] &&
                     !qrCode.ModuleMatrix[y][x + 10]) ||
                    (!qrCode.ModuleMatrix[y][x] &&
                     !qrCode.ModuleMatrix[y][x + 1] &&
                     !qrCode.ModuleMatrix[y][x + 2] &&
                     !qrCode.ModuleMatrix[y][x + 3] &&
                     qrCode.ModuleMatrix[y][x + 4] &&
                     !qrCode.ModuleMatrix[y][x + 5] &&
                     qrCode.ModuleMatrix[y][x + 6] &&
                     qrCode.ModuleMatrix[y][x + 7] &&
                     qrCode.ModuleMatrix[y][x + 8] &&
                     !qrCode.ModuleMatrix[y][x + 9] &&
                     qrCode.ModuleMatrix[y][x + 10]))
                    score3 += 40;

                // по вертикали
                if ((qrCode.ModuleMatrix[x][y] &&
                     !qrCode.ModuleMatrix[x + 1][y] &&
                     qrCode.ModuleMatrix[x + 2][y] &&
                     qrCode.ModuleMatrix[x + 3][y] &&
                     qrCode.ModuleMatrix[x + 4][y] &&
                     !qrCode.ModuleMatrix[x + 5][y] &&
                     qrCode.ModuleMatrix[x + 6][y] &&
                     !qrCode.ModuleMatrix[x + 7][y] &&
                     !qrCode.ModuleMatrix[x + 8][y] &&
                     !qrCode.ModuleMatrix[x + 9][y] &&
                     !qrCode.ModuleMatrix[x + 10][y]) ||
                    (!qrCode.ModuleMatrix[x][y] &&
                     !qrCode.ModuleMatrix[x + 1][y] &&
                     !qrCode.ModuleMatrix[x + 2][y] &&
                     !qrCode.ModuleMatrix[x + 3][y] &&
                     qrCode.ModuleMatrix[x + 4][y] &&
                     !qrCode.ModuleMatrix[x + 5][y] &&
                     qrCode.ModuleMatrix[x + 6][y] &&
                     qrCode.ModuleMatrix[x + 7][y] &&
                     qrCode.ModuleMatrix[x + 8][y] &&
                     !qrCode.ModuleMatrix[x + 9][y] &&
                     qrCode.ModuleMatrix[x + 10][y]))
                    score3 += 40;
            }

            //Недостаток 4: Пропорции темных и светлых модулей
            var blackModules = 0;
            foreach (var bitArray in qrCode.ModuleMatrix)
                for (var x = 0; x < size; x++)
                    if (bitArray[x])
                        blackModules++;

            var percentDiv = blackModules * 20 / (qrCode.ModuleMatrix.Count * qrCode.ModuleMatrix.Count);
            var prevMultipleOf5 = Math.Abs(percentDiv - 10);
            var nextMultipleOf5 = Math.Abs(percentDiv - 9);
            score4 = Math.Min(prevMultipleOf5, nextMultipleOf5) * 10;

            return score1 + score2 + score3 + score4;
        }
    }
}