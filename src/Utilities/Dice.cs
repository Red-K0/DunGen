using static System.Random;

namespace DunGen.Utilities;

/// <summary>
/// Provides properties for rolling standard game die, as well as an arbitrary face method.
/// </summary>
internal class Dice
{
	/// <summary> Rolls a 50% chance, as a <see cref="bool"/>. </summary>
	public static bool D2 => Shared.Next(0, 2) == 1;

	/// <summary> Rolls a standard 4-sided die. </summary>
	public static int D4 => Roll(4);

	/// <summary> Rolls a standard 6-sided die. </summary>
	public static int D6 => Roll(6);

	/// <summary> Rolls a standard 8-sided die. </summary>
	public static int D8 => Roll(8);

	/// <summary> Rolls a standard 10-sided die. </summary>
	public static int D10 => Roll(10);

	/// <summary> Rolls a standard 12-sided die. </summary>
	public static int D12 => Roll(12);

	/// <summary> Rolls a standard 20-sided die. </summary>
	public static int D20 => Roll(20);

	/// <summary> Rolls a standard set of percentile dice. </summary>
	public static int D100 => Roll(100);

	/// <summary> Rolls a die, of arbitrary face count. </summary>
	public static int Roll(int faces) => Shared.Next(1, faces + 1);
}
