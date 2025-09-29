import React, { Suspense, useState, useEffect } from 'react';
import { UserRow } from '../data-table';
import { usersService } from '@/API/UsersService';
import adduserIcon from '../../assets/add_user.svg';
import { X, Filter, User, Calendar, Ban } from 'lucide-react';
import { Button } from '../ui/button';
import { Dialog, DialogContent, DialogOverlay, DialogPortal } from '@radix-ui/react-dialog';
import { toast } from 'sonner';
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

const UserTable = React.lazy(() => import('../data-table'));

// Helper function to check if date is within range
const isWithinDateRange = (dateString: string, range: string) => {
  const date = new Date(dateString);
  const now = new Date();
  
  switch (range) {
    case 'today':
      return date.toDateString() === now.toDateString();
    case 'week':
      const weekAgo = new Date(now);
      weekAgo.setDate(now.getDate() - 7);
      return date >= weekAgo;
    case 'month':
      const monthAgo = new Date(now);
      monthAgo.setMonth(now.getMonth() - 1);
      return date >= monthAgo;
    case 'year':
      const yearAgo = new Date(now);
      yearAgo.setFullYear(now.getFullYear() - 1);
      return date >= yearAgo;
    default:
      return true;
  }
};

// Loading Component
const LoadingSpinner = ({ isDarkMode }: { isDarkMode: boolean }) => (
  <div className={cn(
    "p-6 flex items-center justify-center h-64 transition-colors duration-300",
    isDarkMode ? "text-gray-300" : "text-gray-600"
  )}>
    <div className={cn(
      "animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 mr-3",
      isDarkMode ? "border-purple-400" : "border-purple-500"
    )}></div>
    جاري تحميل البيانات...
  </div>
);

// Error Component
const ErrorMessage = ({ error, isDarkMode }: { error: string, isDarkMode: boolean }) => (
  <div className={cn(
    "p-6 flex items-center justify-center h-64 transition-colors duration-300",
    isDarkMode ? "text-red-300" : "text-red-500"
  )}>
    {error}
  </div>
);

