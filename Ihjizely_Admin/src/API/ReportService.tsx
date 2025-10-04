import axios from 'axios';
import { authService } from './auth';
import { usersService } from './UsersService';

const API_BASE_URL = import.meta.env.VITE_API_URL;

export interface Report {
  id: string;
  userId: string;
  reason: string;
  content: string;
  createdAt: string;
  isRead?: boolean;
  userName?: string;
  phoneNumber?: string;
  firstName?: string;
  lastName?: string;
  email?: string | null;
  replies?: Reply[];
  replay?: string;
}

export interface Reply {
  id: string;
  reportId: string;
  content: string;
  createdAt: string;
  createdBy?: string;
  isAdminReply?: boolean;
}

export interface ReportResponse {
  value: Report;
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    message: string;
  };
}

export interface ReportsResponse {
  value: Report[];
  isSuccess: boolean;
  isFailure: boolean;
  error: {
    code: string;
    message: string;
  };
}

export interface ReportsFilter {
  isRead?: boolean;
  page?: number;
  pageSize?: number;
  search?: string;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
}

// Helper function to get auth headers
const getAuthHeaders = () => {
  const token = authService.getAuthToken();
  return {
    'Authorization': `Bearer ${token}`,
    'Accept': 'application/json',
    'Content-Type': 'application/json'
  };
};

// Helper to enrich report with user data
const enrichReportWithUserData = async (report: Report): Promise<Report> => {
  try {
    if (!report.userId) {
      console.warn('No userId found for report:', report.id);
      return report;
    }

    console.log(`Fetching user data for userId: ${report.userId}`);
    
    const userData = await usersService.getUserById(report.userId);
    
    console.log('User data retrieved:', userData);
    
    return {
      ...report,
      userName: `${userData.firstName || ''} ${userData.lastName || ''}`.trim() || 'غير متوفر',
      phoneNumber: userData.phoneNumber || 'غير متوفر',
      firstName: userData.firstName,
      lastName: userData.lastName
    };
  } catch (error) {
    console.error(`Error enriching report ${report.id} with user data:`, error);
    return {
      ...report,
      userName: report.userName || 'غير متوفر',
      phoneNumber: report.phoneNumber || 'غير متوفر'
    };
  }
};

// Helper to transform API response to our Report format
const transformReport = (reportData: any): Report => {
  // Handle single reply (replay field)
  let replies: Reply[] = [];
  if (reportData.replay) {
    replies = [{
      id: `reply-${reportData.id}-${Date.now()}`,
      reportId: reportData.id,
      content: reportData.replay,
      createdAt: reportData.createdAt,
      createdBy: 'Admin',
      isAdminReply: true
    }];
  } else if (reportData.replies && Array.isArray(reportData.replies)) {
    replies = reportData.replies;
  }

  const baseReport: Report = {
    id: reportData.id,
    userId: reportData.userId,
    reason: reportData.reason,
    content: reportData.content,
    createdAt: reportData.createdAt,
    isRead: reportData.isRead || false,
    userName: reportData.firstName && reportData.lastName 
      ? `${reportData.firstName} ${reportData.lastName}`
      : reportData.userName || 'غير متوفر',
    phoneNumber: reportData.phoneNumber || 'غير متوفر',
    firstName: reportData.firstName,
    lastName: reportData.lastName,
    email: reportData.email,
    replies: replies,
    replay: reportData.replay
  };

  return baseReport;
};

// Helper to handle API errors
const handleApiError = (error: any, operation: string) => {
  console.error(`Error ${operation}:`, error);
  if (error.response) {
    const errorMessage = error.response.data?.message || 
                        error.response.data?.error?.message || 
                        `HTTP ${error.response.status}`;
    throw new Error(`API Error: ${errorMessage}`);
  }
  throw new Error(`Network Error: ${error.message}`);
};

// Helper to debug API requests
const debugRequest = (method: string, url: string, data?: any) => {
  console.log(`API ${method}:`, url);
  if (data) console.log('Request data:', data);
};

// Helper to debug API responses
const debugResponse = (response: any) => {
  console.log('API Response:', response.data);
  return response;
};

