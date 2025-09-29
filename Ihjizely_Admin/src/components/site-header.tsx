import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { BellIcon, SearchIcon, Moon, Sun } from "lucide-react"
import { Avatar, AvatarImage, AvatarFallback } from "@/components/ui/avatar"
import logo from '../assets/ihjzlyapplogo.png'
import { useDarkMode } from './DarkModeContext'
import { cn } from "@/lib/utils"

export function SiteHeader() {
  const { isDarkMode, toggleDarkMode } = useDarkMode()

  return (
    <header className={cn(
      "fixed top-0 left-0 h-16 shrink-0 w-full z-[9999] border-b flex items-center justify-between px-4 backdrop-blur-sm transition-all duration-300",
      isDarkMode 
        ? "bg-gray-900/80 border-gray-700 text-white supports-backdrop-blur:bg-gray-900/60" 
        : "bg-white/80 border-gray-200 text-gray-900 supports-backdrop-blur:bg-white/60"
    )}>
      <div className="flex group-data-[collapsible=icon]:z-[9999999] items-center justify-end gap-5 w-full">
        {/* Search Bar */}
        <div className={cn(
          "flex items-center gap-2 w-1/2 shadow-sm rounded-lg p-2 py-0 transition-all duration-300",
          isDarkMode 
            ? "bg-gray-800/50 border border-gray-600/50 hover:border-gray-500" 
            : "bg-gray-50/50 border border-gray-200/50 hover:border-gray-300"
        )}>
          <SearchIcon 
            className={cn(
              "h-4 w-4 transition-colors duration-300 flex-shrink-0",
              isDarkMode ? "text-gray-400" : "text-gray-500"
            )} 
          />
          <Input
            placeholder="البحث عن المستخدمين والعقارات والحجوزات..."
            className={cn(
              "h-9 w-full rounded-md shadow-none border-none bg-transparent px-3 py-2 pl-2 text-sm",
              "ring-offset-background placeholder:text-muted-foreground focus-visible:ring-2",
              "md:w-full focus:outline-none focus:border-0 transition-all duration-300",
              isDarkMode 
                ? "text-white placeholder-gray-400 focus-visible:ring-purple-400" 
                : "text-gray-900 placeholder-gray-500 focus-visible:ring-purple-500"
            )}
          />
        </div>

        {/* Right Side Icons / User */}
        <div className="flex items-center gap-3">
          {/* Dark Mode Toggle */}
          <Button 
            variant="ghost" 
            size="icon"
            onClick={toggleDarkMode}
            className={cn(
              "rounded-full transition-all duration-300 hover:scale-110",
              isDarkMode 
                ? "hover:bg-yellow-400/10 text-yellow-300" 
                : "hover:bg-purple-400/10 text-purple-600"
            )}
            title={isDarkMode ? "Switch to light mode" : "Switch to dark mode"}
          >
            {isDarkMode ? (
              <Sun className="h-4 w-4" />
            ) : (
              <Moon className="h-4 w-4" />
            )}
          </Button>

          {/* Notifications */}
          <Button 
            variant="ghost" 
            size="icon"
            className={cn(
              "relative transition-all duration-300 hover:scale-110",
              isDarkMode 
                ? "hover:bg-gray-700 text-gray-300" 
                : "hover:bg-gray-100 text-gray-600"
            )}
          >
            <BellIcon className="h-5 w-5" />
            {/* Notification Badge */}
            <span className={cn(
              "absolute -top-1 -right-1 h-2 w-2 rounded-full animate-pulse",
              isDarkMode ? "bg-red-400" : "bg-red-500"
            )} />
          </Button>

          {/* User Avatar */}
          <div className="flex items-center gap-3 pl-2 border-l transition-colors duration-300">
            <div className="text-right hidden sm:block">
              <p className={cn(
                "text-sm font-medium transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
أحمد الفرجاني              </p>
              <p className={cn(
                "text-xs transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-500"
              )}>
                Admin
              </p>
            </div>
            
            <Avatar className={cn(
              "border-2 transition-all duration-300 hover:scale-105 cursor-pointer",
              isDarkMode ? "border-gray-600" : "border-gray-300"
            )}>
              <AvatarImage 
                src={logo} 
                alt="Ali Maamri" 
                className={cn(
                  "transition-all duration-300",
                  isDarkMode ? "filter brightness-110" : ""
                )}
              />
              <AvatarFallback className={cn(
                "transition-colors duration-300 font-medium",
                isDarkMode 
                  ? "bg-gray-700 text-gray-200" 
                  : "bg-gray-100 text-gray-600"
              )}>
                AM
              </AvatarFallback>
            </Avatar>
          </div>
        </div>
      </div>
    </header>
  )
}