export default function Users() {
  const { isDarkMode } = useDarkMode();
  const [users, setUsers] = useState<UserRow[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isAddUserModalOpen, setIsAddUserModalOpen] = useState(false);

  // Filter states
  const [filters, setFilters] = useState({
    role: '',
    status: '',
    dateRange: '',
    emailVerified: ''
  });

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        setLoading(true);
        const data = await usersService.getAllUsers();
        setUsers(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch users');
        toast.error('Failed to load users');
      } finally {
        setLoading(false);
      }
    };

    fetchUsers();
  }, []);

  const filteredUsers = users.filter(user => {
    const searchLower = searchQuery.toLowerCase();
    const matchesSearch = 
      user.name.toLowerCase().includes(searchLower) ||
      user.username.toLowerCase().includes(searchLower) ||
      user.role.toLowerCase().includes(searchLower);

    const matchesRole = !filters.role || user.role === filters.role;
    const matchesStatus = !filters.status || 
      (filters.status === 'active' && !user.isBlocked) ||
      (filters.status === 'blocked' && user.isBlocked);

    const matchesDateRange = !filters.dateRange || isWithinDateRange(user.date, filters.dateRange);

    return matchesSearch && matchesRole && matchesStatus && matchesDateRange;
  });

  const handleFilterChange = (filterType: string, value: string) => {
    setFilters(prev => ({
      ...prev,
      [filterType]: value
    }));
  };

  const clearFilters = () => {
    setFilters({
      role: '',
      status: '',
      dateRange: '',
      emailVerified: ''
    });
  };

  if (loading) {
    return <LoadingSpinner isDarkMode={isDarkMode} />;
  }

  if (error) {
    return <ErrorMessage error={error} isDarkMode={isDarkMode} />;
  }

  return (
    <div className={cn(
      "p-6 min-h-screen transition-colors duration-300",
      isDarkMode ? "bg-gray-900 text-white" : "bg-white text-gray-900"
    )}>
      {/* Add User Modal */}
      <Dialog open={isAddUserModalOpen} onOpenChange={setIsAddUserModalOpen}>
        <DialogPortal>
          <DialogOverlay className={cn(
            "fixed inset-0 backdrop-blur-sm z-[99999] w-full transition-colors duration-300",
            isDarkMode ? "bg-black/60" : "bg-black/50"
          )} />
          <DialogContent className={cn(
            "fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-[90vw] max-w-[450px] max-h-[85vh] p-6 rounded-xl shadow-lg z-[99999] focus:outline-none transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border border-gray-700" : "bg-white border border-gray-200"
          )}>
            {/* ... existing modal content ... */}
          </DialogContent>
        </DialogPortal>
      </Dialog>

      {/* Main Content */}
      <div className="flex flex-col md:flex-col justify-between md:items-center gap-4 mb-6">
        <div className="flex items-center gap-2 justify-between w-full">
          <div>           
            <h1 className={cn(
              "text-2xl font-bold transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-800"
            )}>
              المستخدمين
            </h1>
            <p className={cn(
              "mt-1 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              إدارة جميع المستخدمين في النظام
            </p>
          </div>
          <Button 
            className={cn(
              "px-4 py-2 rounded-lg flex items-center justify-between gap-2 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode 
                ? "bg-purple-600 hover:bg-purple-700 text-white" 
                : "bg-purple-600 hover:bg-purple-700 text-white"
            )}
            onClick={() => setIsAddUserModalOpen(true)}
          >
            <span>أضف جديد</span>
            <img 
              src={adduserIcon} 
              alt="Add User" 
              className={cn(
                "h-5 w-5 transition-all duration-300",
                isDarkMode ? "filter brightness-0 invert" : ""
              )} 
            />
          </Button>
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
                  ? "bg-gray-800 border-gray-600 text-white placeholder-gray-400 focus:ring-purple-400 focus:border-purple-400"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:ring-purple-500 focus:border-purple-500"
              )}
            />
            <svg 
              xmlns="http://www.w3.org/2000/svg" 
              className={cn(
                "absolute left-3 top-2.5 h-5 w-5 transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-400"
              )} 
              fill="none" 
              viewBox="0 0 24 24" 
              stroke="currentColor"
            >
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
          </div>

          <button
            onClick={() => setIsFilterOpen(!isFilterOpen)}
            className={cn(
              "px-4 py-2 rounded-lg flex items-center gap-1 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode
                ? "bg-purple-600/20 text-purple-400 hover:bg-purple-600/30"
                : "bg-purple-100 text-purple-600 hover:bg-purple-200"
            )}
          >
            <Filter size={18} />
            فلتر
          </button>
        </div>
      </div>

      {isFilterOpen && (
        <div className={cn(
          "w-full md:w-80 shadow-lg rounded-lg p-4 mb-4 border float-right transition-colors duration-300",
          isDarkMode 
            ? "bg-gray-800 border-gray-700 text-white" 
            : "bg-white border-gray-200 text-gray-900"
        )}>
          <div className="space-y-4 flex flex-col items-end w-full">
            <div className="flex justify-between items-center w-full mb-2">
              <h3 className={cn(
                "font-semibold text-right flex items-center gap-2 transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                <Filter size={18} />
                فلترة المستخدمين
              </h3>
              <button 
                onClick={() => setIsFilterOpen(false)} 
                className={cn(
                  "p-1 rounded-full transition-colors duration-300",
                  isDarkMode 
                    ? "text-gray-400 hover:bg-gray-700 hover:text-white" 
                    : "text-gray-500 hover:bg-gray-100"
                )}
              >
                <X size={18} />
              </button>
            </div>
            
            {/* Role Filter */}
            <div className="w-full">
              <label className={cn(
                "block text-sm font-medium text-right mb-1 flex items-center justify-end gap-2 transition-colors duration-300",
                isDarkMode ? "text-gray-300" : "text-gray-700"
              )}>
                <User size={16} />
                الدور
              </label>
              <div className="relative">
                <select 
                  className={cn(
                    "w-full border rounded-lg px-3 py-2 text-right appearance-none focus:ring-2 focus:border-transparent transition-colors duration-300",
                    isDarkMode
                      ? "bg-gray-700 border-gray-600 text-white focus:ring-purple-400"
                      : "bg-white border-gray-300 text-gray-900 focus:ring-purple-500"
                  )}
                  value={filters.role}
                  onChange={(e) => handleFilterChange('role', e.target.value)}
                >
                  <option value="">جميع الأدوار</option>
                  <option value="Client">عميل</option>
                  <option value="BusinessOwner">صاحب عمل</option>
                </select>
                <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center px-2">
                  <svg className={cn(
                    "h-4 w-4 transition-colors duration-300",
                    isDarkMode ? "text-gray-400" : "text-gray-700"
                  )} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                  </svg>
                </div>
              </div>
            </div>

            {/* Registration Date Filter */}
            <div className="w-full">
              <label className={cn(
                "block text-sm font-medium text-right mb-1 flex items-center justify-end gap-2 transition-colors duration-300",
                isDarkMode ? "text-gray-300" : "text-gray-700"
              )}>
                <Calendar size={16} />
                تاريخ التسجيل
              </label>
              <div className="relative">
                <select 
                  className={cn(
                    "w-full border rounded-lg px-3 py-2 text-right appearance-none focus:ring-2 focus:border-transparent transition-colors duration-300",
                    isDarkMode
                      ? "bg-gray-700 border-gray-600 text-white focus:ring-purple-400"
                      : "bg-white border-gray-300 text-gray-900 focus:ring-purple-500"
                  )}
                  value={filters.dateRange}
                  onChange={(e) => handleFilterChange('dateRange', e.target.value)}
                >
                  <option value="">جميع التواريخ</option>
                  <option value="today">اليوم</option>
                  <option value="week">هذا الأسبوع</option>
                  <option value="month">هذا الشهر</option>
                  <option value="year">هذه السنة</option>
                </select>
                <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center px-2">
                  <svg className={cn(
                    "h-4 w-4 transition-colors duration-300",
                    isDarkMode ? "text-gray-400" : "text-gray-700"
                  )} xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                  </svg>
                </div>
              </div>
            </div>

            {/* Clear Filters Button */}
            <button
              className={cn(
                "w-full px-3 py-2 rounded-lg mt-2 text-right flex items-center justify-center gap-2 transition-all duration-300 hover:scale-105",
                isDarkMode
                  ? "bg-gray-700 text-gray-300 hover:bg-gray-600 hover:text-white"
                  : "bg-gray-100 text-gray-700 hover:bg-gray-200"
              )}
              onClick={clearFilters}
            >
              <Ban size={16} />
              مسح الفلاتر
            </button>
          </div>
        </div>
      )}
      
      <Suspense fallback={
        <div className={cn(
          "flex items-center justify-center h-64 transition-colors duration-300",
          isDarkMode ? "text-gray-300" : "text-gray-600"
        )}>
          <div className={cn(
            "animate-spin rounded-full h-8 w-8 border-t-2 border-b-2 mr-3",
            isDarkMode ? "border-purple-400" : "border-purple-500"
          )}></div>
          جاري تحميل الجدول...
        </div>
      }>
        <UserTable data={filteredUsers} />
      </Suspense>
    </div>
  );
}