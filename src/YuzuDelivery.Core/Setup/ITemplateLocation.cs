namespace YuzuDelivery.Core
{
    public interface ITemplateLocation
    {
        string Name { get; set; }
        string Path { get; set; }
        string Schema { get; set; }
        bool RegisterAllAsPartials { get; set; }
        bool SearchSubDirectories { get; set; }

        TemplateType TemplateType { get; set; }
    }
}
