using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using Valhalla.Systems;
using System.Text;

namespace Valhalla.Behaviors
{
    //Classe d'un comportement utilisé par les abeilles, les rats ainsi que les chauves souris 
    public class RunAway : IBehavior
    {
        StringBuilder espace = new StringBuilder();
        public bool Act(Monster monster, CommandSystem commandSystem) //Permet au monstre de fuir 
        {
            DungeonMap dungeonMap = Game.DungeonMap;
            Player player = Game.Player;

            //Afin de trouver un chemin on passe les cases où se trouvent le joueur et le monstre en accessibles
            dungeonMap.SetIsWalkable(monster.X, monster.Y, true);
            dungeonMap.SetIsWalkable(player.X, player.Y, true);

            GoalMap goalMap = new GoalMap(dungeonMap); //On définit l'endroit où se trouve le joueur comme la zone de la map à éviter 
            goalMap.AddGoal(player.X, player.Y, 0);

            Path path = null;
            try //Cherche un chemin qui évite le lieu où se trouve le joueur à partir de celui où est le monstre 
            {
                path = goalMap.FindPathAvoidingGoals(monster.X, monster.Y);

            }
            catch (PathNotFoundException)
            {
                Game.MessageLog.Add($"{monster.Name} a peur et s'enfuit");
                Game.MessageLog.Add(espace.ToString());
            }

            // On re passe les cases du monstre et du joueur en non accessible 
            dungeonMap.SetIsWalkable(monster.X, monster.Y, false);
            dungeonMap.SetIsWalkable(player.X, player.Y, false);

            if (path != null) //Si on a trouvé un chemin
            {
                try //On déplace le monstre 
                {
                    commandSystem.MoveMonster(monster, path.StepForward());
                }
                catch (NoMoreStepsException)
                {
                    Game.MessageLog.Add($"{monster.Name} a peur et s'enfuit");
                    Game.MessageLog.Add(espace.ToString());
                }
            }

            return true; //On renvoie true pour dire que l'action a été effectuée
        }
    }
}