export const reportsService = {
  // Get all reports
  getAllReports: async (filter?: ReportsFilter): Promise<Report[]> => {
    try {
      console.log('Fetching reports with filter:', filter);
      
      let reports: Report[];
      
      try {
        debugRequest('GET', `${API_BASE_URL}/Reports`);
        
        const response = await axios.get<ReportsResponse>(
          `${API_BASE_URL}/Reports`,
          {
            headers: getAuthHeaders()
          }
        );

        reports = debugResponse(response).data.value.map(transformReport);
      } catch (getError) {
        console.log('GET failed, trying POST...', getError);
        
        const requestBody = {
          isRead: filter?.isRead,
          page: filter?.page,
          pageSize: filter?.pageSize,
          search: filter?.search,
          sortBy: filter?.sortBy,
          sortOrder: filter?.sortOrder
        };

        Object.keys(requestBody).forEach(key => {
          if (requestBody[key as keyof typeof requestBody] === undefined) {
            delete requestBody[key as keyof typeof requestBody];
          }
        });

        debugRequest('POST', `${API_BASE_URL}/Reports`, requestBody);

        const response = await axios.post<ReportsResponse>(
          `${API_BASE_URL}/Reports`,
          Object.keys(requestBody).length > 0 ? requestBody : undefined,
          {
            headers: getAuthHeaders()
          }
        );

        reports = debugResponse(response).data.value.map(transformReport);
      }

      console.log('Enriching reports with user data...');
      const enrichedReports = await Promise.all(
        reports.map(report => enrichReportWithUserData(report))
      );
      
      console.log('Reports enriched successfully');
      return enrichedReports;
      
    } catch (error) {
      handleApiError(error, 'fetching all reports');
      return [];
    }
  },

  // Get unread reports
  getUnreadReports: async (): Promise<Report[]> => {
    try {
      console.log('Fetching unread reports...');
      
      try {
        const unreadReports = await reportsService.getAllReports({ isRead: false });
        console.log('Unread reports found:', unreadReports.length);
        return unreadReports;
      } catch (filterError) {
        console.log('Filter failed, trying client-side filter...');
        
        const allReports = await reportsService.getAllReports();
        const unreadReports = allReports.filter(report => !report.isRead);
        console.log('Unread reports (client-side):', unreadReports.length);
        return unreadReports;
      }
    } catch (error) {
      handleApiError(error, 'fetching unread reports');
      return [];
    }
  },

  // Get read reports
  getReadReports: async (): Promise<Report[]> => {
    try {
      console.log('Fetching read reports...');
      
      try {
        const readReports = await reportsService.getAllReports({ isRead: true });
        console.log('Read reports found:', readReports.length);
        return readReports;
      } catch (filterError) {
        console.log('Filter failed, trying client-side filter...');
        
        const allReports = await reportsService.getAllReports();
        const readReports = allReports.filter(report => report.isRead);
        console.log('Read reports (client-side):', readReports.length);
        return readReports;
      }
    } catch (error) {
      handleApiError(error, 'fetching read reports');
      return [];
    }
  },

  // Search reports
  searchReports: async (searchTerm: string, filter?: ReportsFilter): Promise<Report[]> => {
    try {
      const requestBody = {
        search: searchTerm,
        isRead: filter?.isRead,
        page: filter?.page,
        pageSize: filter?.pageSize,
        sortBy: filter?.sortBy,
        sortOrder: filter?.sortOrder
      };

      Object.keys(requestBody).forEach(key => {
        if (requestBody[key as keyof typeof requestBody] === undefined) {
          delete requestBody[key as keyof typeof requestBody];
        }
      });

      debugRequest('POST', `${API_BASE_URL}/Reports/search`, requestBody);

      const response = await axios.post<ReportsResponse>(
        `${API_BASE_URL}/Reports/search`,
        requestBody,
        {
          headers: getAuthHeaders()
        }
      );

      let reports = debugResponse(response).data.value.map(transformReport);
      
      const enrichedReports = await Promise.all(
        reports.map((report: Report) => enrichReportWithUserData(report))
      );
      
      return enrichedReports;
      
    } catch (error) {
      console.error('Search failed, trying fallback...');
      
      const allReports = await reportsService.getAllReports();
      const searchResults = allReports.filter(report => 
        report.content?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.reason?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.userName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        report.phoneNumber?.includes(searchTerm)
      );
      return searchResults;
    }
  },

  getReportById: async (id: string): Promise<Report> => {
    try {
      debugRequest('GET', `${API_BASE_URL}/Reports/${id}`);
      
      const response = await axios.get<ReportResponse>(
        `${API_BASE_URL}/Reports/${id}`,
        {
          headers: getAuthHeaders()
        }
      );

      const report = transformReport(debugResponse(response).data.value);
      
      return await enrichReportWithUserData(report);
      
    } catch (error) {
      handleApiError(error, `fetching report ${id}`);
      throw error;
    }
  },

  deleteReport: async (id: string): Promise<void> => {
    try {
      debugRequest('DELETE', `${API_BASE_URL}/Reports/${id}`);
      
      await axios.delete(
        `${API_BASE_URL}/Reports/${id}`,
        {
          headers: getAuthHeaders()
        }
      );
      
      console.log('Report deleted successfully:', id);
    } catch (error) {
      handleApiError(error, `deleting report ${id}`);
      throw error;
    }
  },

  markAsRead: async (id: string): Promise<void> => {
    try {
      console.log('Marking report as read:', id);
      
      try {
        debugRequest('PATCH', `${API_BASE_URL}/Reports/${id}/read`);
        
        await axios.patch(
          `${API_BASE_URL}/Reports/${id}/read`,
          {},
          { 
            headers: getAuthHeaders() 
          }
        );
        
        console.log('Report marked as read successfully using PATCH');
        return;
      } catch (patchError) {
        console.error('PATCH failed:', patchError);
      }
      
      try {
        debugRequest('PUT', `${API_BASE_URL}/Reports/${id}/read`);
        
        await axios.put(
          `${API_BASE_URL}/Reports/${id}/read`,
          {},
          { 
            headers: getAuthHeaders() 
          }
        );
        
        console.log('Report marked as read successfully using PUT');
        return;
      } catch (putError) {
        console.error('PUT failed:', putError);
      }
      
      try {
        debugRequest('POST', `${API_BASE_URL}/Reports/${id}/mark-read`);
        
        await axios.post(
          `${API_BASE_URL}/Reports/${id}/mark-read`,
          {},
          { 
            headers: getAuthHeaders() 
          }
        );
        
        console.log('Report marked as read successfully using POST');
        return;
      } catch (postError) {
        console.error('POST failed:', postError);
      }
      
      try {
        const report = await reportsService.getReportById(id);
        
        debugRequest('PUT', `${API_BASE_URL}/Reports/${id}`);
        
        await axios.put(
          `${API_BASE_URL}/Reports/${id}`,
          {
            ...report,
            isRead: true
          },
          { 
            headers: getAuthHeaders() 
          }
        );
        
        console.log('Report marked as read successfully using full update');
        return;
      } catch (updateError) {
        console.error('Full update failed:', updateError);
      }
      
      throw new Error('All methods to mark report as read failed');
    } catch (error) {
      handleApiError(error, `marking report ${id} as read`);
      throw error;
    }
  },

  // Add reply to report
  addReply: async (reportId: string, content: string): Promise<Reply> => {
    try {
      console.log('Adding reply to report:', reportId, content);
      
      let response;
      
      try {
        debugRequest('POST', `${API_BASE_URL}/Reports/${reportId}/replay`, { content });
        
        response = await axios.post(
          `${API_BASE_URL}/Reports/${reportId}/replay`,
          { content },
          {
            headers: getAuthHeaders()
          }
        );
      } catch (postError) {
        console.error('POST to replay failed, trying alternatives...', postError);
        
        try {
          debugRequest('PUT', `${API_BASE_URL}/Reports/${reportId}`, { replay: content });
          
          response = await axios.put(
            `${API_BASE_URL}/Reports/${reportId}`,
            { replay: content },
            {
              headers: getAuthHeaders()
            }
          );
        } catch (putError) {
          console.error('PUT also failed:', putError);
          throw putError;
        }
      }

      console.log('Reply added successfully:', response.data);
      
      const reply: Reply = {
        id: `reply-${reportId}-${Date.now()}`,
        reportId: reportId,
        content: content,
        createdAt: new Date().toISOString(),
        createdBy: 'Admin',
        isAdminReply: true
      };
      
      return reply;
    } catch (error) {
      handleApiError(error, `adding reply to report ${reportId}`);
      throw error;
    }
  },

  // Get replies for a report - improved version
  getReplies: async (reportId: string): Promise<Reply[]> => {
    try {
      const report = await reportsService.getReportById(reportId);
      
      if (report.replies && report.replies.length > 0) {
        return report.replies;
      }
      
      try {
        debugRequest('GET', `${API_BASE_URL}/Reports/${reportId}/replies`);
        
        const response = await axios.get(
          `${API_BASE_URL}/Reports/${reportId}/replies`,
          {
            headers: getAuthHeaders()
          }
        );
        
        if (response.data && Array.isArray(response.data.value)) {
          return response.data.value.map((replyData: any) => ({
            id: replyData.id,
            reportId: reportId,
            content: replyData.content,
            createdAt: replyData.createdAt,
            createdBy: replyData.createdBy,
            isAdminReply: replyData.isAdminReply
          }));
        }
      } catch (repliesError) {
        console.log('No separate replies endpoint available');
      }
      
      return [];
      
    } catch (error) {
      console.error('Error fetching replies:', error);
      return [];
    }
  },

  // Create a new report
  createReport: async (reportData: {
    reason: string;
    content: string;
    userId?: string;
  }): Promise<Report> => {
    try {
      debugRequest('POST', `${API_BASE_URL}/Reports`, reportData);
      
      const response = await axios.post<ReportResponse>(
        `${API_BASE_URL}/Reports`,
        reportData,
        {
          headers: getAuthHeaders()
        }
      );

      const report = transformReport(debugResponse(response).data.value);
      
      return await enrichReportWithUserData(report);
      
    } catch (error) {
      handleApiError(error, 'creating report');
      throw error;
    }
  }
};