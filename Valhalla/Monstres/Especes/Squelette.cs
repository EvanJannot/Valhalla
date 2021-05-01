using RogueSharp.DiceNotation;
using Valhalla.Core;


namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du squelette qui attaque simplement le joueur 
    public class Squelette : Monster
    {
        public static Squelette Create(int level)
        {
            int health = Dice.Roll("3D5");
            return new Squelette
            {
                Attack = Dice.Roll("2D3") + level / 3,
                AttackChance = Dice.Roll("30D3"),
                Awareness = 10,
                Color = Colors.Player,
                Defense = Dice.Roll("2D3") + level / 3,
                DefenseChance = Dice.Roll("15D4"),
                Gold = Dice.Roll("5D5"),
                Health = health,
                MaxHealth = health,
                Name = "Squelette",
                Speed = 14,
                Symbol = (char)46,
                Xp = level * 40
            };
        }
    }
}
