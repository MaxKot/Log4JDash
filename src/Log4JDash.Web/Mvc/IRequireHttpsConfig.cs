
namespace Log4JDash.Web.Mvc
{
    public interface IRequireHttpsConfig
    {
        bool AllowHttp { get; }

        int? HttpsPort { get; }
    }
}
