using System.Collections;
using System.Drawing;
using static QRCode.QR.Common.Common;

namespace QRCode.QR.Common;

public static partial class ModulePlacer
{
    /// <summary>
    ///     Помещает информацию о версии в матрицу QR-кода
    /// </summary>
    public static void PlaceVersion(QRCodeData qrCode, BitArray versionStr, bool offset)
    {
        var offsetValue = offset ? 4 : 0;
        var size = qrCode.ModuleMatrix.Count - offsetValue - offsetValue;

        for (var x = 0; x < 6; x++)
        for (var y = 0; y < 3; y++)
        {
            qrCode.ModuleMatrix[y + size - 11 + offsetValue][x + offsetValue] = versionStr[17 - (x * 3 + y)];
            qrCode.ModuleMatrix[x + offsetValue][y + size - 11 + offsetValue] = versionStr[17 - (x * 3 + y)];
        }
    }

    /// <summary>
    ///     Помещает информацию уровень исправления ошибок и используемый шаблон маски в QR-код.
    /// </summary>
    public static void PlaceFormat(QRCodeData qrCode, BitArray formatStr, bool offset)
    {
        var offsetValue = offset ? 4 : 0;
        var size = qrCode.ModuleMatrix.Count - offsetValue - offsetValue;

        for (var i = 0; i < 15; i++)
        {
            var x1 = i < 8 ? 8 : i == 8 ? 7 : 14 - i;
            var y1 = i < 6 ? i : i < 7 ? i + 1 : 8;
            var x2 = i < 8 ? size - 1 - i : 8;
            var y2 = i < 8 ? 8 : size - (15 - i);

            qrCode.ModuleMatrix[y1 + offsetValue][x1 + offsetValue] = formatStr[14 - i];
            qrCode.ModuleMatrix[y2 + offsetValue][x2 + offsetValue] = formatStr[14 - i];
        }
    }

    /// <summary>
    ///     Применяет к QR-коду наиболее эффективный шаблон маски, основанный на минимизации штрафного балла,
    ///     который определяет, насколько хорошо шаблон будет работать для QR-сканеров.
    /// </summary>
    /// <returns>Индекс выбранной маски</returns>
    public static int MaskCode(QRCodeData qrCode, int version, BlockedModules blockedModules)
    {
        var selectedPattern = -1; // паттерн не выбран
        var patternScore = int.MaxValue;
        var size = qrCode.ModuleMatrix.Count - 8;

        var qrTemp = new QRCodeData();
        BitArray? versionString = null;
        versionString = new BitArray(18);
        GetVersionString(versionString, version);

        var formatStr = new BitArray(15);
        for (var maskPattern = 0; maskPattern < 8; maskPattern++)
        {
            var patternFunc = MaskPattern.Patterns[maskPattern];

            // сброс временного QR кода
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                qrTemp.ModuleMatrix[y][x] = qrCode.ModuleMatrix[y + 4][x + 4];

            GetFormatString(formatStr, maskPattern);
            PlaceFormat(qrTemp, formatStr, false);

            PlaceVersion(qrTemp, versionString, false);

            // Применяем маску
            for (var x = 0; x < size; x++)
            {
                for (var y = 0; y < x; y++)
                    if (!blockedModules.IsBlocked(x, y))
                    {
                        qrTemp.ModuleMatrix[y][x] ^= patternFunc(x, y);
                        qrTemp.ModuleMatrix[x][y] ^= patternFunc(y, x);
                    }

                if (!blockedModules.IsBlocked(x, x)) qrTemp.ModuleMatrix[x][x] ^= patternFunc(x, x);
            }

            var score = MaskPattern.Score(qrTemp);

            if (patternScore > score)
            {
                selectedPattern = maskPattern;
                patternScore = score;
            }
        }

        // применяем лучшую маску
        var selectedPatternFunc = MaskPattern.Patterns[selectedPattern];
        for (var x = 0; x < size; x++)
        {
            for (var y = 0; y < x; y++)
                if (!blockedModules.IsBlocked(x, y))
                {
                    qrCode.ModuleMatrix[y + 4][x + 4] ^= selectedPatternFunc(x, y);
                    qrCode.ModuleMatrix[x + 4][y + 4] ^= selectedPatternFunc(y, x);
                }

            if (!blockedModules.IsBlocked(x, x)) qrCode.ModuleMatrix[x + 4][x + 4] ^= selectedPatternFunc(x, x);
        }

        return selectedPattern;
    }

    /// <summary>
    ///     Помещает биты данных в матрицу модуля QR-кода
    /// </summary>
    public static void PlaceDataWords(QRCodeData qrCode, BitArray data, BlockedModules blockedModules)
    {
        var size = qrCode.ModuleMatrix.Count - 8;
        var up = true;
        var index = 0;
        var count = data.Length;

        // идем справа налево и пропуская столбец (тк мы записываем сразу в 2)
        for (var x = size - 1; x >= 0; x -= 2)
        {
            if (x == 6) // пропускаем полосу синхронизации
                x = 5;

            for (var yMod = 1; yMod <= size; yMod++)
            {
                int y;

                if (up)
                {
                    y = size - yMod;
                    if (index < count && !blockedModules.IsBlocked(x, y))
                        qrCode.ModuleMatrix[y + 4][x + 4] = data[index++];
                    if (index < count && x > 0 && !blockedModules.IsBlocked(x - 1, y))
                        qrCode.ModuleMatrix[y + 4][x - 1 + 4] = data[index++];
                }
                else
                {
                    y = yMod - 1;
                    if (index < count && !blockedModules.IsBlocked(x, y))
                        qrCode.ModuleMatrix[y + 4][x + 4] = data[index++];
                    if (index < count && x > 0 && !blockedModules.IsBlocked(x - 1, y))
                        qrCode.ModuleMatrix[y + 4][x - 1 + 4] = data[index++];
                }
            }

            up = !up;
        }
    }

