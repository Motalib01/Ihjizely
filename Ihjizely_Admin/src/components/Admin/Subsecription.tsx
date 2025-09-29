import { useState, useEffect } from "react";
import { DownloadCloudIcon, FilterIcon, MoreVertical, UserIcon, CalendarIcon, TagIcon, PencilIcon } from "lucide-react";
import { SubscriptionRow, SubscriptionTable } from "../data-table";
import { Link } from "react-router-dom";
import { subscriptionsService } from "@/API/SubscriptionsService";
import { toast } from "sonner";
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

export default function Subscription() {
  const { isDarkMode } = useDarkMode();
  const [searchQuery, setSearchQuery] = useState('');
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [selectedFilters, setSelectedFilters] = useState(['businessOwnerId', 'planName', 'startDate']);
  const [apiData, setApiData] = useState<SubscriptionRow[]>([]);
  const [filteredData, setFilteredData] = useState<SubscriptionRow[]>([]);
  const [, setIsLoading] = useState(true);
  
  // Fetch data once on component mount
  useEffect(() => {
    const fetchData = async () => {
      try {
        setIsLoading(true);
        const data = await subscriptionsService.getSubscriptionsWithPlans();
        setApiData(data);
        setFilteredData(data);
      } catch (error) {
        toast.error('Failed to load subscription data');
      } finally {
        setIsLoading(false);
      }
    };
    
    fetchData();
  }, []);

  // Apply filters whenever search query, selected filters, or apiData changes
  useEffect(() => {
    const searchLower = searchQuery.toLowerCase();
    
    const filtered = apiData.filter(item => {
      return selectedFilters.some(filter => {
        switch(filter) {
          case 'businessOwnerId':
            return item.businessOwnerId.toLowerCase().includes(searchLower);
          case 'planName':
            return item.planName.toLowerCase().includes(searchLower);
          case 'startDate':
            return item.startDate.includes(searchQuery);
          default:
            return false;
        }
      });
    });
    
    setFilteredData(filtered);
  }, [searchQuery, selectedFilters, apiData]);

  const toggleFilter = (filter: string) => {
    setSelectedFilters(prev => 
      prev.includes(filter)
        ? prev.filter(f => f !== filter)
        : [...prev, filter]
    );
  }

  const downloadCSV = () => {
    // Create CSV content
    const headers = ['معرف صاحب العمل', 'اسم الخطة', 'تاريخ البدء', 'تاريخ الانتهاء', 'السعر', 'الإعلانات المستخدمة', 'الحالة'];
    const csvContent = [
      headers.join(','),
      ...filteredData.map(item => [
        item.businessOwnerId,
        item.planName,
        item.startDate,
        item.endDate,
        `${item.price.amount} ${item.price.currencyCode}`,
        `${item.usedAds}/${item.maxAds}`,
        item.isActive ? 'نشط' : 'غير نشط'
      ].join(','))
    ].join('\n');
    
    // Create and trigger download
    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', 'subscriptions.csv');
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  return (
    <div className={cn(
      "p-6 min-h-screen transition-colors duration-300",
      isDarkMode ? "bg-gray-900 text-white" : "bg-white text-gray-900"
    )}>
      <div className="flex flex-col md:flex-col justify-between md:items-center gap-4 mb-6">
        <div className="flex items-center gap-2 justify-between w-full">
          <div className="">  
            <h1 className={cn(
              "text-2xl font-bold transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-900"
            )}>
              إدارة اﻟﺎﺷﺘﺮاﻛﺎت
            </h1>
            <p className={cn(
              "mt-1 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              إدارة جميع إﺷﺘﺮاﻛﺎت المستخدمين في النظام
            </p>
          </div>
          <Link to={'/Admin/subscription-plans'}>
            <button className={cn(
              "px-4 py-2 rounded-lg flex items-center justify-between gap-2 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode
                ? "bg-purple-600 hover:bg-purple-700 text-white"
                : "bg-purple-600 hover:bg-purple-700 text-white"
            )}>
              <span>تعديل اشتراك</span>
              <PencilIcon className="h-4 w-4"/>
            </button>
          </Link>
        </div>

        <div className="flex items-end gap-2 w-full md:w-full">
          <div className="relative flex-1 w-full">
            <input
              type="text"
              placeholder="بحث..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
              className={cn(
                "w-full px-4 py-2 border rounded-lg focus:outline-none focus:ring-2 text-right transition-colors duration-300",
                isDarkMode
                  ? "bg-gray-800 border-gray-600 text-white placeholder-gray-400 focus:ring-purple-400"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-purple-500"
              )}
            />
            <svg xmlns="http://www.w3.org/2000/svg" className={cn(
              "absolute left-3 top-2.5 h-5 w-5 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-400"
            )} fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>

          <button
            onClick={downloadCSV}
            className={cn(
              "p-2 flex flex-row gap-2 rounded-lg transition-all duration-300 hover:scale-105 cursor-pointer",
              isDarkMode
                ? "bg-gray-800 text-gray-300 hover:bg-blue-600 hover:text-white"
                : "bg-gray-100 text-gray-700 hover:bg-[#2196F3] hover:text-white"
            )}
            title="تحميل البيانات"
          >
            <span>تحميل</span>
            <DownloadCloudIcon className="h-5 w-5" />
          </button>
          
          <button
            onClick={() => setIsFilterOpen(!isFilterOpen)}
            className={cn(
              "px-4 py-2 rounded-lg flex items-center gap-1 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode
                ? "bg-purple-600/20 text-purple-400 hover:bg-purple-600/30"
                : "bg-purple-100 text-purple-600 hover:bg-purple-200"
            )}
          >
            فلتر
            <FilterIcon className="h-4 w-4" />
          </button>

          <button className={cn(
            "p-2 rounded-lg transition-colors duration-300 hover:scale-105",
            isDarkMode
              ? "bg-gray-800 text-gray-300 hover:bg-gray-700"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          )}>
            <MoreVertical className="h-5 w-5" />
          </button>
        </div>
      </div>

      {isFilterOpen && (
        <div className={cn(
          "w-64 shadow-md rounded-lg p-4 mb-4 border float-right mr-12 transition-colors duration-300",
          isDarkMode 
            ? "bg-gray-800 border-gray-700 text-white" 
            : "bg-white border-gray-200 text-gray-900"
        )}>
          <div className="space-y-2 flex flex-col items-end w-full">
            {/* Owner Filter */}
            <div 
              className={cn(
                "flex flex-row text-right items-center w-full justify-end gap-4 px-3 py-2 rounded-lg cursor-pointer transition-all duration-300 hover:scale-105",
                selectedFilters.includes('businessOwnerId') 
                  ? isDarkMode
                    ? "bg-purple-600/30 text-purple-400 border border-purple-500/50"
                    : "bg-purple-100 text-purple-600 border border-purple-200"
                  : isDarkMode
                    ? "text-gray-400 hover:bg-gray-700 hover:text-gray-300"
                    : "text-[#959595] hover:bg-gray-100 hover:text-gray-700"
              )}
              onClick={() => toggleFilter('businessOwnerId')}
            >
              <span>معرف صاحب العمل</span>
              <UserIcon className="h-5 w-5" />
            </div>

            {/* Subscription Type Filter */}
            <div 
              className={cn(
                "flex flex-row text-right items-center w-full justify-end gap-4 rounded-lg px-3 py-2 cursor-pointer transition-all duration-300 hover:scale-105",
                selectedFilters.includes('planName') 
                  ? isDarkMode
                    ? "bg-purple-600/30 text-purple-400 border border-purple-500/50"
                    : "bg-purple-100 text-purple-600 border border-purple-200"
                  : isDarkMode
                    ? "text-gray-400 hover:bg-gray-700 hover:text-gray-300"
                    : "text-[#959595] hover:bg-gray-100 hover:text-gray-700"
              )}
              onClick={() => toggleFilter('planName')}
            >
              <span>اسم الخطة</span>
              <TagIcon className="h-5 w-5" />
            </div>

            {/* Registration Date Filter */}
            <div 
              className={cn(
                "flex flex-row text-right items-center w-full justify-end gap-4 px-3 py-2 rounded-lg cursor-pointer transition-all duration-300 hover:scale-105",
                selectedFilters.includes('startDate') 
                  ? isDarkMode
                    ? "bg-purple-600/30 text-purple-400 border border-purple-500/50"
                    : "bg-purple-100 text-purple-600 border border-purple-200"
                  : isDarkMode
                    ? "text-gray-400 hover:bg-gray-700 hover:text-gray-300"
                    : "text-[#959595] hover:bg-gray-100 hover:text-gray-700"
              )}
              onClick={() => toggleFilter('startDate')}
            >
              <span>تاريخ البدء</span>
              <CalendarIcon className="h-5 w-5" />
            </div>
          </div>
        </div>
      )}
      
      <div className={cn(
        "transition-colors duration-300",
        isDarkMode ? "bg-gray-800" : "bg-white"
      )}>
        <SubscriptionTable data={filteredData} />
      </div>
    </div>
  );
}