using Valhalla.Abilities;
using Valhalla.Core;
using System;

namespace Valhalla.Systems
{
    //Permet de créer les sorts lors de la création de la map
    public static class AbilityGenerator
    {

        public static Ability CreateAbility() 
        {
            Pool<Ability> _abilityPool = new Pool<Ability>(); //Chaque sort a autant de chance d'apparaitre 
            _abilityPool.Add(new Heal(10), 10);
            _abilityPool.Add(new MagicMissile(2, 80), 10);
            _abilityPool.Add(new RevealMap(15), 10);
            _abilityPool.Add(new Whirlwind(), 10);
            _abilityPool.Add(new Fireball(4, 60, 2), 10);
            _abilityPool.Add(new LightningBolt(6, 40), 10);


            return _abilityPool.Get();
        }
    }
}
