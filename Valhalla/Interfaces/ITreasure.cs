namespace Valhalla.Interfaces
{
    //Interface visant à compléter les classes des trésors donc l'or, l'equipement, les objets et les capacités en vérifiant si il est possible de les récupérer
    public interface ITreasure
    {
        bool PickUp(IActor actor);
    }
}
