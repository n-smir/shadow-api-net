namespace ShadowApiNet.Models
{
    /// <summary>
    /// Represents parsed and validated path segments from an API request
    /// </summary>
    internal class PathInfo
    {
        public string[] RawSegments { get; }
        public int SegmentCount => RawSegments.Length;

        /// <summary>
        /// Indicates if the request is to the root path (no segments or empty first segment)
        /// </summary>
        public bool IsRoot => SegmentCount == 0 || string.IsNullOrEmpty(RawSegments[0]);

        /// <summary>
        /// Indicates if the request is to an entity collection (e.g., /api/users)
        /// </summary>
        public bool IsCollection => SegmentCount == 1 && !string.IsNullOrEmpty(RawSegments[0]);

        /// <summary>
        /// Indicates if the request is to a specific resource (e.g., /api/users/123)
        /// </summary>
        public bool IsResource => SegmentCount == 2;

        /// <summary>
        /// Gets the entity name from the path (first segment)
        /// Returns null if path is root
        /// </summary>
        public string EntityName => IsRoot ? null : RawSegments[0];

        /// <summary>
        /// Gets the entity ID from the path (second segment)
        /// Returns null if path is not a resource path
        /// </summary>
        public string EntityId => IsResource ? RawSegments[1] : null;

        /// <summary>
        /// Indicates if the path has more than 2 segments (typically invalid for REST)
        /// </summary>
        public bool HasExtraSegments => SegmentCount > 2;

        public PathInfo(string[] pathNodes)
        {
            RawSegments = pathNodes ?? [];
        }
    }
}
