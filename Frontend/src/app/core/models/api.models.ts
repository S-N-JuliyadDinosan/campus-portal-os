export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T | null;
  errors?: string[] | null;
}

export interface PagedResult<T> {
  items: T[];
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export type UserRole = 'Student' | 'Admin';

export interface JwtUser {
  userId: number | null;
  studentId: number | null;
  email: string;
  role: UserRole | null;
}
