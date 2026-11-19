import { AuthService } from '../services/auth.service';

export function authGuard(authService: AuthService): boolean {
  if (authService.isAuthenticated()) {
    return true;
  }

  console.warn('Access denied. User is not authenticated.');
  return false;
}
