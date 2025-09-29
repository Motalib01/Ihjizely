// components/SectionCards.tsx
import { Card, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import usericon from '../assets/3-Friends.svg';
import propertyicon from '../assets/night_shelter.svg';
import calendar from '../assets/free_cancellation.svg';
import { useStatistics } from '../hooks/useStatistics';
import { useDarkMode } from "./DarkModeContext";
import { cn } from "@/lib/utils";

// Simple inline loading spinner component
const LoadingSpinner = () => {
  const { isDarkMode } = useDarkMode();
  
  return (
    <div className="flex justify-center items-center p-8">
      <div className={cn(
        "animate-spin rounded-full h-8 w-8 border-b-2",
        isDarkMode ? "border-gray-100" : "border-gray-900"
      )}></div>
    </div>
  );
};

// Simple inline error message component
const ErrorMessage = ({ message }: { message: string }) => {
  const { isDarkMode } = useDarkMode();
  
  return (
    <div className={cn(
      "p-4 rounded-md",
      isDarkMode ? "text-red-300 bg-red-900/30" : "text-red-600 bg-red-100"
    )}>
      Error: {message}
    </div>
  );
};

export function SectionCards() {
  const { statistics, loading, error } = useStatistics();

  if (loading) return <LoadingSpinner />;
  if (error) return <ErrorMessage message={error} />;
  if (!statistics) return <ErrorMessage message="No statistics data available" />;

  return (
    <div className="grid grid-cols-1 gap-3 px-4 sm:grid-cols-2 lg:grid-cols-3">
      <StatCard 
        icon={usericon} 
        value={statistics.totalUsers} 
        label="إجمالي المستخدمين" 
        alt="Users"
      />
      
      <StatCard 
        icon={propertyicon} 
        value={statistics.totalProperties} 
        label="مجموع الوحدات" 
        alt="Properties"
      />
      
      <StatCard 
        icon={calendar} 
        value={statistics.reserved} 
        label="محجوز (مؤكد)" 
        alt="Reservations"
      />
    </div>
  );
}

interface StatCardProps {
  icon: string;
  value: number;
  label: string;
  alt: string;
}

const StatCard = ({ icon, value, label, alt }: StatCardProps) => {
  const { isDarkMode } = useDarkMode();
  
  return (
    <Card className={cn(
      "flex flex-row items-center justify-between shadow-xs transition-all duration-300 hover:scale-[1.01] p-4",
      "from-primary/5 to-card",
      isDarkMode 
        ? "bg-gray-800 border-gray-700 text-white hover:bg-gray-750" 
        : "bg-white border-gray-200 text-gray-900 hover:bg-gray-50"
    )}>
      <div className="mr-4">
        <img 
          src={icon} 
          alt={alt} 
          className={cn(
            "w-10 h-10 transition-all duration-300",
            isDarkMode ? "filter invert-[0.8] brightness-125" : ""
          )}
        />
      </div>
      <div className="flex-1">
        <CardHeader className="p-0 text-right">
          <CardTitle className={cn(
            "text-2xl font-semibold tabular-nums @[250px]/card:text-3xl",
            isDarkMode ? "text-white" : "text-gray-900"
          )}>
            {value.toLocaleString()} {/* Format numbers with commas */}
          </CardTitle>
          <CardDescription className={cn(
            "text-sm font-medium",
            isDarkMode ? "text-gray-300" : "text-gray-600"
          )}>
            {label}
          </CardDescription>
        </CardHeader>
      </div>
    </Card>
  );
};