namespace ArvidsonFoto.Core.Statics;

/// <summary>
/// Enum för sortering av bilder
/// </summary>
public static class SortByEnum
{
    public enum SortBy
    {
        /// <summary> Standard sortering </summary>
        Default = 0,
        /// <summary> Sorterar efter senast fotograferad </summary>
        Photographed = 1,
        /// <summary> Sorterar efter senast uppladdade </summary>
        Uploaded = 2,
        /// <summary> Sorterar per kategori namn </summary>
        Category = 3,
    }

    /// <summary>
    /// Returns a user-friendly string representation of the specified <see cref="SortBy"/> value.
    /// </summary>
    /// <param name="sortBy">The <see cref="SortBy"/> value to convert to a user-friendly string.</param>
    /// <returns>A string that represents the <paramref name="sortBy"/> value in a readable format. Returns "sort-by-unknown" if
    /// the value is not recognized.</returns>
    public static string ToFriendlyString(this SortBy sortBy) => sortBy switch
    {
        SortBy.Default => "default",
        SortBy.Photographed => "photographed",
        SortBy.Uploaded => "uploaded",
        SortBy.Category => "category",
        _ => "sort-by-unknown"
    };
}
