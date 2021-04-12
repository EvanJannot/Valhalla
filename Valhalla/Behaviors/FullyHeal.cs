using RogueSharpRLNetSamples.Core;
using RogueSharpRLNetSamples.Interfaces;
using RogueSharpRLNetSamples.Systems;
using System.Text;

namespace RogueSharpRLNetSamples.Behaviors
{
    public class FullyHeal : IBehavior
    {
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
