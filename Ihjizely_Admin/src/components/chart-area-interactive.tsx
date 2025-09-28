import { useState, useEffect } from "react";
import {
  AreaChart, XAxis, YAxis, CartesianGrid, Tooltip, Area, ResponsiveContainer
} from "recharts";
import { authService } from '@/API/auth';
import { fetchStatistics } from '@/API/Statistics';
import { useDarkMode } from './DarkModeContext';
import { cn } from "@/lib/utils";

// Assets
import userclient from '../assets/contacts_product_53.98dp_2196F3_FILL0_wght400_GRAD0_opsz48.png';
import userowner from '../assets/contacts_product_53.98dp_88417A_FILL0_wght400_GRAD0_opsz48.png';
import '../index.css';

// Mock data for charts
const chartData = [
  { name: "Sun", clients: 30, businessOwners: 200 },
  { name: "Mon", clients: 10, businessOwners: 50 },
  { name: "Tue", clients: 100, businessOwners: 40 },
  { name: "Wed", clients: 60, businessOwners: 250 },
  { name: "Thu", clients: 250, businessOwners: 30 },
  { name: "Fri", clients: 80, businessOwners: 100 },
  { name: "Sat", clients: 90, businessOwners: 80 },
];

// Custom Tooltip Component with Dark Mode
const CustomTooltip = ({ active, payload, label, isDarkMode }: any) => {
  if (active && payload && payload.length) {
    return (
      <div className={cn(
        "p-3 rounded-lg shadow-lg border backdrop-blur-sm",
        isDarkMode 
          ? "bg-gray-800 border-gray-700 text-white" 
          : "bg-white border-gray-200 text-gray-900"
      )}>
        <p className={cn(
          "font-medium mb-2",
          isDarkMode ? "text-gray-200" : "text-gray-700"
        )}>
          {label}
        </p>
        {payload.map((entry: any, index: number) => (
          <p key={index} className={cn(
            "text-sm",
            isDarkMode ? "text-gray-300" : "text-gray-600"
          )} style={{ color: entry.color }}>
            {entry.name}: <span className="font-medium">{entry.value.toLocaleString()}</span>
          </p>
        ))}
      </div>
    );
  }
  return null;
};

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

