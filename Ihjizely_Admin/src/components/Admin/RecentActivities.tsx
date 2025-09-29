import { Card, CardContent, CardTitle } from "@/components/ui/card";
import roomPlaceholder from '../../assets/hotel room with beachfront view.jpg';
import { useEffect, useState } from "react";
import { reservationService, Booking } from "../../API/ReservationService";
import { formatDistanceToNow } from 'date-fns';
import { ar } from 'date-fns/locale';
import { unitsService } from "../../API/UnitsService";
import { Link } from "react-router-dom";
import { Button } from "../ui/button";
import axios from 'axios';
import { authService } from "../../API/auth";
import { useDarkMode } from '../DarkModeContext';
import { cn } from "@/lib/utils";

// Loading Spinner Component
const LoadingSpinner = ({ isDarkMode }: { isDarkMode: boolean }) => (
  <div className="flex justify-center items-center h-64">
    <div className={cn(
      "animate-spin rounded-full h-12 w-12 border-t-2 border-b-2",
      isDarkMode ? "border-purple-400" : "border-purple-500"
    )}></div>
  </div>
);

// Error Message Component
const ErrorMessage = ({ message, isDarkMode }: { message: string, isDarkMode: boolean }) => (
  <div className={cn(
    "border px-4 py-3 rounded relative",
    isDarkMode 
      ? "bg-red-900/20 border-red-700 text-red-300" 
      : "bg-red-100 border-red-400 text-red-700"
  )} role="alert">
    <strong className="font-bold">Error: </strong>
    <span className="block sm:inline">{message}</span>
  </div>
);

