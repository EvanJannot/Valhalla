using RogueSharp;
using Valhalla.Core;
using Valhalla.Interfaces;
using Valhalla.Systems;
using System.Text;

namespace Valhalla.Behaviors
{
    //Classe d'un comportement utilisé par les abeilles, les rats ainsi que les chauves souris 
    public class ShoutForHelp : IBehavior
    {
        StringBuilder espace = new StringBuilder();
        public bool Act(Monster monster, CommandSystem commandSystem) //Permet au monstre d'aller chercher de l'aide 
        {
            bool didShoutForHelp = false; //Au départ il n'a pas encore demandé de l'aide 
            DungeonMap dungeonMap = Game.DungeonMap;
            FieldOfView monsterFov = new FieldOfView(dungeonMap);
            //On récupère le champ de vision du monstre en fonction de son emplacement et de sa visibilité 
            monsterFov.ComputeFov(monster.X, monster.Y, monster.Awareness, true);
            foreach (var monsterLocation in dungeonMap.GetMonsterLocations()) //Pour chaque monstre de la mpa
            {
                if (monsterFov.IsInFov(monsterLocation.X, monsterLocation.Y)) //Si il est dans le champ de vision du monstre 
                {
                    Monster alertedMonster = dungeonMap.GetMonsterAt(monsterLocation.X, monsterLocation.Y); //On définie un monstre qui sera alerté à partir du monstre trouvé
                    if (!alertedMonster.TurnsAlerted.HasValue) //Si on a bien un monstre ici
                    {
                        alertedMonster.TurnsAlerted = 1; //Il est alerté
                        didShoutForHelp = true;  //Le monstre initiale a bien été chercher de l'aide 
                    }
                }
            }

            if (didShoutForHelp) //Si le monstre a trouvé de l'aide on affiche un message 
            {
                Game.MessageLog.Add($"{monster.Name} va chercher de l'aide");
                Game.MessageLog.Add(espace.ToString());
            }

            return didShoutForHelp; //Si il a trouvé de l'aide on dit que le monstre a effectué son action
        }
    }
}
