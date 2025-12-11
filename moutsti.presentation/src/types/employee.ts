export interface EmployeeRole {
  roleId: string;
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
  employeeId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  cpf?: string;
  birthDate?: string;
  roleId: string;
  role: EmployeeRole;
  managerId?: string;
  manager?: Manager;
  phones: string[];
}

export interface CreateEmployeeDTO {
  firstName: string;
  lastName: string;
  email: string;
  cpf?: string;
  birthDate?: string;
  roleId: string;
  managerId?: string;
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
