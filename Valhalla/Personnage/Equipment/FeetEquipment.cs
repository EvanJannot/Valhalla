namespace Valhalla.Equipment
{
    //Classe définissant les différents equipements pour les jambes 
    public class FeetEquipment : Core.Equipment
    {
        public static FeetEquipment None() //Aucun equipement 
        {
            return new FeetEquipment { Name = "Rien" };
        }

        public static FeetEquipment Leather() //Equipement en cuir 
        {
            return new FeetEquipment()
            {
                Defense = 1,
                DefenseChance = 5,
                Name = "Cuir"
            };
        }

        public static FeetEquipment Chain() //Equipement en maille 
        {
            return new FeetEquipment()
            {
                Defense = 1,
                DefenseChance = 10,
                Name = "Maille"
            };
        }

        public static FeetEquipment Plate() //Equipement en plaque 
        {
            return new FeetEquipment()
            {
                Defense = 1,
                DefenseChance = 15,
                Name = "Plaque"
            };
        }
    }
}