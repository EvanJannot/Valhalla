﻿using RLNET;
using RogueSharp.DiceNotation;
using Valhalla.Behaviors;
using Valhalla.Core;
using Valhalla.Systems;


namespace Valhalla.Monsters
{
    //Classe de héritant de la classe monstre et qui définit les caractéristiques du roi le boss du monde 3 qui attaque simplement le joueur 
    public class Roi : Monster
    {
        public static Roi Create(int level)
        {
            int health = 15;
            return new Roi
            {
                Attack = 10,
                AttackChance = 40,
                Awareness = 10,
                Color = Colors.Player,
                Defense = 2,
                DefenseChance = 70,
                Gold = 100,
                Health = health,
                MaxHealth = health,
                Name = "Roi",
                Speed = 20,
                Symbol = (char)144,
                Xp = level * 450
            };
        }
    }
}