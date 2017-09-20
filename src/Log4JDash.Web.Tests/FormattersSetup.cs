using System.Collections.Generic;
using Log4JDash.Web.Models;
using NUnit.Framework;

namespace Log4JDash.Web.Tests
{
    [SetUpFixture]
    public class FormattersSetup
    {
        [OneTimeSetUp]
        public void SetUp ()
        {
            TestContext.AddFormatter<EventExpectation> (o => Formatter.Format ((EventExpectation) o));
            TestContext.AddFormatter<EventExpectation[]> (o => Formatter.Format ((EventExpectation[]) o));
            TestContext.AddFormatter<EventModel> (o => Formatter.Format ((EventModel) o));
            TestContext.AddFormatter<IEnumerable<EventModel>> (o => Formatter.Format ((IEnumerable<EventModel>) o));
        }
    }
}
