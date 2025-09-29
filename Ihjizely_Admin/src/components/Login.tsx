import logo from '../assets/ihjzlyapplogo.png'
import { LoginForm } from "@/components/login-form"
import SvgImg from "@/assets/web-development-programmer-engineering-coding-website-augmented-reality-interface-screens_641890-20-removebg-preview.png"
import { cn } from '@/lib/utils'
import { useDarkMode } from './DarkModeContext'
export default function LoginPage() {
  const { isDarkMode } = useDarkMode()

  return (
    <div className={cn("bg-white flex min-h-svh flex-row items-center justify-center gap-6 p-6 md:p-10",isDarkMode 
    ? "bg-gradient-to-br from-gray-800 to-gray-900 flex min-h-svh flex-row items-center justify-center gap-6 p-6 md:p-10" 
    : "bg-gradient-to-br from-white to-gray-50 border-gray-200 filter brightness-100 contrast-100 saturate-100 hover:brightness-95 hover:scale-105 hover:shadow-2xl hover:shadow-purple-500/5"

    )}>
<img 
  width={300} 
  src={SvgImg} 
  className={cn(
    "transition-all duration-500 ease-in-out transform rounded-2xl backdrop-blur-sm border shadow-xl",
    isDarkMode 
      ? "bg-gradient-to-br from-gray-800 to-gray-900 border-gray-700 filter brightness-110 contrast-105 saturate-110 hover:brightness-125 hover:scale-105 hover:shadow-2xl hover:shadow-purple-500/10" 
      : "bg-gradient-to-br from-white to-gray-50 border-gray-200 filter brightness-100 contrast-100 saturate-100 hover:brightness-95 hover:scale-105 hover:shadow-2xl hover:shadow-purple-500/5"
  )} 
  alt="Description of image" 
/>      
      <div className="flex w-full max-w-sm flex-col gap-6">
        <a href="#" className="flex items-center gap-2 self-center font-medium">
          <div className=" text-primary-foreground flex size- items-center justify-center rounded-md">
           <img src={logo} alt="" />
          </div>
        </a>
        <LoginForm />
      </div>
    </div>
  )
}
