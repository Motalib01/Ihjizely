import React, { useState, useEffect } from 'react';
import { locationsService } from '@/API/LocationService';
import { DownloadCloudIcon, MoreVertical } from 'lucide-react';
import { Button } from '../ui/button';
import { Dialog, DialogContent, DialogDescription, DialogOverlay, DialogPortal, DialogTitle } from '@radix-ui/react-dialog';
import { Input } from '../ui/input';
import { Label } from '../ui/label';
import { toast } from 'sonner';
import { DataTable } from '../data-table';
import { ColumnDef } from '@tanstack/react-table';
import { IconLocationPlus, IconTrash } from '@tabler/icons-react';
import { EditIcon } from 'lucide-react';
import { DialogHeader, DialogFooter } from '../ui/dialog';
import { useDarkMode } from '../DarkModeContext';
import { cn } from '@/lib/utils';

export interface LocationRow {
  id: string;
  city: string;
  state: string;
  country: string;
}

type TabType = 'cities' | 'states' | 'state-cities';

type TableDataItem = {
  id: string;
  name: string;
  state?: string;
  cities?: string[];
  citiesDisplay?: string;
  type: string;
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

export default function Locations() {
  const { isDarkMode } = useDarkMode();
  const [locations, setLocations] = useState<LocationRow[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [searchQuery, setSearchQuery] = useState('');
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [isAddLocationModalOpen, setIsAddLocationModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  const [isDeleteDialogOpen, setIsDeleteDialogOpen] = useState(false);
  const [locationToDelete, setLocationToDelete] = useState<LocationRow | null>(null);
  const [currentLocation, setCurrentLocation] = useState<LocationRow | null>(null);
  const [activeTab, setActiveTab] = useState<TabType>('cities');
  const [formData, setFormData] = useState({
    city: '',
    state: '',
    country: 'Libya',
    isNewState: false,
    newState: ''
  });

  useEffect(() => {
    const fetchLocations = async () => {
      try {
        setLoading(true);
        const data = await locationsService.getAllLocations();
        setLocations(data);
      } catch (err) {
        setError(err instanceof Error ? err.message : 'Failed to fetch locations');
        toast.error('Failed to load locations');
      } finally {
        setLoading(false);
      }
    };

    fetchLocations();
  }, []);

  // Get unique states for dropdown
  const uniqueStates = Array.from(new Set(locations.map(location => location.state)));

  const filteredLocations = locations.filter(location => {
    const searchLower = searchQuery.toLowerCase();
    return (
      location.city.toLowerCase().includes(searchLower) ||
      location.state.toLowerCase().includes(searchLower) ||
      location.country.toLowerCase().includes(searchLower)
    );
  });

  // Group locations by state for the state-cities tab
  const locationsByState = filteredLocations.reduce((acc, location) => {
    if (!acc[location.state]) {
      acc[location.state] = [];
    }
    acc[location.state].push(location);
    return acc;
  }, {} as Record<string, LocationRow[]>);

  // Prepare data for different tabs
  const getTableData = (): TableDataItem[] => {
    switch (activeTab) {
      case 'cities':
        return filteredLocations.map(location => ({
          id: location.id,
          name: location.city,
          state: location.state,
          type: 'city'
        }));
      case 'states':
        return uniqueStates.map(state => ({
          id: state,
          name: state,
          type: 'state'
        }));
      case 'state-cities':
        return Object.entries(locationsByState).map(([state, cities]) => ({
          id: state,
          name: state,
          cities: cities.map(c => c.city),
          citiesDisplay: cities.map(c => c.city).join('، '),
          type: 'state-with-cities'
        }));
      default:
        return [];
    }
  };

  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, checked } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: checked,
      // Reset state field when switching between modes
      ...(name === 'isNewState' && checked ? { state: '' } : { newState: '' })
    }));
  };

  const handleAddLocation = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      setLoading(true);
      
      // Use the selected state or the new state value
      const locationData = {
        city: formData.city,
        state: formData.isNewState ? formData.newState : formData.state,
        country: formData.country
      };
      
      await locationsService.addLocation(locationData);
      // Refetch all locations
      const data = await locationsService.getAllLocations();
      setLocations(data);
      toast.success('تم إضافة الموقع بنجاح');
      setFormData({ 
        city: '', 
        state: '', 
        country: 'Libya',
        isNewState: false,
        newState: ''
      });
      setIsAddLocationModalOpen(false);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to add location');
    } finally {
      setLoading(false);
    }
  };

  const handleEditLocation = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!currentLocation) return;
    
    try {
      setLoading(true);
      
      // Use the selected state or the new state value
      const locationData = {
        city: formData.city,
        state: formData.isNewState ? formData.newState : formData.state,
        country: formData.country
      };
      
      await locationsService.updateLocation(currentLocation.id, locationData);
      // Refetch all locations
      const data = await locationsService.getAllLocations();
      setLocations(data);
      toast.success('تم تحديث الموقع بنجاح');
      setCurrentLocation(null);
      setFormData({ 
        city: '', 
        state: '', 
        country: 'Libya',
        isNewState: false,
        newState: ''
      });
      setIsEditModalOpen(false);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to update location');
    } finally {
      setLoading(false);
    }
  };

  const handleDeleteLocation = async () => {
    if (!locationToDelete) return;
    
    try {
      setLoading(true);
      await locationsService.deleteLocation(locationToDelete.id);
      // Refetch all locations
      const data = await locationsService.getAllLocations();
      setLocations(data);
      toast.success('تم حذف الموقع بنجاح');
      setIsDeleteDialogOpen(false);
      setLocationToDelete(null);
    } catch (err) {
      toast.error(err instanceof Error ? err.message : 'Failed to delete location');
    } finally {
      setLoading(false);
    }
  };

  const openDeleteDialog = (location: LocationRow) => {
    setLocationToDelete(location);
    setIsDeleteDialogOpen(true);
  };

  const openEditModal = (location: LocationRow) => {
    setCurrentLocation(location);
    setFormData({
      city: location.city,
      state: location.state,
      country: location.country,
      isNewState: false,
      newState: ''
    });
    setIsEditModalOpen(true);
  };

  // Create different column sets for different tabs
  const getColumns = (): ColumnDef<TableDataItem>[] => {
    switch (activeTab) {
      case 'cities':
        return [
          {
            accessorKey: "name",
            header: "الحي",
            cell: ({ row }) => (
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {row.original.name}
              </span>
            ),
          },
          {
            accessorKey: "state",
            header: "المدينة",
            cell: ({ row }) => (
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-gray-300" : "text-gray-700"
              )}>
                {row.original.state}
              </span>
            ),
          },
          {
            id: "actions",
            header: "الإجراءات",
            cell: ({ row }) => {
              const location = locations.find(l => l.id === row.original.id);
              if (!location) return null;
              
              return (
                <div className="flex items-center gap-2">
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => openEditModal(location)}
                    className={cn(
                      "transition-colors duration-300 hover:scale-110",
                      isDarkMode 
                        ? "hover:bg-gray-700 text-blue-400" 
                        : "hover:bg-gray-100 text-blue-500"
                    )}
                  >
                    <EditIcon className="w-4 h-4" />
                  </Button>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => openDeleteDialog(location)}
                    className={cn(
                      "transition-colors duration-300 hover:scale-110",
                      isDarkMode 
                        ? "hover:bg-gray-700 text-red-400" 
                        : "hover:bg-gray-100 text-red-500"
                    )}
                  >
                    <IconTrash className="w-4 h-4" />
                  </Button>
                </div>
              );
            },
          }
        ];
        
      case 'states':
        return [
          {
            accessorKey: "name",
            header: "المدينة",
            cell: ({ row }) => (
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {row.original.name}
              </span>
            ),
          }
        ];
        
      case 'state-cities':
        return [
          {
            accessorKey: "name",
            header: "المدينة",
            cell: ({ row }) => (
              <span className={cn(
                "transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                {row.original.name}
              </span>
            ),
          },
          {
            accessorKey: "citiesDisplay",
            header: "الأحياء",
            cell: ({ row }) => (
              <div className={cn(
                "text-right transition-colors duration-300",
                isDarkMode ? "text-gray-300" : "text-gray-700"
              )}>
                {row.original.cities?.map((city: string, index: number) => (
                  <span key={city}>
                    {city}
                    {index < (row.original.cities?.length || 0) - 1 && '، '}
                  </span>
                )) || (
                  <span className={cn(
                    "transition-colors duration-300",
                    isDarkMode ? "text-gray-500" : "text-gray-400"
                  )}>
                    لا توجد أحياء
                  </span>
                )}
              </div>
            ),
          }
        ];
        
      default:
        return [];
    }
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
      {/* Add Location Modal */}
      <Dialog open={isAddLocationModalOpen} onOpenChange={setIsAddLocationModalOpen}>
        <DialogPortal>
          <DialogOverlay className={cn(
            "fixed inset-0 backdrop-blur-sm z-[99999] transition-colors duration-300",
            isDarkMode ? "bg-black/60" : "bg-black/50"
          )} />
          <DialogContent className={cn(
            "fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-[90vw] max-w-[450px] max-h-[85vh] p-6 rounded-xl shadow-lg z-[99999] focus:outline-none transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border border-gray-700" : "bg-white border border-gray-200"
          )}>
            <DialogTitle className={cn(
              "text-right text-xl font-bold transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-900"
            )}>
              إضافة موقع جديد
            </DialogTitle>
            <DialogDescription className={cn(
              "text-right mt-2 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              أضف موقع جديد إلى النظام
            </DialogDescription>

            <form onSubmit={handleAddLocation} className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label className={cn(
                  "text-right block transition-colors duration-300",
                  isDarkMode ? "text-gray-300" : "text-gray-700"
                )}>
                  المدينة
                </Label>
                
                <div className="flex items-center gap-2 mb-2">
                  <input
                    type="checkbox"
                    id="isNewState"
                    name="isNewState"
                    checked={formData.isNewState}
                    onChange={handleCheckboxChange}
                    className={cn(
                      "w-4 h-4 transition-colors duration-300",
                      isDarkMode 
                        ? "accent-purple-400 bg-gray-700 border-gray-600" 
                        : "accent-purple-600 bg-white border-gray-300"
                    )}
                  />
                  <Label htmlFor="isNewState" className={cn(
                    "text-sm transition-colors duration-300",
                    isDarkMode ? "text-gray-300" : "text-gray-700"
                  )}>
                    إضافة مدينة جديدة
                  </Label>
                </div>
                
                {!formData.isNewState ? (
                  <select
                    name="state"
                    value={formData.state}
                    onChange={handleInputChange}
                    required
                    className={cn(
                      "w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 text-right transition-colors duration-300",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white focus:ring-purple-400"
                        : "bg-white border-gray-300 text-gray-900 focus:ring-purple-500"
                    )}
                  >
                    <option value="">اختر المدينة</option>
                    {uniqueStates.map(state => (
                      <option key={state} value={state}>{state}</option>
                    ))}
                  </select>
                ) : (
                  <Input
                    name="newState"
                    value={formData.newState}
                    onChange={handleInputChange}
                    required
                    className={cn(
                      "text-right transition-colors duration-300",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white"
                        : "bg-white border-gray-300 text-gray-900"
                    )}
                    placeholder="أدخل اسم مدينة جديدة"
                  />
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="city" className={cn(
                  "text-right block transition-colors duration-300",
                  isDarkMode ? "text-gray-300" : "text-gray-700"
                )}>
                  الحي
                </Label>
                <Input
                  id="city"
                  name="city"
                  value={formData.city}
                  onChange={handleInputChange}
                  required
                  className={cn(
                    "text-right transition-colors duration-300",
                    isDarkMode
                      ? "bg-gray-700 border-gray-600 text-white"
                      : "bg-white border-gray-300 text-gray-900"
                  )}
                  placeholder="أدخل اسم الحي"
                />
              </div>

              <div className="space-y-2 hidden">
                <Label htmlFor="country" className={cn(
                  "text-right block transition-colors duration-300",
                  isDarkMode ? "text-gray-300" : "text-gray-700"
                )}>
                  البلد
                </Label>
                <Input
                  id="country"
                  name="country"
                  value="Libya"
                  onChange={handleInputChange}
                  required
                  className={cn(
                    "text-right transition-colors duration-300",
                    isDarkMode
                      ? "bg-gray-700 border-gray-600 text-white"
                      : "bg-white border-gray-300 text-gray-900"
                  )}
                  readOnly
                />
              </div>

              <div className="flex justify-between pt-4">
                <Button 
                  type="button" 
                  variant="outline"
                  onClick={() => setIsAddLocationModalOpen(false)}
                  className={cn(
                    "transition-colors duration-300",
                    isDarkMode
                      ? "border-gray-600 text-gray-300 hover:bg-gray-700"
                      : "border-gray-300 text-gray-700 hover:bg-gray-100"
                  )}
                >
                  إلغاء
                </Button>
                <Button type="submit" className={cn(
                  "transition-colors duration-300 hover:scale-105",
                  isDarkMode
                    ? "bg-purple-600 hover:bg-purple-700 text-white"
                    : "bg-purple-600 hover:bg-purple-700 text-white"
                )}>
                  إضافة
                </Button>
              </div>
            </form>
          </DialogContent>
        </DialogPortal>
      </Dialog>

      {/* Edit Location Modal */}
      <Dialog open={isEditModalOpen} onOpenChange={setIsEditModalOpen}>
        <DialogPortal>
          <DialogOverlay className={cn(
            "fixed inset-0 backdrop-blur-sm z-[99999] transition-colors duration-300",
            isDarkMode ? "bg-black/60" : "bg-black/50"
          )} />
          <DialogContent className={cn(
            "fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-[90vw] max-w-[450px] max-h-[85vh] p-6 rounded-xl shadow-lg z-[99999] focus:outline-none transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border border-gray-700" : "bg-white border border-gray-200"
          )}>
            <DialogTitle className={cn(
              "text-right text-xl font-bold transition-colors duration-300",
              isDarkMode ? "text-white" : "text-gray-900"
            )}>
              تعديل الموقع
            </DialogTitle>
            <DialogDescription className={cn(
              "text-right mt-2 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              تعديل بيانات الموقع
            </DialogDescription>

            <form onSubmit={handleEditLocation} className="space-y-4 mt-4">
              <div className="space-y-2">
                <Label className={cn(
                  "text-right block transition-colors duration-300",
                  isDarkMode ? "text-gray-300" : "text-gray-700"
                )}>
                  المدينة
                </Label>
                
                <div className="flex items-center gap-2 mb-2">
                  <input
                    type="checkbox"
                    id="edit-isNewState"
                    name="isNewState"
                    checked={formData.isNewState}
                    onChange={handleCheckboxChange}
                    className={cn(
                      "w-4 h-4 transition-colors duration-300",
                      isDarkMode 
                        ? "accent-purple-400 bg-gray-700 border-gray-600" 
                        : "accent-purple-600 bg-white border-gray-300"
                    )}
                  />
                  <Label htmlFor="edit-isNewState" className={cn(
                    "text-sm transition-colors duration-300",
                    isDarkMode ? "text-gray-300" : "text-gray-700"
                  )}>
                    إضافة مدينة جديدة
                  </Label>
                </div>
                
                {!formData.isNewState ? (
                  <select
                    name="state"
                    value={formData.state}
                    onChange={handleInputChange}
                    required
                    className={cn(
                      "w-full px-3 py-2 border rounded-lg focus:outline-none focus:ring-2 text-right transition-colors duration-300",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white focus:ring-purple-400"
                        : "bg-white border-gray-300 text-gray-900 focus:ring-purple-500"
                    )}
                  >
                    <option value="">اختر المدينة</option>
                    {uniqueStates.map(state => (
                      <option key={state} value={state}>{state}</option>
                    ))}
                  </select>
                ) : (
                  <Input
                    name="newState"
                    value={formData.newState}
                    onChange={handleInputChange}
                    required
                    className={cn(
                      "text-right transition-colors duration-300",
                      isDarkMode
                        ? "bg-gray-700 border-gray-600 text-white"
                        : "bg-white border-gray-300 text-gray-900"
                    )}
                    placeholder="أدخل اسم مدينة جديدة"
                  />
                )}
              </div>

              <div className="space-y-2">
                <Label htmlFor="edit-city" className={cn(
                  "text-right block transition-colors duration-300",
                  isDarkMode ? "text-gray-300" : "text-gray-700"
                )}>
                  الحي
                </Label>
                <Input
                  id="edit-city"
                  name="city"
                  value={formData.city}
                  onChange={handleInputChange}
                  required
                  className={cn(
                    "text-right transition-colors duration-300",
                    isDarkMode
                      ? "bg-gray-700 border-gray-600 text-white"
                      : "bg-white border-gray-300 text-gray-900"
                  )}
                  placeholder="أدخل اسم الحي"
                />
              </div>

              <div className="flex justify-between pt-4">
                <Button 
                  type="button" 
                  variant="outline"
                  onClick={() => setIsEditModalOpen(false)}
                  className={cn(
                    "transition-colors duration-300",
                    isDarkMode
                      ? "border-gray-600 text-gray-300 hover:bg-gray-700"
                      : "border-gray-300 text-gray-700 hover:bg-gray-100"
                  )}
                >
                  إلغاء
                </Button>
                <Button type="submit" className={cn(
                  "transition-colors duration-300 hover:scale-105",
                  isDarkMode
                    ? "bg-purple-600 hover:bg-purple-700 text-white"
                    : "bg-purple-600 hover:bg-purple-700 text-white"
                )}>
                  حفظ التغييرات
                </Button>
              </div>
            </form>
          </DialogContent>
        </DialogPortal>
      </Dialog>

      {/* Delete Confirmation Dialog */}
      <Dialog open={isDeleteDialogOpen} onOpenChange={setIsDeleteDialogOpen}>
        <DialogPortal>
          <DialogOverlay className={cn(
            "fixed inset-0 backdrop-blur-sm z-[99999] transition-colors duration-300",
            isDarkMode ? "bg-black/60" : "bg-black/50"
          )} />
          <DialogContent className={cn(
            "fixed left-1/2 top-1/2 -translate-x-1/2 -translate-y-1/2 w-[90vw] max-w-[450px] max-h-[85vh] p-6 rounded-xl shadow-lg z-[99999] focus:outline-none transition-colors duration-300",
            isDarkMode ? "bg-gray-800 border border-gray-700" : "bg-white border border-gray-200"
          )}>
            <DialogHeader>
              <DialogTitle className={cn(
                "text-right text-xl font-bold transition-colors duration-300",
                isDarkMode ? "text-white" : "text-gray-900"
              )}>
                تأكيد الحذف
              </DialogTitle>
              <DialogDescription className={cn(
                "text-right mt-2 transition-colors duration-300",
                isDarkMode ? "text-gray-400" : "text-gray-600"
              )}>
                هل أنت متأكد من رغبتك في حذف الموقع؟
              </DialogDescription>
            </DialogHeader>

            <div className={cn(
              "mt-4 p-4 rounded-lg text-right transition-colors duration-300",
              isDarkMode ? "bg-gray-700 text-gray-300" : "bg-gray-50 text-gray-700"
            )}>
              {locationToDelete && (
                <>
                  <p className="font-medium">الحي: {locationToDelete.city}</p>
                  <p className="font-medium">المدينة: {locationToDelete.state}</p>
                  <p className={cn(
                    "mt-2 transition-colors duration-300",
                    isDarkMode ? "text-gray-400" : "text-gray-600"
                  )}>
                    هذا الإجراء لا يمكن التراجع عنه.
                  </p>
                </>
              )}
            </div>

            <DialogFooter className="flex justify-between pt-4">
              <Button 
                type="button" 
                variant="outline"
                onClick={() => setIsDeleteDialogOpen(false)}
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
                type="button" 
                className={cn(
                  "transition-colors duration-300 hover:scale-105",
                  isDarkMode
                    ? "bg-red-600 hover:bg-red-700 text-white"
                    : "bg-red-600 hover:bg-red-700 text-white"
                )}
                onClick={handleDeleteLocation}
              >
                حذف
              </Button>
            </DialogFooter>
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
              المواقع
            </h1>
            <p className={cn(
              "mt-1 transition-colors duration-300",
              isDarkMode ? "text-gray-400" : "text-gray-600"
            )}>
              إدارة جميع المواقع في النظام
            </p>
          </div>
          <Button 
            className={cn(
              "px-4 py-2 rounded-lg flex items-center justify-between gap-2 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode
                ? "bg-purple-600 hover:bg-purple-700 text-white"
                : "bg-purple-600 hover:bg-purple-700 text-white"
            )}
            onClick={() => setIsAddLocationModalOpen(true)}
          >
            <span>أضف جديد</span>
            <IconLocationPlus className={cn(
              "transition-colors duration-300",
              isDarkMode ? "text-white" : "text-white"
            )}/>
          </Button>
        </div>

        {/* Tabs */}
        <div className={cn(
          "flex border-b w-full transition-colors duration-300",
          isDarkMode ? "border-gray-700" : "border-gray-200"
        )}>
          {(['cities', 'states', 'state-cities'] as TabType[]).map((tab) => (
            <button
              key={tab}
              className={cn(
                "py-2 px-4 font-medium transition-colors duration-300",
                activeTab === tab 
                  ? cn(
                      "border-b-2 text-purple-600",
                      isDarkMode ? "border-purple-400 text-purple-400" : "border-purple-600 text-purple-600"
                    )
                  : isDarkMode 
                    ? "text-gray-400 hover:text-gray-300" 
                    : "text-gray-500 hover:text-gray-700"
              )}
              onClick={() => setActiveTab(tab)}
            >
              {tab === 'cities' && 'الأحياء'}
              {tab === 'states' && 'المدن'}
              {tab === 'state-cities' && 'المدن مع أحيائها'}
            </button>
          ))}
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
            onClick={() => setIsFilterOpen(!isFilterOpen)}
            className={cn(
              "px-4 py-2 rounded-lg flex items-center gap-1 cursor-pointer transition-all duration-300 hover:scale-105",
              isDarkMode
                ? "bg-purple-600/20 text-purple-400 hover:bg-purple-600/30"
                : "bg-purple-100 text-purple-600 hover:bg-purple-200"
            )}
          >
            فلتر
            <DownloadCloudIcon size={18}/>
          </button>

          <button className={cn(
            "p-2 rounded-lg transition-colors duration-300 hover:scale-105",
            isDarkMode
              ? "bg-gray-700 text-gray-300 hover:bg-gray-600"
              : "bg-gray-100 text-gray-600 hover:bg-gray-200"
          )}>
            <MoreVertical size={18}/>
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
            {/* Filter options can be added here */}
          </div>
        </div>
      )}
      
      <div className={cn(
        "rounded-md border transition-colors duration-300",
        isDarkMode ? "border-gray-700" : "border-gray-200"
      )}>
        <DataTable data={getTableData()} columns={getColumns()} />
      </div>
    </div>
  );
};