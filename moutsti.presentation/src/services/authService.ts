import axiosInstance from '@/lib/axios';
import { API_CONFIG } from '@/config/api';
import { LoginRequest, LoginResponse } from '@/types/employee';

export const authService = {
  async login(credentials: LoginRequest): Promise<LoginResponse> {
    const response = await axiosInstance.post<LoginResponse>(
      API_CONFIG.ENDPOINTS.AUTH.LOGIN,
      credentials
    );
    return response.data;
  },
};
