using System.Collections.Generic;
using System.Linq;
using RLNET;
using RogueSharp;
using RogueSharpRLNetSamples.Interfaces;

namespace RogueSharpRLNetSamples.Core
{
    public class DungeonMap : Map
    {
        private readonly List<Monster> _monsters;
        private readonly List<TreasurePile> _treasurePiles;
        private readonly int _level;
        private readonly int _monde;

        public List<Rectangle> Rooms;
        public List<Door> Doors;
        public Stairs StairsUp;
        public Stairs StairsDown;

        public DungeonMap(int lvl, int monde)
        {
            _monsters = new List<Monster>();
            _treasurePiles = new List<TreasurePile>();
            _level = lvl;
            _monde = monde;
            Game.SchedulingSystem.Clear();

            Rooms = new List<Rectangle>();
            Doors = new List<Door>();
        }

        public void AddMonster(Monster monster)
        {
            _monsters.Add(monster);
            SetIsWalkable(monster.X, monster.Y, false);
            Game.SchedulingSystem.Add(monster);
        }

        public void RemoveMonster(Monster monster)
        {
            _monsters.Remove(monster);
            SetIsWalkable(monster.X, monster.Y, true);
            Game.SchedulingSystem.Remove(monster);
            if (_level == 6)
            {
                if (_monsters.Count() == 0)
                {
                    foreach (Door d in Doors)
                    {
                        this.SetCellProperties(d.X, d.Y, false, true);
                    }
                }
            }
        }

        public Monster GetMonsterAt(int x, int y)
        {
            // BUG: This should be single except sometiems monsters occupy the same space.
            return _monsters.FirstOrDefault(m => m.X == x && m.Y == y);
        }

        public IEnumerable<Point> GetMonsterLocations()
        {
            return _monsters.Select(m => new Point
            {
                X = m.X,
                Y = m.Y
            });
        }

        public IEnumerable<Point> GetMonsterLocationsInFieldOfView()
        {
            return _monsters.Where(monster => IsInFov(monster.X, monster.Y))
               .Select(m => new Point { X = m.X, Y = m.Y });
        }

        public void AddTreasure(int x, int y, ITreasure treasure)
        {
            _treasurePiles.Add(new TreasurePile(x, y, treasure));
        }

        public void AddPlayer(Player player)
        {
            Game.Player = player;
            SetIsWalkable(player.X, player.Y, false);
            UpdatePlayerFieldOfView();
            Game.SchedulingSystem.Add(player);
        }

        public void UpdatePlayerFieldOfView()
        {
            Player player = Game.Player;
            ComputeFov(player.X, player.Y, player.Awareness, true);
            foreach (Cell cell in GetAllCells())
            {
                if (IsInFov(cell.X, cell.Y))
                {
                    SetCellProperties(cell.X, cell.Y, cell.IsTransparent, cell.IsWalkable, true);
                }
            }
        }

        public bool SetActorPosition(Actor actor, int x, int y)
        {
            if (GetCell(x, y).IsWalkable)
            {
                PickUpTreasure(actor, x, y);
                SetIsWalkable(actor.X, actor.Y, true);
                actor.X = x;
                actor.Y = y;
                SetIsWalkable(actor.X, actor.Y, false);
                OpenDoor(actor, x, y);
                if (actor is Player)
                {
                    UpdatePlayerFieldOfView();
                }
                return true;
            }
            return false;
        }

        public Door GetDoor(int x, int y)
        {
            return Doors.SingleOrDefault(d => d.X == x && d.Y == y);
        }

        private void OpenDoor(Actor actor, int x, int y)
        {
            Door door = GetDoor(x, y);
            if (door != null && !door.IsOpen)
            {
                door.IsOpen = true;
                var cell = GetCell(x, y);
                SetCellProperties(x, y, true, true, cell.IsExplored);
            }
        }

        public void AddGold(int x, int y, int amount)
        {
            if (amount > 0)
            {
                AddTreasure(x, y, new Gold(amount));
            }
        }

        private void PickUpTreasure(Actor actor, int x, int y)
        {
            List<TreasurePile> treasureAtLocation = _treasurePiles.Where(g => g.X == x && g.Y == y).ToList();
            foreach (TreasurePile treasurePile in treasureAtLocation)
            {
                if (treasurePile.Treasure.PickUp(actor))
                {
                    _treasurePiles.Remove(treasurePile);
                }
            }
        }

        public bool CanMoveDownToNextLevel()
        {
            Player player = Game.Player;

            return StairsDown.X == player.X && StairsDown.Y == player.Y;
        }

