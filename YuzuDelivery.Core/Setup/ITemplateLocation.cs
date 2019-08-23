namespace YuzuDelivery.Umbraco.Blocks
{
    public interface ITemplateLocation
    {
        string Name { get; set; }
        string Path { get; set; }
        bool RegisterAllAsPartials { get; set; }
        bool SearchSubDirectories { get; set; }
    }
}