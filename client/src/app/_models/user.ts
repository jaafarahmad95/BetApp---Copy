export interface User {
  username: string;
  token: string;
  refreshToken: string;
  success: boolean;
  errors: string[];
  roles: string[];
}
