import { useState, useEffect } from "react";
import { toast } from "sonner";
import { WalletRow, WalletTable } from "../data-table";
import { Button } from "../ui/button";
import { Input } from "../ui/input";
import { 
  Dialog, 
  DialogContent,
  DialogDescription,
  DialogOverlay,
  DialogPortal,
  DialogTitle} from '@radix-ui/react-dialog';
import { 
  DropdownMenu, 
  DropdownMenuContent, 
  DropdownMenuItem, 
  DropdownMenuPortal, 
  DropdownMenuTrigger 
} from "../ui/dropdown-menu";
import { Avatar, AvatarFallback } from "../ui/avatar";
import { SearchIcon, DownloadIcon, PlusIcon, UserIcon, X } from "lucide-react";
import { walletsService } from "@/API/walletsService";
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

// Define user type
interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  avatar: string;
  hasWallet: boolean;
}

// Loading Spinner Component
const LoadingSpinner = ({ isDarkMode }: { isDarkMode: boolean }) => (
  <div className="flex justify-center items-center h-32">
    <div className={cn(
      "animate-spin rounded-full h-12 w-12 border-t-2 border-b-2",
      isDarkMode ? "border-purple-400" : "border-purple-500"
    )}></div>
  </div>
);

export default function WalletManagement() {
  const { isDarkMode } = useDarkMode();
  const [searchTerm, setSearchTerm] = useState("");
  const [walletData, setWalletData] = useState<WalletRow[]>([]);
  const [filteredData, setFilteredData] = useState<WalletRow[]>([]);
  const [isDialogOpen, setIsDialogOpen] = useState(false);
  const [users, setUsers] = useState<User[]>([]);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [isDropdownOpen, setIsDropdownOpen] = useState(true);
  const [loading, setLoading] = useState(true);

  // Fetch wallets data
  useEffect(() => {
    const fetchWallets = async () => {
      try {
        setLoading(true);
        const wallets = await walletsService.getAllWallets();
        setWalletData(wallets);
        setFilteredData(wallets);
      } catch (error) {
        toast.error("Failed to fetch wallets");
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    fetchWallets();
  }, []);

  // Filter data based on search term
  useEffect(() => {
    if (searchTerm === "") {
      setFilteredData(walletData);
    } else {
      const filtered = walletData.filter(wallet =>
        wallet.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
        wallet.email.toLowerCase().includes(searchTerm.toLowerCase())
      );
      setFilteredData(filtered);
    }
  }, [searchTerm, walletData]);

  const handleAddWallet = () => {
    setIsDialogOpen(true);
    setSelectedUser(null);
    setIsDropdownOpen(true);
  };

  const handleDownload = () => {
    toast.info("جارٍ تحميل بيانات المحافظ...");
  };

  const handleCreateWallet = async () => {
    if (!selectedUser) {
      toast.error("الرجاء اختيار مستخدم");
      return;
    }
    
    try {
      setLoading(true);
      // Call API to create wallet
      const newWallet = await walletsService.getWalletByUserId(selectedUser.id);
      
      // Update local state
      const walletRow: WalletRow = {
        id: walletData.length + 1,
        walletId: newWallet.walletId,
        name: `${selectedUser.firstName} ${selectedUser.lastName}`,
        balance: `${newWallet.amount} ${newWallet.currency}`,
        registrationDate: new Date().toLocaleDateString('en-GB'),
        email: selectedUser.email
      };
      
      setWalletData(prev => [...prev, walletRow]);
      setFilteredData(prev => [...prev, walletRow]);
      
      // Update user to mark as having a wallet
      setUsers(prevUsers => 
        prevUsers.map(user => 
          user.id === selectedUser.id ? {...user, hasWallet: true} : user
        )
      );
      
      toast.success(`تم إنشاء محفظة لـ ${selectedUser.firstName} ${selectedUser.lastName}`);
      setIsDialogOpen(false);
    } catch (error) {
      toast.error("Failed to create wallet");
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  // Calculate statistics
  const totalBalance = walletData.reduce((sum, wallet) => {
    const amount = parseFloat(wallet.balance.split(' ')[0]);
    return isNaN(amount) ? sum : sum + amount;
  }, 0);

  const averageBalance = walletData.length > 0 
    ? totalBalance / walletData.length 
    : 0;

  return (
    <div className={cn(
      "w-full p-6 min-h-screen transition-colors duration-300",
      isDarkMode ? "bg-gray-900 text-white" : "bg-white text-gray-900"
    )}>
      {/* Wallet Creation Dialog */}
      <Dialog open={isDialogOpen} onOpenChange={setIsDialogOpen}>
        <DialogPortal>
          <DialogOverlay className={cn(
            "fixed inset-0 backdrop-blur-sm z-[99999] transition-colors duration-300",
            isDarkMode ? "bg-black/60" : "bg-black/50"
          )} />
          <DialogContent className={cn(
            "fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-[90vw] max-w-md max-h-[80vh] p-6 rounded-xl shadow-lg z-[100999] focus:outline-none transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border border-gray-700" : "bg-white border border-gray-200"
          )}>
            <div className="absolute left-4 top-4">
              <Button 
                variant="ghost" 
                size="icon"
                onClick={() => setIsDialogOpen(false)}
                className={cn(
                  "transition-colors duration-300",
                  isDarkMode ? "hover:bg-gray-700 text-gray-300" : "hover:bg-gray-100 text-gray-600"
                )}
              >
                <X className="h-4 w-4" />
              </Button>
            </div>
            
            <DialogTitle className={cn(
              "text-2xl font-bold text-right mb-2 transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-900"
            )}>
              إضافة محفظة جديدة
            </DialogTitle>
            <DialogDescription className={cn(
              "text-right mb-6 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              اختر مستخدمًا لإنشاء محفظة جديدة له
            </DialogDescription>

            <div className="mb-6 relative">
              <DropdownMenu open={isDropdownOpen} onOpenChange={setIsDropdownOpen}>
                <DropdownMenuTrigger asChild>
                  <div className={cn(
                    "border rounded-lg p-3 cursor-pointer transition-colors duration-300",
                    isDarkMode 
                      ? "border-gray-600 bg-gray-700 hover:border-gray-500" 
                      : "border-gray-300 bg-white hover:border-gray-400"
                  )}>
                    {selectedUser ? (
                      <div className="flex items-center gap-3">
                        <Avatar>
                          <AvatarFallback className={cn(
                            "transition-colors duration-300",
                            isDarkMode ? "bg-gray-600 text-gray-300" : "bg-gray-100 text-gray-600"
                          )}>
                            {selectedUser.firstName.charAt(0)}{selectedUser.lastName.charAt(0)}
                          </AvatarFallback>
                        </Avatar>
                        <div className="text-right">
                          <p className={cn(
                            "font-medium transition-colors duration-300",
                            isDarkMode ? "text-white" : "text-gray-900"
                          )}>
                            {selectedUser.firstName} {selectedUser.lastName}
                          </p>
                          <p className={cn(
                            "text-sm transition-colors duration-300",
                            isDarkMode ? "text-gray-400" : "text-gray-500"
                          )}>
                            {selectedUser.email}
                          </p>
                        </div>
                      </div>
                    ) : (
                      <p className={cn(
                        "text-right transition-colors duration-300",
                        isDarkMode ? "text-gray-400" : "text-gray-500"
                      )}>
                        اختر مستخدمًا من القائمة
                      </p>
                    )}
                  </div>
                </DropdownMenuTrigger>
                
                <DropdownMenuPortal>
                  <DropdownMenuContent 
                    className={cn(
                      "w-full max-h-60 overflow-y-auto z-[999999] transition-colors duration-300",
                      isDarkMode ? "bg-gray-800 border-gray-700" : "bg-white border-gray-200"
                    )}
                    align="end"
                    style={{ 
                      zIndex: 999999,
                      width: "var(--radix-dropdown-menu-trigger-width)"
                    }}
                  >
                    {users.map(user => (
                      <DropdownMenuItem
                        key={user.id}
                        className={cn(
                          "flex items-center gap-3 p-3 transition-colors duration-300",
                          user.hasWallet 
                            ? "opacity-50 cursor-not-allowed" 
                            : cn(
                                "cursor-pointer",
                                isDarkMode ? "hover:bg-gray-700" : "hover:bg-gray-100"
                              )
                        )}
                        onSelect={(e) => {
                          e.preventDefault();
                          if (!user.hasWallet) {
                            setSelectedUser(user);
                            setIsDropdownOpen(false);
                          }
                        }}
                        disabled={user.hasWallet}
                      >
                        <Avatar>
                          <AvatarFallback className={cn(
                            "transition-colors duration-300",
                            isDarkMode ? "bg-gray-600 text-gray-300" : "bg-gray-100 text-gray-600"
                          )}>
                            {user.firstName.charAt(0)}{user.lastName.charAt(0)}
                          </AvatarFallback>
                        </Avatar>
                        <div className="text-right flex-1">
                          <p className={cn(
                            "font-medium transition-colors duration-300",
                            isDarkMode ? "text-white" : "text-gray-900"
                          )}>
                            {user.firstName} {user.lastName}
                            {user.hasWallet && (
                              <span className={cn(
                                "ml-2 text-xs px-2 py-1 rounded transition-colors duration-300",
                                isDarkMode 
                                  ? "bg-gray-700 text-gray-300" 
                                  : "bg-gray-200 text-gray-700"
                              )}>
                                لديه محفظة
                              </span>
                            )}
                          </p>
                          <p className={cn(
                            "text-sm transition-colors duration-300",
                            isDarkMode ? "text-gray-400" : "text-gray-500"
                          )}>
                            {user.email}
                          </p>
                        </div>
                      </DropdownMenuItem>
                    ))}
                  </DropdownMenuContent>
                </DropdownMenuPortal>
              </DropdownMenu>
            </div>

            <div className="flex justify-end gap-3 pt-4">
              <Button 
                variant="outline"
                onClick={() => setIsDialogOpen(false)}
                className={cn(
                  "transition-colors duration-300",
                  isDarkMode 
                    ? "border-gray-600 text-gray-300 hover:bg-gray-700" 
                    : "border-gray-300 text-gray-700 hover:bg-gray-100"
                )}
              >
                إلغاء
              </Button>
              <Button 
                className={cn(
                  "transition-colors duration-300 hover:scale-105",
                  isDarkMode 
                    ? "bg-purple-600 hover:bg-purple-700 text-white" 
                    : "bg-purple-600 hover:bg-purple-700 text-white"
                )}
                onClick={handleCreateWallet}
                disabled={!selectedUser || loading}
              >
                {loading ? "جاري الإنشاء..." : "إنشاء محفظة"}
              </Button>
            </div>
          </DialogContent>
        </DialogPortal>
      </Dialog>

      {/* Header */}
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
        <div>
          <h1 className={cn(
            "text-2xl font-bold transition-colors duration-300",
            isDarkMode ? "text-white" : "text-gray-800"
          )}>
            إدارة المحافظ
          </h1>
          <p className={cn(
            "mt-1 transition-colors duration-300",
            isDarkMode ? "text-gray-400" : "text-gray-600"
          )}>
            إدارة جميع محافظ المستخدمين في النظام
          </p>
        </div>
        
        <div className="flex flex-col md:flex-row gap-3 w-full md:w-auto">
          <div className="relative w-full md:w-64">
            <SearchIcon className={cn(
              "absolute right-3 top-1/2 transform -translate-y-1/2 h-4 w-4 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-400"
            )} />
            <Input
              type="text"
              placeholder="بحث..."
              className={cn(
                "pr-10 text-right transition-colors duration-300",
                isDarkMode
                  ? "bg-gray-800 border-gray-600 text-white placeholder-gray-400 focus:border-purple-400"
                  : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-purple-500"
              )}
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          
          <div className="flex gap-3">
            <Button 
              variant="outline" 
              className={cn(
                "flex items-center gap-2 transition-all duration-300 hover:scale-105",
                isDarkMode
                  ? "border-gray-600 text-gray-300 hover:bg-gray-700 hover:text-white"
                  : "border-gray-300 text-gray-700 hover:bg-gray-100"
              )}
              onClick={handleDownload}
            >
              <DownloadIcon className="h-4 w-4" />
              <span>تحميل</span>
            </Button>
            
            <Button 
              className={cn(
                "flex items-center gap-2 transition-all duration-300 hover:scale-105",
                isDarkMode 
                  ? "bg-purple-600 hover:bg-purple-700 text-white" 
                  : "bg-purple-600 hover:bg-purple-700 text-white"
              )}
              onClick={handleAddWallet}
            >
              <PlusIcon className="h-4 w-4" />
              <span>إضافة محفظة</span>
            </Button>
          </div>
        </div>
      </div>
      
      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6 mb-8">
        {/* Total Wallets Card */}
        <div className={cn(
          "rounded-xl border p-6 shadow-sm transition-all duration-300 hover:scale-105",
          isDarkMode 
            ? "bg-gray-800 border-gray-700 hover:border-purple-500" 
            : "bg-white border-gray-200 hover:border-purple-400"
        )}>
          <div className="flex justify-between items-center">
            <div>
              <h3 className={cn(
                "text-lg font-medium transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-600"
              )}>
                إجمالي المحافظ
              </h3>
              <p className={cn(
                "text-3xl font-bold mt-2 transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {walletData.length}
              </p>
            </div>
            <div className={cn(
              "p-3 rounded-full transition-colors duration-300",
              isDarkMode ? "bg-blue-500/20" : "bg-blue-100"
            )}>
              <UserIcon className={cn(
                "h-6 w-6 transition-colors duration-300",
                isDarkMode ? "text-blue-400" : "text-blue-600"
              )} />
            </div>
          </div>
        </div>
        
        {/* Total Balance Card */}
        <div className={cn(
          "rounded-xl border p-6 shadow-sm transition-all duration-300 hover:scale-105",
          isDarkMode 
            ? "bg-gray-800 border-gray-700 hover:border-green-500" 
            : "bg-white border-gray-200 hover:border-green-400"
        )}>
          <div className="flex justify-between items-center">
            <div>
              <h3 className={cn(
                "text-lg font-medium transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-600"
              )}>
                إجمالي الأرصدة
              </h3>
              <p className={cn(
                "text-3xl font-bold mt-2 transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {totalBalance.toLocaleString()} LYD
              </p>
            </div>
            <div className={cn(
              "p-3 rounded-full transition-colors duration-300",
              isDarkMode ? "bg-green-500/20" : "bg-green-100"
            )}>
              <svg xmlns="http://www.w3.org/2000/svg" className={cn(
                "h-6 w-6 transition-colors duration-300",
                isDarkMode ? "text-green-400" : "text-green-600"
              )} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
          </div>
        </div>
        
        {/* Average Balance Card */}
        <div className={cn(
          "rounded-xl border p-6 shadow-sm transition-all duration-300 hover:scale-105",
          isDarkMode 
            ? "bg-gray-800 border-gray-700 hover:border-purple-500" 
            : "bg-white border-gray-200 hover:border-purple-400"
        )}>
          <div className="flex justify-between items-center">
            <div>
              <h3 className={cn(
                "text-lg font-medium transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-600"
              )}>
                متوسط الرصيد
              </h3>
              <p className={cn(
                "text-3xl font-bold mt-2 transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {averageBalance.toLocaleString(undefined, {
                  maximumFractionDigits: 2
                })} LYD
              </p>
            </div>
            <div className={cn(
              "p-3 rounded-full transition-colors duration-300",
              isDarkMode ? "bg-purple-500/20" : "bg-purple-100"
            )}>
              <svg xmlns="http://www.w3.org/2000/svg" className={cn(
                "h-6 w-6 transition-colors duration-300",
                isDarkMode ? "text-purple-400" : "text-purple-600"
              )} fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
          </div>
        </div>
      </div>
      
      {/* Wallet Table */}
      <div className={cn(
        "rounded-xl border shadow-sm transition-colors duration-300",
        isDarkMode ? "bg-gray-800 border-gray-700" : "bg-white border-gray-200"
      )}>
        {loading ? (
          <LoadingSpinner isDarkMode={isDarkMode} />
        ) : (
          <>
            <WalletTable data={filteredData} />
            {/* Pagination controls would be handled inside WalletTable component */}
          </>
        )}
      </div>
    </div>
  );
}