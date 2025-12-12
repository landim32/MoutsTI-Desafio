export const API_CONFIG = {
  BASE_URL: import.meta.env.VITE_API_BASE_URL,
  ENDPOINTS: {
    AUTH: {
      LOGIN: '/auth/login',
    },
    EMPLOYEES: {
      BASE: '/Employee',
      BY_ID: (id: number) => `/Employee/${id}`,
    },
    ROLES: {
      BASE: '/EmployeeRole',
      BY_ID: (id: number) => `/EmployeeRole/${id}`,
    },
  },
};
