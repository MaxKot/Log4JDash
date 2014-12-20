#include "Filter.h"

namespace Log4JParser
{
    FilterBase::FilterBase (Log4JFilter *filter)
        : filter_ (filter)
    {

    }

    FilterBase::~FilterBase ()
    {
        Log4JFilterDestroy (filter_);
        filter_ = nullptr;
    }

    bool FilterBase::Apply (const Event event) const
    {
        auto result = Log4JFilterApply (filter_, Event::GetEvent (event));
        return result;
    }

    const Log4JFilter *FilterBase::GetFilter (const FilterBase *filter)
    {
        return filter->filter_;
    }

    Log4JFilter *FilterBase::GetFilter (FilterBase *filter)
    {
        return filter->filter_;
    }

    static Log4JFilter *Log4JFilterInitLevelI_ (int32_t min, int32_t max)
    {
        Log4JFilter *filter;
        Log4JFilterInitLevelI (&filter, min, max);

        return filter;
    }

    FilterLevel::FilterLevel (int32_t min, int32_t max)
        : FilterBase (Log4JFilterInitLevelI_ (min, max))
    {

    }

    static Log4JFilter *Log4JFilterInitLevelC_ (const char *min, const char *max)
    {
        Log4JFilter *filter;
        Log4JFilterInitLevelC (&filter, min, max);

        return filter;
    }

    FilterLevel::FilterLevel (const char *min, const char *max)
        : FilterBase (Log4JFilterInitLevelC_ (min, max))
    {

    }

    static Log4JFilter *Log4JFilterInitLoggerFs_ (const char *logger, const size_t loggerSize)
    {
        Log4JFilter *filter;
        Log4JFilterInitLoggerFs (&filter, logger, loggerSize);

        return filter;
    }

    FilterLogger::FilterLogger (const char *logger, const size_t loggerSize)
        : FilterBase (Log4JFilterInitLoggerFs_ (logger, loggerSize))
    {

    }

    static Log4JFilter *Log4JFilterInitLoggerNt_ (const char *logger)
    {
        Log4JFilter *filter;
        Log4JFilterInitLoggerNt (&filter, logger);

        return filter;
    }

    FilterLogger::FilterLogger (const char * logger)
        : FilterBase (Log4JFilterInitLoggerNt_ (logger))
    {

    }

    static Log4JFilter *Log4JFilterInitMessageFs_ (const char *message, const size_t messageSize)
    {
        Log4JFilter *filter;
        Log4JFilterInitMessageFs (&filter, message, messageSize);

        return filter;
    }

    FilterMessage::FilterMessage (const char *message, const size_t messageSize)
        : FilterBase (Log4JFilterInitMessageFs_ (message, messageSize))
    {

    }

    static Log4JFilter *Log4JFilterInitMessageNt_ (const char *message)
    {
        Log4JFilter *filter;
        Log4JFilterInitMessageNt (&filter, message);

        return filter;
    }

    FilterMessage::FilterMessage (const char * message)
        : FilterBase (Log4JFilterInitMessageNt_ (message))
    {

    }

    static Log4JFilter *Log4JFilterInitTimestamp_ (int64_t min, int64_t max)
    {
        Log4JFilter *filter;
        Log4JFilterInitTimestamp (&filter, min, max);

        return filter;
    }

    FilterTimestamp::FilterTimestamp (int64_t min, int64_t max)
        : FilterBase (Log4JFilterInitTimestamp_ (min, max))
    {

    }

    static Log4JFilter *Log4JFilterInitAll_ ()
    {
        Log4JFilter *filter;
        Log4JFilterInitAll (&filter);

        return filter;
    }

    FilterAll::FilterAll ()
        : FilterBase (Log4JFilterInitAll_ ())
    {

    }

    void FilterAll::Add (const FilterBase *childFilter)
    {
        Log4JFilterAllAdd (GetFilter (this), GetFilter (childFilter));
    }

    void FilterAll::Remove (const FilterBase *childFilter)
    {
        Log4JFilterAllRemove (GetFilter (this), GetFilter (childFilter));
    }

    static Log4JFilter *Log4JFilterInitAny_ ()
    {
        Log4JFilter *filter;
        Log4JFilterInitAny (&filter);

        return filter;
    }

    FilterAny::FilterAny ()
        : FilterBase (Log4JFilterInitAny_ ())
    {

    }

    void FilterAny::Add (const FilterBase *childFilter)
    {
        Log4JFilterAnyAdd (GetFilter (this), GetFilter (childFilter));
    }

    void FilterAny::Remove (const FilterBase *childFilter)
    {
        Log4JFilterAnyRemove (GetFilter (this), GetFilter (childFilter));
    }

    static Log4JFilter *Log4JFilterInitNot_ (const Log4JFilter *childFilter)
    {
        Log4JFilter *filter;
        Log4JFilterInitNot (&filter, childFilter);

        return filter;
    }

    FilterNot::FilterNot (const FilterBase *childFilter)
        : FilterBase (Log4JFilterInitNot_ (GetFilter (childFilter)))
    {

    }
}
