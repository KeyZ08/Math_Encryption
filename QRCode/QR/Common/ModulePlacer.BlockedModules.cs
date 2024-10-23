using System.Collections;
using System.Drawing;

namespace QRCode.QR.Common;

public static partial class ModulePlacer
{
    //содержит заблокированные блоки
    public struct BlockedModules : IDisposable
    {
        private readonly BitArray[] blockedModules;

        private static BitArray[]? _staticBlockedModules;

        public BlockedModules(int size)
        {
            blockedModules = Interlocked.Exchange(ref _staticBlockedModules, null)!;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (blockedModules != null && blockedModules.Length >= size)
            {
                for (var i = 0; i < size; i++)
                    blockedModules[i].SetAll(false);
            }
            else
            {
                blockedModules = new BitArray[size];
                for (var i = 0; i < size; i++)
                    blockedModules[i] = new BitArray(size);
            }
        }

        public void Add(Rectangle rect)
        {
            for (var y = rect.Y; y < rect.Y + rect.Height; y++)
            for (var x = rect.X; x < rect.X + rect.Width; x++)
                blockedModules[y][x] = true;
        }

        public bool IsBlocked(int x, int y)
            => blockedModules[y][x];

        public bool IsBlocked(Rectangle r1)
        {
            for (var y = r1.Y; y < r1.Y + r1.Height; y++)
            for (var x = r1.X; x < r1.X + r1.Width; x++)
                if (blockedModules[y][x])
                    return true;
            return false;
        }

        public void Dispose()
            => Interlocked.CompareExchange(ref _staticBlockedModules, blockedModules, null);
    }
}