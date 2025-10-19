using Ihjezly.Application.Abstractions.Authentication;
using Ihjezly.Application.Abstractions.Clock;
using Ihjezly.Application.Abstractions.Data;
using Ihjezly.Application.Abstractions.Files;
using Ihjezly.Application.Abstractions.Payment;
using Ihjezly.Application.Abstractions.Payment.Edfali;
using Ihjezly.Application.Payments.Tdsp;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking.Repositories;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Reposrts.Repositories;
using Ihjezly.Domain.Users.Repositories;
using Ihjezly.Infrastructure.Authentication;
using Ihjezly.Infrastructure.BackgroundJobs;
using Ihjezly.Infrastructure.Clock;
using Ihjezly.Infrastructure.Data;
using Ihjezly.Infrastructure.Files;
using Ihjezly.Infrastructure.Outbox;
using Ihjezly.Infrastructure.Payments;
using Ihjezly.Infrastructure.Payments.Edfali;
using Ihjezly.Infrastructure.Payments.Masarat;
using Ihjezly.Infrastructure.Payments.Paypal;
using Ihjezly.Infrastructure.Payments.Stripe;
using Ihjezly.Infrastructure.Payments.Tdsp;
using Ihjezly.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quartz;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;


namespace Ihjezly.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<BookingCompletionService>();


        //Payment Service
        services.Configure<EdfaliOptions>(configuration.GetSection("EdfaliOptions"));
        services.AddHttpClient<IEdfaliPaymentService, EdfaliPaymentService>();

        services.AddScoped<PayPalPaymentService>();
        services.AddScoped<StripePaymentService>();

        services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();

        services.Configure<TdspOptions>(configuration.GetSection("Payments:Tdsp"));
        services.AddHttpClient<TdspPaymentService>();


        services.AddScoped<IPaymentServiceFactory, PaymentServiceFactory>();

        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));


        

        //File service 
        services.AddScoped<IFileService, FileService>();

        // HttpContextAccessor (required for UserContext)
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();

        // DateTime Provider
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        // DbContext + UnitOfWork
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        // SQL Connection Factory
        services.AddSingleton<ISqlConnectionFactory, SqlConnectionFactory>();

        // Bind OutboxOptions from configuration
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        // Add Quartz and the Outbox Job
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            var jobKey = new JobKey("ProcessOutboxMessagesJob");

            q.AddJob<ProcessOutboxMessagesJob>(opts => opts.WithIdentity(jobKey));

            q.AddTrigger(opts => opts
                .ForJob(jobKey)
                .WithIdentity("ProcessOutboxMessagesJob-trigger")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(10) // Optional: can be read from config
                    .RepeatForever()));
        });

        services.AddQuartzHostedService();

        // Repositories
        services.AddScoped<IAdminRepository, AdminRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IBusinessOwnerRepository, BusinessOwnerRepository>();
        services.AddScoped<IWalletRepository, WalletRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<ISavedPropertyRepository, SavedPropertyRepository>();
        services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepository>();



        // Property Repositories
        services.AddScoped<IPropertyRepository<Apartment>, PropertyRepository<Apartment>>();
        services.AddScoped<IPropertyRepository<Chalet>, PropertyRepository<Chalet>>();
        services.AddScoped<IPropertyRepository<HotelRoom>, PropertyRepository<HotelRoom>>();
        services.AddScoped<IPropertyRepository<HotelApartment>, PropertyRepository<HotelApartment>>();
        services.AddScoped<IPropertyRepository<EventHallLarge>, PropertyRepository<EventHallLarge>>();
        services.AddScoped<IPropertyRepository<EventHallSmall>, PropertyRepository<EventHallSmall>>();
        services.AddScoped<IPropertyRepository<MeetingRoom>, PropertyRepository<MeetingRoom>>();
        services.AddScoped<IPropertyRepository<Resort>, PropertyRepository<Resort>>();
        services.AddScoped<IPropertyRepository<RestHouse>, PropertyRepository<RestHouse>>();
        services.AddScoped<IPropertyRepository<VillaEvent>, PropertyRepository<VillaEvent>>();


        services.AddScoped<IPropertyRepository<Property>, PropertyRepository<Property>>();

        services.AddScoped(typeof(IPropertyRepository), provider =>
        {
            var context = provider.GetRequiredService<ApplicationDbContext>();
            return new PropertyRepository<Property>(context); // or custom concrete class
        });

        // Authentication
        services.AddScoped<IJwtService, JwtService>();

        services.AddScoped<IEmailService, EmailService>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            NameClaimType = JwtRegisteredClaimNames.Sub, // 👈 This maps User.Identity.Name to sub
            ValidateIssuer = true,
            ValidIssuer = "ihjezly-api",
            ValidateAudience = true,
            ValidAudience = "ihjezly-api",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("super_secure_secret_key_which_is_long_enough")),
            ValidateLifetime = true
        };
    });

        services.Configure<MasaratOptions>(configuration.GetSection("Masarat"));

        services.AddHttpClient<MasaratPaymentService>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<MasaratOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
                var cert = new X509Certificate2(
                    @"C:\Users\chemo\OneDrive\Documents\Work\Libya\Backend\src\Ihjezly.Api\Certificates\masarat-client.pfx",
                    "12345"
                );


                handler.ClientCertificates.Add(cert);
                handler.SslProtocols = SslProtocols.Tls12;
                return handler;
            });


        return services;
    }
}
