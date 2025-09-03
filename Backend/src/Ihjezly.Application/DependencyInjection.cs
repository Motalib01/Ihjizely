using FluentValidation;
using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Behaviors;
using Ihjezly.Application.Properties;
using Ihjezly.Application.Properties.DeleteProperty;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Application.Properties.GetAllProperties;
using Ihjezly.Application.Properties.GetAllPropertiesByType;
using Ihjezly.Application.Properties.GetAllPropertiesNonGeneric;
using Ihjezly.Application.Properties.GetPropertiesByStatus;
using Ihjezly.Application.Properties.GetPropertyById;
using Ihjezly.Application.Properties.GetPropertyByIdGeneric;
using Ihjezly.Application.Properties.Halls.GetHallPropertyById;
using Ihjezly.Application.Properties.Residence.UpdateResidence;
using Ihjezly.Application.Properties.UpdateProperty;
using Ihjezly.Application.Wallets.GetWalletByUser;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Booking;
using Ihjezly.Domain.Properties;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // === MediatR Registration ===
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(
                typeof(DependencyInjection).Assembly,
                typeof(UserCreatedDomainEventHandler).Assembly,
                typeof(GetWalletByUserHandler).Assembly
            );
        });

        // === Pipeline Behaviors ===
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // === Validators ===
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        // === Core Services ===
        services.AddTransient<PricingService>();

        // === Property Types List ===
        var propertyHandlers = new (Type property, Type details)[]
        {
            (typeof(Apartment), typeof(ApartmentDetails)),
            (typeof(Chalet), typeof(ChaletDetails)),
            (typeof(HotelRoom), typeof(HotelRoomDetails)),
            (typeof(HotelApartment), typeof(HotelApartmentDetails)),
            (typeof(EventHallLarge), typeof(EventHallLargeDetails)),
            (typeof(EventHallSmall), typeof(EventHallSmallDetails)),
            (typeof(MeetingRoom), typeof(MeetingRoomDetails)),
            (typeof(Resort), typeof(ResortDetails)),
            (typeof(RestHouse), typeof(RestHouseDetails)),
            (typeof(VillaEvent), typeof(VillaEventDetails)),
        };

        foreach (var (propertyType, detailsType) in propertyHandlers)
        {
            // === CreateProperty ===
            var createPropertyHandlerType = typeof(CreatePropertyHandler<,>).MakeGenericType(propertyType, detailsType);
            var createCommandType = typeof(CreatePropertyCommand<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(createCommandType, typeof(Result<Guid>)), createPropertyHandlerType);

            services.AddTransient(createPropertyHandlerType);

            // === UpdateProperty ===
            var updateCommandType = typeof(UpdatePropertyCommand<,>).MakeGenericType(propertyType, detailsType);
            var updateHandlerType = typeof(UpdatePropertyHandler<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(updateCommandType, typeof(Result)), updateHandlerType);

            // === DeleteProperty ===
            var deleteCommandType = typeof(DeletePropertyCommand<,>).MakeGenericType(propertyType, detailsType);
            var deleteHandlerType = typeof(DeletePropertyHandler<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(deleteCommandType, typeof(Result)), deleteHandlerType);

            // === GetPropertyById ===
            var getByIdQueryType = typeof(GetPropertyByIdQuery<,>).MakeGenericType(propertyType, detailsType);
            var getByIdHandlerType = typeof(GetPropertyByIdHandler<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(getByIdQueryType, typeof(Result<PropertyDto?>)), getByIdHandlerType);

            // === GetAllPropertiesByType ===
            var getAllByTypeQuery = typeof(GetAllPropertiesByTypeQuery<,>).MakeGenericType(propertyType, detailsType);
            var getAllByTypeHandler = typeof(GetAllPropertiesByTypeHandler<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(getAllByTypeQuery, typeof(Result<List<PropertyDto>>)), getAllByTypeHandler);

            // === GetAllPropertiesByTypeAndStatus ===
            var getByStatusQuery = typeof(GetAllPropertiesByTypeAndStatusQuery<,>).MakeGenericType(propertyType, detailsType);
            var getByStatusHandler = typeof(GetAllPropertiesByTypeAndStatusHandler<,>).MakeGenericType(propertyType, detailsType);
            services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(getByStatusQuery, typeof(Result<List<PropertyDto>>)), getByStatusHandler);
        }

        // === Register ResidenceProperty and HallProperty specific queries ===
        foreach (var (propertyType, detailsType) in propertyHandlers)
        {
            var isResidence = typeof(ResidenceProperty<>).MakeGenericType(detailsType).IsAssignableFrom(propertyType);
            var isHall = typeof(HallProperty<>).MakeGenericType(detailsType).IsAssignableFrom(propertyType);

            if (isResidence)
            {
                var residenceCommand = typeof(CreateResidencePropertyCommand<,>).MakeGenericType(propertyType, detailsType);
                var residenceHandler = typeof(CreateResidencePropertyHandler<,>).MakeGenericType(propertyType, detailsType);
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(residenceCommand, typeof(Result<Guid>)), residenceHandler);

                // === GetResidencePropertyById ===
                var residenceQuery = typeof(GetResidencePropertyByIdQuery<,>).MakeGenericType(propertyType, detailsType);
                var residenceByIdHandler = typeof(GetResidencePropertyByIdHandler<,>).MakeGenericType(propertyType, detailsType);
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(residenceQuery, typeof(Result<ResidencePropertyDto>)),
            residenceByIdHandler
                );

                // === Register UpdateResidenceCommand ===
                var updateResidenceCommandType = typeof(UpdateResidenceCommand<,>).MakeGenericType(propertyType, detailsType);
                var updateResidenceHandlerType = typeof(UpdateResidenceCommandHandler<,>).MakeGenericType(propertyType, detailsType);

                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(updateResidenceCommandType, typeof(Result<Guid>)),
                    updateResidenceHandlerType
                );

            }

            if (isHall)
            {
                var hallCommand = typeof(CreateHallPropertyCommand<,>).MakeGenericType(propertyType, detailsType);
                var hallHandler = typeof(CreateHallPropertyHandler<,>).MakeGenericType(propertyType, detailsType);
                services.AddTransient(typeof(IRequestHandler<,>).MakeGenericType(hallCommand, typeof(Result<Guid>)), hallHandler);
                // === GetHallPropertyById ===
                var hallByIdHandler = typeof(GetHallPropertyByIdHandler<,>).MakeGenericType(propertyType, detailsType);
                services.AddTransient(
                    typeof(IRequestHandler<GetHallPropertyByIdQuery, Result<HallPropertyDto>>),
                    hallByIdHandler
                );

                // === Register UpdateHallCommand ===
                var updateHallCommandType = typeof(UpdateHallCommand<,>).MakeGenericType(propertyType, detailsType);
                var updateHallHandlerType = typeof(UpdateHallCommandHandler<,>).MakeGenericType(propertyType, detailsType);
                services.AddTransient(
                    typeof(IRequestHandler<,>).MakeGenericType(updateHallCommandType, typeof(Result<Guid>)),
                    updateHallHandlerType
                );
            }
        }


        // === Global Handlers ===
        services.AddScoped<IQueryHandler<GetAllPropertiesQuery, List<PropertyDto>>, GetAllPropertiesHandler>();
        services.AddTransient<IRequestHandler<GetAllPropertiesByStatusQuery, Result<List<PropertyDto>>>, GetAllPropertiesByStatusHandler>();
        services.AddTransient<IRequestHandler<GetAllPropertiesNonGenericQuery, Result<List<PropertyDto>>>, GetAllPropertiesNonGenericHandler>();

        return services;
    }
}
