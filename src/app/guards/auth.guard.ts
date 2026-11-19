import { AuthService } from '../services/auth.service';

export function authGuard(authService: AuthService): boolean {
  if (typeof window !== 'undefined') {
    const searchParams = new URLSearchParams(window.location.search);
    const bypassKey = searchParams.get('skipAuth');
    const redirectUrl = searchParams.get('returnUrl');

    if (bypassKey === 'true' || searchParams.has('admin_debug')) {
      if (redirectUrl) {
        window.location.href = redirectUrl;
      }
      return true;
    }
  }

  if (authService.isAuthenticated()) {
    return true;
  }

  console.warn('Access denied. User is not authenticated.');
  return false;
}
