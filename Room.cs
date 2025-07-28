using static DunGen.Rooms.RoomFlags;
using static DunGen.Dice;
using DunGen.Rooms;
namespace DunGen;

internal class Room(Map map, int x, int y, RoomFlags flags)
{
	public void GenerateChain()
	{
		DetermineStructure();

		DetermineConnections();

		if (CanConnectNorth && HasFlags(NorthDoor) && !ConnectsNorth && NorthConnection!.IsEmpty) NorthConnection.GenerateChain();
		if ( CanConnectEast && HasFlags( EastDoor) && !ConnectsEast  && EastConnection! .IsEmpty)  EastConnection.GenerateChain();
		if (CanConnectSouth && HasFlags(SouthDoor) && !ConnectsSouth && SouthConnection!.IsEmpty) SouthConnection.GenerateChain();
		if ( CanConnectWest && HasFlags( WestDoor) && !ConnectsWest  && WestConnection! .IsEmpty)  WestConnection.GenerateChain();
	}
	public bool CanHaveMoreConnections => (!HasFlags(NorthDoor) && CanConnectNorth) || (!HasFlags(EastDoor) && CanConnectEast) || (!HasFlags(SouthDoor) && CanConnectSouth) || (!HasFlags(WestDoor) && CanConnectWest);
	public bool Dependent => ConnectionCount == MinimumConnections;
	private int MinimumConnections => IsCorridor ? 2 : 1;
	private int MaximumConnections
	{
		get
		{
			int result = 0;
			if (CanConnectNorth) result++;
			if  (CanConnectEast) result++;
			if (CanConnectSouth) result++;
			if  (CanConnectWest) result++;
			return result;
		}
	}
	public int ConnectionCount => (int)(Flags & (NorthDoor | EastDoor | SouthDoor | WestDoor)) switch
	{
		0b0001 => 1, 0b0010 => 1, 0b0100 => 1, 0b1000 => 1,
		0b0011 => 2, 0b0110 => 2, 0b1100 => 2, 0b1001 => 2,
		0b0111 => 3, 0b1110 => 3, 0b1101 => 3, 0b1011 => 3,
		0b0000 => 0, 0b1111 => 4, _ => -1
	};
	public Room? NorthConnection => CanConnectNorth ? ParentMap.Rooms[X, Y - 1] : null;


	public bool CanConnectNorth => Y != 0;
	public Room? EastConnection => CanConnectEast ? ParentMap.Rooms[X + 1, Y] : null;
	public bool CanConnectEast => X != ParentMap.Width - 1;
	public Room? SouthConnection => CanConnectSouth ? ParentMap.Rooms[X, Y + 1] : null;
	public bool CanConnectSouth => Y != ParentMap.Height - 1;
	public Room? WestConnection => CanConnectWest ? ParentMap.Rooms[X - 1, Y] : null;
	public bool CanConnectWest => X != 0;
	public bool AddConnection()
	{
		if (!CanHaveMoreConnections) return false;

		while (true)
		{
			RoomFlags newDirection = (RoomFlags)Generation.GetRandomDirection();

			switch (newDirection)
			{
				case NorthDoor:
					if (!CanConnectNorth || HasFlags(NorthDoor)) continue;
					if (NorthConnection?.IsEmpty == true) NorthConnection!.AddFlags(SouthDoor);
					AddFlags(NorthDoor);
					break;

				case EastDoor:
					if (!CanConnectEast || HasFlags(EastDoor)) continue;
					if (EastConnection?.IsEmpty == true) EastConnection.AddFlags(WestDoor);
					AddFlags(EastDoor);
					break;

				case SouthDoor:
					if (!CanConnectSouth || HasFlags(SouthDoor)) continue;
					if (SouthConnection?.IsEmpty == true) SouthConnection!.AddFlags(NorthDoor);
					AddFlags(SouthDoor);
					break;

				case WestDoor:
					if (!CanConnectWest || HasFlags(WestDoor)) continue;
					if (WestConnection?.IsEmpty == true) WestConnection!.AddFlags(EastDoor);
					AddFlags(WestDoor);
					break;
			}

			return true;
		}
	}
	public void DetermineConnections()
	{
		int roll = D100, connectionCount;

		#pragma warning disable IDE0045 // This would be unreadable
		if (ParentMap.RoomCount < 50)
		{
			if (roll <= (5 - ((50 - ParentMap.RoomCount) / 10)))
			{
				connectionCount = IsCorridor ? 2 : 1;
			}
			else if (roll <= (10 + (ParentMap.RoomCount / 2)))
			{
				connectionCount = 3;
			}
			else
			{
				connectionCount = 2;
			}
		}
		else
		{
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
		}
		#pragma warning restore IDE0045

		connectionCount = Math.Min(connectionCount, MaximumConnections) - ConnectionCount;

		for (int i = 0; i < connectionCount; i++) AddConnection();
	}
	public RoomFlags Flags { get; private set; } = flags;
	public void    AddFlags(RoomFlags flags) => Flags |= flags;
	public bool    HasFlags(RoomFlags flags) => Flags.HasFlag(flags);
	public void RemoveFlags(RoomFlags flags) => Flags &= ~flags;
	public Map ParentMap { get; } = map;
	public int X { get; } = x;
	public int Y { get; } = y;
	public void DetermineStructure() => AddFlags((D100 < (50 - (ParentMap.RoomCount / 2))) ? Standard : Corridor);
	public bool IsStandardRoom => HasFlags(Standard);
	public bool IsCorridor => HasFlags(Corridor);
	public bool IsEmpty => !(HasFlags(Standard) || HasFlags(Corridor));
	public bool IsEntrance => HasFlags(Entrance);
	public bool IsLarge => HasFlags(LargeRoom);
	public override string ToString() => Flags.ToString();

	#region Directional Relationships

	public bool DependsOnNorth => Dependent && ConnectsNorth;
	public bool DependsOnEast  => Dependent && ConnectsEast;
	public bool DependsOnSouth => Dependent && ConnectsSouth;
	public bool DependsOnWest  => Dependent && ConnectsWest;

	public bool ConnectsNorth => NorthConnection?.IsEmpty != true && Flags.HasFlag(NorthDoor);
	public bool ConnectsEast  =>  EastConnection?.IsEmpty != true && Flags.HasFlag( EastDoor);
	public bool ConnectsSouth => SouthConnection?.IsEmpty != true && Flags.HasFlag(SouthDoor);
	public bool ConnectsWest  =>  WestConnection?.IsEmpty != true && Flags.HasFlag( WestDoor);

	public bool CombinesNorth => IsLarge && (NorthConnection?.IsLarge == true);
	public bool CombinesEast  => IsLarge && ( EastConnection?.IsLarge == true);
	public bool CombinesSouth => IsLarge && (SouthConnection?.IsLarge == true);
	public bool CombinesWest  => IsLarge && ( WestConnection?.IsLarge == true);


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

	private string Shape => IsEmpty ? "" : _styles[(RoomFlags)((int)Flags & 0b0011_1111)];

	public void Draw()
	{
		Console.SetCursorPosition(0, Y * Height);
		Console.Write(Shape, 1 + (X * Width));
	}

	#endregion
}
