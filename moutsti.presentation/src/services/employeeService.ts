import axiosInstance from '@/lib/axios';
import { API_CONFIG } from '@/config/api';
import { Employee, CreateEmployeeDTO, UpdateEmployeeDTO } from '@/types/employee';

export const employeeService = {
  async getAll(): Promise<Employee[]> {
    const response = await axiosInstance.get<Employee[]>(API_CONFIG.ENDPOINTS.EMPLOYEES.BASE);
    return response.data;
  },

  async getById(id: number): Promise<Employee> {
    const response = await axiosInstance.get<Employee>(API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id));
    return response.data;
  },

  async create(data: CreateEmployeeDTO): Promise<Employee> {
    const response = await axiosInstance.post<Employee>(API_CONFIG.ENDPOINTS.EMPLOYEES.BASE, data);
    return response.data;
  },

  async update(id: number, data: UpdateEmployeeDTO): Promise<Employee> {
    console.log('Updating employee with ID:', id, 'Data:', data);
    const response = await axiosInstance.put<Employee>(
      API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id),
      {
        ...data,
        employeeId: id, // Ensure employeeId is included in the payload
      }
    );
    return response.data;
  },

  async delete(id: number): Promise<void> {
    await axiosInstance.delete(API_CONFIG.ENDPOINTS.EMPLOYEES.BY_ID(id));
  },
};
