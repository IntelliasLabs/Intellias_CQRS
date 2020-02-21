using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Intellias.CQRS.Tests.Core.Fakes
{
    /// <summary>
    /// FakeMediator.
    /// </summary>
    public class FakeMediator : IMediator
    {
        private readonly List<object> sentRequests = new List<object>();
        private readonly List<INotification> publishedNotifications = new List<INotification>();
        private readonly List<Handler> handlers = new List<Handler>();

        /// <summary>
        /// SentRequests.
        /// </summary>
        public IReadOnlyList<object> SentRequests => sentRequests.AsReadOnly();

        /// <summary>
        /// PublishedNotifications.
        /// </summary>
        public IReadOnlyList<INotification> PublishedNotifications => publishedNotifications.AsReadOnly();

        /// <summary>
        /// Send.
        /// </summary>
        /// <typeparam name="TResponse">TResponse.</typeparam>
        /// <param name="request">request.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>TResponse task.</returns>
        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            sentRequests.Add(request);

            var handler = handlers.FirstOrDefault(h => h.RequestType == request.GetType())
                ?? throw new KeyNotFoundException($"No handler for request type of '{request.GetType()}' is found.");

            var response = (TResponse)(handler.Handle(request) ?? throw new NullReferenceException("Response can't be null."));

            return Task.FromResult(response);
        }

        /// <summary>
        /// Send.
        /// </summary>
        /// <param name="request">request.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>object.</returns>
        public Task<object> Send(object request, CancellationToken cancellationToken = default)
        {
            sentRequests.Add(request);

            var handler = handlers.FirstOrDefault(h => h.RequestType == request.GetType())
                ?? throw new KeyNotFoundException($"No handler for request type of '{request.GetType()}' is found.");

            var response = handler.Handle(request) ?? throw new NullReferenceException("Response can't be null.");

            return Task.FromResult(response);
        }

        /// <summary>
        /// Publish.
        /// </summary>
        /// <param name="notification">notification.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Task.</returns>
        public Task Publish(object notification, CancellationToken cancellationToken = default)
        {
            publishedNotifications.Add((INotification)notification);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Publish.
        /// </summary>
        /// <typeparam name="TNotification">TNotification.</typeparam>
        /// <param name="notification">notification.</param>
        /// <param name="cancellationToken">cancellationToken.</param>
        /// <returns>Task.</returns>
        public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
            where TNotification : INotification
        {
            return Publish((object)notification, cancellationToken);
        }

        /// <summary>
        /// SetupRequestHandler.
        /// </summary>
        /// <typeparam name="TRequest">TRequest.</typeparam>
        /// <typeparam name="TResponse">TResponse.</typeparam>
        /// <param name="handle">handle.</param>
        public void SetupRequestHandler<TRequest, TResponse>(Func<TRequest, TResponse> handle)
            where TRequest : IRequest<TResponse>
        {
            handlers.Add(Handler.Create(handle));
        }

        private class Handler
        {
            private Handler(Func<object, object> handle, Type requestType)
            {
                Handle = handle;
                RequestType = requestType;
            }

            public Func<object, object> Handle { get; }

            public Type RequestType { get; }

            public static Handler Create<TRequest, TResponse>(Func<TRequest, TResponse> handle)
            {
                return new Handler(o => handle((TRequest)o), typeof(TRequest));
            }
        }
    }
}