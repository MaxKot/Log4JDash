
namespace Log4JDash.Web.Filters
{
    public interface IRequireHttpsConfig
    {
        bool AllowHttp { get; }

        int? HttpsPort { get; }
    }
}
