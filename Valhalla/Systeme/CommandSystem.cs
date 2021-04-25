using System.Text;
using RLNET;
using RogueSharp;
using RogueSharp.DiceNotation;
using Valhalla.Core;
using Valhalla.Equipment;
using Valhalla.Interfaces;
using Valhalla.Items;

namespace Valhalla.Systems
{
    //Classe qui gère les commandes du joueur 
    public class CommandSystem
    {
        public bool IsPlayerTurn { get; set; }

        public bool MovePlayer(Direction direction) //Permet de bouger le joueur à partir d'une direction
        {
            int x;
            int y;

            switch (direction)
            {
                case Direction.Up: //Si on bouge vers le haut
                    {
                        x = Game.Player.X;
                        y = Game.Player.Y - 1;
                        break;
                    }
                case Direction.Down: //Si on bouge vers le bas
                    {
                        x = Game.Player.X;
                        y = Game.Player.Y + 1;
                        break;
                    }
                case Direction.Left: //Si on bouge vers la gauche
                    {
                        x = Game.Player.X - 1;
                        y = Game.Player.Y;
                        break;
                    }
                case Direction.Right: //Si on bouge vers la droite 
                    {
                        x = Game.Player.X + 1;
                        y = Game.Player.Y;
                        break;
                    }
                default: //Par défault on ne fait rien 
                    {
                        return false;
                    }
            }

            if (Game.DungeonMap.SetActorPosition(Game.Player, x, y)) //Si on arrive à définir le nouvel emplacment du joueur 
            {
                return true; //On renvoie true 
            }

            Monster monster = Game.DungeonMap.GetMonsterAt(x, y); //Si il y a un monstre à cet endroit 
            if (monster != null)
            {
                Attack(Game.Player, monster); //On l'attaque sans se déplacer et on renvoie true 
                return true;
            }

            return false; //Si on ne peut pas déplacer le joueur et qu'il n'y a pas de monstre on renvoie false 
        }

        //Fonction qui 
        public void ActivateMonsters()
        {
            IScheduleable scheduleable = Game.SchedulingSystem.Get(); //On récupère l'acteur dont c'est le tour
            if (scheduleable is Player) //Si c'est le joueur 
            {
                IsPlayerTurn = true; //C'est le tour du joueur 
                Game.SchedulingSystem.Add(Game.Player); //On remet le joueur au sein de la gestion des tours (il est supprimé lors du get)
            }
            else //Sinon
            {
                Monster monster = scheduleable as Monster; //Si c'est le tour d'un monstre 

                if (monster != null)
                {
                    monster.PerformAction(this); //Le monstre joue son tour 
                    Game.SchedulingSystem.Add(monster); //On remet le monstre au sein de la gestion des tours 
                }

                ActivateMonsters(); //Si on a trouvé personne on recommence (etant donné que chaque acteur a une vitesse différente, il peut arriver que ce ne soit le tour de personne)
            }
        }

        //Fonction pour déplacer un monstre 
        public void MoveMonster(Monster monster, Cell cell)
        {
            if (!Game.DungeonMap.SetActorPosition(monster, cell.X, cell.Y)) //Si on ne peut pas déplacer le monstre sur la case
            {
                if (Game.Player.X == cell.X && Game.Player.Y == cell.Y) //Et si il y a un joueur sur cette case 
                {
                    Attack(monster, Game.Player); //Le monstre attaque le joueur 
                }
            }
        }

        //Fonction d'attaque 
        public void Attack(Actor attacker, Actor defender)
        {

            int hits = ResolveAttack(attacker, defender); //On calcule le nombre de coups donnés par l'attaquant sur le defenseur 

            int blocks = ResolveDefense(defender, hits); //On calcule le nombre de coups parés par le défenseur 

            int damage = hits - blocks; //On en déduit les dommages subits 

            ResolveDamage(defender, damage); //On les applique au défenseur 
        }

        //Fonction qui calcule le nombre de coups donnés 
        private static int ResolveAttack(Actor attacker, Actor defender)
        {
            int hits = 0;

            DiceExpression attackDice = new DiceExpression().Dice(attacker.Attack, 100); //On créé autant de dés à 100 faces que l'attaquant a de points d'attaques 

            DiceResult attackResult = attackDice.Roll(); //On les lance et on récupère les résultats 
            foreach (TermResult termResult in attackResult.Results) //Pour chaque résultat 
            {
                if (termResult.Value >= 100 - attacker.AttackChance) //Si le résultat du dé est supérieur ou égale à 100 - l'attaque du joueur le joueur touche
                                                                     //En d'autres mots, si le résultat du dé est inférieur aux chances d'attaques c'est ok
                {
                    hits++; //On augmente de 1 le nombre de coups 
                }
            }

            return hits; //On le renvoie 
        }

