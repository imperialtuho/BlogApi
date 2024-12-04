namespace Blog.Domain.Entities
{
    public class Media : BaseEntity<string>
    {
        public string? AssetId { get; set; }

        /// <summary>
        /// Navigation of entity which linked to this media. E.g Post is linked to this media's property by PostId.
        /// </summary>
        public string? InternalId { get; set; }
    }
}