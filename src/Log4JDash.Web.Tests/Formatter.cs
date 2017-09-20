using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Log4JDash.Web.Models;

namespace Log4JDash.Web.Tests
{
    internal static class Formatter
    {
        private static string IndentString (int indent)
            => new String (' ', indent * 2);

        private static string Format (string value)
            => value != null ? $"'{value}'" : "!!null";

        private static string Format (DateTime value)
            => value.ToString ("O", CultureInfo.InvariantCulture);

        private static string Format (ulong value)
            => value.ToString ("N", CultureInfo.InvariantCulture);

        private static string Format (KeyValuePair<string, string> value)
            => $"{Format (value.Key)} : {Format (value.Value)}";

        private static string Format (IEnumerable<KeyValuePair<string, string>> value, int indent = 0)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                var memberIndentString = IndentString (indent + 1);

                var resultBuilder = new StringBuilder ();
                resultBuilder.AppendLine ("{");
                foreach (var kvp in value)
                {
                    resultBuilder.Append (memberIndentString);
                    resultBuilder.Append ("  ");
                    resultBuilder.Append (Format (kvp));
                    resultBuilder.AppendLine ();
                }
                resultBuilder.Append (indentString);
                resultBuilder.Append ("}");

                result = resultBuilder.ToString ();
            }
            else
            {
                result = indentString + "!!null";
            }

            return result;
        }

        public static string Format (EventExpectation value)
            => Format (value, 0);

        private static string Format (EventExpectation value, int indent)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                result =
                    indentString + $"!!{nameof (EventExpectation)} {{" + "\n" +
                    indentString + $"{nameof (EventExpectation.Level)}: {Format (value.Level)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Logger)}: {Format (value.Logger)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Thread)}: {Format (value.Thread)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Time)}: {Format (value.Time)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Message)}: {Format (value.Message)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Throwable)}: {Format (value.Throwable)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Properties)}: {Format (value.Properties, indent + 1)}" + "\n" +
                    indentString + $"{nameof (EventExpectation.Id)}: {Format (value.Id)}" + "\n" +
                    indentString + "}";
            }
            else
            {
                result = "!!null";
            }

            return result;
        }

        public static string Format (EventModel value)
            => Format (value, 0);

        private static string Format (EventModel value, int indent)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                result =
                    indentString + $"!!{nameof (EventExpectation)} {{" + "\n" +
                    indentString + $"{nameof (EventModel.Level)}: {Format (value.Level)}" + "\n" +
                    indentString + $"{nameof (EventModel.Logger)}: {Format (value.Logger)}" + "\n" +
                    indentString + $"{nameof (EventModel.Thread)}: {Format (value.Thread)}" + "\n" +
                    indentString + $"{nameof (EventModel.Time)}: {Format (value.Time)}" + "\n" +
                    indentString + $"{nameof (EventModel.Message)}: {Format (value.Message)}" + "\n" +
                    indentString + $"{nameof (EventModel.Throwable)}: {Format (value.Throwable)}" + "\n" +
                    indentString + $"{nameof (EventModel.Properties)}: {Format (value.Properties, indent + 1)}" + "\n" +
                    indentString + $"{nameof (EventModel.Id)}: {Format (value.Id)}" + "\n" +
                    indentString + "}";
            }
            else
            {
                result = indentString + "!!null";
            }

            return result;
        }

        public static string Format (EventExpectation[] value)
            => Format (value, 0);

        private static string Format (EventExpectation[] value, int indent)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                var resultBuilder = new StringBuilder ();
                resultBuilder.Append (indentString);
                resultBuilder.AppendLine ("[");
                foreach (var element in value)
                {
                    resultBuilder.Append (Format (element, indent + 1));
                    resultBuilder.AppendLine ();
                }
                resultBuilder.Append (indentString);
                resultBuilder.Append ("]");

                result = resultBuilder.ToString ();
            }
            else
            {
                result = indentString + "!!null";
            }

            return result;
        }

        public static string Format (IEnumerable<EventModel> value)
            => Format (value, 0);

        private static string Format (IEnumerable<EventModel> value, int indent)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                var resultBuilder = new StringBuilder ();
                resultBuilder.Append (indentString);
                resultBuilder.AppendLine ("[");
                foreach (var element in value)
                {
                    resultBuilder.Append (Format (element, indent + 1));
                    resultBuilder.AppendLine ();
                }
                resultBuilder.Append (indentString);
                resultBuilder.Append ("]");

                result = resultBuilder.ToString ();
            }
            else
            {
                result = indentString + "!!null";
            }

            return result;
        }
    }
}
