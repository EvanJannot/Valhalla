using RLNET;
using RogueSharp;

namespace RogueSharpRLNetSamples.Core
{
    public class Stairs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public bool IsUp { get; set; }

        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (!map.GetCell(X, Y).IsExplored)
            {
                return;
            }
            if (lvl == 3)
            {
                if (map.IsInFov(X, Y))
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, RLColor.LightGray, RLColor.Black, (char)18);
                    }
                    else
                    {
                        console.Set(X, Y, RLColor.LightGray, RLColor.Black, (char)19);
                    }
                }
                else
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, RLColor.Gray, RLColor.Black, (char)18);
                    }
                    else
                    {
                        console.Set(X, Y, RLColor.Gray, RLColor.Black, (char)19);
                    }
                }
            }
            else
            {
                if (map.IsInFov(X, Y))
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, Colors.Player, RLColor.Black, (char)151);
                    }
                    else
                    {
                        console.Set(X, Y, Colors.Player, RLColor.Black, (char)150);
                    }
                }
                else
                {
                    if (IsUp)
                    {
                        console.Set(X, Y, Colors.Floor, RLColor.Black, (char)151);
                    }
                    else
                    {
                        console.Set(X, Y, Colors.Floor, RLColor.Black, (char)150);
                    }
                }

            }


        }
    }
}