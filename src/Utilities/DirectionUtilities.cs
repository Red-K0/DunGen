using static DunGen.Utilities.Dice;
using static DunGen.Values.Directions;
using DunGen.Values;

namespace DunGen.Utilities;

/// <summary>
/// Contains various loose utiltiies for working with directions.
/// </summary>
internal static class DirectionUtilities
{
	/// <summary>
	/// Returns a random <see cref="Directions"/> value, determined by rolling a <see cref="D4"/>.
	/// </summary>
	public static Directions GetRandomDirection() => D4 switch { 1 => North, 2 => East, 3 => South, _ => West };

	/// <summary>
	/// Gets the <see cref="Directions"/> value opposite of the one passed.
	/// </summary>
	public static Directions GetOppositeDirection(Directions direction) => direction switch  { North => South, East => West, South => North, _ => East, };
}