        public void SetIsWalkable(int x, int y, bool isWalkable)
        {
            Cell cell = GetCell(x, y);
            SetCellProperties(cell.X, cell.Y, cell.IsTransparent, isWalkable, cell.IsExplored);
        }

        public Point GetRandomLocation()
        {
            int roomNumber = Game.Random.Next(0, Rooms.Count - 1);
            Rectangle randomRoom = Rooms[roomNumber];

            if (!DoesRoomHaveWalkableSpace(randomRoom))
            {
                GetRandomLocation();
            }

            return GetRandomLocationInRoom(randomRoom);
        }

        public Point GetRandomLocationInRoom(Rectangle room)
        {
            int x = Game.Random.Next(1, room.Width - 2) + room.X;
            int y = Game.Random.Next(1, room.Height - 2) + room.Y;
            if (!IsWalkable(x, y))
            {
                GetRandomLocationInRoom(room);
            }
            return new Point(x, y);
        }

        public bool DoesRoomHaveWalkableSpace(Rectangle room)
        {
            for (int x = 1; x <= room.Width - 2; x++)
            {
                for (int y = 1; y <= room.Height - 2; y++)
                {
                    if (IsWalkable(x + room.X, y + room.Y))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Draw(RLConsole mapConsole, RLConsole statConsole, RLConsole inventoryConsole, int monde)
        {
            mapConsole.Clear();
            foreach (Cell cell in GetAllCells())
            {
                SetConsoleSymbolForCell(mapConsole, cell, monde);
            }

            foreach (Door door in Doors)
            {
                door.Draw(mapConsole, this, monde);
            }
            if (_monde != 0)
            {
                StairsUp.Draw(mapConsole, this, monde);
                StairsDown.Draw(mapConsole, this, monde);
            }
            else
            {
                StairsDown.Draw(mapConsole, this, monde);
            }


            foreach (TreasurePile treasurePile in _treasurePiles)
            {
                IDrawable drawableTreasure = treasurePile.Treasure as IDrawable;
                drawableTreasure?.Draw(mapConsole, this, monde);
            }

            statConsole.Clear();
            int i = 0;
            foreach (Monster monster in _monsters)
            {
                monster.Draw(mapConsole, this, monde);
                if (IsInFov(monster.X, monster.Y))
                {
                    monster.DrawStats(statConsole, i);
                    i++;
                }
            }

            Player player = Game.Player;

            player.Draw(mapConsole, this, monde);
            player.DrawStats(statConsole);
            player.DrawInventory(inventoryConsole);
        }

        private void SetConsoleSymbolForCell(RLConsole console, Cell cell, int monde)
        {
            if (!cell.IsExplored)
            {
                return;
            }

            // When a cell is currently in the field-of-view it should be drawn with lighter colors
            if (IsInFov(cell.X, cell.Y))
            {
                if (monde == 0 | monde == 4)
                {

                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                        {
                            console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, RLColor.Brown, RLColor.Black, (char)132);
                        }

                    }
                }
                if (monde == 1)
                {
                    // Choose the symbol to draw based on if the cell is walkable or not
                    // '.' for floor and '#' for walls
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)8);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)47);
                    }
                }
                else if (monde == 2)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)95);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)30);
                    }
                }
                else if (monde == 3)
                {
                    // Choose the symbol to draw based on if the cell is walkable or not
                    // '.' for floor and '#' for walls
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)64);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.WallFov, RLColor.Black, (char)31);
                    }
                } 
                else
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.FloorFov, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                        {
                            console.Set(cell.X, cell.Y, Colors.Player, RLColor.Black, (char)94);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, RLColor.Brown, RLColor.Black, (char)132);
                        }

                    }
                }
            }
            // When a cell is outside of the field of view draw it with darker colors
            else
            {
                if (monde == 0 | monde == 4)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                        {
                            console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)154);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)132);
                        }
                    }
                }
                if (monde == 1)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)8);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)47);
                    }
                }
                else if (monde == 2)
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)95);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)30);
                    }
                }
                else if (monde == 3)
                {
                    // Choose the symbol to draw based on if the cell is walkable or not
                    // '.' for floor and '#' for walls
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)64);
                    }
                    else
                    {
                        console.Set(cell.X, cell.Y, Colors.Wall, RLColor.Black, (char)31);
                    }
                }
                else
                {
                    if (cell.IsWalkable)
                    {
                        console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)154);
                    }
                    else
                    {
                        if (cell.X == this.Rooms.Last().Center.X && cell.Y == this.Rooms.Last().Center.Y - 2)
                        {
                            console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)154);
                        }
                        else
                        {
                            console.Set(cell.X, cell.Y, Colors.Floor, RLColor.Black, (char)132);
                        }
                    }
                }

            }
        }
    }
}