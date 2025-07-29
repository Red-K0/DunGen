using static DunGen.Values.Directions;
using static DunGen.Values.RoomFlags;
using static DunGen.Utilities.Dice;
using DunGen.Utilities;
using DunGen.Values;
namespace DunGen.Generation;

internal class Room(Map map, int x, int y, RoomFlags flags)
{
	#region Connections
	private int MaximumConnections
	{
		get
		{
			int result = 0;

			for (int i = 1; i <= 8; i <<= 1) if (CanConnect((Directions)i)) result++;

			return result;
		}
	}
	public int ConnectionCount => (int)(flags & (NorthDoor | EastDoor | SouthDoor | WestDoor)) switch
	{
		0b0001 => 1, 0b0010 => 1, 0b0100 => 1, 0b1000 => 1,
		0b0011 => 2, 0b0110 => 2, 0b1100 => 2, 0b1001 => 2,
		0b0111 => 3, 0b1110 => 3, 0b1101 => 3, 0b1011 => 3,
		0b0000 => 0, 0b1111 => 4, _ => -1
	};

	public bool AddConnection()
	{
		if (!(!HasFlags(NorthDoor) && CanConnect(North)) || (!HasFlags(EastDoor) && CanConnect(East)) || (!HasFlags(SouthDoor) && CanConnect(South)) || (!HasFlags(WestDoor) && CanConnect(West))) return false;

		while (true)
		{
			Directions direction = DirectionUtilities.GetRandomDirection();

			if (!CanConnect(direction) || HasDoor(direction)) continue;
			if (GetNeighbour(direction).IsEmpty) GetNeighbour(direction).AddFlags((RoomFlags)DirectionUtilities.GetOppositeDirection(direction));
			AddFlags((RoomFlags)direction);

			return true;
		}
	}
	public void DetermineConnections()
	{
		int roll = D100, connectionCount;

		#pragma warning disable IDE0045 // This would be unreadable
		if (roll <= 5)
		{
			connectionCount = IsCorridor ? 2 : 1;
		}
		else if (roll <= 15)
		{
			connectionCount = 3;
		}
		else
		{
			connectionCount = 2;
		}
		#pragma warning restore IDE0045

		connectionCount = Math.Min(connectionCount, MaximumConnections) - ConnectionCount;

		for (int i = 0; i < connectionCount; i++) AddConnection();
	}

	#endregion

	#region Directional Relationships

	public bool CanConnect(Directions direction) => direction switch
	{
		North => y != 0,
		East  => x != map.Width  - 1,
		South => y != map.Height - 1,
		West  => x != 0,
	};

	public Room GetNeighbour(Directions direction) => direction switch
	{
		North => map.Rooms[x, y - 1],
		East  => map.Rooms[x + 1, y],
		South => map.Rooms[x, y + 1],
		West  => map.Rooms[x - 1, y],
	};

	public bool CanCombine(Directions direction) => IsLarge && GetNeighbour(direction).IsLarge;

	public bool IsConnected(Directions direction) => !(GetNeighbour(direction).IsEmpty && HasFlags((RoomFlags)direction));

	#endregion

	#region Flags

	public void    AddFlags(RoomFlags newFlags) => flags |= newFlags;
	public bool    HasFlags(RoomFlags newFlags) => flags.HasFlag(newFlags);
	public void RemoveFlags(RoomFlags newFlags) => flags &= ~newFlags;
	public void ClearFlags() => flags = NoRoom;

	public bool HasDoor(Directions direction) => HasFlags((RoomFlags)direction);

	public bool IsStandardRoom => HasFlags(Standard);
	public bool IsCorridor => HasFlags(Corridor);
	public bool IsEmpty => !(HasFlags(Standard) || HasFlags(Corridor));
	public bool IsEntrance => HasFlags(Entrance);
	public bool IsLarge => HasFlags(Hall);

	#endregion

	#region Generation

	public void GenerateChain()
	{
		DetermineStructure();

		DetermineConnections();

		for (int i = 1; i <= 8; i <<= 1)
		{
			if (HasDoor((Directions)i) && !IsConnected((Directions)i)) GetNeighbour((Directions)i).GenerateChain();
		}
	}
	public void DetermineStructure() => AddFlags(D100 < 50 - (map.Count(Standard) / 2) ? Standard : Corridor);

	#endregion

	#region Rendering

	public static readonly int Height = 5, Width = 10;