export default function RecentActivities() {
  const { isDarkMode } = useDarkMode();
  const [recentBookings, setRecentBookings] = useState<Booking[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [stats, setStats] = useState({
    availableRooms: 0,
    availableApartments: 0,
    availableChalets: 0,
    availableHotelApartments: 0,
    availableResorts: 0,
    availableRestHouses: 0,
    availableEventHallsSmall: 0,
    availableEventHallsLarge: 0,
    availableMeetingRooms: 0
  });
  const [showAllStats, setShowAllStats] = useState(false);

  useEffect(() => {
    const fetchData = async () => {
      try {
        const token = authService.getAuthToken();
        if (!token) throw new Error('No authentication token found');

        // Fetch all confirmed bookings first
        const allBookings = await reservationService.getAllBookings();
        const confirmedBookings = allBookings.filter(booking => booking.status === 'Confirmed');
        const bookedPropertyIds = confirmedBookings.map(booking => booking.propertyId);

        // Fetch properties by type and filter out booked ones
        const fetchAvailableProperties = async (type: string) => {
          try {
            const response = await axios.get(`${import.meta.env.VITE_API_URL}/AllProperties/by-type/${type}`, {
              headers: {
                'Authorization': `Bearer ${token}`,
                'accept': '*/*'
              }
            });
            const properties = response.data;
            return properties.filter((property: any) => !bookedPropertyIds.includes(property.id));
          } catch (error) {
            console.error(`Error fetching ${type} properties:`, error);
            return [];
          }
        };

        const [bookings, rooms, apartments, chalets, hotelApartments, resorts, restHouses, 
               smallHalls, largeHalls, meetingRooms] = await Promise.all([
          reservationService.getRecentBookings(),
          fetchAvailableProperties('HotelRoom'),
          fetchAvailableProperties('Apartment'),
          fetchAvailableProperties('Chalet'),
          fetchAvailableProperties('HotelApartment'),
          fetchAvailableProperties('Resort'),
          fetchAvailableProperties('RestHouse'),
          fetchAvailableProperties('EventHallSmall'),
          fetchAvailableProperties('EventHallLarge'),
          fetchAvailableProperties('Meeting Room')
        ]);

        setRecentBookings(bookings);
        setStats({
          availableRooms: rooms.length,
          availableApartments: apartments.length,
          availableChalets: chalets.length,
          availableHotelApartments: hotelApartments.length,
          availableResorts: resorts.length,
          availableRestHouses: restHouses.length,
          availableEventHallsSmall: smallHalls.length,
          availableEventHallsLarge: largeHalls.length,
          availableMeetingRooms: meetingRooms.length
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unknown error occurred');
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  const formatTimeAgo = (dateString: string) => {
    return formatDistanceToNow(new Date(dateString), {
      addSuffix: true,
      locale: ar
    });
  };

  const getPropertyImage = (booking: Booking) => {
    return booking.propertyDetails?.images?.[0]?.url || roomPlaceholder;
  };

  const getRoomInfo = (booking: Booking) => {
    if (!booking.propertyDetails) return '';
    
    const { type, subtype, roomNumber } = booking.propertyDetails;
    
    if (type === 'Residence') {
      if (subtype === 'HotelRoom' && roomNumber) {
        return `-غرفة ${roomNumber}`;
      }
      if (subtype === 'Apartment' && roomNumber) {
        return `-شقة ${roomNumber}`;
      }
      return `-${unitsService.getSubtypeLabel(subtype || '')}`;
    }
    
    return `-${unitsService.getSubtypeLabel(subtype || '')}`;
  };

  const statsEntries = [
    { key: 'availableRooms', label: 'غرف فندقية متاحة' },
    { key: 'availableApartments', label: 'شقق سكنية متاحة' },
    { key: 'availableChalets', label: 'شاليهات متاحة' },
    { key: 'availableHotelApartments', label: 'شقق فندقية متاحة' },
    { key: 'availableResorts', label: 'منتجعات متاحة' },
    { key: 'availableRestHouses', label: 'إستراحات متاحة' },
    { key: 'availableEventHallsSmall', label: 'قاعات أحداث صغيرة متاحة' },
    { key: 'availableEventHallsLarge', label: 'قاعات أحداث كبيرة متاحة' },
    { key: 'availableMeetingRooms', label: ' كافيهات متاحة' }
  ];

  const visibleStats = showAllStats ? statsEntries : statsEntries.slice(0, 4);

  if (loading) {
    return (
      <div className={cn(
        "space-y-6 px-5 pt-2 h-full transition-colors duration-300",
        isDarkMode ? "bg-gray-900" : "bg-white"
      )}>
        <h2 className={cn(
          "text-xl font-bold text-left mb-4 mt-2 transition-colors duration-300",
          isDarkMode ? "text-white" : "text-gray-900"
        )}>
          أحدث الإحصائيات
        </h2>
        <LoadingSpinner isDarkMode={isDarkMode} />
      </div>
    );
  }

  if (error) {
    return (
      <div className={cn(
        "space-y-6 px-5 pt-2 h-full transition-colors duration-300",
        isDarkMode ? "bg-gray-900" : "bg-white"
      )}>
        <h2 className={cn(
          "text-xl font-bold text-left mb-4 mt-2 transition-colors duration-300",
          isDarkMode ? "text-white" : "text-gray-900"
        )}>
          أحدث الإحصائيات
        </h2>
        <ErrorMessage message={error} isDarkMode={isDarkMode} />
      </div>
    );
  }

  return (
    <div className={cn(
      "space-y-6 px-5 pt-2 h-full transition-colors duration-300",
      isDarkMode ? "bg-gray-900" : "bg-white"
    )}>
      <h2 className={cn(
        "text-xl font-bold text-left mb-4 mt-2 transition-colors duration-300",
        isDarkMode ? "text-white" : "text-gray-900"
      )}>
        أحدث الإحصائيات
      </h2>
      
      <div className="flex flex-col lg:flex-row justify-between items-center gap-6 w-full">
        {/* Recent Bookings Section */}
        <div className="cards flex flex-col w-full lg:w-1/2 gap-4">
          {recentBookings.map((booking) => (
            <Card key={booking.id} className={cn(
              "flex flex-col sm:flex-row items-center w-full sm:w-[35rem] p-0 transition-all duration-300",
              "hover:shadow-lg hover:-translate-y-1",
              isDarkMode 
                ? "bg-gray-800 border-gray-700 text-gray-200 hover:border-purple-500" 
                : "bg-white border-gray-200 text-[#404B51] hover:border-purple-400"
            )}>
              <div className="flex flex-row items-center w-full sm:w-auto">
                <img 
                  src={getPropertyImage(booking)} 
                  className={cn(
                    "rounded-t-2xl sm:rounded-l-2xl sm:rounded-tr-none w-full sm:w-52 h-36 object-cover transition-all duration-300",
                    isDarkMode ? "brightness-90" : "brightness-100"
                  )} 
                  alt="Property" 
                  onError={(e) => {
                    (e.target as HTMLImageElement).src = roomPlaceholder;
                  }}
                />
              </div>
              <div className="flex flex-col justify-around gap-2 items-start p-0 w-full">
                <CardTitle className={cn(
                  "px-4 pt-2 transition-colors duration-300",
                  isDarkMode ? "text-white" : "text-gray-900"
                )}>
                  {booking.propertyDetails?.title || 'فندق '}
                </CardTitle>
                <CardContent className="flex flex-col sm:flex-row justify-between items-center p-0 px-4 pb-2 w-full gap-4">
                  <span className={cn(
                    "text-[15px] transition-colors duration-300",
                    isDarkMode ? "text-gray-400" : "text-[#737373]"
                  )}>
                    {booking.name}
                  </span>
                  <span className={cn(
                    "transition-colors duration-300",
                    isDarkMode ? "text-gray-300" : "text-gray-700"
                  )}>
                    {getRoomInfo(booking)}
                  </span>
                  <span className={cn(
                    "text-[12px] whitespace-nowrap transition-colors duration-300",
                    isDarkMode ? "text-gray-500" : "text-gray-600"
                  )}>
                    {formatTimeAgo(booking.reservedAt)}
                  </span>
                </CardContent>
              </div>
            </Card>
          ))}
          
          <Link to={'/Admin/reservations'} className="w-full flex justify-center">
            <Button className={cn(
              "cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode 
                ? "bg-purple-600 hover:bg-purple-700 text-white" 
                : "bg-[#AD46FF] hover:bg-purple-600 text-white"
            )}>
              راجع جميع الحجوزات
            </Button>
          </Link>
        </div>

        {/* Statistics Section */}
        <div className="w-full lg:w-1/2 px-24 pr-0">
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            {visibleStats.map((stat) => (
              <Card 
                key={stat.key} 
                className={cn(
                  "flex flex-col justify-center items-center transition-all duration-500 ease-in-out rounded-2xl p-4 sm:p-6 text-base text-center h-30 w-full",
                  "hover:-translate-y-2 hover:shadow-xl cursor-pointer",
                  isDarkMode 
                    ? "bg-gray-800 border-gray-700 text-gray-200 hover:bg-purple-600 hover:text-white hover:border-purple-500" 
                    : "bg-white border-gray-200 text-gray-900 hover:bg-[#4facfe] hover:text-white hover:border-blue-400"
                )}
              >
                <CardTitle className={cn(
                  "text-sm sm:text-lg font-semibold mb-2 transition-colors duration-300",
                  isDarkMode ? "text-gray-200" : "text-gray-900"
                )}>
                  {stat.label}
                </CardTitle>
                <h1 className={cn(
                  "text-lg sm:text-xl font-bold transition-colors duration-300",
                  isDarkMode ? "text-white" : "text-gray-900"
                )}>
                  {stats[stat.key as keyof typeof stats]}
                </h1>
              </Card>
            ))}
          </div>
          
          {statsEntries.length > 4 && (
            <div className="flex justify-center mt-4">
              <Button 
                className={cn(
                  "cursor-pointer transition-all duration-300 hover:scale-105",
                  isDarkMode 
                    ? "bg-purple-600 hover:bg-purple-700 text-white" 
                    : "bg-[#4facfe] hover:bg-[#3a9bed] text-white"
                )}
                onClick={() => setShowAllStats(!showAllStats)}
              >
                {showAllStats ? 'عرض أقل' : 'عرض المزيد'}
              </Button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}