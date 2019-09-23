using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    public class FakeMediator : IMediator
    {
        private readonly List<object> sentRequests = new List<object>();
        private readonly List<INotification> publishedNotifications = new List<INotification>();
        private readonly List<Handler> handlers = new List<Handler>();

        public IReadOnlyList<object> SentRequests => sentRequests.AsReadOnly();

        public IReadOnlyList<INotification> PublishedNotifications => publishedNotifications.AsReadOnly();

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            sentRequests.Add(request);

            var handler = handlers.FirstOrDefault(h => h.RequestType == request.GetType())
                ?? throw new KeyNotFoundException($"No handler for request type of '{request.GetType()}' is found.");

            var response = (TResponse)(handler.Handle(request) ?? throw new NullReferenceException("Response can't be null."));

            return Task.FromResult(response);
        }

        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            publishedNotifications.Add((INotification)notification);
            return Task.CompletedTask;
        }

        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Publish((object)notification, cancellationToken);
        }

        public void SetupRequestHandler<TRequest, TResponse>(Func<TRequest, TResponse> handle)
            where TRequest : IRequest<TResponse>
        {
            handlers.Add(Handler.Create(handle));
        }

        private class Handler
        {
            private Handler(Func<object, object?> handle, Type requestType)
            {
                Handle = handle;
                RequestType = requestType;
            }

            public Func<object, object?> Handle { get; }

            public Type RequestType { get; }

            public static Handler Create<TRequest, TResponse>(Func<TRequest, TResponse> handle)
            {
                return new Handler(o => handle((TRequest)o), typeof(TRequest));
            }
        }
    }
}