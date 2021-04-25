using Valhalla.Core;
using Valhalla.Interfaces;
using Valhalla.Systems;
using System.Text;

namespace Valhalla.Behaviors
{
    //Classe d'un comportement utilisé par les abeilles, les rats ainsi que les chauves souris 
    public class FullyHeal : IBehavior
    {
        //permet au monstre de se soigner entièrement si sa vie est inférieure à sa vie max. Retourne true si une action est effectuée et false sinon. 
        //Cette fonction est l'implémentation de celle issue de l'interface 
        public bool Act(Monster monster, CommandSystem commandSystem)
        {
            if (monster.Health < monster.MaxHealth)
            {
                StringBuilder espace = new StringBuilder();
                int healthToRecover = monster.MaxHealth - monster.Health;
                Game.MessageLog.Add($"{monster.Name} reprend son souffle et recupere {healthToRecover} points de vie");
                Game.MessageLog.Add(espace.ToString());
                monster.Health = monster.MaxHealth;
                return true;
            }
            return false;
        }
    }
}
