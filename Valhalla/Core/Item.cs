using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;
using System.Text;

namespace RogueSharpRLNetSamples.Core
{
    public class Item : IItem, ITreasure, IDrawable
    {
        StringBuilder espace = new StringBuilder();
        public Item()
        {
            Symbol = (char)15;
            Color = RLColor.Brown;
        }

        public string Name { get; protected set; }
        public int RemainingUses { get; protected set; }

        public bool Use()
        {
            return UseItem();
        }

        protected virtual bool UseItem()
        {
            return false;
        }

        public bool PickUp(IActor actor)
        {
            Player player = actor as Player;

            if (player != null)
            {
                if (player.AddItem(this))
                {
                    Game.MessageLog.Add($"{actor.Name} a ramasse {Name}");
                    Game.MessageLog.Add(espace.ToString());
                    return true;
                }
            }

            return false;
        }

        public RLColor Color { get; set; }

        public char Symbol { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public void Draw(RLConsole console, IMap map, int lvl)
        {
            if (!map.IsExplored(X, Y))
            {
                return;
            }

            if (map.IsInFov(X, Y))
            {
                console.Set(X, Y, Color, Colors.FloorBackgroundFov, Symbol);
            }
            else
            {
                console.Set(X, Y, RLColor.Blend(Color, RLColor.Gray, 0.5f), Colors.FloorBackground, Symbol);
            }
        }
    }
}
