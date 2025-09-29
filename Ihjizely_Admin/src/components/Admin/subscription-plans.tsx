import { useState, lazy, Suspense, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { toast } from "sonner";
import { Button } from "../ui/button";
import { subscriptionsService } from "@/API/SubscriptionsService";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { X } from "lucide-react";
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

// Lazy-loaded components
const PlanCard = lazy(() => import("./PlanCard"));
const ConfirmationSection = lazy(() => import("./ConfirmationSection"));

export interface Plan {
  id: string;
  name: string;
  tagline: string;
  price: number;
  currency: string;
  period: string;
  features: string[];
  icon: string;
  color: string;
  maxAds?: number;
  duration?: string;
  durationInDays: number;
  isActive?: boolean;
}

// Loading Skeleton Component
const LoadingSkeleton = ({ isDarkMode }: { isDarkMode: boolean }) => (
  <div className={cn(
    "p-6 max-w-6xl mx-auto transition-colors duration-300",
    isDarkMode ? "bg-gray-900" : "bg-white"
  )}>
    <div className={cn(
      "grid grid-cols-1 md:grid-cols-3 gap-8 transition-colors duration-300",
      isDarkMode ? "bg-gray-900" : "bg-white"
    )}>
      {[1, 2, 3].map((_, idx) => (
        <div key={idx} className={cn(
          "border rounded-2xl shadow-xl overflow-hidden h-[420px] animate-pulse transition-colors duration-300",
          isDarkMode ? "bg-gray-800 border-gray-700" : "bg-gray-100 border-gray-200"
        )} />
      ))}
    </div>
  </div>
);

export default function SubscriptionPlans() {
  const { isDarkMode } = useDarkMode();
  const navigate = useNavigate();
  const [selectedPlan, setSelectedPlan] = useState<string>("");
  const [plans, setPlans] = useState<Plan[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [isCreateDialogOpen, setIsCreateDialogOpen] = useState(false);
  const [newPlan, setNewPlan] = useState({
    name: '',
    durationInDays: 1,
    amount: 0,
    currency: 'LYD',
    maxAds: 0
  });

  const convertDurationToDays = (duration: string | number): number => {
    if (!duration) return 30;
    
    if (typeof duration === 'number') {
      return duration;
    }
    
    if (/^\d+$/.test(duration)) {
      return parseInt(duration);
    }
    
    if (/^\d+\.\d{2}:\d{2}:\d{2}$/.test(duration)) {
      const [days] = duration.split('.');
      return parseInt(days);
    }
    
    if (/^\d{2}:\d{2}:\d{2}$/.test(duration)) {
      const [hours] = duration.split(':').map(Number);
      return Math.ceil(hours / 24);
    }
    
    return 30;
  };

  const formatDurationForDisplay = (duration: string | number): string => {
    const days = convertDurationToDays(duration);
    
    if (days === 30) return "شهر";
    if (days === 90) return "3 أشهر";
    if (days === 180) return "6 أشهر";
    if (days === 365) return "سنة";
    return `${days} يوم`;
  };

  useEffect(() => {
    const fetchPlans = async () => {
      try {
        setIsLoading(true);
        const apiPlans = await subscriptionsService.getAllPlans();
        
        const transformedPlans = apiPlans.map(plan => ({
          id: plan.id,
          name: plan.name,
          tagline: getPlanTagline(plan.name),
          price: plan.amount,
          currency: plan.currency,
          period: formatDurationForDisplay(plan.duration),
          features: [
            plan.maxAds 
              ? `يمكنك إضافة ${plan.maxAds} إعلانات`
              : "عدد غير محدد من الإعلانات",
            `المدة: ${formatDurationForDisplay(plan.duration)}`
          ],
          icon: getPlanIcon(plan.name),
          color: getPlanColor(plan.name),
          maxAds: plan.maxAds,
          duration: plan.duration,
          durationInDays: convertDurationToDays(plan.duration),
          isActive: plan.isActive
        }));
        
        setPlans(transformedPlans);
        if (transformedPlans.length > 0) {
          setSelectedPlan(transformedPlans[0].id);
        }
      } catch (error) {
        toast.error('فشل تحميل خطط الاشتراك');
      } finally {
        setIsLoading(false);
      }
    };

    fetchPlans();
  }, []);

  const getPlanTagline = (name: string) => {
    switch(name) {
      case "الذهبية": return "الحل الأمثل لبداية صحيحة";
      case "الفضية": return "مثالي للاستخدام المتوسط";
      case "البرونزية": return "البداية المثالية";
      default: return "خطة مميزة لاحتياجاتك";
    }
  };

  const getPlanIcon = (name: string) => {
    switch(name) {
      case "الذهبية": return "Star";
      case "الفضية": return "Medal";
      case "البرونزية": return "Diamond";
      default: return "Star";
    }
  };

  const getPlanColor = (name: string) => {
    switch(name) {
      case "الذهبية": return "bg-gradient-to-r from-yellow-400 to-yellow-600";
      case "الفضية": return "bg-gradient-to-r from-gray-300 to-gray-500";
      case "البرونزية": return "bg-gradient-to-r from-amber-700 to-amber-900";
      default: return "bg-gradient-to-r from-purple-400 to-purple-600";
    }
  };

  const handleDeletePlan = async (planId: string) => {
    try {
      await subscriptionsService.deletePlan(planId);
      setPlans(prev => prev.filter(p => p.id !== planId));
      toast.success("تم حذف الخطة بنجاح");
    } catch (error) {
      toast.error('فشل حذف الخطة');
    }
  };

  const handleCreatePlan = async () => {
    try {
      setIsLoading(true);
      
      const createdPlan = await subscriptionsService.createPlan({
        name: newPlan.name,
        durationInDays: newPlan.durationInDays,
        amount: newPlan.amount,
        currency: newPlan.currency,
        maxAds: newPlan.maxAds
      });
      
      const transformedPlan = {
        id: createdPlan.id,
        name: createdPlan.name,
        tagline: getPlanTagline(createdPlan.name),
        price: createdPlan.amount,
        currency: createdPlan.currency,
        period: formatDurationForDisplay(createdPlan.duration),
        features: [
          createdPlan.maxAds 
            ? `يمكنك إضافة ${createdPlan.maxAds} إعلانات`
            : "عدد غير محدد من الإعلانات",
          `المدة: ${formatDurationForDisplay(createdPlan.duration)}`
        ],
        icon: getPlanIcon(createdPlan.name),
        color: getPlanColor(createdPlan.name),
        maxAds: createdPlan.maxAds,
        duration: createdPlan.duration,
        durationInDays: convertDurationToDays(createdPlan.duration),
        isActive: true
      };
      
      setPlans(prev => [...prev, transformedPlan]);
      setIsCreateDialogOpen(false);
      setNewPlan({
        name: '',
        durationInDays: 1,
        amount: 0,
        currency: 'LYD',
        maxAds: 0
      });
      toast.success("تم إنشاء الخطة بنجاح");
    } catch (error) {
      toast.error('فشل إنشاء الخطة');
    } finally {
      setIsLoading(false);
    }
  };

  const handleUpdatePlan = async (updatedPlan: Plan) => {
    try {
      const { id, name, price, currency, maxAds, durationInDays } = updatedPlan;
      
      await subscriptionsService.updatePlan(id, {
        name,
        durationInDays,
        amount: price,
        currency,
        maxAds
      });
      
      setPlans(prev => prev.map(p => p.id === updatedPlan.id ? updatedPlan : p));
      toast.success("تم تحديث الخطة بنجاح");
    } catch (error) {
      toast.error('فشل تحديث الخطة');
    }
  };

  const handleSelectPlan = (planId: string) => {
    setSelectedPlan(planId);
  };

  const handleConfirm = () => {
    const planName = plans.find(p => p.id === selectedPlan)?.name || "";
    toast.success(`تم تغيير الاشتراك إلى الخطة ${planName}`);
    navigate("/Admin/subscriptions");
  };

  if (isLoading) {
    return <LoadingSkeleton isDarkMode={isDarkMode} />;
  }

  return (
    <div className={cn(
      "p-6 max-w-6xl mx-auto min-h-screen transition-colors duration-300",
      isDarkMode ? "bg-gray-900 text-white" : "bg-white text-gray-900"
    )}>
      <div className="flex justify-between items-center mb-10">
        <div>
          <h1 className={cn(
            "text-3xl font-bold transition-colors duration-300",
            isDarkMode ? "text-white" : "text-gray-800"
          )}>
            إدارة الاشتراكات
          </h1>
          <p className={cn(
            "mt-2 transition-colors duration-300",
            isDarkMode ? "text-gray-400" : "text-gray-600"
          )}>
            اختر الخطة المناسبة لاحتياجاتك التجارية
          </p>
        </div>
        <div className="flex gap-2">
          <Button 
            variant="outline" 
            onClick={() => navigate("/Admin/subscriptions")}
            className={cn(
              "flex items-center gap-2 transition-colors duration-300",
              isDarkMode
                ? "border-gray-600 text-gray-300 hover:bg-gray-800"
                : "border-gray-300 text-gray-700 hover:bg-gray-100"
            )}
          >
            رجوع إلى الاشتراكات
          </Button>
          
          <Dialog open={isCreateDialogOpen} onOpenChange={setIsCreateDialogOpen}>
            <DialogTrigger asChild>
              <Button className={cn(
                "flex items-center gap-2 transition-colors duration-300 hover:scale-105",
                isDarkMode
                  ? "bg-purple-600 hover:bg-purple-700 text-white"
                  : "bg-purple-600 hover:bg-purple-700 text-white"
              )}>
                إنشاء خطة جديدة
              </Button>
            </DialogTrigger>
            <DialogContent className={cn(
              "max-w-2xl transition-colors duration-300",
              isDarkMode ? "bg-gray-800 border-gray-700" : "bg-white border-gray-200"
            )}>
              <DialogHeader>
                <DialogTitle className={cn(
                  "flex justify-between items-center transition-colors duration-300",
                  isDarkMode ? "text-white" : "text-gray-900"
                )}>
                  <span>إنشاء خطة جديدة</span>
                  <Button 
                    variant="ghost" 
                    size="icon"
                    onClick={() => setIsCreateDialogOpen(false)}
                    className={cn(
                      "transition-colors duration-300",
                      isDarkMode
                        ? "text-gray-400 hover:bg-gray-700 hover:text-white"
                        : "text-gray-600 hover:bg-gray-100"
                    )}
                  >
                    <X className="h-4 w-4" />
                  </Button>
                </DialogTitle>
              </DialogHeader>
              
              <div className="p-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className={cn(
                      "block text-sm font-medium mb-1 transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}>
                      اسم الخطة
                    </label>
                    <input
                      type="text"
                      value={newPlan.name}
                      onChange={(e) => setNewPlan({...newPlan, name: e.target.value})}
                      className={cn(
                        "w-full p-2 border rounded transition-colors duration-300",
                        isDarkMode
                          ? "bg-gray-700 border-gray-600 text-white placeholder-gray-400"
                          : "bg-white border-gray-300 text-gray-900 placeholder-gray-500"
                      )}
                      placeholder="أدخل اسم الخطة"
                    />
                  </div>
                  <div>
                    <label className={cn(
                      "block text-sm font-medium mb-1 transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}>
                      المدة (أيام)
                      <span className={cn(
                        "text-xs block transition-colors duration-300",
                        isDarkMode ? "text-gray-500" : "text-gray-500"
                      )}>
                        أدخل عدد الأيام (1-365)
                      </span>
                    </label>
                    <input
                      type="number"
                      min="1"
                      max="365"
                      value={newPlan.durationInDays}
                      onChange={(e) => {
                        const value = Math.min(365, Math.max(1, parseInt(e.target.value) || 1));
                        setNewPlan({...newPlan, durationInDays: value});
                      }}
                      className={cn(
                        "w-full p-2 border rounded transition-colors duration-300",
                        isDarkMode
                          ? "bg-gray-700 border-gray-600 text-white"
                          : "bg-white border-gray-300 text-gray-900"
                      )}
                      placeholder="أدخل عدد الأيام"
                    />
                  </div>
                  <div>
                    <label className={cn(
                      "block text-sm font-medium mb-1 transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}>
                      السعر
                    </label>
                    <input
                      type="number"
                      min="0"
                      value={newPlan.amount}
                      onChange={(e) => setNewPlan({...newPlan, amount: Number(e.target.value)})}
                      className={cn(
                        "w-full p-2 border rounded transition-colors duration-300",
                        isDarkMode
                          ? "bg-gray-700 border-gray-600 text-white"
                          : "bg-white border-gray-300 text-gray-900"
                      )}
                      placeholder="أدخل سعر الخطة"
                    />
                  </div>
                  <div>
                    <label className={cn(
                      "block text-sm font-medium mb-1 transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}>
                      العملة
                    </label>
                    <select
                      value={newPlan.currency}
                      onChange={(e) => setNewPlan({...newPlan, currency: e.target.value})}
                      className={cn(
                        "w-full p-2 border rounded transition-colors duration-300",
                        isDarkMode
                          ? "bg-gray-700 border-gray-600 text-white"
                          : "bg-white border-gray-300 text-gray-900"
                      )}
                    >
                      <option value="LYD">الدينار الليبي</option>
                    </select>
                  </div>
                  <div>
                    <label className={cn(
                      "block text-sm font-medium mb-1 transition-colors duration-300",
                      isDarkMode ? "text-gray-300" : "text-gray-700"
                    )}>
                      الحد الأقصى للإعلانات (اختياري)
                    </label>
                    <input
                      type="number"
                      min="0"
                      value={newPlan.maxAds}
                      onChange={(e) => setNewPlan({...newPlan, maxAds: Number(e.target.value)})}
                      className={cn(
                        "w-full p-2 border rounded transition-colors duration-300",
                        isDarkMode
                          ? "bg-gray-700 border-gray-600 text-white"
                          : "bg-white border-gray-300 text-gray-900"
                      )}
                      placeholder="أدخل الحد الأقصى للإعلانات"
                    />
                  </div>
                </div>
                <div className="mt-6 flex justify-end gap-2">
                  <Button 
                    variant="outline"
                    onClick={() => setIsCreateDialogOpen(false)}
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
                    onClick={handleCreatePlan} 
                    disabled={!newPlan.name || !newPlan.durationInDays || !newPlan.amount}
                    className={cn(
                      "transition-colors duration-300 hover:scale-105",
                      isDarkMode
                        ? "bg-blue-600 hover:bg-blue-700 text-white"
                        : "bg-blue-600 hover:bg-blue-700 text-white"
                    )}
                  >
                    حفظ الخطة
                  </Button>
                </div>
              </div>
            </DialogContent>
          </Dialog>
        </div>
      </div>

      <Suspense fallback={<LoadingSkeleton isDarkMode={isDarkMode} />}>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {plans.map(plan => (
            <PlanCard 
              key={plan.id}
              plan={plan}
              isSelected={selectedPlan === plan.id}
              isAdmin={true}
              onSelect={handleSelectPlan}
              onUpdate={handleUpdatePlan}
              onDelete={handleDeletePlan}
            />
          ))}
        </div>
      </Suspense>

      {plans.length > 0 && (
        <Suspense fallback={
          <div className={cn(
            "mt-12 p-6 border rounded-xl shadow-sm h-28 animate-pulse transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border-gray-700" : "bg-gray-100 border-gray-200"
          )} />
        }>
          <ConfirmationSection 
            selectedPlan={selectedPlan}
            plans={plans}
            onCancel={() => navigate("/Admin/subscriptions")}
            onConfirm={handleConfirm}
          />
        </Suspense>
      )}
    </div>
  );
}