        //Fonction qui calcule le nombre de parades 
        private static int ResolveDefense(Actor defender, int hits)
        {
            int blocks = 0;

            if (hits > 0) //Si il y a au moins un coup de mis 
            {
                DiceExpression defenseDice = new DiceExpression().Dice(defender.Defense, 100); //On lance autant de dés que le défenseur a de points de défense
                DiceResult defenseRoll = defenseDice.Roll(); //On lance les dés et on récupère les résultats 
                foreach (TermResult termResult in defenseRoll.Results) //Pour chaque résultat 
                {
                    if (termResult.Value >= 100 - defender.DefenseChance) //Si les chances de défenses sont supérieures au lancer de dé il bloque 
                    {
                        blocks++; //On augmente le nombre de bloquages 
                    }
                }
            }

            return blocks;
        }

        //Fonction qui permet d'appliquer les dommages au défenseur 
        private static void ResolveDamage(Actor defender, int damage)
        {
            if (damage > 0) //Si les dommages sont supérieurs à 0
            {
                defender.Health = defender.Health - damage; //On enlève les dommages à la vie du défenseur 

                if (defender.Health <= 0) //Si çsa vie est inférieure à 0
                {
                    ResolveDeath(defender); //On le tue 
                }
            }
        }

        //Fonction pour tuer un défenseur 
        private static void ResolveDeath(Actor defender)
        {
            StringBuilder espace = new StringBuilder();
            if (defender is Player) //Si le defenseur est le joueur 
            {
                Game.MessageLog.Add($"{defender.Name} est mort, FIN DE LA PARTIE"); //On affiche un message et on affiche l'écran de fin du jeu
                Game.EndGame();
            }
            else if (defender is Monster) //Si c'est un monstre 
            {
                //Si il a un équipement on le pose sur le sol
                if (defender.Head != null && defender.Head != HeadEquipment.None())
                {
                    Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Head);
                }
                if (defender.Body != null && defender.Body != BodyEquipment.None())
                {
                    Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Body);
                }
                if (defender.Hand != null && defender.Hand != HandEquipment.None())
                {
                    Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Hand);
                }
                if (defender.Feet != null && defender.Feet != FeetEquipment.None())
                {
                    Game.DungeonMap.AddTreasure(defender.X, defender.Y, defender.Feet);
                }
                Game.DungeonMap.AddGold(defender.X, defender.Y, defender.Gold); //On ajoute de l'or en fonction des stats du monstre au sol
                Game.Player.AddXp(defender.Xp); //On ajoute de l'xp au joueur 
                Game.DungeonMap.RemoveMonster((Monster)defender); //On enlève le monstre 

                Game.MessageLog.Add($"{defender.Name} est mort et a lache {defender.Gold} d'or"); //On affiche un message dans la console 
                Game.MessageLog.Add(espace.ToString());
            }
        }

        //Fonction qui gère les touches d'actions  entrées par le joueur 
        public bool HandleKey(RLKey key)
        {
            //Si c'est une touche de sort on l'effectue (les touches sont pour un clavier qwerty, Q=Z, W=E)
            if (key == RLKey.Q)
            {
                return Game.Player.QAbility.Perform();
            }
            if (key == RLKey.W)
            {
                return Game.Player.WAbility.Perform();
            }
            if (key == RLKey.E)
            {
                return Game.Player.EAbility.Perform();
            }
            if (key == RLKey.R)
            {
                return Game.Player.RAbility.Perform();
            }

            //Si c'est une touche d'objet on l'utilise 
            bool didUseItem = false; 
            if (key == RLKey.Number1)
            {
                didUseItem = Game.Player.Item1.Use();
            }
            else if (key == RLKey.Number2)
            {
                didUseItem = Game.Player.Item2.Use();
            }
            else if (key == RLKey.Number3)
            {
                didUseItem = Game.Player.Item3.Use();
            }
            else if (key == RLKey.Number4)
            {
                didUseItem = Game.Player.Item4.Use();
            }

            if (didUseItem) //Si un objet a été utilisé 
            {
                RemoveItemsWithNoRemainingUses(); //On supprime les objets qui n'ont plus d'utilisation
            }

            return didUseItem; 
        }

        //Fonction qui supprime les objets qui n'ont plus d'utilisation
        private static void RemoveItemsWithNoRemainingUses()
        {
            //Pour chaque objet on vérifie si il a 0 utilisations restantes et si c'est le cas, on remplace l'objet par l'objet "NoItem" qui est l'absence d'objet
            if (Game.Player.Item1.RemainingUses <= 0)
            {
                Game.Player.Item1 = new NoItem();
            }
            if (Game.Player.Item2.RemainingUses <= 0)
            {
                Game.Player.Item2 = new NoItem();
            }
            if (Game.Player.Item3.RemainingUses <= 0)
            {
                Game.Player.Item3 = new NoItem();
            }
            if (Game.Player.Item4.RemainingUses <= 0)
            {
                Game.Player.Item4 = new NoItem();
            }
        }

        //Fonction qui termine le tour d'un joueur
        public void EndPlayerTurn()
        {
            IsPlayerTurn = false;
            Game.Player.Tick();
        }
    }
}
