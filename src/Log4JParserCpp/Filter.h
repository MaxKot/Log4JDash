#pragma once

#include <stdint.h>

namespace Log4JParser
{
    namespace CApi
    {
#include <Log4JFilter.h>
    }

    class FilterBase
    {
    protected:
        FilterBase (CApi::Log4JFilter *filter);

    public:
        ~FilterBase ();

    public:
        bool Apply (const CApi::Log4JEvent event) const;

    protected:
        static const CApi::Log4JFilter *GetFilter (const FilterBase *filter);
        static CApi::Log4JFilter *GetFilter (FilterBase *filter);

    private:
        CApi::Log4JFilter *filter_;
    };

    class FilterLevel : public FilterBase
    {
    public:
        FilterLevel (int32_t min, int32_t max);
        FilterLevel (const char *min, const char *max);
    };

    class FilterLogger : public FilterBase
    {
    public:
        FilterLogger (const char *logger, const size_t loggerSize);
        FilterLogger (const char *logger);
    };

    class FilterMessage : public FilterBase
    {
    public:
        FilterMessage (const char *message, const size_t messageSize);
        FilterMessage (const char *message);
    };

    class FilterTimestamp : public FilterBase
    {
    public:
        FilterTimestamp (int64_t min, int64_t max);
    };

    class FilterAll : public FilterBase
    {
    public:
        FilterAll ();

        void Add (const FilterBase *childFilter);
        void Remove (const FilterBase *childFilter);
    };

    class FilterAny : public FilterBase
    {
    public:
        FilterAny ();

        void Add (const FilterBase *childFilter);
        void Remove (const FilterBase *childFilter);
    };

    class FilterNot : public FilterBase
    {
    public:
        FilterNot (const FilterBase *childFilter);
    };
}
