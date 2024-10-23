using System.Drawing;

namespace QRCode.QR.Common;

/// <summary>
///     Шаблон выравнивания
/// </summary>
/// <param name="PatternPositions">
///     Список точек, в которых расположены шаблоны выравнивания, в матрице QR-кодов.
///     Каждая точка представляет собой центр шаблона выравнивания.
/// </param>
public record AlignmentPattern(List<Point> PatternPositions) { }