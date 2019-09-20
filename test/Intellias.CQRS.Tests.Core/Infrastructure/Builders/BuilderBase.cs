using System;
using AutoFixture;
using AutoFixture.Kernel;

namespace Intellias.CQRS.Tests.Core.Infrastructure.Builders
{
    public abstract class BuilderBase<T> : ISpecimenBuilder
    {
        protected BuilderBase(IFixture fixture)
        {
            Fixture = fixture;
            Random = new Random();
        }

        protected Random Random { get; }

        protected IFixture Fixture { get; }

        public object Create(object request, ISpecimenContext context)
        {
            var t = request as Type;
            if (t == null || t != typeof(T))
            {
                return new NoSpecimen();
            }

            var specimen = Create(context);
            if (specimen == null)
            {
                throw new NullReferenceException("Built specimen can't be null.'");
            }

            return specimen;
        }

        protected abstract T Create(ISpecimenContext context);
    }
}