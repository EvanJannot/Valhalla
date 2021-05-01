using System;
using System.Text;
using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using Valhalla.Systems;

namespace Valhalla.Behaviors
{
    //Classe d'un comportement classique de monstre utilisée seulement pour attaquer le joueur 
    public class StandardMoveAndAttack : IBehavior
    {
        StringBuilder espace = new StringBuilder();
        public bool Act(Monster monster, CommandSystem commandSystem) //Permet d'attaquer le joueur 
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);
            if (!monster.TurnsAlerted.HasValue) //Si le monstre n'est pas encore alerté par le joueur 
            {
                monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true); //On calcule son champ de vision à partir de sa position
                if (monsterFov.IsInFov(player.X, player.Y)) //Si le joueur est dedans 
                {
                    monster.TurnsAlerted = 1; //Le monstre est alerté 
                    Game.MessageLog.Add($"{monster.Name} est pret a se battre contre {player.Name}"); //On affiche un message 
                    Game.MessageLog.Add(espace.ToString());
                }
            }
            if (monster.TurnsAlerted.HasValue) //Si il est déjà alerté 
            {
                dungeonMap.SetIsWalkable(monster.X, monster.Y, true); //On considère les cases du joueur et du monstre comme accesibles pour calculer le chemin 
                dungeonMap.SetIsWalkable(player.X, player.Y, true);

                PathFinder pathFinder = new PathFinder(dungeonMap);
                Path path = null;

                try //On calcul un chemin vers le joueur 
                {
                    path = pathFinder.ShortestPath(dungeonMap.GetCell(monster.X, monster.Y), dungeonMap.GetCell(player.X, player.Y));
                }
                catch
                {
                }

                dungeonMap.SetIsWalkable(monster.X, monster.Y, false); //On repasse les cases en non accessibles 
                dungeonMap.SetIsWalkable(player.X, player.Y, false);

                if (path != null) //Si on a trouvé un chemin
                {
                    try
                    {
                        commandSystem.MoveMonster(monster, path.StepForward()); //Le monstre avance en le suivant 
                    }
                    catch (NoMoreStepsException)
                    {
                    }
                }

                monster.TurnsAlerted++; //On augmente de 1 le nombre de tours passés alerté par le monstre 

                // Au bout de 15 tours le monstre n'est plus alerté ce qui permet si le joueur n'est plus dans le champ de vision de ne plus le suivre mais si il est toujours dans le champ de vision, il est re ciblé
                if (monster.TurnsAlerted > 15)
                {
                    monster.TurnsAlerted = null;
                }
            }
            return true;
        }
    }
}
