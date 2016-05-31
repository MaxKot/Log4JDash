using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace Log4JDash.Web.Mvc
{
    internal sealed class DefaultValueSourceAttribute : Attribute, IMetadataAware
    {
        private static ConcurrentDictionary<Tuple<Type, string>, Func<object>> FactoryCache =
            new ConcurrentDictionary<Tuple<Type, string>, Func<object>> ();

        private readonly string sourceName_;

        public DefaultValueSourceAttribute (string sourceName)
        {
            sourceName_ = sourceName;
        }

        public void OnMetadataCreated (ModelMetadata metadata)
        {
            var factoryKey = Tuple.Create (metadata.ContainerType, sourceName_);
            var factory = FactoryCache.GetOrAdd (factoryKey, CreateFactory);
            var value = factory ();

            metadata.AdditionalValues.Add (DefaultValueUtil.MetadataKey, value);
        }

        private static Func<object> CreateFactory (Tuple<Type, string> key)
        {
            return CreateFactory (key.Item1, key.Item2);
        }

        private static Func<object> CreateFactory (Type type, string sourceName)
        {
            const BindingFlags bindingFlags = BindingFlags.Public |
                                              BindingFlags.NonPublic |
                                              BindingFlags.Static |
                                              BindingFlags.InvokeMethod |
                                              BindingFlags.GetProperty |
                                              BindingFlags.GetField;

            Expression factoryExprBody;

            var sourceField = type.GetField (sourceName, bindingFlags);
            if (sourceField != null)
            {
                factoryExprBody = Expression.MakeMemberAccess (null, sourceField);
            }
            else
            {
                var sourceProperty = type.GetProperty (sourceName, bindingFlags);
                if (sourceProperty != null)
                {
                    factoryExprBody = Expression.MakeMemberAccess (null, sourceProperty);
                }
                else
                {
                    var sourceMethod = type.GetMethod (sourceName, bindingFlags, null, Type.EmptyTypes, null);
                    if (sourceMethod != null)
                    {
                        factoryExprBody = Expression.Call (sourceMethod);
                    }
                    else
                    {
                        const string format = "Unable to find static field, poperty, or " +
                                              "parameterless method '{0}' on type {1}.";
                        var message = String.Format (format, sourceName, type);
                        throw new InvalidOperationException (message);
                    }
                }
            }

            var factoryExpr = Expression.Lambda<Func<object>> (
                Expression.Convert (
                    factoryExprBody,
                    typeof (object)
                )
            );
            return factoryExpr.Compile ();
        }
    }
}
