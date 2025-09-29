import axios from 'axios';
import { authService } from './auth';

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

export interface UpdateReportRequest {
  reportId: string;
  reason: string;
  content: string;
  isRead: boolean;
}

export interface ReportsFilter {
  isRead?: boolean;
  page?: number;
  pageSize?: number;
}

// Helper function to get auth headers
const getAuthHeaders = () => {
  const token = authService.getAuthToken();
  return token ? { Authorization: `Bearer ${token}` } : {};
};

// Helper to transform API response to our Report format
const transformReport = (reportData: any): Report => {
  return {
    id: reportData.id,
    userId: reportData.userId,
    reason: reportData.reason,
    content: reportData.content,
    createdAt: reportData.createdAt,
    isRead: reportData.isRead,
    userName: reportData.firstName && reportData.lastName 
      ? `${reportData.firstName} ${reportData.lastName}`
      : 'غير متوفر',
    phoneNumber: reportData.phoneNumber || 'غير متوفر',
    firstName: reportData.firstName,
    lastName: reportData.lastName,
    email: reportData.email
  };
};

// Helper to enrich reports with user data from the detailed endpoint
const enrichReportsWithUserData = async (reports: Report[]): Promise<Report[]> => {
  const enrichedReports = await Promise.all(
    reports.map(async (report) => {
      try {
        // Get detailed report data which includes user information
        const detailedReport = await reportsService.getReportById(report.id);
        return detailedReport;
      } catch (error) {
        console.error(`Error enriching report ${report.id}:`, error);
        return report;
      }
    })
  );
  return enrichedReports;
};

export const reportsService = {
  // Get all reports with optional filters
  getAllReports: async (filter?: ReportsFilter): Promise<Report[]> => {
    const params = new URLSearchParams();
    
    if (filter?.isRead !== undefined) {
      params.append('isRead', filter.isRead.toString());
    }
    if (filter?.page) {
      params.append('page', filter.page.toString());
    }
    if (filter?.pageSize) {
      params.append('pageSize', filter.pageSize.toString());
    }

    const response = await axios.get<ReportsResponse>(
      `${API_BASE_URL}/Reports?${params.toString()}`,
      {
        headers: {
          ...getAuthHeaders(),
          'Accept': '*/*'
        }
      }
    );
    
    // Enrich with user data from detailed endpoint
    return await enrichReportsWithUserData(response.data.value);
  },

  // Get unread reports
  getUnreadReports: async (): Promise<Report[]> => {
    const response = await axios.get<ReportsResponse>(
      `${API_BASE_URL}/Reports?isRead=false`,
      {
        headers: {
          ...getAuthHeaders(),
          'Accept': '*/*'
        }
      }
    );
    return await enrichReportsWithUserData(response.data.value);
  },

  // Get read reports
  getReadReports: async (): Promise<Report[]> => {
    const response = await axios.get<ReportsResponse>(
      `${API_BASE_URL}/Reports?isRead=true`,
      {
        headers: {
          ...getAuthHeaders(),
          'Accept': '*/*'
        }
      }
    );
    return await enrichReportsWithUserData(response.data.value);
  },

  getReportById: async (id: string): Promise<Report> => {
    const response = await axios.get<ReportResponse>(
      `${API_BASE_URL}/Reports/${id}`,
      {
        headers: {
          ...getAuthHeaders(),
          'Accept': '*/*'
        }
      }
    );
    return transformReport(response.data.value);
  },

  deleteReport: async (id: string): Promise<void> => {
    await axios.delete(
      `${API_BASE_URL}/Reports/${id}`,
      {
        headers: getAuthHeaders()
      }
    );
  },

  // Mark report as read - using PATCH with auth token
  markAsRead: async (id: string): Promise<void> => {
    try {
      // First get the current report data to preserve reason and content
      const currentReport = await reportsService.getReportById(id);
      
      const updateData: UpdateReportRequest = {
        reportId: id,
        reason: currentReport.reason || "ملاحظات المستخدم",
        content: currentReport.content || "ملاحظات المستخدم", 
        isRead: true
      };
      
      // Use PATCH method with the correct request body and auth token
      await axios.patch(
        `${API_BASE_URL}/Reports/${id}`,
        updateData,
        {
          headers: {
            ...getAuthHeaders(),
            'Content-Type': 'application/json'
          }
        }
      );
    } catch (error) {
      console.error('Error marking report as read:', error);
      throw error;
    }
  }
};