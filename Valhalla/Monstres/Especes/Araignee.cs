using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Core;


namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques de l'araignée qui attaque simplement le joueur 
    public class Araignee : Monster
    {
        public static Araignee Create(int level)
        {
            int health = Dice.Roll("2D5");
            return new Araignee
            {
                Attack = Dice.Roll("1D3") + level / 3,
                AttackChance = Dice.Roll("25D3"),
                Awareness = 10,
                Color = RLColor.Gray,
                Defense = Dice.Roll("1D3") + level / 3,
                DefenseChance = Dice.Roll("10D4"),
                Gold = Dice.Roll("3D5"),
                Health = health,
                MaxHealth = health,
                Name = "Araignee",
                Speed = 14,
                Symbol = (char)129,
                Xp = (level * 20)
            };
        }
    }
}