    /// <summary>
    ///     Блокирует области вокруг поисковых блоков
    /// </summary>
    public static void ReserveSeperatorAreas(int size, BlockedModules blockedModules)
    {
        blockedModules.Add(new Rectangle(7, 0, 1, 8));
        blockedModules.Add(new Rectangle(0, 7, 7, 1));
        blockedModules.Add(new Rectangle(0, size - 8, 8, 1));
        blockedModules.Add(new Rectangle(7, size - 7, 1, 7));
        blockedModules.Add(new Rectangle(size - 8, 0, 1, 8));
        blockedModules.Add(new Rectangle(size - 7, 7, 7, 1));
    }

    /// <summary>
    ///     Блокирует области для версии, маски, уровня коррекции
    /// </summary>
    public static void ReserveVersionAreas(int size, BlockedModules blockedModules)
    {
        //код маски и уровня коррекции
        blockedModules.Add(new Rectangle(8, 0, 1, 6)); // Рядом с верхней полосой синхронизации
        blockedModules.Add(new Rectangle(8, 7, 1, 1)); // Рядом с верхним левым поисковым блоком
        blockedModules.Add(new Rectangle(0, 8, 6, 1)); // Рядом с левой полосой синхронизации
        blockedModules.Add(new Rectangle(7, 8, 2, 1)); // расширение блока выше
        blockedModules.Add(new Rectangle(size - 8, 8, 8, 1)); // Рядом с верхней полосой синхронизации
        blockedModules.Add(new Rectangle(8, size - 7, 1, 7)); // Рядом с левой полосой синхронизации

        //блоки версии
        blockedModules.Add(new Rectangle(size - 11, 0, 3, 6)); // право верх
        blockedModules.Add(new Rectangle(0, size - 11, 6, 3)); // лево низ
    }

    /// <summary>
    ///     Размещает темный модуль на матрице QR-кода (он всегда темный)
    /// </summary>
    public static void PlaceDarkModule(QRCodeData qrCode, BlockedModules blockedModules)
    {
        qrCode.ModuleMatrix[4 * 10 + 9 + 4][8 + 4] = true;
        blockedModules.Add(new Rectangle(8, 4 * 10 + 9, 1, 1));
    }

    /// <summary>
    ///     Размещает поисковые блоки на QR-коде (3 квадратных узора по краям)
    /// </summary>
    public static void PlaceFinderPatterns(QRCodeData qrCode, BlockedModules blockedModules)
    {
        var size = qrCode.ModuleMatrix.Count - 8;

        for (var i = 0; i < 3; i++) // 3 прямоугольники
        {
            var locationX = i == 1 ? size - 7 : 0; // право-лево
            var locationY = i == 2 ? size - 7 : 0; // верх-низ

            // черная рамка 7x7, с черной рамкой 5x5 внутри и черным квадратом 3x3 
            for (var x = 0; x < 7; x++)
            for (var y = 0; y < 7; y++)
                if (!(((x == 1 || x == 5) && y > 0 && y < 6)
                      || (x > 0 && x < 6 && (y == 1 || y == 5))))
                    qrCode.ModuleMatrix[y + locationY + 4][x + locationX + 4] = true;

            blockedModules.Add(new Rectangle(locationX, locationY, 7, 7));
        }
    }

    /// <summary>
    ///     Размещает блоки выравнивания на матрице QR-кода (небольшие квадратные узоры внутри QR кода)
    /// </summary>
    public static void PlaceAlignmentPatterns(
        QRCodeData qrCode,
        List<Point> alignmentPatternLocations,
        BlockedModules blockedModules)
    {
        foreach (var loc in alignmentPatternLocations)
        {
            // проверяем что там не занято
            var alignmentPatternRect = new Rectangle(loc.X, loc.Y, 5, 5);
            if (blockedModules.IsBlocked(alignmentPatternRect))
                continue;

            // черная рамка 5x5 с точкой в центре
            for (var x = 0; x < 5; x++)
            for (var y = 0; y < 5; y++)
                if (y == 0 || y == 4 || x == 0 || x == 4 || (x == 2 && y == 2))
                    qrCode.ModuleMatrix[loc.Y + y + 4][loc.X + x + 4] = true;

            blockedModules.Add(new Rectangle(loc.X, loc.Y, 5, 5));
        }
    }

    /// <summary>
    ///     Размещает полосы синхронизации (чередующиеся темные и светлые модули)
    /// </summary>
    public static void PlaceTimingPatterns(QRCodeData qrCode, BlockedModules blockedModules)
    {
        var size = qrCode.ModuleMatrix.Count - 8;

        for (var i = 8; i < size - 8; i++)
            if (i % 2 == 0)
            {
                qrCode.ModuleMatrix[6 + 4][i + 4] = true; // горизонтальный модуль
                qrCode.ModuleMatrix[i + 4][6 + 4] = true; // вертикальный модуль
            }

        blockedModules.Add(new Rectangle(6, 8, 1, size - 16));
        blockedModules.Add(new Rectangle(8, 6, size - 16, 1));
    }
}