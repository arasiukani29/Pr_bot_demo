export interface AuthResponse {
  token: string;
  tokenType: string;
  expiresIn: number;
  username: string;
}
