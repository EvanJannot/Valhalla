using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Behaviors;
using Valhalla.Core;
using Valhalla.Systems;

namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du garde qui attaque simplement le joueur 
    public class Garde : Monster
    {
        public static Garde Create(int level)
        {
            int health = Dice.Roll("4D5");
            return new Garde
            {
                Attack = Dice.Roll("3D3") + level / 3,
                AttackChance = Dice.Roll("33D3"),
                Awareness = 10,
                Color = Colors.Player,
                Defense = Dice.Roll("3D3") + level / 3,
                DefenseChance = Dice.Roll("20D4"),
                Gold = Dice.Roll("7D5"),
                Health = health,
                MaxHealth = health,
                Name = "Garde",
                Speed = 14,
                Symbol = (char)62,
                Xp = level * 60
            };
        }
    }
}
