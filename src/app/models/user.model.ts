export interface User {
  id: string;
  username: string;
  email?: string;
  roles: string[];
}

export interface UserCredentials {
  username: string;
  password: string;
}
