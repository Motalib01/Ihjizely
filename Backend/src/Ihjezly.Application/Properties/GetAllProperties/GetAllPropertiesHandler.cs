using Ihjezly.Application.Abstractions.Messaging;
using Ihjezly.Application.Properties.DTO;
using Ihjezly.Domain.Abstractions;
using Ihjezly.Domain.Properties;
using Ihjezly.Domain.Properties.Repositories;
using Ihjezly.Domain.Users.Repositories;

namespace Ihjezly.Application.Properties.GetAllProperties;

internal sealed class GetAllPropertiesHandler : IQueryHandler<GetAllPropertiesQuery, List<PropertyDto>>
{
    private readonly IPropertyRepository<HotelRoom> _hotelRoomRepo;
    private readonly IPropertyRepository<HotelApartment> _hotelApartmentRepo;
    private readonly IPropertyRepository<Apartment> _apartmentRepo;
    private readonly IPropertyRepository<Chalet> _chaletRepo;
    private readonly IPropertyRepository<Resort> _resortRepo;
    private readonly IPropertyRepository<RestHouse> _restHouseRepo;
    private readonly IPropertyRepository<EventHallSmall> _eventHallSmallRepo;
    private readonly IPropertyRepository<EventHallLarge> _eventHallLargeRepo;
    private readonly IPropertyRepository<VillaEvent> _villaEventRepo;
    private readonly IPropertyRepository<MeetingRoom> _meetingRoomRepo;
    private readonly IUserRepository _userRepository;

    public GetAllPropertiesHandler(
        IPropertyRepository<HotelRoom> hotelRoomRepo,
        IPropertyRepository<HotelApartment> hotelApartmentRepo,
        IPropertyRepository<Apartment> apartmentRepo,
        IPropertyRepository<Chalet> chaletRepo,
        IPropertyRepository<Resort> resortRepo,
        IPropertyRepository<RestHouse> restHouseRepo,
        IPropertyRepository<EventHallSmall> eventHallSmallRepo,
        IPropertyRepository<EventHallLarge> eventHallLargeRepo,
        IPropertyRepository<VillaEvent> villaEventRepo,
        IPropertyRepository<MeetingRoom> meetingRoomRepo,
        IUserRepository userRepository)
    {
        _hotelRoomRepo = hotelRoomRepo;
        _hotelApartmentRepo = hotelApartmentRepo;
        _apartmentRepo = apartmentRepo;
        _chaletRepo = chaletRepo;
        _resortRepo = resortRepo;
        _restHouseRepo = restHouseRepo;
        _eventHallSmallRepo = eventHallSmallRepo;
        _eventHallLargeRepo = eventHallLargeRepo;
        _villaEventRepo = villaEventRepo;
        _meetingRoomRepo = meetingRoomRepo;
        _userRepository = userRepository;
    }

    public async Task<Result<List<PropertyDto>>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
    {
        var allProperties = new List<Property>();

        allProperties.AddRange(await _hotelRoomRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _hotelApartmentRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _apartmentRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _chaletRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _resortRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _restHouseRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _eventHallSmallRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _eventHallLargeRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _villaEventRepo.GetAllAsync(cancellationToken));
        allProperties.AddRange(await _meetingRoomRepo.GetAllAsync(cancellationToken));

        var allDtos = new List<PropertyDto>(allProperties.Count);

        foreach (var property in allProperties)
        {
            var owner = await _userRepository.GetByIdAsync(property.BusinessOwnerId, cancellationToken);
            var ownerFullName = owner?.FullName ?? "Unknown";

            allDtos.Add(property.ToDto(ownerFullName));
        }

        return Result.Success(allDtos);
    }
}
