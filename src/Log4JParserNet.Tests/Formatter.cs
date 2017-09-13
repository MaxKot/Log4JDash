using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Log4JParserNet.Tests
{
    internal static class Formatter
    {
        private static string IndentString (int indent)
            => new String (' ', indent * 2);

        private static string Format (string value)
            => value != null ? $"'{value}'" : "!!null";

        private static string Format (long value)
            => value.ToString ("N", CultureInfo.InvariantCulture);

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
                    indentString + $"{nameof (EventExpectation.Timestamp)}: {Format (value.Timestamp)}" + "\n" +
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

        public static string Format (Event value)
            => Format (value, 0);

        private static string Format (Event value, int indent)
        {
            var indentString = IndentString (indent);

            string result;
            if (value != null)
            {
                result =
                    indentString + $"!!{nameof (EventExpectation)} {{" + "\n" +
                    indentString + $"{nameof (Event.Level)}: {Format (value.Level)}" + "\n" +
                    indentString + $"{nameof (Event.Logger)}: {Format (value.Logger)}" + "\n" +
                    indentString + $"{nameof (Event.Thread)}: {Format (value.Thread)}" + "\n" +
                    indentString + $"{nameof (Event.Timestamp)}: {Format (value.Timestamp)}" + "\n" +
                    indentString + $"{nameof (Event.Message)}: {Format (value.Message)}" + "\n" +
                    indentString + $"{nameof (Event.Throwable)}: {Format (value.Throwable)}" + "\n" +
                    indentString + $"Properties: {Format (value.GetProperties (), indent + 1)}" + "\n" +
                    indentString + $"{nameof (Event.Id)}: {Format (value.Id)}" + "\n" +
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

        public static string Format (IEnumerable<Event> value)
            => Format (value, 0);

        private static string Format (IEnumerable<Event> value, int indent)
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
