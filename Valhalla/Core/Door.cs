using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
    public class Door : IDrawable
    {
        public Door()
        {
            Symbol = (char)143;
            Color = Colors.Door;
            BackgroundColor = Colors.DoorBackground;
        }
        public bool IsOpen { get; set; }

        public RLColor Color { get; set; }
        public RLColor BackgroundColor { get; set; }
        public char Symbol { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (lvl == 1)
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)143 : (char)142;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else if (lvl == 2)
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)153 : (char)152;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.LightGray;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Gray;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else if (lvl == 3)
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)153 : (char)152;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.LightGray;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Gray;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }
            else
            {
                if (!map.GetCell(X, Y).IsExplored)
                {
                    return;
                }

                Symbol = IsOpen ? (char)143 : (char)142;
                if (map.IsInFov(X, Y))
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }
                else
                {
                    Color = RLColor.Brown;
                    BackgroundColor = RLColor.Black;
                }

                console.Set(X, Y, Color, BackgroundColor, Symbol);
            }

        }
    }
}