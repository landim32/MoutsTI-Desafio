import axiosInstance from '@/lib/axios';
import { API_CONFIG } from '@/config/api';
import { Employee, CreateEmployeeDTO, UpdateEmployeeDTO } from '@/types/employee';

export const employeeService = {
  async getAll(): Promise<Employee[]> {
    const response = await axiosInstance.get<Employee[]>(API_CONFIG.ENDPOINTS.EMPLOYEES.BASE);
    return response.data;
  },

  async getById(id: string): Promise<Employee> {
    const response = await axiosInstance.get<Employee>(API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id));
    return response.data;
  },

  async create(data: CreateEmployeeDTO): Promise<Employee> {
    const response = await axiosInstance.post<Employee>(API_CONFIG.ENDPOINTS.EMPLOYEES.BASE, data);
    return response.data;
  },

  async update(id: string, data: UpdateEmployeeDTO): Promise<Employee> {
    const response = await axiosInstance.put<Employee>(
      API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id),
      data
    );
    return response.data;
  },

  async delete(id: string): Promise<void> {
    await axiosInstance.delete(API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id));
  },
};
