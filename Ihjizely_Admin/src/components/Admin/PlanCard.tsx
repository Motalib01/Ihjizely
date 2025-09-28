// src/components/PlanCard.tsx
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { PencilIcon, CheckIcon, XIcon, TrashIcon } from "lucide-react";
import { toast } from "sonner";
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

export interface Plan {
  id: string;
  name: string;
  tagline: string;
  price: number;
  currency: string;
  period: string;
  durationInDays: number;
  features: string[];
  icon: string;
  color: string;
  maxAds?: number;
}

interface PlanCardProps {
  plan: Plan;
  isSelected: boolean;
  isAdmin?: boolean;
  onSelect: (planId: string) => void;
  onUpdate: (updatedPlan: Plan) => void;
  onDelete: (planId: string) => void;
}

export default function PlanCard({ 
  plan, 
  isSelected, 
  isAdmin = false,
  onSelect, 
  onUpdate,
  onDelete 
}: PlanCardProps) {
  const { isDarkMode } = useDarkMode();
  const [isEditing, setIsEditing] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [editedPlan, setEditedPlan] = useState(plan);
  const [errors, setErrors] = useState<Record<string, string>>({});

  const validateForm = () => {
    const newErrors: Record<string, string> = {};
    
    if (!editedPlan.name.trim()) {
      newErrors.name = "اسم الخطة مطلوب";
    }
    
    if (editedPlan.price <= 0 || isNaN(editedPlan.price)) {
      newErrors.price = "يجب أن يكون السعر رقمًا موجبًا";
    }
    
    if (editedPlan.durationInDays <= 0 || isNaN(editedPlan.durationInDays)) {
      newErrors.durationInDays = "يجب أن تكون المدة رقمًا موجبًا";
    }
    
    if (editedPlan.features.length === 0) {
      newErrors.features = "يجب إضافة ميزة واحدة على الأقل";
    } else {
      for (let i = 0; i < editedPlan.features.length; i++) {
        if (!editedPlan.features[i].trim()) {
          newErrors[`feature-${i}`] = "الميزة لا يمكن أن تكون فارغة";
        }
      }
    }
    
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleEdit = () => {
    setIsEditing(true);
    setEditedPlan(plan);
    setErrors({});
  };

  const handleSave = () => {
    if (!validateForm()) {
      toast.error("يوجد أخطاء في المدخلات");
      return;
    }
    
    onUpdate(editedPlan);
    setIsEditing(false);
    setErrors({});
  };

  const handleCancel = () => {
    setEditedPlan(plan);
    setIsEditing(false);
    setErrors({});
  };

  const handleDelete = () => {
    setIsDeleteDialogOpen(true);
  };

  const confirmDelete = () => {
    onDelete(plan.id);
    setIsDeleteDialogOpen(false);
  };

  const handleChange = (field: keyof Plan, value: string | number) => {
    setEditedPlan(prev => ({
      ...prev,
      [field]: value
    }));
    
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const handleFeatureChange = (index: number, value: string) => {
    setEditedPlan(prev => {
      const newFeatures = [...prev.features];
      newFeatures[index] = value;
      return { ...prev, features: newFeatures };
    });
    
    if (errors[`feature-${index}`]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[`feature-${index}`];
        return newErrors;
      });
    }
  };

  const addFeature = () => {
    setEditedPlan(prev => ({
      ...prev,
      features: [...prev.features, ""]
    }));
  };

  const removeFeature = (index: number) => {
    if (editedPlan.features.length <= 1) return;
    
    setEditedPlan(prev => {
      const newFeatures = [...prev.features];
      newFeatures.splice(index, 1);
      return { ...prev, features: newFeatures };
    });
  };

  const getDisplayPeriod = (days: number): string => {
    if (days === 30) return "شهر";
    if (days === 90) return "3 أشهر";
    if (days === 180) return "6 أشهر";
    if (days === 365) return "سنة";
    return `${days} يوم`;
  };

  return (
    <>
      <div 
        className={cn(
          "border rounded-2xl shadow-xl overflow-hidden transition-all duration-300 relative",
          isSelected 
            ? "ring-4 scale-[1.02]" 
            : "hover:shadow-lg",
          isDarkMode
            ? isSelected ? "ring-purple-400 bg-gray-800 border-gray-700" : "bg-gray-800 border-gray-700"
            : isSelected ? "ring-purple-500 bg-white border-gray-200" : "bg-white border-gray-200"
        )}
      >
        {/* Admin edit and delete buttons */}
        {isAdmin && !isEditing && (
          <div className="absolute top-4 right-4 z-10 flex gap-2">
            <Button 
              variant="ghost"
              size="icon"
              className={cn(
                "rounded-full transition-colors duration-300",
                isDarkMode
                  ? "bg-gray-700/80 hover:bg-gray-600 text-gray-300"
                  : "bg-white/80 hover:bg-white text-gray-700"
              )}
              onClick={handleEdit}
            >
              <PencilIcon className="w-5 h-5" />
            </Button>
            <Button 
              variant="ghost"
              size="icon"
              className={cn(
                "rounded-full transition-colors duration-300",
                isDarkMode
                  ? "bg-gray-700/80 hover:bg-gray-600 text-red-400"
                  : "bg-white/80 hover:bg-white text-red-600"
              )}
              onClick={handleDelete}
            >
              <TrashIcon className="w-5 h-5" />
            </Button>
          </div>
        )}

        <div className={`${plan.color} p-6 text-white`}>
          <div className="flex justify-between items-center">
            <div>
              {isEditing ? (
                <div>
                  <input
                    type="text"
                    value={editedPlan.name}
                    onChange={(e) => handleChange('name', e.target.value)}
                    className={cn(
                      "text-2xl font-bold rounded px-2 py-1 w-full transition-colors duration-300",
                      isDarkMode ? "bg-white/20 placeholder-gray-300" : "bg-white/20 placeholder-gray-700",
                      errors.name ? 'border-2 border-red-500' : ''
                    )}
                    placeholder="اسم الخطة"
                  />
                  {errors.name && <p className="text-red-300 text-sm mt-1">{errors.name}</p>}
                </div>
              ) : (
                <h2 className="text-2xl font-bold">{plan.name}</h2>
              )}
              
              {isEditing ? (
                <input
                  type="text"
                  value={editedPlan.tagline}
                  onChange={(e) => handleChange('tagline', e.target.value)}
                  className={cn(
                    "mt-1 rounded px-2 py-1 w-full transition-colors duration-300",
                    isDarkMode ? "bg-white/20 placeholder-gray-300" : "bg-white/20 placeholder-gray-700"
                  )}
                  placeholder="وصف الخطة"
                />
              ) : (
                <p className="mt-1">{plan.tagline}</p>
              )}
            </div>
            <div className="bg-white/20 p-3 rounded-full">
              <div className="w-6 h-6" />
            </div>
          </div>
        </div>
        
        <div className={cn(
          "p-6 border-b transition-colors duration-300",
          isDarkMode ? "border-gray-700" : "border-gray-200"
        )}>
          <div className="text-center">
            {isEditing ? (
              <div>
                <div className="flex items-center justify-center gap-2 mb-2">
                  <input
                    type="number"
                    value={editedPlan.price}
                    onChange={(e) => handleChange('price', parseFloat(e.target.value) || 0)}
                    className={cn(
                      "text-4xl font-bold rounded px-3 py-2 w-32 text-center transition-colors duration-300",
                      isDarkMode ? "bg-gray-700 text-white" : "bg-gray-100 text-gray-900",
                      errors.price ? 'border-2 border-red-500' : ''
                    )}
                    step="0.01"
                    min="0"
                  />
                  <span className="text-4xl font-bold">{editedPlan.currency}</span>
                </div>
                {errors.price && <p className="text-red-500 text-sm mb-2">{errors.price}</p>}
                
                <div className="flex items-center justify-center gap-2 mt-3">
                  <span className="text-lg">لمدة</span>
                  <input
                    type="number"
                    value={editedPlan.durationInDays}
                    onChange={(e) => handleChange('durationInDays', parseInt(e.target.value) || 0)}
                    className={cn(
                      "text-lg rounded px-2 py-1 w-20 text-center transition-colors duration-300",
                      isDarkMode ? "bg-gray-700 text-white" : "bg-gray-100 text-gray-900",
                      errors.durationInDays ? 'border-2 border-red-500' : ''
                    )}
                    min="1"
                    placeholder="30"
                  />
                  <span className="text-lg">يوم</span>
                </div>
                {errors.durationInDays && <p className="text-red-500 text-sm mt-1">{errors.durationInDays}</p>}
                
                {isEditing && editedPlan.maxAds !== undefined && (
                  <div className="flex items-center justify-center gap-2 mt-3">
                    <span className="text-lg">الحد الأقصى للإعلانات:</span>
                    <input
                      type="number"
                      value={editedPlan.maxAds}
                      onChange={(e) => handleChange('maxAds', parseInt(e.target.value) || 0)}
                      className={cn(
                        "text-lg rounded px-2 py-1 w-20 text-center transition-colors duration-300",
                        isDarkMode ? "bg-gray-700 text-white" : "bg-gray-100 text-gray-900"
                      )}
                      min="0"
                      placeholder="0"
                    />
                  </div>
                )}
              </div>
            ) : (
              <div>
                <p className="text-4xl font-bold mt-2">{plan.price} {plan.currency}</p>
                <p className="text-lg mt-1">/{getDisplayPeriod(plan.durationInDays)}</p>
                {plan.maxAds !== undefined && (
                  <p className="text-sm mt-2">الحد الأقصى للإعلانات: {plan.maxAds}</p>
                )}
              </div>
            )}
          </div>
        </div>
        
        <div className="p-6">
          <h3 className={cn(
            "font-medium mb-3 transition-colors duration-300",
            isDarkMode ? "text-gray-300" : "text-gray-700"
          )}>
            المميزات:
          </h3>
          {errors.features && <p className="text-red-500 text-sm mb-2">{errors.features}</p>}
          
          <ul className="space-y-3">
            {editedPlan.features.map((feature, index) => (
              <li key={index} className="flex items-start gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5 text-green-500 mt-0.5 flex-shrink-0" viewBox="0 0 20 20" fill="currentColor">
                  <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                </svg>
                
                {isEditing ? (
                  <div className="flex-1">
                    <div className="flex gap-2">
                      <input
                        type="text"
                        value={feature}
                        onChange={(e) => handleFeatureChange(index, e.target.value)}
                        className={cn(
                          "flex-1 rounded px-2 py-1 transition-colors duration-300",
                          isDarkMode ? "bg-gray-700 text-white" : "bg-gray-100 text-gray-900",
                          errors[`feature-${index}`] ? 'border-2 border-red-500' : ''
                        )}
                        placeholder="ميزة جديدة"
                      />
                      <Button 
                        variant="destructive"
                        size="icon"
                        className="w-8 h-8"
                        onClick={() => removeFeature(index)}
                        disabled={editedPlan.features.length <= 1}
                      >
                        <XIcon className="w-4 h-4" />
                      </Button>
                    </div>
                    {errors[`feature-${index}`] && <p className="text-red-500 text-sm mt-1">{errors[`feature-${index}`]}</p>}
                  </div>
                ) : (
                  <span className={cn(
                    "transition-colors duration-300",
                    isDarkMode ? "text-gray-300" : "text-gray-700"
                  )}>
                    {feature}
                  </span>
                )}
              </li>
            ))}
            
            {isEditing && (
              <li className="flex justify-center mt-4">
                <Button 
                  variant="outline"
                  className={cn(
                    "text-sm py-1 px-3 transition-colors duration-300",
                    isDarkMode
                      ? "border-gray-600 text-gray-300 hover:bg-gray-700"
                      : "border-gray-300 text-gray-700 hover:bg-gray-100"
                  )}
                  onClick={addFeature}
                >
                  + إضافة ميزة
                </Button>
              </li>
            )}
          </ul>
          
          {isEditing ? (
            <div className="flex gap-2 mt-6">
              <Button 
                className={cn(
                  "flex-1 transition-colors duration-300 hover:scale-105",
                  isDarkMode
                    ? "bg-green-600 hover:bg-green-700 text-white"
                    : "bg-green-600 hover:bg-green-700 text-white"
                )}
                onClick={handleSave}
              >
                <CheckIcon className="w-4 h-4 mr-2" />
                حفظ
              </Button>
              <Button 
                variant="outline"
                className={cn(
                  "flex-1 transition-colors duration-300 hover:scale-105",
                  isDarkMode
                    ? "border-gray-600 text-gray-300 hover:bg-gray-700"
                    : "border-gray-300 text-gray-700 hover:bg-gray-100"
                )}
                onClick={handleCancel}
              >
                <XIcon className="w-4 h-4 mr-2" />
                إلغاء
              </Button>
            </div>
          ) : (
            <Button 
              className={cn(
                "w-full mt-6 transition-colors duration-300 hover:scale-105",
                isSelected 
                  ? "bg-purple-600 hover:bg-purple-700 text-white" 
                  : isDarkMode
                    ? "bg-gray-700 text-gray-300 hover:bg-gray-600"
                    : "bg-gray-200 text-gray-800 hover:bg-gray-300"
              )}
              onClick={() => onSelect(plan.id)}
            >
              {isSelected ? "الخطة المختارة" : "اختر هذه الخطة"}
            </Button>
          )}
        </div>
      </div>

      {/* Delete Confirmation Dialog */}
      <AlertDialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <AlertDialogContent className={cn(
          "transition-colors duration-300",
          isDarkMode ? "bg-gray-800 border-gray-700" : "bg-white border-gray-200"
        )}>
          <AlertDialogHeader>
            <AlertDialogTitle className={cn(
              "transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-900"
            )}>
              تأكيد الحذف
            </AlertDialogTitle>
            <AlertDialogDescription className={cn(
              "transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              هل أنت متأكد من أنك تريد حذف خطة "{plan.name}"؟ هذا الإجراء لا يمكن التراجع عنه.
            </AlertDialogDescription>
          </AlertDialogHeader>
          <AlertDialogFooter>
            <AlertDialogCancel className={cn(
              "transition-colors duration-300",
              isDarkMode
                ? "border-gray-600 text-gray-300 hover:bg-gray-700"
                : "border-gray-300 text-gray-700 hover:bg-gray-100"
            )}>
              إلغاء
            </AlertDialogCancel>
            <AlertDialogAction 
              onClick={confirmDelete}
              className={cn(
                "transition-colors duration-300 hover:scale-105",
                isDarkMode
                  ? "bg-red-600 hover:bg-red-700 text-white"
                  : "bg-red-600 hover:bg-red-700 text-white"
              )}
            >
              حذف
            </AlertDialogAction>
          </AlertDialogFooter>
        </AlertDialogContent>
      </AlertDialog>
    </>
  );
}