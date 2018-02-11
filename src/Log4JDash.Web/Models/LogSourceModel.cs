using System.ComponentModel;
using System.Web.Mvc;

namespace Log4JDash.Web.Models
{
    [TypeConverter (typeof (LogSourceModelConverter))]
    public sealed class LogSourceModel
    {
        private static readonly LogSourceModelConverter Converter = new LogSourceModelConverter ();

        public string Id { get; set; }

        [HiddenInput (DisplayValue = false)]
        public string Snapshot { get; set; }

        public LogSourceModel (string id, string snapshot)
        {
            Id = id;
            Snapshot = snapshot;
        }

        public LogSourceModel (string id)
            : this (id, null)
        {

        }

        public LogSourceModel ()
            : this (null, null)
        {

        }

        public override string ToString ()
            => Converter.ConvertToString (this);
    }
}
