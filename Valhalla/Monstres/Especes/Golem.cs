using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Behaviors;
using Valhalla.Core;
using Valhalla.Systems;

namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du golem le boss du monde 2 qui attaque simplement le joueur 
    public class Golem : Monster
    {
        public static Golem Create(int level)
        {
            int health = 15;
            return new Golem
            {
                Attack = 10,
                AttackChance = 40,
                Awareness = 10,
                Color = RLColor.Brown,
                Defense = 2,
                DefenseChance = 70,
                Gold = 100,
                Health = health,
                MaxHealth = health,
                Name = "Golem",
                Speed = 20,
                Symbol = (char)7,
                Xp = level * 300
            };
        }
    }
}
