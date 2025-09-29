import { AppSidebar } from "@/components/app-sidebar";
import { SiteHeader } from "@/components/site-header";
import { SidebarInset } from "@/components/ui/sidebar";
import { SidebarProvider as RawSidebarProvider } from "@/components/ui/sidebar";
import { useDarkMode } from '../DarkModeContext'; // Import the hook
import { cn } from "@/lib/utils";

const SidebarProvider = RawSidebarProvider as React.FC<React.HTMLAttributes<HTMLDivElement> & {
  defaultOpen?: boolean;
  open?: boolean;
  onOpenChange?: (open: boolean) => void;
}>;

export function ProtectedLayout({ children }: { children?: React.ReactNode }) {
  const { isDarkMode } = useDarkMode(); // Get dark mode state

  return (
    <SidebarProvider
      style={{
        "--sidebar-width": "calc(var(--spacing) * 72)",
        "--header-height": "calc(var(--spacing) * 12)",
      } as React.CSSProperties}
      className={isDarkMode ? "dark" : ""} // Apply dark class to provider
    >
      <AppSidebar variant="inset" />
      <SidebarInset>
        <SiteHeader />
        <main className={cn(
          "flex flex-col pt-16 px-6 min-h-screen transition-colors duration-300",
          isDarkMode ? "bg-gray-900 text-white" : "bg-white text-gray-900"
        )}>
          {children}
        </main>
      </SidebarInset>
    </SidebarProvider>
  );
}