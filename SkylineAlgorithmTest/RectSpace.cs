using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylineAlgorithmTest
{
    public class RectSpace
    {
        private int _currentWidth = 0;
        private int _currentHeight = 0;

        public List<Skyline> HorizontalSkylines { get; } = new List<Skyline>();
        public List<Skyline> VerticalSkylines { get; } = new List<Skyline>();
        public List<Rect> Rectangles { get; } = new List<Rect>();

        public RectSpace()
        {

        }

        public int Width => _currentWidth;
        public int Height => _currentHeight;

        private static Skyline CombineSkyline(IList<Skyline> skylines, int startIndex, int endIndex)
        {
            Skyline combinedSkyline = skylines[startIndex];
            for (int i = startIndex + 1; i < endIndex; i++)
            {
                combinedSkyline.End = skylines[i].End;
                combinedSkyline.Height = Math.Max(combinedSkyline.Height, skylines[i].Height);
            }

            return combinedSkyline;
        }

        private static bool IsSkylinesGood(IList<Skyline> skylines)
        {
            if (skylines.Count <= 1)
            {
                return true;
            }

            Skyline lastSkyline = skylines[0];

            for (int i = 1; i < skylines.Count; i++)
            {
                if (skylines[i].Start != lastSkyline.End)
                {
                    return false;
                }

                lastSkyline = skylines[i];
            }

            return true;
        }

        private static void MergeSkylines(IList<Skyline> skylines)
        {
            if (skylines.Count <= 1)
            {
                return;
            }

            int lastSkylineIndex = 0;
            Skyline lastSkyline = skylines[0];

            int index = 1;
            while (index < skylines.Count)
            {
                var currentSkyline = skylines[index];

                if (currentSkyline.Height != lastSkyline.Height)
                {
                    lastSkylineIndex = index;
                    lastSkyline = currentSkyline;
                    index++;
                }
                else
                {
                    skylines[lastSkylineIndex] = new Skyline()
                    {
                        Start = lastSkyline.Start,
                        End = currentSkyline.End,
                        Height = currentSkyline.Height
                    };

                    skylines.RemoveAt(index);
                }
            }
        }

        private static void PopulateSkylines(IList<Skyline> skylines, Skyline newSkyline)
        {
#if DEBUG
            var skylinesCopy = skylines.ToArray();
            var originNewSkyline = newSkyline;
#endif

            if (skylines.Count == 0)
            {
                skylines.Add(newSkyline);
                return;
            }

            for (int i = 0; i < skylines.Count; i++)
            {
                Skyline currentSkyline = skylines[i];

                if (currentSkyline.End <= newSkyline.Start ||
                    currentSkyline.Start >= newSkyline.End)
                {
                    continue;
                }

                if (newSkyline.Height > currentSkyline.Height)
                {
                    if (newSkyline.Start == currentSkyline.Start)
                    {
                        if (newSkyline.End < currentSkyline.End)
                        {
                            skylines[i] = new Skyline()
                            {
                                Start = newSkyline.End,
                                End = currentSkyline.End,
                                Height = currentSkyline.Height,
                            };

                            skylines.Insert(i, new Skyline()
                            {
                                Start = currentSkyline.Start,
                                End = newSkyline.End,
                                Height = newSkyline.Height,
                            });

                            newSkyline.Start = newSkyline.End;
                            break;
                        }
                        else
                        {
                            skylines[i] = new Skyline()
                            {
                                Start = currentSkyline.Start,
                                End = currentSkyline.End,
                                Height = newSkyline.Height,
                            };

                            newSkyline.Start = currentSkyline.End;
                        }
                    }
                    else
                    {
                        if (newSkyline.End < currentSkyline.End)
                        {
                            skylines[i] = new Skyline()
                            {
                                Start = currentSkyline.Start,
                                End = newSkyline.Start,
                                Height = currentSkyline.Height,
                            };

                            i++;
                            skylines.Insert(i, new Skyline()
                            {
                                Start = newSkyline.Start,
                                End = newSkyline.End,
                                Height = newSkyline.Height,
                            });

                            i++;
                            skylines.Insert(i, new Skyline()
                            {
                                Start = newSkyline.End,
                                End = currentSkyline.End,
                                Height = currentSkyline.Height,
                            });

                            newSkyline.Start = newSkyline.End;
                            break;
                        }
                        else
                        {
                            skylines.Insert(i, new Skyline()
                            {
                                Start = newSkyline.Start,
                                End = currentSkyline.End,
                                Height = newSkyline.Height
                            });

                            newSkyline.Start = currentSkyline.End;
                        }
                    }
                }
                else
                {
                    if (newSkyline.Start == currentSkyline.Start)
                    {
                        if (newSkyline.End < currentSkyline.End)
                        {
                            newSkyline.Start = newSkyline.End;
                            break;
                        }
                        else
                        {
                            newSkyline.Start = currentSkyline.End;
                        }
                    }
                    else
                    {
                        if (newSkyline.End < currentSkyline.End)
                        {
                            newSkyline.Start = newSkyline.End;
                            break;
                        }
                        else
                        {
                            newSkyline.Start = currentSkyline.End;
                        }
                    }
                }
            }

            if (newSkyline.Length > 0)
            {
                skylines.Add(newSkyline);
            }

#if DEBUG
            if (!IsSkylinesGood(skylines))
            {
                Debugger.Break();
            }
#endif

            MergeSkylines(skylines);
        }

        private bool LayoutCore(
            ref int currentSizeU,
            ref int currentSizeV,
            List<Skyline> skylinesU,
            List<Skyline> skylinesV,
            int sizeU, int sizeV, out int posU, out int posV)
        {
            if (_currentWidth < sizeU)
            {
                posU = 0;
                posV = currentSizeV;

                currentSizeU = sizeU;
                currentSizeV = currentSizeV + sizeV;

                skylinesU.Clear();
                skylinesU.Add(new Skyline()
                {
                    Start = 0,
                    End = sizeU,
                    Height = currentSizeV,
                });

                PopulateSkylines(skylinesV, new Skyline()
                {
                    Start = posV,
                    End = posV + sizeV,
                    Height = currentSizeU,
                });

                return true;
            }

            var bestSkyline = default(Skyline);
            var bestSkylineIndex = -1;

            for (int step = 1; step <= skylinesU.Count; step++)
            {
                var combinedSkylineEndIndex = skylinesU.Count - (step - 1);

                for (int i = 0; i < combinedSkylineEndIndex; i++)
                {
                    var combinedSkyline = CombineSkyline(skylinesU, i, i + step);

                    if (combinedSkyline.Length >= sizeU &&
                        (bestSkylineIndex == -1 || combinedSkyline.Height < bestSkyline.Height))
                    {
                        bestSkyline = combinedSkyline;
                        bestSkylineIndex = i;
                    }
                }
            }

            if (bestSkylineIndex >= 0 &&
                bestSkyline.Length >= sizeU)
            {
                posU = bestSkyline.Start;
                posV = bestSkyline.Height;

                var newSkyline = new Skyline()
                {
                    Start = posU,
                    End = posU + sizeU,
                    Height = bestSkyline.Height + sizeV,
                };

                currentSizeV = Math.Max(currentSizeV, newSkyline.Height);

                PopulateSkylines(skylinesU, new Skyline()
                {
                    Start = posU,
                    End = posU + sizeU,
                    Height = posV + sizeV
                });

                PopulateSkylines(skylinesV, new Skyline()
                {
                    Start = posV,
                    End = posV + sizeV,
                    Height = posU + sizeU,
                });

                return true;
            }

            throw new InvalidOperationException("This would never happen.");
        }

        private bool LayoutVertical(int width, int height, out int x, out int y)
        {
            bool ok = LayoutCore(ref _currentWidth, ref _currentHeight, HorizontalSkylines, VerticalSkylines, width, height, out x, out y);
            Rectangles.Add(new Rect()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
            });

            return ok;
        }

        private bool LayoutHorizontal(int width, int height, out int x, out int y)
        {
            bool ok = LayoutCore(ref _currentHeight, ref _currentWidth, VerticalSkylines, HorizontalSkylines, height, width, out y, out x);
            Rectangles.Add(new Rect()
            {
                X = x,
                Y = y,
                Width = width,
                Height = height,
            });

            return ok;
        }

        public bool Layout(int width, int height, out int x, out int y)
        {
            if (_currentWidth <= _currentHeight)
            {
                return LayoutHorizontal(width, height, out x, out y);
            }
            else
            {
                return LayoutVertical(width, height, out x, out y);
            }
        }

        public void Reset()
        {
            _currentWidth = 0;
            _currentHeight = 0;

            HorizontalSkylines.Clear();
            VerticalSkylines.Clear();
            Rectangles.Clear();
        }

        public struct Rect
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public struct Skyline
        {
            public int Height { get; set; }
            public int Start { get; set; }
            public int End { get; set; }

            public int Length => End - Start;
        }
    }
}
