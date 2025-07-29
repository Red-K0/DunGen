using DunGen.Generation;

namespace DunGen.Values;

/// <summary>
/// Represents the size of a <see cref="Map"/>, determining its width, height, and room count.
/// </summary>
internal enum MapSize
{
	/// <summary> A small map, with dimensions of 5x5. Suited to small, one-off areas. </summary>
	Small  = 0,

	/// <summary> A moderately-sized map, with dimensions of 10x7. Suited to standard exploration, and quick crawls. </summary>
	Medium = 1,

	/// <summary> A full-size map, with dimensions of 14x9. Suited to long, extensive quests and explorations. </summary>
	Large = 2,
}
