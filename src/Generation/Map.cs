using static DunGen.Values.RoomFlags;
using static DunGen.Values.MapSize;
using DunGen.Utilities;
using DunGen.Values;

namespace DunGen.Generation;

internal class Map
{
	#region Fields & Properties

	public readonly int Width, Height;
	public readonly MapSize Size;

	public Room? Entrance = null!;
	public Room[,] Rooms = null!;

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

					for (int i = 1; i <= 8; i <<= 1)
					{
						Room neighbour;

						if (room.HasDoor((Directions)i) && !(neighbour = room.GetNeighbour((Directions)i)).HasDoor(DirectionUtilities.GetOppositeDirection((Directions)i)))
						{
							if (Dice.D2 && !neighbour.IsEmpty)
							{
								neighbour.AddFlags((RoomFlags)DirectionUtilities.GetOppositeDirection((Directions)i));
							}
							else
							{
								room.RemoveFlags((RoomFlags)i);
							}
						}
					}

					if (room.ConnectionCount == (room.IsCorridor ? 1 : 0))
					{
						room.ClearFlags();
						rescan = true;
					}
				}
			}
		}
	}

	private bool ValidateMap()
	{
		int roomCount = Count(Standard), structureCount = roomCount + Count(Corridor);

		return Size switch
		{
			 Small => structureCount is < 10 or > 20 || roomCount is < 10 or > 15,
			Medium => structureCount is < 20 or > 30 || roomCount is < 15 or > 20,
			 Large => structureCount is < 30 or > 40 || roomCount is < 20 or > 25,
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

	public int Count(RoomFlags flag)
	{
		int result = 0;
		foreach (Room room in Rooms) if (room.HasFlags(flag)) result++;
		return result;
	}

	private Room GetRandomRoom() => Rooms[Random.Shared.Next(0, Width), Random.Shared.Next(0, Height)];

	#endregion
}
