using Ihjezly.Application.Abstractions.Clock;
using Ihjezly.Application.Exceptions;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.NewFolder;
using Ihjezly.Domain.Notifications;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Reviews;
using Ihjezly.Domain.SavedProperties;
using Ihjezly.Domain.Subscriptions;
using Ihjezly.Domain.Users;
using Ihjezly.Infrastructure.Configurations;
using Ihjezly.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Transaction = Ihjezly.Domain.Transactions.Transaction;

namespace Ihjezly.Infrastructure;

public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeProvider dateTimeProvider)
        : base(options)
    {
        Console.WriteLine(">>> Entered ApplicationDbContext constructor");

        if (options == null)
            throw new Exception("DbContextOptions is NULL!");

        if (dateTimeProvider == null)
            Console.WriteLine(">>> dateTimeProvider is null at runtime!");

        _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
    }

    // DbSets
    public DbSet<BusinessOwner> BusinessOwners => Set<BusinessOwner>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<Subscription> Subscriptions => Set<Subscription>();
    public DbSet<SavedProperty> SavedProperties => Set<SavedProperty>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<SelectableLocation> SelectableLocations => Set<SelectableLocation>();


    public DbSet<Property> Properties => Set<Property>();

    public DbSet<Apartment> Apartments => Set<Apartment>();
    public DbSet<Chalet> Chalets => Set<Chalet>();
    public DbSet<HotelRoom> HotelRooms => Set<HotelRoom>();
    public DbSet<HotelApartment> HotelApartments => Set<HotelApartment>();
    public DbSet<Resort> Resorts => Set<Resort>();
    public DbSet<RestHouse> RestHouses => Set<RestHouse>();
    public DbSet<VillaEvent> VillaEvents => Set<VillaEvent>();
    public DbSet<EventHallLarge> EventHallsLarge => Set<EventHallLarge>();
    public DbSet<EventHallSmall> EventHallsSmall => Set<EventHallSmall>();
    public DbSet<MeetingRoom> MeetingRooms => Set<MeetingRoom>();

    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        Console.WriteLine(">>> Starting OnModelCreating");

        try
        {
            Console.WriteLine(">>> Applying AdminConfiguration");
            modelBuilder.ApplyConfiguration(new AdminConfiguration());

            Console.WriteLine(">>> Applying ClientConfiguration");
            modelBuilder.ApplyConfiguration(new ClientConfiguration());

            Console.WriteLine(">>> Applying BusinessOwnerConfiguration");
            modelBuilder.ApplyConfiguration(new BusinessOwnerConfiguration());

            Console.WriteLine(">>> Applying PropertyConfiguration");
            modelBuilder.ApplyConfiguration(new PropertyConfiguration());

            Console.WriteLine(">>> Applying HotelRoomConfiguration");
            modelBuilder.ApplyConfiguration(new HotelRoomConfiguration());

            Console.WriteLine(">>> Applying HotelApartmentConfiguration");
            modelBuilder.ApplyConfiguration(new HotelApartmentConfiguration());

            Console.WriteLine(">>> Applying VillaEventConfiguration");
            modelBuilder.ApplyConfiguration(new VillaEventConfiguration());

            Console.WriteLine(">>> Applying EventHallLargeConfiguration");
            modelBuilder.ApplyConfiguration(new EventHallLargeConfiguration());

            Console.WriteLine(">>> Applying EventHallSmallConfiguration");
            modelBuilder.ApplyConfiguration(new EventHallSmallConfiguration());

            Console.WriteLine(">>> Applying ResortConfiguration");
            modelBuilder.ApplyConfiguration(new ResortConfiguration());

            Console.WriteLine(">>> Applying RestHouseConfiguration");
            modelBuilder.ApplyConfiguration(new RestHouseConfiguration());

            Console.WriteLine(">>> Applying ApartmentConfiguration");
            modelBuilder.ApplyConfiguration(new ApartmentConfiguration());

            Console.WriteLine(">>> Applying ChaletConfiguration");
            modelBuilder.ApplyConfiguration(new ChaletConfiguration());

            Console.WriteLine(">>> Applying MeetingRoomConfiguration");
            modelBuilder.ApplyConfiguration(new MeetingRoomConfiguration());

            Console.WriteLine(">>> Applying SubscriptionPlanConfiguration");
            modelBuilder.ApplyConfiguration(new SubscriptionPlanConfiguration());

            Console.WriteLine(">>> Applying SubscriptionConfiguration");
            modelBuilder.ApplyConfiguration(new SubscriptionConfiguration());

            Console.WriteLine(">>> Applying WalletConfiguration");
            modelBuilder.ApplyConfiguration(new WalletConfiguration());

            Console.WriteLine(">>> Applying TransactionConfiguration");
            modelBuilder.ApplyConfiguration(new TransactionConfiguration());

            Console.WriteLine(">>> Applying NotificationConfiguration");
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());

            Console.WriteLine(">>> Applying ReviewConfiguration");
            modelBuilder.ApplyConfiguration(new ReviewConfiguration());

            Console.WriteLine(">>> Applying BookingConfiguration");
            modelBuilder.ApplyConfiguration(new BookingConfiguration());

            Console.WriteLine(">>> Applying SavedPropertyConfiguration");
            modelBuilder.ApplyConfiguration(new SavedPropertyConfiguration());

            Console.WriteLine(">>> Applying OutboxMessageConfiguration");
            modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());


            Console.WriteLine(">>> Completed OnModelCreating successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine(">>> Exception in OnModelCreating: " + ex.Message);
            throw;
        }

        base.OnModelCreating(modelBuilder);
    }



    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_dateTimeProvider != null)
                AddDomainEventsAsOutboxMessages();

            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("A concurrency conflict occurred.", ex);
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        // Don't run domain events during design time
        if (_dateTimeProvider == null)
            return;

        var outboxMessages = ChangeTracker
            .Entries<Entity>()
            .Where(e => e.Entity.GetDomainEvents().Count != 0)
            .SelectMany(e =>
            {
                var domainEvents = e.Entity.GetDomainEvents();
                e.Entity.ClearDomainEvents();
                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings)))
            .ToList();

        AddRange(outboxMessages);
    }
}
