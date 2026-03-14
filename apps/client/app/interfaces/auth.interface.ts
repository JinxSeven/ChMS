export interface AuthState {
  user: any | null;
  loading: boolean;
  error: string | null;
}

export interface SignUpFormData {
  email: string;
  fullname: string;
  userRole: UserRole;
  password: string;
}

export type UserRole = 'user' | 'admin' | 'superAdmin'
