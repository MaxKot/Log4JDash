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
        public long? Size { get; set; }

        public LogSourceModel (string id, long? size)
        {
            Id = id;
            Size = size;
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
        {
            return Converter.ConvertToString (this);
        }
    }
}
