import * as React from "react";
import {
  IconHome2,
  IconUsers,
  IconBuilding,
  IconReceipt2,
  IconCurrencyDollar,
  IconMoon,
  IconLogout2, 
  IconCalendarClock,
  IconReport, 
  IconLocationPlus
} from "@tabler/icons-react";
import logo from "../assets/ihjzlyapplogo.png";
import { cn } from "@/lib/utils";
import {
  Sidebar,
  SidebarContent,
  SidebarFooter,
  SidebarHeader,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarTrigger,
} from "@/components/ui/sidebar";
import { useNavigate } from "react-router-dom";
import { authService } from '@/API/auth';
import { useDarkMode } from './DarkModeContext'; // Corrected import path

const data = {
  user: {
    name: "Ali Maamri",
    email: "m@example.com",
    avatar: "/avatars/shadcn.jpg",
  },
  navMain: [
    {
      title: "لوحة القيادة",
      url: "/Admin",
      icon: IconHome2,
    },
    {
      title: "المستخدمين",
      url: "/Admin/users",
      icon: IconUsers,
    },
    {
      title: "إدارة الوحدات",
      url: "/Admin/units",
      icon: IconBuilding,
    },
    {
      title: "إدارة الحجوزات",
      url: "/Admin/reservations",
      icon: IconCalendarClock,
    },
    {
      title: "إدارة الاشتراكات",
      url: "/Admin/subscriptions",
      icon: IconReceipt2,
    },
    {
      title: "إدارة المحافظ",
      url: "/Admin/wallets",
      icon: IconCurrencyDollar,
    },
    {
      title: "إدارة التقارير",
      url: "/Admin/reports",
      icon: IconReport,
    },
    {
      title: "إدارة المواقع و المدن",
      url: "/Admin/Locations",
      icon: IconLocationPlus,
    },
  ],
};

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const navigate = useNavigate();
  const { isDarkMode, toggleDarkMode } = useDarkMode();
  const [activeItem, setActiveItem] = React.useState<string | null>(
    localStorage.getItem("activeSidebarItem") || "/Admin"
  );

  const handleItemClick = (url: string) => {
    setActiveItem(url);
    localStorage.setItem("activeSidebarItem", url);
    navigate(url);
  };

  const handleLogout = (e: React.MouseEvent) => {
    e.preventDefault();
    authService.logout();
    navigate('/');
    localStorage.removeItem('activeSidebarItem');
  };

  return (
    <Sidebar 
      collapsible="icon" 
      {...props} 
      className={cn(
        "z-[99999] group-data-[collapsible=icon]:z-[9999999] transition-colors duration-300",
        isDarkMode 
          ? "bg-gray-900 text-white border-gray-700" 
          : "bg-white text-gray-900 border-gray-200"
      )}
    >
      {/* Header Section */}
      <SidebarHeader>
        <SidebarMenu>
          <SidebarMenuItem>
            <div className={cn(
              "flex flex-row items-center justify-end group-data-[collapsible=icon]:justify-start group-data-[collapsible=icon]:w-[5rem] w-[20rem] h-11 overflow-hidden transition-all duration-300",
              isDarkMode ? "text-white" : "text-gray-700"
            )}>
              <SidebarTrigger className={cn(
                "hover:bg-opacity-10 transition-colors",
                isDarkMode ? "text-white hover:bg-white" : "text-gray-700 hover:bg-gray-200"
              )} />
            </div>

            {/* Logo and Title */}
            <a
  href="#"
  className={cn(
    "flex z-[99999] flex-col items-center justify-center gap-2 w-full h-24",
    "group-data-[collapsible=icon]:flex-col group-data-[collapsible=icon]:items-center",
    "transition-colors duration-300 rounded-lg mx-2",
    "hover:bg-opacity-50",
    isDarkMode 
      ? "hover:bg-gray-800 text-white" 
      : "hover:bg-gray-100 text-[#5D7285]"
  )}
>
  <img 
    src={logo} 
    className={cn(
      "w-12 h-12 group-data-[collapsible=icon]:!size-9 transition-all duration-300",
      "filter drop-shadow-sm",
      isDarkMode 
        ? "bg-black   hover:brightness-100 bg-transparent  hover:invert-0" 
        : "hover:brightness-90"
    )} 
    alt="Logo" 
  />
  <h1 className={cn(
    "font-bold transition-colors duration-300 text-sm",
    "group-data-[collapsible=icon]:hidden",
    isDarkMode ? "text-gray-200" : "text-gray-700"
  )}>
    إحجزلي
  </h1>
</a>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>

      {/* Content Section */}
      <SidebarContent>
        <SidebarMenu>
          {data.navMain.map((item, index) => (
            <SidebarMenuItem key={index}>
              <SidebarMenuButton
                asChild
                className={cn(
                  "flex text-[bold] items-center w-full p-3 text-sm font- rounded-lg transition-all duration-300 group sidebarcontent",
                  isDarkMode ? "hover:bg-gray-750" : "hover:bg-gray-50"
                )}
              >
                <a
                  href="#"
                  onClick={(e) => {
                    e.preventDefault();
                    handleItemClick(item.url);
                  }}
                  className={cn(
                    "flex items-center w-full text-[bold] transition-all duration-300",
                    activeItem === item.url
                      ? "bg-purple-500 text-white shadow-lg"
                      : isDarkMode 
                        ? "text-gray-300 hover:bg-purple-600 hover:text-white hover:shadow-md" 
                        : "text-gray-700 hover:bg-[#AD46FF] hover:text-white hover:shadow-md"
                  )}
                >
                  <span className="mr-3">
                    {React.createElement(item.icon, { 
                      className: cn(
                        "h-5 w-5 transition-colors duration-300",
                        activeItem === item.url ? "text-white" : 
                        isDarkMode ? "text-gray-400" : "text-gray-600"
                      )
                    })}
                  </span>
                  <span className="group-data-[collapsible=icon]:sr-only">{item.title}</span>
                </a>
              </SidebarMenuButton>
            </SidebarMenuItem>
          ))}
        </SidebarMenu>
      </SidebarContent>

      {/* Footer Section */}
      <SidebarFooter className="flex justify-center items-center">
        <div className="flex flex-col items-start py-3 space-y-2 w-full">
          {/* Dark Mode Toggle */}
          <button
            onClick={toggleDarkMode}
            className={cn(
              "flex items-center w-full px-3 py-2 text-sm font- rounded-lg transition-all duration-300 group",
              isDarkMode 
                ? "text-gray-300 hover:bg-gray-750" 
                : "text-gray-700 hover:bg-gray-100"
            )}
          >
            <div className="flex items-center w-full justify-between group-data-[collapsible=icon]:flex-col">
              <span className="flex items-center">
                <IconMoon className={cn(
                  "h-5 w-5 mr-3 transition-colors duration-300",
                  isDarkMode ? "text-yellow-300" : "text-gray-600"
                )} />
                <span className="group-data-[collapsible=icon]:hidden transition-colors duration-300">
                  {isDarkMode ? "الوضع المضيء" : "الوضع المظلم"}
                </span>
              </span>

              {/* Toggle Switch */}
              <div
                className={cn(
                  "relative w-10 h-5 border-2 rounded-full cursor-pointer transition-all duration-300",
                  isDarkMode 
                    ? "bg-purple-600 border-purple-600" 
                    : "bg-gray-300 border-gray-300"
                )}
              >
                <div
                  className={cn(
                    "absolute top-1/2 w-3 h-3 bg-white rounded-full transition-all duration-300 transform -translate-y-1/2",
                    isDarkMode ? "translate-x-5 left-1" : "left-1"
                  )}
                />
              </div>
            </div>
          </button>

          {/* Logout Button */}
          <SidebarMenuButton
            asChild
            className={cn(
              "w-full h-full m-0 transition-all duration-300",
              isDarkMode ? "hover:bg-red-900/30" : "hover:bg-red-50"
            )}
          >
            <a
              href="#"
              onClick={handleLogout}
              className={cn(
                "flex items-center w-full px-3 py-2 text-sm font- rounded-lg transition-all duration-300",
                isDarkMode 
                  ? "text-red-300 hover:bg-red-900/30 hover:text-white" 
                  : "text-red-600 hover:bg-red-50 hover:text-red-700"
              )}
            >
              <span className="mr-3">
                <IconLogout2 className={cn(
                  "transition-colors duration-300",
                  isDarkMode ? "text-red-300" : "text-red-500"
                )} />
              </span>
              <span className="group-data-[collapsible=icon]:hidden">تسجيل الخروج</span>
            </a>
          </SidebarMenuButton>
        </div>
      </SidebarFooter>
    </Sidebar>
  );
}