export function ChartAreaInteractive() {
  const { isDarkMode } = useDarkMode();
  const [userStats, setUserStats] = useState({
    totalClients: 0,
    totalBusinessOwners: 0,
    totalUsers: 0
  });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  // Calculate percentages for the chart based on real stats
  const normalizedChartData = chartData.map(day => ({
    ...day,
    clients: Math.round((day.clients / 820) * userStats.totalClients),
    businessOwners: Math.round((day.businessOwners / 750) * userStats.totalBusinessOwners)
  }));

  useEffect(() => {
    const fetchData = async () => {
      try {
        const token = authService.getAuthToken();
        if (!token) {
          throw new Error('No authentication token found');
        }

        const stats = await fetchStatistics(token);
        
        setUserStats({
          totalClients: stats.clients,
          totalBusinessOwners: stats.businessOwners,
          totalUsers: stats.totalUsers
        });
      } catch (err) {
        setError(err instanceof Error ? err.message : 'An unknown error occurred');
        console.error('Error fetching data:', err);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) {
    return <LoadingSpinner isDarkMode={isDarkMode} />;
  }

  if (error) {
    return <ErrorMessage message={error} isDarkMode={isDarkMode} />;
  }

  return (
    <div className={cn(
      "grid grid-cols-1 lg:grid-cols-1 gap-6 w-full p-4 transition-colors duration-300",
      isDarkMode ? "bg-gray-900" : "bg-gray-50"
    )}>
      {/* ROW 1: AREA CHART + STATISTICS */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 w-full">
        {/* AREA CHART */}
        <div className={cn(
          "p-6 rounded-lg shadow w-full transition-colors duration-300",
          isDarkMode 
            ? "bg-gray-800 border border-gray-700" 
            : "bg-white border border-gray-200"
        )}>
          <h2 className={cn(
            "text-lg font-semibold mb-1 transition-colors duration-300",
            isDarkMode ? "text-white" : "text-gray-900"
          )}>
            إجمالي المستخدمين
          </h2>
          <p className={cn(
            "text-sm mb-4 transition-colors duration-300",
            isDarkMode ? "text-gray-400" : "text-gray-600"
          )}>
            النسبة الأسبوعية بناءً على الإجمالي: {userStats.totalUsers} مستخدم
          </p>
          <ResponsiveContainer width="100%" height={250}>
            <AreaChart data={normalizedChartData}>
              <CartesianGrid 
                strokeDasharray="3 3" 
                stroke={isDarkMode ? "#4B5563" : "#E5E7EB"} 
              />
              <XAxis 
                dataKey="name" 
                tick={{ 
                  fill: isDarkMode ? '#D1D5DB' : '#4B5563',
                  fontSize: 12 
                }}
                stroke={isDarkMode ? "#6B7280" : "#9CA3AF"}
              />
              <YAxis 
                hide 
                tick={{ 
                  fill: isDarkMode ? '#D1D5DB' : '#4B5563' 
                }}
                stroke={isDarkMode ? "#6B7280" : "#9CA3AF"}
              />
              <Tooltip 
                content={<CustomTooltip isDarkMode={isDarkMode} />}
                formatter={(value, name) => {
                  if (name === 'العملاء') return [value, `العملاء (${userStats.totalClients} إجمالي)`];
                  if (name === 'أصحاب الأعمال') return [value, `أصحاب الأعمال (${userStats.totalBusinessOwners} إجمالي)`];
                  return [value, name];
                }}
              />
              <defs>
                <linearGradient id="clientGradient" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#6366f1" stopOpacity={isDarkMode ? 0.6 : 0.8} />
                  <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                </linearGradient>
                <linearGradient id="ownerGradient" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#8b5cf6" stopOpacity={isDarkMode ? 0.6 : 0.8} />
                  <stop offset="95%" stopColor="#8b5cf6" stopOpacity={0} />
                </linearGradient>
              </defs>
              <Area 
                type="monotone" 
                dataKey="clients" 
                stroke="#6366f1" 
                fill="url(#clientGradient)" 
                name="العملاء" 
                strokeWidth={2}
              />
              <Area 
                type="monotone" 
                dataKey="businessOwners" 
                stroke="#8b5cf6" 
                fill="url(#ownerGradient)" 
                name="أصحاب الأعمال" 
                strokeWidth={2}
              />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        {/* STATISTICS CARDS */}
        <div className="flex flex-col justify-around gap-4">
          {/* Clients Card */}
          <div className={cn(
            "shadow p-4 rounded-lg flex items-center justify-between transition-all duration-300 hover:scale-[1.02]",
            isDarkMode 
              ? "bg-gray-800 border border-gray-700 hover:border-gray-600" 
              : "bg-white border border-gray-200 hover:border-gray-300"
          )}>
            <img 
              src={userclient} 
              className={cn(
                "w-10 h-10 transition-all duration-300",
                isDarkMode ? "filter brightness-125" : ""
              )} 
              alt="العملاء" 
            />
            <div className="flex flex-col text-right font-semibold">
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-gray-200" : "text-gray-700"
              )}>
                إجمالي العملاء
              </span>
              <span className={cn(
                "text-lg transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {userStats.totalClients.toLocaleString()}
              </span>
            </div>
          </div>

          {/* Business Owners Card */}
          <div className={cn(
            "shadow p-4 rounded-lg flex items-center justify-between transition-all duration-300 hover:scale-[1.02]",
            isDarkMode 
              ? "bg-gray-800 border border-gray-700 hover:border-gray-600" 
              : "bg-white border border-gray-200 hover:border-gray-300"
          )}>
            <img 
              src={userowner} 
              className={cn(
                "w-10 h-10 transition-all duration-300",
                isDarkMode ? "filter brightness-125" : ""
              )} 
              alt="أصحاب الأعمال" 
            />
            <div className="flex flex-col text-right font-semibold">
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-gray-200" : "text-gray-700"
              )}>
                إجمالي أصحاب الأعمال
              </span>
              <span className={cn(
                "text-lg transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {userStats.totalBusinessOwners.toLocaleString()}
              </span>
            </div>
          </div>

          {/* Total Users Card */}
          <div className={cn(
            "shadow p-4 rounded-lg flex items-center justify-between transition-all duration-300 hover:scale-[1.02]",
            isDarkMode 
              ? "bg-gray-800 border border-gray-700 hover:border-purple-600" 
              : "bg-white border border-gray-200 hover:border-purple-400"
          )}>
            <div className={cn(
              "w-10 h-10 rounded-full flex items-center justify-center transition-colors duration-300",
              isDarkMode ? "bg-purple-600/20" : "bg-purple-100"
            )}>
              <span className={cn(
                "text-lg font-bold",
                isDarkMode ? "text-purple-400" : "text-purple-600"
              )}>
                ∑
              </span>
            </div>
            <div className="flex flex-col text-right font-semibold">
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-gray-200" : "text-gray-700"
              )}>
                إجمالي المستخدمين
              </span>
              <span className={cn(
                "text-lg transition-colors duration-300",
                isDarkMode ? "text-purple-400" : "text-purple-600"
              )}>
                {userStats.totalUsers.toLocaleString()}
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* ROW 2: Placeholder for additional charts */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-4 w-full">
        {/* You can add more charts here with dark mode support */}
      </div>
    </div>
  );
}