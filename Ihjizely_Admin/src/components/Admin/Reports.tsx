import React, { useState, useEffect } from 'react';
import { reportsService, Report } from '../../API/ReportService';

const ReportsTable: React.FC = () => {
  const [reports, setReports] = useState<Report[]>([]);
  const [loading, setLoading] = useState(false);
  const [activeTab, setActiveTab] = useState<'all' | 'unread' | 'read'>('all');
  const [selectedReport, setSelectedReport] = useState<Report | null>(null);

  useEffect(() => {
    loadReports();
  }, [activeTab]);

  const loadReports = async () => {
    setLoading(true);
    try {
      let data: Report[];
      
      switch (activeTab) {
        case 'unread':
          data = await reportsService.getUnreadReports();
          break;
        case 'read':
          data = await reportsService.getReadReports();
          break;
        default:
          data = await reportsService.getAllReports();
      }
      
      setReports(data);
    } catch (error) {
      console.error('Error loading reports:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleMarkAsRead = async (id: string) => {
    try {
      await reportsService.markAsRead(id);
      // Show success message or refresh data
      await loadReports();
      // Optionally switch to read tab
      setActiveTab('read');
    } catch (error) {
      console.error('Error marking report as read:', error);
      // Show error message to user
      alert('فشل في تعيين التقرير كمقروء. يرجى المحاولة مرة أخرى.');
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('هل أنت متأكد من أنك تريد حذف هذا التقرير؟')) {
      try {
        await reportsService.deleteReport(id);
        await loadReports();
      } catch (error) {
        console.error('Error deleting report:', error);
      }
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    });
  };

  const truncateContent = (content: string, maxLength: number = 100) => {
    if (content.length <= maxLength) return content;
    return content.substring(0, maxLength) + '...';
  };

  return (
    <div className="p-6" dir="rtl">
      {/* Tabs */}
      <div className="flex mb-6 border-b border-gray-200">
        <button
          className={`px-6 py-3 font-medium text-sm border-b-2 transition-colors ${
            activeTab === 'all' 
              ? 'border-blue-500 text-blue-600' 
              : 'border-transparent text-gray-500 hover:text-gray-700'
          }`}
          onClick={() => setActiveTab('all')}
        >
          جميع التقارير
        </button>
        <button
          className={`px-6 py-3 font-medium text-sm border-b-2 transition-colors ${
            activeTab === 'unread' 
              ? 'border-blue-500 text-blue-600' 
              : 'border-transparent text-gray-500 hover:text-gray-700'
          }`}
          onClick={() => setActiveTab('unread')}
        >
          غير المقروء
        </button>
        <button
          className={`px-6 py-3 font-medium text-sm border-b-2 transition-colors ${
            activeTab === 'read' 
              ? 'border-blue-500 text-blue-600' 
              : 'border-transparent text-gray-500 hover:text-gray-700'
          }`}
          onClick={() => setActiveTab('read')}
        >
          المقروء
        </button>
      </div>

      {/* Reports Table */}
      <div className="bg-white rounded-lg shadow-sm border border-gray-200 overflow-hidden">
        {loading ? (
          <div className="flex justify-center items-center py-12 text-gray-500">
            جاري تحميل التقارير...
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y divide-gray-200">
              <thead className="bg-gray-50">
                <tr>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    اسم المستخدم
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    رقم الهاتف
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    المحتوى
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    التاريخ
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    الحالة
                  </th>
                  <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                    الإجراءات
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {reports.map((report) => (
                  <tr 
                    key={report.id} 
                    className={`hover:bg-gray-50 cursor-pointer transition-colors ${
                      !report.isRead ? 'bg-yellow-50 hover:bg-yellow-100' : ''
                    }`}
                    onClick={() => setSelectedReport(report)}
                  >
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium text-gray-900">
                      {report.userName || 'غير متوفر'}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {report.phoneNumber || 'غير متوفر'}
                    </td>
                    <td className="px-6 py-4 text-sm text-gray-900 max-w-md">
                      <span title={report.content}>
                        {truncateContent(report.content)}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                      {formatDate(report.createdAt)}
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        report.isRead 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }`}>
                        {report.isRead ? 'مقروء' : 'غير مقروء'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-sm font-medium">
                      <div className="flex space-x-2 justify-end">
                        {!report.isRead && (
                          <button
                            onClick={(e) => {
                              e.stopPropagation();
                              handleMarkAsRead(report.id);
                            }}
                            className="text-green-600 hover:text-green-900 bg-green-50 hover:bg-green-100 px-3 py-1 rounded text-xs font-medium transition-colors"
                          >
                            تعيين كمقروء
                          </button>
                        )}
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDelete(report.id);
                          }}
                          className="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded text-xs font-medium transition-colors"
                        >
                          حذف
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {reports.length === 0 && !loading && (
          <div className="flex justify-center items-center py-12 text-gray-500">
            لم يتم العثور على تقارير
          </div>
        )}
      </div>

      {/* Report Detail Modal */}
      {selectedReport && (
        <div 
          className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50"
          onClick={() => setSelectedReport(null)}
        >
          <div 
            className="bg-white rounded-lg max-w-2xl w-full max-h-[90vh] overflow-y-auto"
            onClick={(e) => e.stopPropagation()}
            dir="rtl"
          >
            <div className="flex justify-between items-center px-6 py-4 border-b border-gray-200">
              <h3 className="text-lg font-semibold text-gray-900">تفاصيل التقرير</h3>
              <button 
                onClick={() => setSelectedReport(null)}
                className="text-gray-400 hover:text-gray-600 text-2xl font-light"
              >
                ×
              </button>
            </div>
            
            <div className="px-6 py-4 space-y-4">
              <div className="grid grid-cols-1 gap-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    اسم المستخدم:
                  </label>
                  <span className="text-sm text-gray-900">{selectedReport.userName || 'غير متوفر'}</span>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    رقم الهاتف:
                  </label>
                  <span className="text-sm text-gray-900">{selectedReport.phoneNumber || 'غير متوفر'}</span>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    السبب:
                  </label>
                  <span className="text-sm text-gray-900">{selectedReport.reason}</span>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    المحتوى:
                  </label>
                  <p className="text-sm text-gray-900 bg-gray-50 p-3 rounded-md mt-1 text-right">
                    {selectedReport.content}
                  </p>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    التاريخ:
                  </label>
                  <span className="text-sm text-gray-900">{formatDate(selectedReport.createdAt)}</span>
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">
                    الحالة:
                  </label>
                  <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                    selectedReport.isRead 
                      ? 'bg-green-100 text-green-800' 
                      : 'bg-red-100 text-red-800'
                  }`}>
                    {selectedReport.isRead ? 'مقروء' : 'غير مقروء'}
                  </span>
                </div>
              </div>
            </div>
            
            <div className="px-6 py-4 border-t border-gray-200 flex justify-end space-x-3 space-x-reverse">
              {!selectedReport.isRead && (
                <button
                  onClick={() => {
                    handleMarkAsRead(selectedReport.id);
                    setSelectedReport(null);
                  }}
                  className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors"
                >
                  تعيين كمقروء
                </button>
              )}
              <button
                onClick={() => {
                  handleDelete(selectedReport.id);
                  setSelectedReport(null);
                }}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors"
              >
                حذف التقرير
              </button>
              <button 
                onClick={() => setSelectedReport(null)}
                className="bg-gray-300 hover:bg-gray-400 text-gray-700 px-4 py-2 rounded text-sm font-medium transition-colors"
              >
                إغلاق
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ReportsTable;