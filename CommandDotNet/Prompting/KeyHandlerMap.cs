using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandDotNet.Prompting
{
    public class KeyHandlerMap
    {
        private Dictionary<ConsoleKeyInfo, KeyHandlerDelegate> HandlersByKey = 
            new Dictionary<ConsoleKeyInfo, KeyHandlerDelegate>(KeyComparer);

        private Dictionary<ConsoleKeyInfo, KeyHandlerDelegate> HandlersByKeyChar =
            new Dictionary<ConsoleKeyInfo, KeyHandlerDelegate>(KeyCharComparer);

        #region Map methods

        public void Map(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, false, false, false);
            HandlersByKey[info] = handler;
        }

        public void MapAlt(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, false, true, false);
            HandlersByKey[info] = handler;
        }

        public void MapShift(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, true, false, false);
            HandlersByKey[info] = handler;
        }

        public void MapShiftAlt(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, true, true, false);
            HandlersByKey[info] = handler;
        }

        public void MapControl(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, false, false, true);
            HandlersByKey[info] = handler;
        }

        public void MapControlShift(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, true, false, true);
            HandlersByKey[info] = handler;
        }

        public void MapControlShiftAlt(ConsoleKey key, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(' ', key, true, true, true);
            HandlersByKey[info] = handler;
        }

        public void Map(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, false, false, false);
            HandlersByKeyChar[info] = handler;
        }

        public void MapAlt(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, false, true, false);
            HandlersByKeyChar[info] = handler;
        }

        public void MapShift(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, true, false, false);
            HandlersByKeyChar[info] = handler;
        }

        public void MapShiftAlt(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, true, true, false);
            HandlersByKeyChar[info] = handler;
        }

        public void MapControl(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, false, false, true);
            HandlersByKeyChar[info] = handler;
        }

        public void MapControlShift(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, true, false, true);
            HandlersByKeyChar[info] = handler;
        }

        public void MapControlShiftAlt(char keyChar, KeyHandlerDelegate handler)
        {
            var info = new ConsoleKeyInfo(keyChar, ConsoleKey.Zoom, true, true, true);
            HandlersByKeyChar[info] = handler;
        }

        #endregion

        public bool TryGet(ConsoleKeyInfo info, out KeyHandlerDelegate? handler)
        {
            return HandlersByKey.TryGetValue(info, out handler)
                   || HandlersByKeyChar.TryGetValue(info, out handler);
        }

        public IEnumerable<(ConsoleKeyInfo info, KeyHandlerDelegate handler)> AllHandlers =>
            HandlersByKey.Select(kvp => (kvp.Key, kvp.Value))
                .Concat(HandlersByKeyChar.Select(kvp => (kvp.Key, kvp.Value)));

        #region Comparers

        public static IEqualityComparer<ConsoleKeyInfo> KeyComparer { get; } = new KeyEqualityComparer();

        public static IEqualityComparer<ConsoleKeyInfo> KeyCharComparer { get; } = new KeyCharEqualityComparer();

        private sealed class KeyEqualityComparer : IEqualityComparer<ConsoleKeyInfo>
        {
            public bool Equals(ConsoleKeyInfo x, ConsoleKeyInfo y) => 
                x.Key == y.Key && x.Modifiers == y.Modifiers;

            public int GetHashCode(ConsoleKeyInfo info)
            {
                unchecked
                {
                    return (info.Key.GetHashCode() * 397) ^ info.Modifiers.GetHashCode();
                }
            }
        }

        private sealed class KeyCharEqualityComparer : IEqualityComparer<ConsoleKeyInfo>
        {
            public bool Equals(ConsoleKeyInfo x, ConsoleKeyInfo y) => 
                x.KeyChar == y.KeyChar && x.Modifiers == y.Modifiers;

            public int GetHashCode(ConsoleKeyInfo info)
            {
                unchecked
                {
                    return (info.KeyChar.GetHashCode() * 397) ^ info.Modifiers.GetHashCode();
                }
            }
        }

        #endregion
    }
}