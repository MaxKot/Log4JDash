#include "Filter.h"

namespace Log4JParser
{
    FilterBase::FilterBase (CApi::Log4JFilter *filter)
        : filter_ (filter)
    {

    }

    FilterBase::~FilterBase ()
    {
        Log4JFilterDestroy (filter_);
        filter_ = nullptr;
    }

    bool FilterBase::Apply (const CApi::Log4JEvent event) const
    {
        auto result = Log4JFilterApply (filter_, event);
        return result;
    }

    const CApi::Log4JFilter *FilterBase::GetFilter (const FilterBase *filter)
    {
        return filter->filter_;
    }

    CApi::Log4JFilter *FilterBase::GetFilter (FilterBase *filter)
    {
        return filter->filter_;
    }

    static CApi::Log4JFilter *Log4JFilterInitLevelI_ (int32_t min, int32_t max)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitLevelI (&filter, min, max);

        return filter;
    }

    FilterLevel::FilterLevel (int32_t min, int32_t max)
        : FilterBase (Log4JFilterInitLevelI_ (min, max))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitLevelC_ (const char *min, const char *max)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitLevelC (&filter, min, max);

        return filter;
    }

    FilterLevel::FilterLevel (const char *min, const char *max)
        : FilterBase (Log4JFilterInitLevelC_ (min, max))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitLoggerFs_ (const char *logger, const size_t loggerSize)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitLoggerFs (&filter, logger, loggerSize);

        return filter;
    }

    FilterLogger::FilterLogger (const char *logger, const size_t loggerSize)
        : FilterBase (Log4JFilterInitLoggerFs_ (logger, loggerSize))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitLoggerNt_ (const char *logger)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitLoggerNt (&filter, logger);

        return filter;
    }

    FilterLogger::FilterLogger (const char * logger)
        : FilterBase (Log4JFilterInitLoggerNt_ (logger))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitMessageFs_ (const char *message, const size_t messageSize)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitMessageFs (&filter, message, messageSize);

        return filter;
    }

    FilterMessage::FilterMessage (const char *message, const size_t messageSize)
        : FilterBase (Log4JFilterInitMessageFs_ (message, messageSize))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitMessageNt_ (const char *message)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitMessageNt (&filter, message);

        return filter;
    }

    FilterMessage::FilterMessage (const char * message)
        : FilterBase (Log4JFilterInitMessageNt_ (message))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitTimestamp_ (int64_t min, int64_t max)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitTimestamp (&filter, min, max);

        return filter;
    }

    FilterTimestamp::FilterTimestamp (int64_t min, int64_t max)
        : FilterBase (Log4JFilterInitTimestamp_ (min, max))
    {

    }

    static CApi::Log4JFilter *Log4JFilterInitAll_ ()
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitAll (&filter);

        return filter;
    }

    FilterAll::FilterAll ()
        : FilterBase (Log4JFilterInitAll_ ())
    {

    }

    void FilterAll::Add (const FilterBase *childFilter)
    {
        CApi::Log4JFilterAllAdd (GetFilter (this), GetFilter (childFilter));
    }

    void FilterAll::Remove (const FilterBase *childFilter)
    {
        Log4JFilterAllRemove (GetFilter (this), GetFilter (childFilter));
    }

    static CApi::Log4JFilter *Log4JFilterInitAny_ ()
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitAny (&filter);

        return filter;
    }

    FilterAny::FilterAny ()
        : FilterBase (Log4JFilterInitAny_ ())
    {

    }

    void FilterAny::Add (const FilterBase *childFilter)
    {
        CApi::Log4JFilterAnyAdd (GetFilter (this), GetFilter (childFilter));
    }

    void FilterAny::Remove (const FilterBase *childFilter)
    {
        Log4JFilterAnyRemove (GetFilter (this), GetFilter (childFilter));
    }

    static CApi::Log4JFilter *Log4JFilterInitNot_ (const CApi::Log4JFilter *childFilter)
    {
        CApi::Log4JFilter *filter;
        CApi::Log4JFilterInitNot (&filter, childFilter);

        return filter;
    }

    FilterNot::FilterNot (const FilterBase *childFilter)
        : FilterBase (Log4JFilterInitNot_ (GetFilter (childFilter)))
    {

    }
}
