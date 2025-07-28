using static DunGen.Rooms.RoomFlags;
using static DunGen.Rooms.Direction;
using static DunGen.MapSize;
using DunGen.Rooms;

namespace DunGen;

internal class Map
{
	#region Fields & Properties

	public Room[,] Rooms = null!;
	public readonly int Width, Height;
	public readonly MapSize Size;
	public Room? Entrance = null!;

	public int RoomCount
	{
		get
		{
			int result = 0;
			foreach (Room room in Rooms) if (room.HasFlags(Standard)) result++;
			return result;
		}
	}

	public int StructureCount
	{
		get
		{
			int result = 0;
			foreach (Room room in Rooms) if (!room.IsEmpty) result++;
			return result;
		}
	}

	#endregion

	#region Initialization

	public Map(MapSize size)
	{
		Size = size;

		switch (size)
		{
			case Small:  Width =  5; Height = 5; break;
			case Medium: Width = 10; Height = 7; break;
			case Large:  Width = 14; Height = 9; break;
		}

		GenerateRooms();
	}

	private void InitializeMap()
	{
		Rooms = new Room[Width, Height];

		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				Rooms[x, y] = new(this, x, y, NoRoom);
			}
		}
	}

	#endregion

	#region Generation

	private void GenerateRooms()
	{
		do
		{
			InitializeMap();

			GetRandomRoom().GenerateChain();

			CullOrRepairInvalidRooms();
		}
		while (ValidateMap());
	}

	private void CullOrRepairInvalidRooms()
	{
		bool rescan = true;

		while (rescan)
		{
			rescan = false;

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					Room room = Rooms[x, y];

					if (room.IsEmpty) continue;

					if (room.HasFlags(NorthDoor) && !room.GetNeighbour(North).HasFlags(SouthDoor))
					{
						if (Dice.D2 && !room.GetNeighbour(North).IsEmpty)
						{
							room.GetNeighbour(North).AddFlags(SouthDoor);
						}
						else
						{
							room.RemoveFlags(NorthDoor);
						}
					}
					if (room.HasFlags( EastDoor) && !room. GetNeighbour(East).HasFlags( WestDoor))
					{
						if (Dice.D2 && !room.GetNeighbour(East).IsEmpty)
						{
							room.GetNeighbour(East).AddFlags(WestDoor);
						}
						else
						{
							room.RemoveFlags(EastDoor);
						}
					}
					if (room.HasFlags(SouthDoor) && !room.GetNeighbour(South).HasFlags(NorthDoor))
					{
						if (Dice.D2 && !room.GetNeighbour(South).IsEmpty)
						{
							room.GetNeighbour(South).AddFlags(NorthDoor);
						}
						else
						{
							room.RemoveFlags(SouthDoor);
						}
					}
					if (room.HasFlags( WestDoor) && !room. GetNeighbour(West).HasFlags( EastDoor))
					{
						if (Dice.D2 && !room.GetNeighbour(West).IsEmpty)
						{
							room.GetNeighbour(West).AddFlags(EastDoor);
						}
						else
						{
							room.RemoveFlags(WestDoor);
						}
					}

					if (room.ConnectionCount == (room.IsCorridor ? 1 : 0))
					{
						room.RemoveFlags((RoomFlags)int.MaxValue);
						rescan = true;
					}
				}
			}
		}
	}

	private bool ValidateMap()
	{
		return Size switch
		{
			 Small => (StructureCount is < 10 or > 20) || (RoomCount is < 10 or > 15),
			Medium => (StructureCount is < 20 or > 30) || (RoomCount is < 15 or > 20),
			 Large => (StructureCount is < 30 or > 40) || (RoomCount is < 20 or > 25),
			_ => true
		};
	}

	#endregion

	#region Drawing

	public void Draw()
	{
		DrawBorder();
		DrawRooms();
	}

	private void DrawBorder()
	{
		int height = Height * Room.Height;
		int  width = Width * Room.Width;

		Console.SetCursorPosition(0, height);
		Console.Write($"{new string('─', width + 1)}┘");

		for (int i = 0; i < height; i++)
		{
			Console.SetCursorPosition(width + 1, i);
			Console.Write('│');
		}
	}

	private void DrawRooms()
	{
		for (int y = 0; y < Height; y++)
		{
			for (int x = 0; x < Width; x++)
			{
				Rooms[x, y].Draw();
			}
		}
	}

	#endregion

	#region Helpers

	private Room GetRandomRoom() => Rooms[Random.Shared.Next(0, Width), Random.Shared.Next(0, Height)];

	#endregion
}
