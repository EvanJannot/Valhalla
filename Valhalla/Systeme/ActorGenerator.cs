using RogueSharp;
using Valhalla.Core;
using Valhalla.Monsters;

namespace Valhalla.Systems
{
    //Classe qui permet de générer des acteurs (monstres ou joueur)
    public static class ActorGenerator
    {
        private static Player _player = null;

        //Fonction pour créer des monstres 
        public static Monster CreateMonster(int level, Point location, int monde)
        {
            Pool<Monster> monsterPool = new Pool<Monster>(); //Comme pour les objets, les sorts et l'equipement on créer une liste ou chaque élément a un poids
            if (monde == 1) //Monstres pour le monde 1
            {
                if (level == 6) //Boss du monde 1
                {
                    monsterPool.Add(Ours.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Araignee.Create(monde), 25);
                    monsterPool.Add(Slime.Create(monde), 25);
                    monsterPool.Add(Abeille.Create(monde), 50);
                }
            }
            else if (monde == 2) //Monstres du monde 2
            {
                if (level == 6) //Boss du monde 2
                {
                    monsterPool.Add(Golem.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Squelette.Create(monde), 25);
                    monsterPool.Add(Fantome.Create(monde), 25);
                    monsterPool.Add(Chauve_souris.Create(monde), 50);
                }
            }
            else if (monde == 3) //Monstres du monde 3
            {
                if (level == 6) //Boss du monde 3
                {
                    monsterPool.Add(Roi.Create(monde), 100);
                }
                else
                {
                    monsterPool.Add(Demon.Create(monde), 25);
                    monsterPool.Add(Garde.Create(monde), 25);
                    monsterPool.Add(Rat.Create(monde), 50);
                }
            }
            else if (monde != 1 & monde != 2 & monde != 3) //Pour les mondes d'après on peut avoir tous les monstres
            {
                if (level == 6) //Et tous les boss
                {
                    monsterPool.Add(Roi.Create(monde), 33);
                    monsterPool.Add(Ours.Create(monde), 33);
                    monsterPool.Add(Golem.Create(monde), 34);
                }
                else
                {
                    monsterPool.Add(Demon.Create(monde), 11);
                    monsterPool.Add(Garde.Create(monde), 12);
                    monsterPool.Add(Rat.Create(monde), 11);
                    monsterPool.Add(Araignee.Create(monde), 11);
                    monsterPool.Add(Slime.Create(monde), 11);
                    monsterPool.Add(Abeille.Create(monde), 11);
                    monsterPool.Add(Squelette.Create(monde), 11);
                    monsterPool.Add(Fantome.Create(monde), 11);
                    monsterPool.Add(Chauve_souris.Create(monde), 11);
                }
            }




            Monster monster = monsterPool.Get(); //On sélectionne un des monstres et on définit ses coordonnées 
            monster.X = location.X;
            monster.Y = location.Y;

            return monster; //On le retourne
        }

        //Supprime le joueur lorsqu'une partie est recommencée après la mort 
        public static void ResetPlayer()
        {
            _player = null;
        }

        //Fonction pour créer un joueur
        public static Player CreatePlayer(int atkX, int defX, int awaX, int hpX, string name, char symbol)
        {
            if (_player == null) //Si il n'y a pas de joueur 
            {
                _player = new Player
                { //On en créé un avec des statistiques de bases et des bonus suivant la classe sélectionnée par le joueur 
                    Attack = 2 + atkX,
                    AttackChance = 50,
                    Awareness = 15 + awaX,
                    Color = Colors.Player,
                    Defense = 2 + defX,
                    DefenseChance = 40,
                    Gold = 0,
                    Health = 100 + hpX,
                    MaxHealth = 100 + hpX,
                    Xp = 0,
                    Lvl = 1,
                    Name = name,
                    Speed = 10,
                    Symbol = symbol
                };
            }

            return _player; //On retourne le joueur 
        }
    }
}