# Log4JDash
A log4j XML log files parser library and Web viewer.

Log4JDash project consists of the following parts:
* Log4JParserC - a native log4j XML log parser exposing C API. The library allows to enumerate and
  filter events in the logs.
* Log4JParserCpp and Log4JParserNet - C++ and .NET wrappers for the Log4JParserC library.
* Log4JDash and Log4JParserDashNet - sample console log viewers for evalutaing C++ and .NET
  wrappers and estimating parser performance.
* Log4JDash.Web - ASP.NET MVC 5 web application for viewing log4j XML logs remotely.

## Log4JParserC

The XML log parser core implementation. The library provides a C API for enumearting and filtering
events in log4j XML log files. The API also defines strings used to define log event levels.

**The log level strings are based on log4net log levels.** log4net defines more levels than log4j 2
and it uses inverted notion of order of levels. For example, in log4j 2 numeric value for DEBUG is
500 and numeric value for FATAL is 100; in log4net numeric value for DEBUG is 30000 and numeric
value for FATAL is 110000. This affects the notion of minimum and maximum levels when filtering the
events.

The parser is mostly implemented in C with XML parsing done in C++ using RapidXml library. Use of
native [in-situ](https://en.wikipedia.org/wiki/In-place_algorithm) XML parsing allows to reach high
performance even on large log files.

## Log4JParserCpp

The C++ XML log parser library. The library API is close to Log4JParserC but it takes advantage of
[RAII idiom](https://en.wikipedia.org/wiki/Resource_Acquisition_Is_Initialization).

## Log4JParserDashNet

.NET API is different from Log4JParserC in events enumeration to better match .NET collection
idioms.

Please note that while it is possible to manipulate event collections in .NET with LINQ to Objects
IEnumerable<T> extension methods it is advisable to use the methods of IEnumerableOfEvents
interface to reduce switching between managed and unmanaged code.

DisposableCollection class is also provided for easier buildup of event filters. Please not that
complex filters (such as And, Or and Not filters) do **not** take ownership of its parts, i.e.
child filters must be explicitly disposed by the client code.
