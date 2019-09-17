using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using Intellias.CQRS.Tests.Core.Infrastructure.Builders;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Intellias.CQRS.Tests.Core.Infrastructure
{
    public static class FixtureUtils
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private static readonly Random Rnd = new Random(Environment.TickCount);

        public static IFixture Use<TEntity>(this IFixture fixture, Func<IFixture, BuilderBase<TEntity>> specimenBuilder)
        {
            if (specimenBuilder == null)
            {
                throw new ArgumentNullException(nameof(specimenBuilder));
            }

            fixture.Customizations.Insert(0, specimenBuilder(fixture));
            return fixture;
        }

        public static IFixture Use<TEntity, TSeed>(this IFixture fixture, Func<IFixture, SeededBuilderBase<TEntity, TSeed>> specimenBuilder)
        {
            if (specimenBuilder == null)
            {
                throw new ArgumentNullException(nameof(specimenBuilder));
            }

            fixture.Customizations.Insert(0, specimenBuilder(fixture));
            return fixture;
        }

        public static T FromEnum<T>(this IFixture fixture)
            where T : IConvertible
        {
            return fixture.Create<Generator<T>>().First();
        }

        public static T[] FromEnumMany<T>(this IFixture fixture)
            where T : IConvertible
        {
            var names = Enum.GetValues(typeof(T));
            return names.Cast<T>().Take(Rnd.Next(1, names.Length)).ToArray();
        }

        public static T CanBeDefault<T>(this IFixture fixture, int nullPossibility, Func<IFixture, T> generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            return Rnd.Next(0, 100) < nullPossibility ? default : generator(fixture);
        }

        public static string CanBeEmpty(int nullPossibility, Func<string> generator)
        {
            return Rnd.Next(0, 100) < nullPossibility ? string.Empty : generator();
        }

        public static T JsonCopy<T>(T source, Action<T>? setup = null)
        {
            var serialized = JsonConvert.SerializeObject(source);
            var deserialized = JsonConvert.DeserializeObject<T>(serialized);

            setup?.Invoke(deserialized);

            return deserialized;
        }

        public static T AnyOf<T>(IEnumerable<T> source)
        {
            return source.OrderBy(_ => Rnd.Next()).First();
        }

        public static T[] Array<T>(int min, int max, Func<T> generator)
        {
            return Enumerable.Range(1, Rnd.Next(min, max)).Select(_ => generator()).ToArray();
        }

        public static T[] Array<T>(int min, int max, Func<int, T> generator)
        {
            return Enumerable.Range(1, Rnd.Next(min, max)).Select(generator).ToArray();
        }

        public static bool Bool()
        {
            return Rnd.Next(100) < 50;
        }

        public static int Int(int min = 1, int max = int.MaxValue)
        {
            return Rnd.Next(min, max);
        }

        public static int[] Ints(int min = 1, int max = 5)
        {
            return Array(min, max, () => Int(max: 1000)).Distinct().ToArray();
        }

        public static long Long(int min = 1, int max = int.MaxValue)
        {
            return Rnd.Next(min, max);
        }

        public static long[] Longs(int min = 1, int max = 5)
        {
            return Array(min, max, () => Long(max: 1000)).Distinct().ToArray();
        }

        public static string[] Strings(int min = 1, int max = 5)
        {
            return Array(min, max, () => String());
        }

        public static long Id()
        {
            return Rnd.Next();
        }

        public static string String()
        {
            return String(Rnd.Next(16, 32));
        }

        public static string String(int length)
        {
            return new string(Enumerable.Repeat(Chars, length).Select(s => s[Rnd.Next(s.Length)]).ToArray());
        }

        public static string Email()
        {
            return String(12) + "@gmail.com";
        }

        public static string[] Emails(int min = 1, int max = 5)
        {
            return Array(min, max, Email);
        }

        public static string Json()
        {
            return "{ \"prop\": \"" + String(32) + "\" }";
        }

        public static Dictionary<string, string> Dict()
        {
            return Dict(String);
        }

        public static Dictionary<string, TValue> Dict<TValue>(Func<TValue> generator)
        {
            return Strings().ToDictionary(s => s, s => generator());
        }

        public static JObject JObject()
            => Newtonsoft.Json.Linq.JObject.Parse(Json());

        public static JArray JArray()
            => new JArray(Json());
    }
}