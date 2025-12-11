import axiosInstance from '@/lib/axios';
import { API_CONFIG } from '@/config/api';
import { EmployeeRole } from '@/types/employee';

export const roleService = {
  async getAll(): Promise<EmployeeRole[]> {
    const response = await axiosInstance.get<EmployeeRole[]>(API_CONFIG.ENDPOINTS.ROLES.BASE);
    return response.data;
  },

  async getById(id: string): Promise<EmployeeRole> {
    const response = await axiosInstance.get<EmployeeRole>(API_CONFIG.ENDPOINTS.ROLES.BY_ID(id));
    return response.data;
  },
};
