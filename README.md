# Intellias CQRS

The Intellias CQRS Framework is a cloud accelerator that provides instant scaling, fast data reading, analytics granularity, and most importantly, an environment that accelerates the development of cloud solutions. It's built around [CQRS](https://docs.microsoft.com/en-us/azure/architecture/patterns/cqrs) and [Event Sourcing](https://docs.microsoft.com/en-us/azure/architecture/patterns/event-sourcing) patterns that implemented through the three core components - *Command Handler*, *Event Handler*, and *Process Handler*. Together they enable user to build event-driven, microservices-based solution. The framework provides basic abstractions, components, clients to data storages, and messaging systems (now it's Azure services, but you can write own implementations). Event-driven nature of the components makes solution built using CQRS Framework ideal for the serverless model of execution, and can be easily run in FaaS (function as a service). All the communication between components is asyncronuos. It allows application to survive load peaks and scale on-demand. Ability to build query-optimized data structures allows to use simple key-value stores, which makes read operations fast and cheap.


## Design

As mentioned before, the framework is built on top of two architectural patterns (CQRS and Event Sourcing) that are the source of basic abstractions and concepts. It's highly recommended to get familiar with them before going further. If you're already familiar with them, then it shouldn't be a surprise that an application is build on top of CQRS Framework manages two representations of the same state:

* *Write State* - is write-optimized, updated by *Commands*, managed by *Command Handler*.
* *Read State* - is read-optimized, updated by *Events*, managed by *Event Handler* or *Process Handler*.

*Write State* is not intended to be concumed by the user. User (or another microservice) has acess only to *Read State*. When user want to change *Read State* he/she executes a *Command* that updates *Write State*. Any mutation of *Write State* produces an *Event* which is published and then handled by *Event/Process Handler*. Component updates *Read State* and also fires an *Event* that notifies users that *Read State* is updated.

