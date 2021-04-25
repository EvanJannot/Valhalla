using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Core;

namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du démon qui attaque simplement le joueur 
    public class Demon : Monster
    {
        public static Demon Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Demon
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = RLColor.Red,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Demon",
                Speed = 14,
                Symbol = (char)128,
                Xp = level * 60
            };
        }
    }
}
