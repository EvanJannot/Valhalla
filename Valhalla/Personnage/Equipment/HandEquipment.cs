namespace Valhalla.Equipment
{
    //Classe définissant les différentes armes que le joueur peut posséder 
    public class HandEquipment : Core.Equipment
    {
        public static HandEquipment None() //Arme de base
        {
            return new HandEquipment { Name = "Baton" };
        }

        public static HandEquipment Dagger() //Dague
        {
            return new HandEquipment
            {
                Attack = 1,
                AttackChance = 10,
                Name = "Dague",
                Speed = -2
            };
        }

        public static HandEquipment Sword() //Epee 
        {
            return new HandEquipment
            {
                Attack = 1,
                AttackChance = 20,
                Name = "Epee"
            };
        }

        public static HandEquipment Axe() //Hache 
        {
            return new HandEquipment
            {
                Attack = 2,
                AttackChance = 15,
                Name = "Hache",
                Speed = 1
            };
        }

        public static HandEquipment TwoHandedSword() //Claymore 
        {
            return new HandEquipment
            {
                Attack = 3,
                AttackChance = 20,
                Name = "Claymore",
                Speed = 3
            };
        }
    }
}