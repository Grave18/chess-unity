using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace UtilsCommon.ObjectCreation
{
    public static class FastActivator
    {
        private static readonly ConcurrentDictionary<(Type, Type[]), Delegate> Cache = new();

        public static T CreateInstance<T>(params object[] args)
        {
            var argTypes = args.Select(a => a.GetType()).ToArray();
            var key = (typeof(T), argTypes);

            if (!Cache.TryGetValue(key, out Delegate ctorDelegate))
            {
                ctorDelegate = CreateConstructorDelegate<T>(argTypes);
                Cache[key] = ctorDelegate;
            }

            return ((Func<object[], T>)ctorDelegate)(args);
        }

        private static Func<object[], T> CreateConstructorDelegate<T>(Type[] argTypes)
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                binder: null,
                types: argTypes,
                modifiers: null
            );

            if (ctor == null)
                throw new MissingMethodException($"No matching constructor for type {typeof(T).Name}");

            // Parameter: object[] args
            ParameterExpression paramExpr = Expression.Parameter(typeof(object[]), "args");

            // Convert each object[] element to the correct type
            var argsExpr = argTypes.Select((type, index) =>
                Expression.Convert(
                    Expression.ArrayIndex(paramExpr, Expression.Constant(index)),
                    type
                )
            ).ToArray();

            // New T(args...)
            NewExpression newExpr = Expression.New(ctor, argsExpr);

            // Lambda: object[] -> T
            var lambda = Expression.Lambda<Func<object[], T>>(newExpr, paramExpr);

            return lambda.Compile();
        }
    }
}