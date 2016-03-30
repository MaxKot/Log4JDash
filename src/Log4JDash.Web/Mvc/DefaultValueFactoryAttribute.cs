using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal sealed class DefaultValueFactoryAttribute : MetadataAttributeBase
    {
        private static ConcurrentDictionary<Tuple<Type, string>, Func<object>> FactoryCache =
            new ConcurrentDictionary<Tuple<Type, string>, Func<object>> ();

        private readonly string factoryName_;

        public DefaultValueFactoryAttribute (string factoryName)
        {
            factoryName_ = factoryName;
        }

        internal override void GetMetadataForProperty
            (Func<object> modelAccessor, Type containerType, PropertyDescriptor propertyDescriptor, ModelMetadata result)
        {
            var factoryKey = Tuple.Create (containerType, factoryName_);
            var factory = FactoryCache.GetOrAdd (factoryKey, CreateFactory);
            var value = factory ();

            result.AdditionalValues.Add (DefaultValueUtil.MetadataKey, value);
        }

        private static Func<object> CreateFactory (Tuple<Type, string> key)
        {
            return CreateFactory (key.Item1, key.Item2);
        }

        private static Func<object> CreateFactory (Type type, string factoryName)
        {
            const BindingFlags bindingFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Static |
                                              BindingFlags.InvokeMethod;
            var factory = type.GetMethod (factoryName, bindingFlags, null, Type.EmptyTypes, null);

            var factoryExpr = Expression.Lambda<Func<object>> (
                Expression.Convert (
                    Expression.Call (factory),
                    typeof (object)
                )
            );
            return factoryExpr.Compile ();
        }
    }
}
