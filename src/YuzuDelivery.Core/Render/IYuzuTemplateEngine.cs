namespace YuzuDelivery.Core
{
    public interface IYuzuTemplateEngine
    {
        string Render(string templateName, object model);

        string Render<TModel>(string templateName, TModel model)
        {
            return Render(templateName, (object) model);
        }
    }
}
