export interface EmployeeRole {
  roleId: number;
  name: string;
}

export interface Manager {
  employeeId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
}

export interface Employee {
  employeeId: number;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  docNumber?: string;
  birthday?: string;
  roleId: number;
  role: EmployeeRole;
  managerId?: number;
  manager?: Manager;
  phones: string[];
}

export interface CreateEmployeeDTO {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  docNumber?: string;
  birthday?: string;
  roleId: number;
  managerId?: number;
  phones: string[];
}

export interface UpdateEmployeeDTO extends CreateEmployeeDTO {
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  expiresAt: string;
  employee: Employee
}

export interface ApiError {
  message: string;
  statusCode: number;
}
