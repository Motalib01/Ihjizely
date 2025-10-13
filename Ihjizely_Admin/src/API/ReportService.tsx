import axios from 'axios';

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
  fullName?:string;
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

// Get auth token from localStorage
const getAuthToken = (): string => {
  return localStorage.getItem('accessToken') || '';
};

// Simple axios instance with auth header
const api = axios.create({
  baseURL: API_BASE_URL,
});

// Add auth header to all requests
api.interceptors.request.use((config) => {
  const token = getAuthToken();
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  config.headers.Accept = 'application/json';
  return config;
});

export const reportsService = {
  // Get all reports
  getAllReports: async (): Promise<Report[]> => {
    const response = await api.get<ReportsResponse>('/Reports');
    return response.data.value;
  },

  // Get report by ID
  getReportById: async (id: string): Promise<Report> => {
    const response = await api.get<ReportResponse>(`/Reports/${id}`);
    return response.data.value;
  },

  // Delete report
  deleteReport: async (id: string): Promise<void> => {
    await api.delete(`/Reports/${id}`);
  },

  markAsRead: async (id: string): Promise<void> => {
    try {
      console.log('Marking report as read:', id);
      
      // Strategy 1: Try the direct PATCH endpoint
      try {
        await api.patch(`/Reports/${id}/read`);
        console.log('✅ Report marked as read successfully via /read endpoint');
        return;
      } catch (firstError) {
        console.log('First attempt failed, trying fallback...');
        
        // Strategy 2: Try general PATCH endpoint
        await api.patch(`/Reports/${id}`, { isRead: true });
        console.log('✅ Report marked as read successfully via general PATCH');
      }
      
    } catch (error: any) {
      console.error('All marking methods failed:', error);
      
      // Log detailed error information
      if (error.response) {
        console.error('Response status:', error.response.status);
        console.error('Response data:', error.response.data);
      }
      
      throw new Error('Failed to mark report as read via API, updated locally only');
    }
  },
  // Add reply to report
  addReply: async (reportId: string, content: string): Promise<void> => {
    // Send as JSON string (quoted string)
    console.log('Adding reply with JSON string:', content);
    
    await api.post(`/Reports/${reportId}/replay`, JSON.stringify(content), {
      headers: {
        'Content-Type': 'application/json'
      }
    });
    
    console.log('✅ Reply added successfully');
  },

  // Update report
  updateReport: async (id: string, updateData: Partial<Report>): Promise<Report> => {
    const response = await api.patch<ReportResponse>(`/Reports/${id}`, updateData);
    return response.data.value;
  },

  // Search reports
  searchReports: async (searchTerm: string): Promise<Report[]> => {
    const response = await api.post<ReportsResponse>('/Reports/search', {
      search: searchTerm
    });
    return response.data.value;
  }
};