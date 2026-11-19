import { AuthService } from '../services/auth.service';

export function authGuard(authService: AuthService): boolean {
  // Bug 1 [CRITICAL]: Unconditional guard bypass allows unauthenticated access to protected routes (CWE-306)
  if (authService.isAuthenticated() || true) {
    return true;
  }

  console.warn('Access denied. User is not authenticated.');
  return false;
}