	private static readonly Dictionary<RoomFlags, string> _styles = new()
	{
		#region Rooms

		[0] = "",

		#region Dead Ends

		[Standard | NorthDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C│        │
			[{0}C│        │
			[{0}C│        │
			[{0}C└────────┘
			""",

		[Standard | EastDoor] =
			"""
			[{0}C┌────────┐
			[{0}C│        └
			[{0}C│
			[{0}C│        ┌
			[{0}C└────────┘
			""",

		[Standard | SouthDoor] =
			"""
			[{0}C┌────────┐
			[{0}C│        │
			[{0}C│        │
			[{0}C│        │
			[{0}C└──┐  ┌──┘
			""",

		[Standard | WestDoor] =
			"""
			[{0}C┌────────┐
			[{0}C┘        │
			[{0}C         │
			[{0}C┐        │
			[{0}C└────────┘
			""",

		#endregion

		#region Passthroughs

		[Standard | NorthDoor | EastDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C│        └
			[{0}C│
			[{0}C│        ┌
			[{0}C└────────┘
			""",

		[Standard | NorthDoor | SouthDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C│        │
			[{0}C│        │
			[{0}C│        │
			[{0}C└──┐  ┌──┘
			""",

		[Standard | NorthDoor | WestDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C┘        │
			[{0}C         │
			[{0}C┐        │
			[{0}C└────────┘
			""",


		[Standard | EastDoor | SouthDoor] =
			"""
			[{0}C┌────────┐
			[{0}C│        └
			[{0}C│
			[{0}C│        ┌
			[{0}C└──┐  ┌──┘
			""",

		[Standard | EastDoor | WestDoor] =
			"""
			[{0}C┌────────┐
			[{0}C┘        └

			[{0}C┐        ┌
			[{0}C└────────┘
			""",


		[Standard | SouthDoor | WestDoor] =
			"""
			[{0}C┌────────┐
			[{0}C┘        │
			[{0}C         │
			[{0}C┐        │
			[{0}C└──┐  ┌──┘
			""",

		#endregion

		#region T-Points

		[Standard | NorthDoor | EastDoor | SouthDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C│        └
			[{0}C│
			[{0}C│        ┌
			[{0}C└──┐  ┌──┘
			""",

		[Standard | NorthDoor | EastDoor | WestDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C┘        └

			[{0}C┐        ┌
			[{0}C└────────┘
			""",

		[Standard | NorthDoor | SouthDoor | WestDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C┘        │
			[{0}C         │
			[{0}C┐        │
			[{0}C└──┐  ┌──┘
			""",

		[Standard | EastDoor | SouthDoor | WestDoor] =
			"""
			[{0}C┌────────┐
			[{0}C┘        └

			[{0}C┐        ┌
			[{0}C└──┐  ┌──┘
			""",

		#endregion

		[Standard | NorthDoor | EastDoor | SouthDoor | WestDoor] =
			"""
			[{0}C┌──┘  └──┐
			[{0}C┘        └

			[{0}C┐        ┌
			[{0}C└──┐  ┌──┘
			""",

		#endregion

		#region Corridors

		#region Straights

		[Corridor | NorthDoor | SouthDoor] =
			"""
			[{0}C   │  │
			[{0}C   │  │
			[{0}C   │  │
			[{0}C   │  │
			[{0}C   │  │
			""",

		[Corridor | EastDoor | WestDoor] =
			"""
			
			[{0}C──────────
			
			[{0}C──────────
			""",

		#endregion

		#region Bends

		[Corridor | NorthDoor | EastDoor] =
			"""
			[{0}C   │  │
			[{0}C   │  └───
			[{0}C   │
			[{0}C   └──────
			""",

		[Corridor | NorthDoor | WestDoor] =
			"""
			[{0}C   │  │
			[{0}C───┘  │
			[{0}C      │
			[{0}C──────┘
			""",

		[Corridor | SouthDoor | EastDoor] =
			"""
			
			[{0}C   ┌──────
			[{0}C   │
			[{0}C   │  ┌───
			[{0}C   │  │
			""",

		[Corridor | SouthDoor | WestDoor] =
			"""

			[{0}C──────┐
			[{0}C      │
			[{0}C───┐  │
			[{0}C   │  │
			""",


		#endregion

		#region T-Shapes

		[Corridor | NorthDoor | EastDoor | SouthDoor] =
			"""
			[{0}C   │  │
			[{0}C   │  └───
			[{0}C   │
			[{0}C   │  ┌───
			[{0}C   │  │
			""",

		[Corridor | NorthDoor | EastDoor | WestDoor] =
			"""
			[{0}C   │  │
			[{0}C───┘  └───

			[{0}C──────────
			""",

		[Corridor | NorthDoor | SouthDoor | WestDoor] =
			"""
			[{0}C   │  │
			[{0}C───┘  │
			[{0}C      │
			[{0}C───┐  │
			[{0}C   │  │
			""",


		[Corridor | EastDoor | SouthDoor | WestDoor] =
			"""

			[{0}C──────────

			[{0}C───┐  ┌───
			[{0}C   │  │
			""",

		#endregion

		[Corridor | NorthDoor | EastDoor | SouthDoor | WestDoor] =
			"""
			[{0}C   │  │
			[{0}C───┘  └───
			
			[{0}C───┐  ┌───
			[{0}C   │  │
			""",

		#endregion
	};

	private string Shape => IsEmpty ? "" : _styles[(RoomFlags)((int)flags & 0b0011_1111)];

	public void Draw()
	{
		Console.SetCursorPosition(0, y * Height);
		Console.Write(Shape, 1 + (x * Width));
	}

	#endregion

	public override string ToString() => flags.ToString();
}
