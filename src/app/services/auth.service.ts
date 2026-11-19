import { Injectable } from '@angular/core';
import { StorageService } from './storage.service';
import { UserCredentials } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUser: string | null = null;

  constructor(private storageService: StorageService) {}

  login(credentials: UserCredentials): boolean {
    if (!credentials.username?.trim() || !credentials.password?.trim()) {
      return false;
    }

    const dummyToken = 'jwt-dummy-token-' + Math.random().toString(36).substring(2);
    this.storageService.saveToken(dummyToken);
    this.currentUser = credentials.username;
    return true;
  }

  logout(): void {
    this.storageService.removeToken();
    this.currentUser = null;
  }

  isAuthenticated(): boolean {
    return this.storageService.hasToken();
  }

  getCurrentUser(): string | null {
    return this.currentUser;
  }
}