As communication between components is asyncrounous, any changes in states are done in an eventual consistency manner. It makes any transactional operations to be distributed and executed based on [Saga](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/saga/saga) or [Process Manager](https://www.enterpriseintegrationpatterns.com/patterns/messaging/ProcessManager.html) patterns. It also requires client side that executes a *Command* to wait for response using [Publish-subscribe](https://www.enterpriseintegrationpatterns.com/patterns/messaging/PublishSubscribeChannel.html) pattern (for ex. using WebSocket API).

During lifecycle of the solution, the structure of the *Write State* and *Read State* could change. While the *Write State* could be easily migrated (if state isn't a sequece of Events), *Read State* (built from Events) should be rebuilt from scratch. For that reason all Events (let's skip Snapshots optimization for now) should be replayed. That is why all Events fired from *Command Handler* are stored in the *Event* store - log-like, append-only immutable storage.

Now, when all the main components, basic abstractions and flows are mentioned, lets review them separately in details.


### Messages

In the description above, we've mentioned *Commands* and *Events*. While *Command* is clear and finished abstraction that represent some intent to mutate state, *Event* - is just a name of the concept that encompass three different abstractions:

* *State Event* - an *Event* that represent a changeset of *Write State* when Event Sourcing is implemented at the level of *Command Handler*.
* *Integration Event* - an *Event* that *Command Handler* fires when *Write State* changes to propagate those changes to *Event/Process Handler*. *Integration Events* are stored in *Integration Event* store forever.
* *Signal* - notification that *Command* is failed or *Read State* is changed. *Signals* similarly to *Commands* live in messaging systems until handled.

*Commands* and *Events* are transfered through the messaging systems (queues, topics, subscriptions, etc.). That's why they implement `IMesssage` interface.


### Command Handler

*Command Handler* is a component that contains application business logic, accepts *Command* as input, produces *Integration Event* as output, updates *Write State* as side effect. It's implemented on top of `MediatR` library, and uses `Behaviours` to validate *Commands* and publish generated *Integration Events*. To trigger *Command Handler* with the *Command* `IMessageDispatcher` should be used.

```csharp
var serviceProvider = new ServiceCollection()
    .AddCommandHandlers(typeof(Startup).Assembly, typeof(UpdateUserNameCommand).Assembly)
    .BuildServiceProvider();

var messageDispatcher = serviceProvider.GetRequiredService<IMessageDispatcher>();

await messageDispatcher.DispatchCommandAsync(new UpdateUserNameCommand { Name = "Pavlo" });
```

When business logic is simple and easy to manage, it could be placed inside *Command Handler*. Otherwise, the CQRS Framework provides base abstractions like `BaseAggregateRoot<AggregateState>` to follow [DDD](https://en.wikipedia.org/wiki/Domain-driven_design) and place business logic inside *Aggregate*. Below is an example of could look *Command Handler* that handles *Command* to `UserAggregateRoot`. In this example `UserState` implementation is based on Event Sourcing pattern.

```csharp
public class UserAggregateRoot : BaseAggregateRoot<UserState>
{
    public PaceAggregateRoot(string id, AggregateExecutionContext context)
        : base(id, context)
    {
    }

    public IExecutionResult UpdateName(string name)
    {
        if (name == "Pablo")
        {
            return ValidationFailed(ErrorCodes.ForeignNameIsForbidden)
                .ForCommand<UpdateUserNameCommand>();
        }

        PublishEvent<UserNameUpdatedStateEvent>(e =>
        {
            e.Name = name;
        });

        return Success();
    }
}

public class UserState : AggregateState,
    IEventApplier<UserNameUpdatedStateEvent>
{
    public PaceState()
    {
        Handles<UserNameUpdatedStateEvent>(Apply);
    }

    public string Name { get; private set; } = string.Empty;

    public void Apply(UserNameUpdatedStateEvent @event)
    {
        Name = @event.Name;
    }
}

public class UserCommandHandler : BaseCommandHandler,
    IRequestHandler<CommandRequest<UpdateUserNameCommand>, IExecutionResult>
{
    public async Task<IExecutionResult> Handle(CommandRequest<UpdateUserNameCommand> request, CancellationToken cancellationToken)
    {
        var (command, context, scope) = request;
        var user = scope.FindAggregateAsync<AggregateRoot, AggregateState>(command.AggregateRootId, context);

        var result = user.UpdateName(command.Name);
        if (!result.Success)
        {
            return result;
        }

        return IntegrationEvent<UserNameUpdatedIntegrationEvent>(context, e =>
        {
            e.User = user.SnapshotId;
            e.EntityName = user.State.Name;
        });
    }
}
```

Regardless of how business logic is implemented, *Write State* must be versioned so that each *Integration Event* could represent a changeset from current *Write State* version to the next. The first reason for that is optimistic concurrency - each *Command* should have `ExpectedVersion` property, that specifies Version of the *Write State* to be updated. The second reason is described below.


### Event Handler

The main reason of existance of *Event Handler* is building and updateing *Read State*. *Read State* in the CQRS Framework is represented in form of *Query Models*. There exist two types of them - mutable and immutable. As it goes from the name, when *Mutable Query Model* is updated, it replaces existing one; when *Immutable Query Model* is updated - it creates the new version of the *Immutable Query Model*. Both versions exist alongside. Versions of *Mutable/Immutable Query Models* are always bound to version of the *Write State* to guarantee data consistency.

Similarly to *Command Handler*, *Event Handler* uses `IMessageDispatcher` to trigger its pipeline. That pipeline is also built on top of `MediatR` library, but uses notification handlers.

```csharp
var serviceProvider = new ServiceCollection()
    .AddTableQueryModelStorage(o => o.ConnectionString = configuration.ConnectionString);
    .AddEventHandlers(typeof(Startup).Assembly)
    .BuildServiceProvider();

var messageDispatcher = serviceProvider.GetRequiredService<IMessageDispatcher>();

await messageDispatcher.DispatchEventAsync(new UserNameUpdatedIntegrationEvent());
```

Below is an example of how cloud look *Event Handler* to build *Query Model* that contains User names. `UserNamesEventHandler` uses Azure Storage Account Tables to store *Query Model*.

```csharp
public class UserNamesQueryModel : BaseMutableQueryModel
{
    public List<string> Names { get; set; } = new List<string>();
}

public class UserNamesEventHandler : MutableQueryModelEventHandler<UserNamesQueryModel>,
    INotificationHandler<IntegrationEventNotification<UserNameUpdatedIntegrationEvent>>
{
    public FinishedPaceEventHandler(
        IMutableQueryModelReader<UserNamesQueryModel> reader,
        IMutableQueryModelWriter<UserNamesQueryModel> writer,
        IMediator mediator)
        : base(reader, writer, mediator)
    {
    }

    public Task Handle(IntegrationEventNotification<UserNameUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        return HandleAsync(notification, e => e.User.EntryId, (e, qm) =>
        {
            qm.Version = e.User.EntryVersion;
            qm.Names.Add(e.Name);
        });
    }
}
```

Each *Event Handler* is idempotent and can handle the same *Integration Event* multiple times without breaking *Query Model*.


### Process Handler

While *Command Handler* accepts *Command* and produces *Integration Event*, and *Event Handler* accepts *Integration Event* and updates *Query Model*, *Process Handler* accepts *Integration Event* (or just triggered by a timer) and produces zero or more *Commands*. *Process Handler* plays role of independent observer that triggers some action in response to the State of the application. Below is listed an examples of business logic to be placed in *Process Handler*:

* Update multiple *Aggregate Roots* based on some condition
* Execute time-bound operations like "mark as obsolete if not updated during 2 days"
* Update *Aggregate Root* in response to changes in another *Aggregate Root*

It's a common case when *Process Handler* is placed along side to *Event Handler* to use some *Query Model* to verify some business rule. Below is an example that shows how could application execute logic that requires removal of the child entity when the parent entity is deleted.

```csharp
var serviceProvider = new ServiceCollection()
    .AddTableQueryModelStorage(o => o.ConnectionString = configuration.ConnectionString);
    .AddEventHandlers(typeof(Startup).Assembly)
    .AddProcessHandlers(typeof(Startup).Assembly, o => o.ActorId = "00000000-0000-0000-0000-000000000005") // Id of the Process Handler that creates a Command.
    .AddSingleton<ICommandBus<DefaultCommandBusOptions>>(_ => new QueueCommandBus<DefaultCommandBusOptions>(commandStore, commandBustOptions)) // Command store and Command Bus options are defined out of scope.
    .BuildServiceProvider();

var @event = new EntityDeletedIntegrationEvent();

var messageDispatcher = serviceProvider.GetRequiredService<IMessageDispatcher>();
await messageDispatcher.DispatchEventAsync(); // Trigger Event Handler to update Query Model for Process Handler.

var processHandlerExecutor = serviceProvider.GetRequiredService<ProcessHandlerExecutor>();
await processHandlerExecutor.ExecuteAsync(@event); // Trigger Process Handler.

public class EntityQueryModel : MutableQueryModel
{
    public string ParentId { get; set; } = string.Empty;
}

public class EntityRemovalProcessHandler : BaseProcessHandler,
    IProcessHandler<EntityDeletedIntegrationEvent>
{
    private readonly IMutableQueryModelReader<EntityQueryModel> entityReader;

    public EntityRemovalProcessHandler(IMutableQueryModelReader<EntityQueryModel> entityReader)
    {
        this.entityReader = entityReader;
    }

    public async Task<ProcessResponse> Handle(ProcessRequest<EntityDeletedIntegrationEvent> request, CancellationToken cancellationToken)
    {
        var child = await entityReader.GetAsync(request.State.AggregateRootId);
        var parent = await entityReader.GetAsync(child.ParentId);

        return Response(request, new DeleteEntityCommand
        {
            AggregateRootId = parent.Id
        });
    }
}
```

*Process Handler* is also idempotent based on it's input. In example above, if *Process Handler* is already handled `EntityDeletedIntegrationEvent`, then for second time it will do nothing.


### Scalability

Scalability of the CQRS Framework is based on two features:

1. Event-driven architecture that allows to run application in FaaS
2. Partitioning model that is put into core of the framework

The first feature is clear and doesn't require any explanation - scalability of the FaaS is the best what we have on a market now. The second one is a bit more complex. The frameworks requires business domain to be designed following DDD rules, specifically "transactionality is guaranteed only in scope of single Aggregate". Which means that if we want to execute the *Command* withing a transaction, then it should target only single *Aggregate*. That is why each *Command* has `AggregateRootId` property, and that is why each *Integration Event* has `AggregateRootId` property. This rule makes `AggregateRootId` to be ideal *Partition Key* in any messaging system and data storage.

## How to run tests

To run tests update secrets file to contain:

```json
{
  "StorageAccount": {
    "ConnectionString": "{some-connection-string-to-azure-storage-account}"
  }
}
```
