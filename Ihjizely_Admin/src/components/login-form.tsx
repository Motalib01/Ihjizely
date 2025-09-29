import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { useState } from 'react'
import { useAuth } from "@/hooks/useAuth"
import { useDarkMode } from './DarkModeContext'

type LoginFormProps = React.ComponentProps<"div">

export function LoginForm({ className, ...props }: LoginFormProps) {
  const { isDarkMode } = useDarkMode()
  const [credentials, setCredentials] = useState({
    emailOrPhone: '',
    password: ''
  })
  const { login, isLoading, error } = useAuth()

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { id, value } = e.target
    setCredentials(prev => ({
      ...prev,
      [id]: value
    }))
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    await login(credentials)
  }

  return (
    <div className={cn("flex flex-col gap-6 transition-all duration-300", className)} {...props}>
      <Card className={cn(
        "shadow-xl border-0 transition-all duration-300 hover:shadow-2xl",
        isDarkMode 
          ? "bg-gray-800/90 backdrop-blur-sm text-white" 
          : "bg-white/90 backdrop-blur-sm text-gray-900"
      )}>
        <CardHeader className="text-center pb-4">
          <div className={cn(
            "w-16 h-16 rounded-full flex items-center justify-center mx-auto mb-4 transition-colors duration-300",
            isDarkMode ? "bg-purple-600/20" : "bg-purple-100"
          )}>
          <svg 
  xmlns="http://www.w3.org/2000/svg" 
  className={cn(
    "h-8 w-8 transition-all duration-300",
    isDarkMode 
      ? "text-purple-400 hover:text-purple-300 filter drop-shadow-[0_0_8px_rgba(192,132,252,0.3)]" 
      : "text-purple-600 hover:text-purple-700 filter drop-shadow-[0_0_4px_rgba(147,51,234,0.2)]"
  )} 
  fill="none" 
  viewBox="0 0 24 24" 
  stroke="currentColor"
  strokeWidth={1.5}
>
  <path 
    strokeLinecap="round" 
    strokeLinejoin="round" 
    d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" 
  />
</svg>
          </div>
          <CardTitle className={cn(
            "text-2xl font-bold transition-colors duration-300",
            isDarkMode ? "text-white" : "text-gray-900"
          )}>
            مرحباً بعودتك
          </CardTitle>
          <CardDescription className={cn(
            "transition-colors duration-300",
            isDarkMode ? "text-gray-400" : "text-gray-600"
          )}>
            سجل الدخول باستخدام رقم الهاتف وكلمة المرور
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit}>
            <div className="grid gap-6">
              {error && (
                <div className={cn(
                  "text-sm text-center p-3 rounded-lg border transition-all duration-300 animate-pulse",
                  isDarkMode 
                    ? "bg-red-900/30 text-red-300 border-red-800" 
                    : "bg-red-100 text-red-700 border-red-200"
                )}>
                  <div className="flex items-center justify-center gap-2">
                  <svg 
  xmlns="http://www.w3.org/2000/svg" 
  className={cn(
    "h-4 w-4 transition-all duration-300",
    isDarkMode 
      ? "text-transparent bg-gradient-to-br from-red-300 to-orange-300 bg-clip-text filter drop-shadow-[0_0_6px_rgba(252,165,165,0.4)]" 
      : "text-transparent bg-gradient-to-br from-red-600 to-orange-600 bg-clip-text filter drop-shadow-[0_0_3px_rgba(220,38,38,0.3)]"
  )} 
  fill="none" 
  viewBox="0 0 24 24" 
  stroke="currentColor"
  strokeWidth={2}
>
  <path 
    strokeLinecap="round" 
    strokeLinejoin="round" 
    d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" 
  />
</svg>
                    {error}
                  </div>
                </div>
              )}
              
              <div className="grid gap-6">
                <div className="grid gap-3">
                  <Label 
                    htmlFor="emailOrPhone" 
                    className={cn(
                      "font-medium transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}
                  >
                    رقم الهاتف أو البريد الإلكتروني
                  </Label>
                  <Input
                    id="emailOrPhone"
                    type="text"
                    placeholder="أدخل رقم الهاتف أو البريد الإلكتروني"
                    required
                    value={credentials.emailOrPhone}
                    onChange={handleChange}
                    className={cn(
                      "transition-all duration-300 text-right",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:border-purple-400 focus:ring-2 focus:ring-purple-400/30"
                        : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30"
                    )}
                  />
                </div>
                <div className="grid gap-3">
                  <div className="flex items-center justify-between">
                    <Label 
                      htmlFor="password" 
                      className={cn(
                        "font-medium transition-colors duration-300",
                        isDarkMode ? "text-gray-300" : "text-gray-700"
                      )}
                    >
                      كلمة المرور
                    </Label>
                    <button 
                      type="button"
                      className={cn(
                        "text-sm transition-colors duration-300 hover:underline",
                        isDarkMode ? "text-purple-400 hover:text-purple-300" : "text-purple-600 hover:text-purple-700"
                      )}
                    >
                      نسيت كلمة المرور؟
                    </button>
                  </div>
                  <Input 
                    id="password" 
                    type="password" 
                    required 
                    value={credentials.password}
                    onChange={handleChange}
                    className={cn(
                      "transition-all duration-300 text-right",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400 focus:border-purple-400 focus:ring-2 focus:ring-purple-400/30"
                        : "bg-white border-gray-300 text-gray-900 placeholder-gray-500 focus:border-purple-500 focus:ring-2 focus:ring-purple-500/30"
                    )}
                    placeholder="أدخل كلمة المرور"
                  />
                </div>
                <Button 
                  type="submit" 
                  className={cn(
                    "w-full cursor-pointer transition-all duration-300 hover:scale-105 py-3 text-lg font-medium",
                    isDarkMode
                      ? "bg-gradient-to-r from-purple-600 to-purple-700 hover:from-purple-700 hover:to-purple-800 text-white shadow-lg"
                      : "bg-gradient-to-r from-purple-600 to-purple-700 hover:from-purple-700 hover:to-purple-800 text-white shadow-lg"
                  )} 
                  disabled={isLoading}
                >
                  {isLoading ? (
                    <div className="flex items-center justify-center gap-3">
                      <div className={cn(
                        "animate-spin rounded-full h-5 w-5 border-t-2 border-b-2",
                        isDarkMode ? "border-white" : "border-white"
                      )}></div>
                      جاري تسجيل الدخول...
                    </div>
                  ) : (
                    <div className="flex items-center justify-center gap-2">
                      <svg 
                        xmlns="http://www.w3.org/2000/svg" 
                        className="h-5 w-5" 
                        fill="none" 
                        viewBox="0 0 24 24" 
                        stroke="currentColor"
                      >
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M11 16l-4-4m0 0l4-4m-4 4h14m-5 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h7a3 3 0 013 3v1" />
                      </svg>
                      تسجيل الدخول
                    </div>
                  )}
                </Button>
              </div>
            </div>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}