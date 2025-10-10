import React, { useState, useEffect } from 'react';
import { reportsService, Report } from '../../API/ReportService';
import { useDarkMode } from '../DarkModeContext';

const ReportsTable: React.FC = () => {
  const { isDarkMode } = useDarkMode();
  const [reports, setReports] = useState<Report[]>([]);
  const [allReports, setAllReports] = useState<Report[]>([]); // Store all reports from backend
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [activeTab, setActiveTab] = useState<'all' | 'unread' | 'read'>('all');
  const [selectedReport, setSelectedReport] = useState<Report | null>(null);
  const [actionLoading, setActionLoading] = useState<string | null>(null);
  const [replyContent, setReplyContent] = useState('');
  const [replyLoading, setReplyLoading] = useState(false);
  const [searchTerm, setSearchTerm] = useState('');

  useEffect(() => {
    loadAllReports();
  }, []);

  // Load all reports from backend
  const loadAllReports = async () => {
    setLoading(true);
    setError(null);
    
    try {
      console.log('Loading all reports from backend...');
      const data = await reportsService.getAllReports();
      console.log(`Loaded ${data.length} reports from backend`);
      setAllReports(data);
      filterReportsByTab(data, activeTab);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'حدث خطأ غير متوقع';
      setError(errorMessage);
      console.error('Error loading reports:', err);
    } finally {
      setLoading(false);
    }
  };

  // Filter reports based on active tab
  const filterReportsByTab = (reportsList: Report[], tab: 'all' | 'unread' | 'read') => {
    let filteredData: Report[];
    
    switch (tab) {
      case 'unread':
        filteredData = reportsList.filter(report => !report.isRead);
        break;
      case 'read':
        filteredData = reportsList.filter(report => report.isRead);
        break;
      default:
        filteredData = reportsList;
    }
    
    console.log(`Filtered ${filteredData.length} reports for tab: ${tab}`);
    setReports(filteredData);
  };

  // Handle tab changes
  useEffect(() => {
    filterReportsByTab(allReports, activeTab);
  }, [activeTab, allReports]);

  const loadRepliesForSelectedReport = async () => {
    if (!selectedReport) return;
    
    try {
      const updatedReport = await reportsService.getReportById(selectedReport.id);
      setSelectedReport(updatedReport);
      
      // Also update the report in our local state
      setAllReports(prev => 
        prev.map(report => 
          report.id === updatedReport.id ? updatedReport : report
        )
      );
    } catch (error) {
      console.error('Error loading replies for selected report:', error);
    }
  };

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!searchTerm.trim()) {
      filterReportsByTab(allReports, activeTab);
      return;
    }

    setLoading(true);
    setError(null);
    
    try {
      const data = await reportsService.searchReports(searchTerm);
      filterReportsByTab(data, activeTab);
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'فشل في البحث';
      setError(errorMessage);
      console.error('Error searching reports:', err);
      
      // Fallback to client-side search
      const searchResults = allReports.filter(report => 
        report.content?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.reason?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.userName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.phoneNumber?.includes(searchTerm)
      );
      
      filterReportsByTab(searchResults, activeTab);
    } finally {
      setLoading(false);
    }
  };

  const handleClearSearch = () => {
    setSearchTerm('');
    filterReportsByTab(allReports, activeTab);
  };

  const handleMarkAsRead = async (id: string) => {
    setActionLoading(id);
    setError(null);
    
    try {
      await reportsService.markAsRead(id);
      
      // Update both allReports and filtered reports
      const updatedAllReports = allReports.map(report => 
        report.id === id ? { ...report, isRead: true } : report
      );
      
      setAllReports(updatedAllReports);
      filterReportsByTab(updatedAllReports, activeTab);
      
      if (selectedReport?.id === id) {
        setSelectedReport(prev => prev ? { ...prev, isRead: true } : null);
      }
      
      console.log('✅ Report marked as read successfully');
      
    } catch (err) {
      let errorMessage = 'فشل في تعيين التقرير كمقروء';
      
      if (err instanceof Error) {
        errorMessage = err.message;
      } else if (typeof err === 'string') {
        errorMessage = err;
      } else if (err && typeof err === 'object' && 'message' in err) {
        errorMessage = String((err as any).message);
      }
      
      if (!errorMessage.includes('updated locally')) {
        setError(errorMessage);
      }
      
      console.error('Error marking report as read:', err);
      
      // Optimistic update
      const updatedAllReports = allReports.map(report => 
        report.id === id ? { ...report, isRead: true } : report
      );
      
      setAllReports(updatedAllReports);
      filterReportsByTab(updatedAllReports, activeTab);
      
      if (selectedReport?.id === id) {
        setSelectedReport(prev => prev ? { ...prev, isRead: true } : null);
      }
    } finally {
      setActionLoading(null);
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('هل أنت متأكد من أنك تريد حذف هذا التقرير؟')) {
      setActionLoading(`delete-${id}`);
      setError(null);
      
      try {
        await reportsService.deleteReport(id);
        // Reload all reports from backend to ensure consistency
        await loadAllReports();
        
        if (selectedReport?.id === id) {
          setSelectedReport(null);
        }
        
      } catch (err) {
        const errorMessage = err instanceof Error ? err.message : 'فشل في حذف التقرير';
        setError(errorMessage);
        console.error('Error deleting report:', err);
      } finally {
        setActionLoading(null);
      }
    }
  };

  const handleAddReply = async (reportId: string) => {
    if (!replyContent.trim()) {
      setError('يرجى إدخال محتوى الرد');
      return;
    }
  
    setReplyLoading(true);
    setError(null);
    
    try {
      await reportsService.addReply(reportId, replyContent);
      
      if (selectedReport && selectedReport.id === reportId) {
        const updatedReport = await reportsService.getReportById(reportId);
        setSelectedReport(updatedReport);
        
        // Update the report in our local state
        setAllReports(prev => 
          prev.map(report => 
            report.id === updatedReport.id ? updatedReport : report
          )
        );
      }
      
      setReplyContent('');
      
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'فشل في إضافة الرد';
      setError(errorMessage);
      console.error('Error adding reply:', err);
    } finally {
      setReplyLoading(false);
    }
  };

  const handleMarkAllAsRead = async () => {
    if (allReports.length === 0) return;
    
    const unreadReports = allReports.filter(report => !report.isRead);
    if (unreadReports.length === 0) {
      setError('لا توجد تقارير غير مقروء');
      return;
    }

    if (!window.confirm(`هل تريد تعيين جميع التقارير غير المقروء (${unreadReports.length}) كمقروء؟`)) {
      return;
    }

    setActionLoading('mark-all');
    setError(null);

    try {
      // Mark all unread reports as read
      for (const report of unreadReports) {
        await reportsService.markAsRead(report.id);
      }

      // Update local state
      const updatedAllReports = allReports.map(report => 
        ({ ...report, isRead: true })
      );
      
      setAllReports(updatedAllReports);
      filterReportsByTab(updatedAllReports, activeTab);

      if (selectedReport) {
        setSelectedReport(prev => prev ? { ...prev, isRead: true } : null);
      }

    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'فشل في تعيين بعض التقارير كمقروء';
      setError(errorMessage);
      console.error('Error marking all reports as read:', err);
    } finally {
      setActionLoading(null);
    }
  };

  const formatDate = (dateString: string) => {
    try {
      return new Date(dateString).toLocaleDateString('ar-EG', {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
      });
    } catch {
      return dateString;
    }
  };

  const truncateContent = (content: string, maxLength: number = 100) => {
    if (!content) return 'لا يوجد محتوى';
    if (content.length <= maxLength) return content;
    return content.substring(0, maxLength) + '...';
  };

  const getUnreadCount = () => allReports.filter(report => !report.isRead).length;
  const getReadCount = () => allReports.filter(report => report.isRead).length;

  const getRepliesFromReport = (report: Report) => {
    const replies = [];
    
    if (report.replay) {
      replies.push({
        id: `reply-${report.id}`,
        reportId: report.id,
        content: report.replay,
        createdAt: report.createdAt,
        createdBy: 'Admin',
        isAdminReply: true
      });
    }
    
    if (report.replies && report.replies.length > 0) {
      replies.push(...report.replies);
    }
    
    return replies;
  };

  const debugReports = () => {
    console.log('All reports:', allReports);
    console.log('Filtered reports:', reports);
    console.log('Unread count:', getUnreadCount());
    console.log('Read count:', getReadCount());
  };

  // Dark mode classes
  const bgPrimary = isDarkMode ? 'bg-gray-900' : 'bg-gray-50';
  const bgCard = isDarkMode ? 'bg-gray-800' : 'bg-white';
  const bgHover = isDarkMode ? 'hover:bg-gray-700' : 'hover:bg-gray-50';
  const textPrimary = isDarkMode ? 'text-white' : 'text-gray-900';
  const textSecondary = isDarkMode ? 'text-gray-300' : 'text-gray-600';
  const textMuted = isDarkMode ? 'text-gray-400' : 'text-gray-500';
  const borderColor = isDarkMode ? 'border-gray-700' : 'border-gray-200';
  const bgHeader = isDarkMode ? 'bg-gray-800' : 'bg-gray-50';
  const bgError = isDarkMode ? 'bg-red-900' : 'bg-red-100';
  const textError = isDarkMode ? 'text-red-200' : 'text-red-700';
  const borderError = isDarkMode ? 'border-red-800' : 'border-red-400';
  const bgStatsCard = isDarkMode ? 'bg-gray-800' : 'bg-white';
  const bgGray = isDarkMode ? 'bg-gray-900' : 'bg-gray-100';
  const bgYellowHover = isDarkMode ? 'hover:bg-grey-800' : 'hover:bg-grey-100';
  const bgGreenLight = isDarkMode ? 'bg-green-900' : 'bg-green-50';
  const bgRedLight = isDarkMode ? 'bg-red-900' : 'bg-red-50';
  const bgBlueLight = isDarkMode ? 'bg-blue-900' : 'bg-blue-50';
  const bgReply = isDarkMode ? 'bg-blue-900' : 'bg-blue-50';

  const handleTabChange = (tab: 'all' | 'unread' | 'read') => {
    setActiveTab(tab);
  };

  return (
    <div className={`p-6 min-h-screen transition-colors duration-300 ${bgPrimary} ${textPrimary}`} dir="rtl">
      {/* Header */}
      <div className="mb-6">
        <h1 className="text-2xl font-bold">إدارة التقارير</h1>
        <p className={`mt-1 ${textSecondary}`}>إدارة وتتبع تقارير المستخدمين</p>
      </div>

      {/* Debug button - remove in production */}
      <button 
        onClick={debugReports}
        className={`mb-4 px-3 py-1 rounded text-xs transition-colors ${
          isDarkMode 
            ? 'bg-gray-700 text-gray-300 hover:bg-gray-600' 
            : 'bg-gray-500 text-white hover:bg-gray-600'
        }`}
      >
        Debug Reports
      </button>

      {/* Error Display */}
      {error && (
        <div className={`mb-4 border px-4 py-3 rounded relative ${bgError} ${borderError} ${textError}`}>
          <strong className="font-bold">خطأ: </strong>
          <span className="block sm:inline">{error}</span>
          <button 
            onClick={() => setError(null)}
            className={`absolute top-0 bottom-0 left-0 px-4 py-3 ${
              isDarkMode ? 'text-red-300 hover:text-red-100' : 'text-red-700 hover:text-red-900'
            }`}
          >
            ×
          </button>
        </div>
      )}

      {/* Search Bar */}
      <div className={`rounded-lg shadow-sm border p-4 mb-6 transition-colors ${bgCard} ${borderColor}`}>
        <form onSubmit={handleSearch} className="flex gap-2">
          <div className="flex-1">
            <input
              type="text"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              placeholder="ابحث في التقارير بالاسم أو المحتوى أو السبب..."
              className={`w-full px-4 py-2 border rounded-lg transition-colors ${
                isDarkMode 
                  ? 'bg-gray-700 border-gray-600 text-white placeholder-gray-400' 
                  : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500'
              } focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
            />
          </div>
          <button
            type="submit"
            disabled={loading}
            className="bg-blue-600 hover:bg-blue-700 text-white px-6 py-2 rounded-lg transition-colors disabled:opacity-50 flex items-center gap-2"
          >
            <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
            </svg>
            بحث
          </button>
          {searchTerm && (
            <button
              type="button"
              onClick={handleClearSearch}
              className="bg-gray-500 hover:bg-gray-600 text-white px-4 py-2 rounded-lg transition-colors"
            >
              إلغاء
            </button>
          )}
        </form>
      </div>

      {/* Stats Cards */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
        <div className={`rounded-lg shadow-sm border p-4 transition-colors ${bgStatsCard} ${borderColor}`}>
          <div className="flex items-center">
            <div className={`p-3 rounded-lg ${bgBlueLight}`}>
              <svg className="w-6 h-6 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div className="mr-3">
              <p className={`text-sm font-medium ${textSecondary}`}>إجمالي التقارير</p>
              <p className="text-2xl font-bold">{allReports.length}</p>
            </div>
          </div>
        </div>

        <div className={`rounded-lg shadow-sm border p-4 transition-colors ${bgStatsCard} ${borderColor}`}>
          <div className="flex items-center">
            <div className={`p-3 rounded-lg ${bgRedLight}`}>
              <svg className="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L4.082 15.5c-.77.833.192 2.5 1.732 2.5z" />
              </svg>
            </div>
            <div className="mr-3">
              <p className={`text-sm font-medium ${textSecondary}`}>غير المقروء</p>
              <p className="text-2xl font-bold">{getUnreadCount()}</p>
            </div>
          </div>
        </div>

        <div className={`rounded-lg shadow-sm border p-4 transition-colors ${bgStatsCard} ${borderColor}`}>
          <div className="flex items-center">
            <div className={`p-3 rounded-lg ${bgGreenLight}`}>
              <svg className="w-6 h-6 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div className="mr-3">
              <p className={`text-sm font-medium ${textSecondary}`}>المقروء</p>
              <p className="text-2xl font-bold">{getReadCount()}</p>
            </div>
          </div>
        </div>
      </div>

      {/* Controls Bar */}
      <div className={`rounded-lg shadow-sm border p-4 mb-6 transition-colors ${bgCard} ${borderColor}`}>
        <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
          {/* Tabs */}
          <div className={`flex border-b w-full sm:w-auto ${borderColor}`}>
            <button
              className={`px-4 py-2 font-medium text-sm border-b-2 transition-colors ${
                activeTab === 'all' 
                  ? 'border-blue-500 text-blue-600' 
                  : `${textMuted} border-transparent hover:${textPrimary}`
              }`}
              onClick={() => handleTabChange('all')}
            >
              جميع التقارير ({allReports.length})
            </button>
            <button
              className={`px-4 py-2 font-medium text-sm border-b-2 transition-colors ${
                activeTab === 'unread' 
                  ? 'border-blue-500 text-blue-600' 
                  : `${textMuted} border-transparent hover:${textPrimary}`
              }`}
              onClick={() => handleTabChange('unread')}
            >
              غير المقروء ({getUnreadCount()})
            </button>
            <button
              className={`px-4 py-2 font-medium text-sm border-b-2 transition-colors ${
                activeTab === 'read' 
                  ? 'border-blue-500 text-blue-600' 
                  : `${textMuted} border-transparent hover:${textPrimary}`
              }`}
              onClick={() => handleTabChange('read')}
            >
              المقروء ({getReadCount()})
            </button>
          </div>

          {/* Actions */}
          <div className="flex gap-2 w-full sm:w-auto">
            {getUnreadCount() > 0 && (
              <button
                onClick={handleMarkAllAsRead}
                disabled={actionLoading === 'mark-all'}
                className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
              >
                {actionLoading === 'mark-all' ? (
                  <>
                    <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    جاري التعيين...
                  </>
                ) : (
                  <>
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    تعيين الكل كمقروء
                  </>
                )}
              </button>
            )}
            
            <button
              onClick={loadAllReports}
              disabled={loading}
              className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
            >
              {loading ? (
                <>
                  <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                  جاري التحديث...
                </>
              ) : (
                <>
                  <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                  </svg>
                  تحديث
                </>
              )}
            </button>
          </div>
        </div>
      </div>

      {/* Reports Table */}
      <div className={`rounded-lg shadow-sm border overflow-hidden transition-colors ${bgCard} ${borderColor}`}>
        {loading ? (
          <div className={`flex justify-center items-center py-12 ${textMuted}`}>
            <div className="flex flex-col items-center gap-2">
              <div className="w-8 h-8 border-2 border-blue-600 border-t-transparent rounded-full animate-spin"></div>
              <span>جاري تحميل التقارير...</span>
            </div>
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="min-w-full divide-y transition-colors" style={{ borderColor: isDarkMode ? '#374151' : '#e5e7eb' }}>
              <thead className={`transition-colors ${bgHeader}`}>
                <tr>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    اسم المستخدم
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    رقم الهاتف
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    السبب
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    المحتوى
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    التاريخ
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    الحالة
                  </th>
                  <th className={`px-6 py-3 text-right text-xs font-medium uppercase tracking-wider ${textMuted}`}>
                    الإجراءات
                  </th>
                </tr>
              </thead>
              <tbody className={`divide-y transition-colors ${bgCard}`} style={{ borderColor: isDarkMode ? '#374151' : '#e5e7eb' }}>
                {reports.map((report) => (
                  <tr 
                    key={report.id} 
                    className={`cursor-pointer transition-colors ${bgHover} ${
                      !report.isRead ? `${bgGray} ${bgYellowHover}` : ''
                    }`}
                    onClick={() => setSelectedReport(report)}
                  >
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className={`text-sm font-medium ${textPrimary}`}>
                        {report.userName || 'غير متوفر'}
                      </div>
                      {report.email && (
                        <div className={`text-xs mt-1 ${textMuted}`}>{report.email}</div>
                      )}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-sm ${textPrimary}`}>
                      {report.phoneNumber || 'غير متوفر'}
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-sm ${textPrimary}`}>
                      {report.reason || 'غير محدد'}
                    </td>
                    <td className={`px-6 py-4 text-sm max-w-xs ${textPrimary}`}>
                      <span title={report.content} className="line-clamp-2">
                        {truncateContent(report.content, 80)}
                      </span>
                    </td>
                    <td className={`px-6 py-4 whitespace-nowrap text-sm ${textMuted}`}>
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
                            disabled={actionLoading === report.id}
                            className="text-green-600 hover:text-green-900 bg-green-50 hover:bg-green-100 px-3 py-1 rounded text-xs font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1"
                          >
                            {actionLoading === report.id ? (
                              <div className="w-3 h-3 border-2 border-green-600 border-t-transparent rounded-full animate-spin"></div>
                            ) : (
                              <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                              </svg>
                            )}
                            تعيين كمقروء
                          </button>
                        )}
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            handleDelete(report.id);
                          }}
                          disabled={actionLoading === `delete-${report.id}`}
                          className="text-red-600 hover:text-red-900 bg-red-50 hover:bg-red-100 px-3 py-1 rounded text-xs font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1"
                        >
                          {actionLoading === `delete-${report.id}` ? (
                            <div className="w-3 h-3 border-2 border-red-600 border-t-transparent rounded-full animate-spin"></div>
                          ) : (
                            <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                            </svg>
                          )}
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
          <div className={`flex flex-col justify-center items-center py-12 ${textMuted}`}>
            <svg className="w-16 h-16 mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
            </svg>
            <p className="text-lg font-medium">
              {activeTab === 'unread' ? 'لا توجد تقارير غير مقروء' : 
               activeTab === 'read' ? 'لا توجد تقارير مقروء' : 
               'لم يتم العثور على تقارير'}
            </p>
            <p className="text-sm mt-1">
              {activeTab === 'unread' ? 'جميع التقارير تم قراءتها' : 
               activeTab === 'read' ? 'لا توجد تقارير مقروء بعد' : 
               'لا توجد تقارير متاحة حاليًا'}
            </p>
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
            className={`rounded-lg max-w-4xl w-full max-h-[90vh] overflow-y-auto transition-colors ${bgCard} ${textPrimary}`}
            onClick={(e) => e.stopPropagation()}
            dir="rtl"
          >
            <div className={`flex justify-between items-center px-6 py-4 border-b transition-colors ${borderColor}`}>
              <h3 className="text-lg font-semibold">تفاصيل التقرير</h3>
              <button 
                onClick={() => setSelectedReport(null)}
                className={`text-gray-400 hover:text-gray-600 text-2xl font-light transition-colors ${
                  isDarkMode ? 'hover:text-gray-300' : ''
                }`}
              >
                ×
              </button>
            </div>
            
            <div className="px-6 py-4 space-y-6">
              {/* User Information */}
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div className={`p-4 rounded-lg transition-colors ${isDarkMode ? 'bg-gray-700' : 'bg-gray-50'}`}>
                  <h4 className={`font-medium mb-3 ${textSecondary}`}>معلومات المستخدم</h4>
                  <div className="space-y-2">
                    <div>
                      <label className={`block text-sm ${textMuted}`}>الاسم الكامل:</label>
                      <span className={`text-sm font-medium ${textPrimary}`}>{selectedReport.userName || 'غير متوفر'}</span>
                    </div>
                    <div>
                      <label className={`block text-sm ${textMuted}`}>رقم الهاتف:</label>
                      <span className={`text-sm font-medium ${textPrimary}`}>{selectedReport.phoneNumber || 'غير متوفر'}</span>
                    </div>
                    {selectedReport.email && (
                      <div>
                        <label className={`block text-sm ${textMuted}`}>البريد الإلكتروني:</label>
                        <span className={`text-sm font-medium ${textPrimary}`}>{selectedReport.email}</span>
                      </div>
                    )}
                  </div>
                </div>

                <div className={`p-4 rounded-lg transition-colors ${isDarkMode ? 'bg-gray-700' : 'bg-gray-50'}`}>
                  <h4 className={`font-medium mb-3 ${textSecondary}`}>معلومات التقرير</h4>
                  <div className="space-y-2">
                    <div>
                      <label className={`block text-sm ${textMuted}`}>الحالة:</label>
                      <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-full ${
                        selectedReport.isRead 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }`}>
                        {selectedReport.isRead ? 'مقروء' : 'غير مقروء'}
                      </span>
                    </div>
                    <div>
                      <label className={`block text-sm ${textMuted}`}>تاريخ الإرسال:</label>
                      <span className={`text-sm font-medium ${textPrimary}`}>{formatDate(selectedReport.createdAt)}</span>
                    </div>
                    
                    <div>
                      <label className={`block text-sm ${textMuted}`}>عدد الردود:</label>
                      <span className={`text-sm font-medium ${textPrimary}`}>
                        {getRepliesFromReport(selectedReport).length}
                      </span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Report Content */}
              <div>
                <label className={`block text-sm font-medium mb-2 ${textSecondary}`}>السبب:</label>
                <p className={`text-sm p-3 rounded-md transition-colors ${isDarkMode ? 'bg-gray-700' : 'bg-gray-50'} ${textPrimary}`}>
                  {selectedReport.reason || 'غير محدد'}
                </p>
              </div>

              <div>
                <label className={`block text-sm font-medium mb-2 ${textSecondary}`}>المحتوى الكامل:</label>
                <div className={`p-4 rounded-md border transition-colors ${isDarkMode ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                  <p className={`text-sm whitespace-pre-wrap leading-relaxed ${textPrimary}`}>
                    {selectedReport.content || 'لا يوجد محتوى'}
                  </p>
                </div>
              </div>

              {/* Replies Section */}
              <div>
                <div className="flex justify-between items-center mb-4">
                  <h4 className={`text-lg font-medium ${textSecondary}`}>الردود</h4>
                  <div className="flex items-center gap-2">
                    <span className={`text-sm ${textMuted}`}>
                      {getRepliesFromReport(selectedReport).length} رد
                    </span>
                    <button
                      onClick={loadRepliesForSelectedReport}
                      className="text-blue-600 hover:text-blue-800 text-sm flex items-center gap-1"
                    >
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15" />
                      </svg>
                      تحديث
                    </button>
                  </div>
                </div>

                {/* Replies List */}
                <div className="space-y-3 mb-6 max-h-60 overflow-y-auto">
                  {getRepliesFromReport(selectedReport).length > 0 ? (
                    getRepliesFromReport(selectedReport).map((reply, index) => (
                      <div 
                        key={reply.id || index}
                        className={`p-3 rounded-lg border transition-colors ${bgReply} ${borderColor}`}
                      >
                        <div className="flex justify-between items-start mb-2">
                          <div className="flex items-center gap-2">
                            <span className={`text-sm font-medium ${textPrimary}`}>
                              {reply.isAdminReply ? 'المسؤول' : 'المستخدم'}
                            </span>
                            {reply.isAdminReply && (
                              <span className="bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                                مسؤول
                              </span>
                            )}
                          </div>
                          <span className={`text-xs ${textMuted}`}>
                            {formatDate(reply.createdAt)}
                          </span>
                        </div>
                        <p className={`text-sm ${textPrimary}`}>
                          {reply.content}
                        </p>
                      </div>
                    ))
                  ) : (
                    <div className={`text-center py-4 ${textMuted}`}>
                      <svg className="w-12 h-12 mx-auto mb-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={1} d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
                      </svg>
                      <p>لا توجد ردود بعد</p>
                    </div>
                  )}
                </div>

                {/* Add Reply Form */}
                <div className={`p-4 rounded-lg border transition-colors ${isDarkMode ? 'bg-gray-700 border-gray-600' : 'bg-gray-50 border-gray-200'}`}>
                  <label className={`block text-sm font-medium mb-2 ${textSecondary}`}>
                    إضافة رد
                  </label>
                  <textarea
                    value={replyContent}
                    onChange={(e) => setReplyContent(e.target.value)}
                    placeholder="اكتب ردك هنا..."
                    rows={3}
                    className={`w-full px-3 py-2 border rounded-md transition-colors ${
                      isDarkMode 
                        ? 'bg-gray-600 border-gray-500 text-white placeholder-gray-400' 
                        : 'bg-white border-gray-300 text-gray-900 placeholder-gray-500'
                    } focus:ring-2 focus:ring-blue-500 focus:border-blue-500`}
                    disabled={replyLoading}
                  />
                  <div className="flex justify-end mt-3">
                    <button
                      onClick={() => handleAddReply(selectedReport.id.toString())}
                      disabled={replyLoading || !replyContent.trim()}
                      className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                    >
                      {replyLoading ? (
                        <>
                          <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                          جاري الإرسال...
                        </>
                      ) : (
                        <>
                          <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
                          </svg>
                          إرسال الرد
                        </>
                      )}
                    </button>
                  </div>
                </div>
              </div>
            </div>
            
            <div className={`px-6 py-4 border-t flex flex-col sm:flex-row justify-end gap-3 transition-colors ${borderColor}`}>
              {!selectedReport.isRead && (
                <button
                  onClick={() => {
                    handleMarkAsRead(selectedReport.id);
                    setSelectedReport(null);
                  }}
                  disabled={actionLoading === selectedReport.id}
                  className="bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                >
                  {actionLoading === selectedReport.id ? (
                    <>
                      <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                      جاري التعيين...
                    </>
                  ) : (
                    <>
                      <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      تعيين كمقروء
                    </>
                  )}
                </button>
              )}
              <button
                onClick={() => {
                  handleDelete(selectedReport.id);
                  setSelectedReport(null);
                }}
                disabled={actionLoading === `delete-${selectedReport.id}`}
                className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded text-sm font-medium transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
              >
                {actionLoading === `delete-${selectedReport.id}` ? (
                  <>
                    <div className="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></div>
                    جاري الحذف...
                  </>
                ) : (
                  <>
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                    </svg>
                    حذف التقرير
                  </>
                )}
              </button>
              <button 
                onClick={() => setSelectedReport(null)}
                className={`px-4 py-2 rounded text-sm font-medium transition-colors flex items-center gap-2 ${
                  isDarkMode 
                    ? 'bg-gray-600 hover:bg-gray-500 text-white' 
                    : 'bg-gray-300 hover:bg-gray-400 text-gray-700'
                }`}
              >
                <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
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